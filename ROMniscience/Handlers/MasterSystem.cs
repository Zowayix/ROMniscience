﻿/*
 * The MIT License
 *
 * Copyright 2018 Megan Leet (Zowayix).
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using ROMniscience.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMniscience.Handlers {
	//http://www.smspower.org/Development/ROMHeader
	//http://www.smspower.org/Development/SDSCHeader
	class MasterSystem : Handler {
		public override IDictionary<string, string> filetypeMap => new Dictionary<string, string>() {
			{"sms", "Sega Master System ROM"}
		};

		public override string name => "Sega Master System";

		readonly static IDictionary<int, string> REGIONS = new Dictionary<int, string> {
			{3, "Japanese SMS"},
			{4, "Export SMS"},
			{5, "Japanese Game Gear"},
			{6, "Export Game Gear"},
			{7, "International Game Gear"}, //What's the difference between this and export?
		};

		readonly static IDictionary<int, long> ROM_SIZES = new Dictionary<int, long> {
			{0xa, 8 * 1024},
			{0xb, 16 * 1024},
			{0xc, 32 * 1024},
			{0xd, 48 * 1024}, //Apparently buggy (some BIOSes have issues calculating checksums for ROMs of this size)
			{0xe, 64 * 1024},
			{0xf, 128 * 1024},
			{0, 256 * 1024},
			{1, 512 * 1024},
			{2, 1024 * 1024}, //Also apparently buggy
		};

		static int decodeBCD(int i) {
			int hi = (i & 0xf0) >> 4;
			int lo = i & 0x0f;
			return ((hi * 10) + lo);
		}

		static int decodeBCD(byte[] b) {
			int hi1 = (b[1] & 0xf0) >> 4;
			int lo1 = b[1] & 0x0f;
			int hi2 = (b[0] & 0xf0) >> 4;
			int lo2 = b[0] & 0x0f;
			return (((hi1 * 10) + lo1) * 100) + ((hi2 * 10) + lo2);
		}

		public static void parseSegaHeader(ROMInfo info, WrappedInputStream s, long offset, bool isGameGear) {
			s.Position = offset;

			byte[] reserved = s.read(2);
			info.addInfo("Reserved", reserved, true);

			ushort checksum = (ushort)s.readShortLE();
			info.addInfo("Checksum", checksum, ROMInfo.FormatMode.HEX, true);

			byte[] productCodeHi = s.read(2);
			int productCodeAndVersion = s.read();
			int productCodeLo = (productCodeAndVersion & 0xf0) >> 4;
			int version = productCodeAndVersion & 0x0f;

			string productCode = String.Format("{0}{1:0000}", productCodeLo == 0 ? "" : productCodeLo.ToString(), decodeBCD(productCodeHi));
			info.addInfo("Product code", productCode);
			if(isGameGear){
				if (productCode.Length >= 5) {
					string makerCode = "T-" + productCode.Substring(0, productCode.Length - 3);
					info.addInfo("Publisher", makerCode, SegaCommon.LICENSEES);
				} else {
					info.addInfo("Publisher", "Sega");
				}
			}
			info.addInfo("Version", version);

			int regionCodeAndROMSize = s.read();
			int regionCode = (regionCodeAndROMSize & 0xf0) >> 4;
			int romSize = regionCodeAndROMSize & 0x0f;

			info.addInfo("Region", regionCode, REGIONS);
			info.addInfo("ROM size", romSize, ROM_SIZES, ROMInfo.FormatMode.SIZE);

			ushort calculatedChecksum = calculateChecksum(s, romSize);
			info.addInfo("Calculated checksum", calculatedChecksum, ROMInfo.FormatMode.HEX, true);
			info.addInfo("Checksum valid?", checksum == calculatedChecksum);
		}

		static ushort calculateChecksum(WrappedInputStream f, int romSizeCode) {
			long origPos = f.Position;
			long length = f.Length;

			int lowStart = 0;
			int lowEnd;
			if(romSizeCode == 0xa) {
				lowEnd = 0x1fef;
			} else if (romSizeCode == 0xb) {
				lowEnd = 0x3fef;
			} else if (romSizeCode == 0xd) {
				lowEnd = 0xbfef; //Does weird things on some real BIOSes, so this may end up incorrect
			} else {
				lowEnd = 0x7fef;
			}

			ushort c = 0;
			
			try {
				f.Position = lowStart;
				while(f.Position <= lowEnd) {
					int b = f.read();
					if(b == -1) {
						//Prevent infinite loop when the ROM size byte is incorrect and larger than the actual ROM size, causing us to go past the end of the stream and hence the position never changes while we keep reading and it's still before lowEnd
						break;
					}
					c = (ushort)((ushort)(c + b) & 0xffff);
				}

				if(romSizeCode == 0xe || romSizeCode == 0xf || romSizeCode == 0 || romSizeCode == 1 || romSizeCode == 2) {
					int highStart = 0x8000;
					int highEnd = 0xffff;
					if(romSizeCode == 0xf) {
						highEnd = 0x1ffff;
					} else if(romSizeCode == 0) {
						highEnd = 0x3ffff;
					} else if(romSizeCode == 1) {
						highEnd = 0x7ffff;
					} else if(romSizeCode == 2) {
						highEnd = 0xfffff; //Also does weird things on some real BIOSes
					}

					f.Position = highStart;
					while (f.Position <= highEnd && f.Position <= f.Length) {
						int b = f.read();
						if (b == -1) {
							break;
						}
						c = (ushort)((ushort)(c + b) & 0xffff);
					}
				}
				return c;
			} finally {
				f.Position = origPos;
			}
		}

		static String readNullTerminatedString(WrappedInputStream s) {
			StringBuilder sb = new StringBuilder();
			while (true) {
				int b = s.read();
				if(b < 32 || b > 127) {
					//If we reached end of file or null, then we're done here. But
					//also the SDSC says strings should only contain printable characters
					//in the range 0 to 127, and that they're ASCII specifically, so
					//if we encounter anything outside that range or unprintable, we've encountered
					//an invalid header and should proceed no further
					break;
				}
				sb.Append(Encoding.ASCII.GetChars(new byte[] {(byte)b}));
			}
			return sb.ToString();
		}

		public static void parseSDSCHeader(ROMInfo info, WrappedInputStream s) {
			int majorVersion = s.read();
			info.addInfo("Major version", decodeBCD(majorVersion));
			int minorVersion = s.read();
			info.addInfo("Minor version", decodeBCD(minorVersion));

			int day = decodeBCD(s.read());
			info.addInfo("Day", day);
			int month = decodeBCD(s.read());
			info.addInfo("Month", (month != 0 && month < 13) ? System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(month) : String.Format("Unknown ({0})", month));
			int year = decodeBCD(s.read(2));
			info.addInfo("Year", year);

			try {
				info.addInfo("Date", new DateTime(year, month, day));
			} catch (ArgumentOutOfRangeException) {
				//Ignore
			}

			ushort authorNameOffset = (ushort)s.readShortLE();
			ushort nameOffset = (ushort)s.readShortLE();
			ushort descriptionOffset = (ushort)s.readShortLE();
			info.addInfo("Author offset", authorNameOffset, ROMInfo.FormatMode.HEX, true);
			info.addInfo("Name offset", nameOffset, ROMInfo.FormatMode.HEX, true);
			info.addInfo("Description offset", descriptionOffset, ROMInfo.FormatMode.HEX, true);

			if(authorNameOffset != 0xfff && authorNameOffset != 0) {
				s.Position = authorNameOffset;
				info.addInfo("Author", readNullTerminatedString(s));
			}
			if(nameOffset != 0xffff) {
				s.Position = nameOffset;
				info.addInfo("Internal name", readNullTerminatedString(s));
			}
			if(descriptionOffset != 0xffff) {
				s.Position = descriptionOffset;
				info.addInfo("Description", readNullTerminatedString(s));
			}
		}

		public static void parseSMSROM(ROMInfo info, ROMFile file) {
			WrappedInputStream s = file.stream;

			long headerOffset = -1;
			//Supposedly the header _could_ be at 0x1ff0 or 0x3ff0 but no known software does that. But let's check it anyway just for something to do
			s.Position = 0x1ff0;
			string magic = "TMR SEGA";
			if (s.read(8, Encoding.ASCII).Equals(magic)) {
				headerOffset = s.Position;
			} else {
				s.Position = 0x3ff0;
				if (s.read(8, Encoding.ASCII).Equals(magic)) {
					headerOffset = s.Position;
				} else {
					s.Position = 0x7ff0;
					if (s.read(8, Encoding.ASCII).Equals(magic)) {
						headerOffset = s.Position;
					}
				}
			}

			if (headerOffset != -1) {
				//It's entirely possible this header isn't there, since Japanese Master Systems don't check for it
				//and therefore Japanese Master System games aren't required to have it
				info.addInfo("Header position", headerOffset, true);
				info.addInfo("Has standard header", true);
				bool isGameGear = "gg".Equals(file.extension);
				parseSegaHeader(info, s, headerOffset, isGameGear);
			} else {
				info.addInfo("Has standard header", false);
			}

			s.Position = 0x7fe0;
			if(s.read(4, Encoding.ASCII).Equals("SDSC")) {
				info.addInfo("Has SDSC header", true);

				parseSDSCHeader(info, s);
			} else {
				info.addInfo("Has SDSC header", false);
			}

			//TODO Codemasters header (tricky to detect presence, and I don't have any Codemasters games anyway)
		}

		public override void addROMInfo(ROMInfo info, ROMFile file) {
			info.addInfo("Platform", name);
			parseSMSROM(info, file);
		}
	}
}
