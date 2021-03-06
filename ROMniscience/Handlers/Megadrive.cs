﻿/*
 * The MIT License
 *
 * Copyright 2017 Megan Leet (Zowayix).
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ROMniscience.IO;
using System.IO;

namespace ROMniscience.Handlers {
	class Megadrive : Handler {
		//Some stuff adapted from https://www.zophar.net/fileuploads/2/10614uauyw/Genesis_ROM_Format.txt
		public override IDictionary<string, string> filetypeMap => new Dictionary<string, string> {
			{"gen", "Sega Genesis/Megadrive ROM"},
			{"bin", "Sega Genesis/Megadrive ROM"},
			{"sgd", "Sega Genesis/Megadrive ROM"},
			{"smd", "Sega Genesis/Megadrive interleaved ROM"},
			{"md", "Sega Genesis/Megadrive ROM"},
		};

		public override string name => "Megadrive/Genesis";

		public readonly static IDictionary<string, string> PRODUCT_TYPES = new Dictionary<string, string> {
			{"AI", "Education"}, //Wonder Library, Time Trax (although that's not educational), Miracle Piano Teaching System... maybe this is actually the wrong thing to call it, especially as no Pico games use it
			{"BR", "Boot ROM"}, //Mega CD BIOS, LaserActive BIOS etc
			{"GM", "Game"},
			{"OS", "Operating system"}, //Genesis OS ROM uses this
			{"PX", "Pictures"}, //Hentai Collection homebrew (lol) etc
			{"SF", "Super Fighter Team game"}, //Beggar Price, Legend of Wukong, Star Odyssey, etc
			{"83", "Samsung Pico"}, //Some Samsung Pico titles also use T-, but 83 is only used by Samsung Pico
			{"MP", "Brazilian Pico game"},
			
			//Also seen:
			//G-: Mega Anser BIOS
			//HP: Many Pico games
			//MK: Many Pico games but also some Megadrive betas, for what it's worth (Dynamite Heddy, Virtua Fighter 2, etc)
			//RO: The 32X SDK programs, but also FIFA Soccer 96 for 32X... did they leave some SDK stuff in there?
			//T-: Many Pico games, but also some Megadrive/32X betas like Wacky Races and Soulstar X

			//I have no idea what the zillions of different types for Pico games are all about, but maybe if I actually knew more about the Pico it would start making sense
			//TE (Angry Birds demo hack)
			//CE (Censor Movie Trailer Demo, probably just stands for "Censor")
			//X- (Magical Christmas Greetings demo)
			//DE (Overdrive 2 demo, possibly stands for "demo")
			//TH (Overdrive demo by Titan)
	
			//Pico doesn't seem to use AI, surprisingly. Instead it has:
			//HP: e.g. A Bug's Life, Cooking Pico
			//MK: e.g. A Year at Pooh Corner, Crayola Crayons: Create a World
			//MP: Cores Magicas, Ecco Jr. No fundo do mar!, Hello Kitty no Castelo (these are all Brazilian)
			//83: Dorehmipa Dongmooleumakhwoe, 
				//Ehko Junieoeui Shinbiroun Badayeohaeng (Ecco Jr. and the Great Ocean Treasure Hunt but Korean), both Korean
			//61: Drive Pico: Saa Shuppatsu Da! Ken-chan to Pepe no Wanpaku Drive
			//A lot of T- as above
			//I can only guess what correlation there even is, or what they mean
	
			//Misused completely in 16 Zhang Majiang, which is a bootleg
		};

		public readonly static IDictionary<char, string> IO_SUPPORT = new Dictionary<char, string> {
			{'0', "Master System joypad"},
			{'4', "Team Play"},
			{'6', "6-button joypad"},
			{'A', "Analog joystick"}, //After Burner (is this the XE-1 AP?)
			{'B', "Control ball"},
			{'C', "CD-ROM"},
			{'F', "Floppy drive"},
			{'G', "Menacer"},
			{'J', "Joypad"},
			{'K', "Keyboard"},
			{'L', "Activator"},
			{'M', "Mouse"},
			{'O', "J-Cart"},
			{'P', "Printer"},
			{'R', "Serial RS232C"},
			{'T', "Tablet"},
			{'V', "Paddle"},
			
			//I would think the Ten Key Pad would have its own entry here but who knows
			//Others I've seen but I don't know:
			//D (pretty much every homebrew, could it mean "demo" or "development"? SGDK inserts this with no explanation)
			//Roadwar 2000 seems to corrupt and misuse this field entirely
			//Outline 2017 demo seems to misuse both this field and the
			//product type field
			//MDEM says "Joypad only!" in ASCII in this field, as well as putting 
			//"The best" as the product type and code
		};

		public readonly static IDictionary<char, string> COUNTRY = new Dictionary<char, string> {
			//Other than the first three, all of this is weird, confusing, and possibly all just a load of bullshit! Yay.
			{'J', "Japan"},
			{'U', "USA"},
			{'E', "Europe"}, //Some Sega Pico games have this twice for some reason, or this plus another more specific European country
			
			{'4', "Brazil + USA"}, //Definitely used in Brazilian Pico games, but also some USA Genesis games (Comix Zone, Beyond Oasis) and USA Pico games
			{'8', "Hong Kong"}, //Questionable... only seen in a few European Pico games
			{'A', "Europe (A)"}, //This is listed in some documentation as Asia, but there are some definitely-Europe-only games which have it (e.g. Darxide) so it's not even PAL Asia or Australia being mistaken for Europe. I don't get it because E is definitely used in other European games as you would expect, even on the 32X so they never changed it from E to A oranything like that...
			{'B', "Brazil (B)"}, //Doesn't seem to be used, all the Brazilian stuff uses 4 (which is also used by some USA games)
			{'C', "USA + Europe"}, //Usually not used in favour of just using U and E together, but Garfield: Caught in the Act uses it
			{'F', "Worldwide / France"}, //Also said to be France, but that isn't quite right except for the French Pico games that have EF for Europe + France
			//I might need to process this field differently.... but then it's used in a Japanese Pico game and there's a French Pico game that doesn't have E! AAAA!!! Did Sega just not give a shit about the headers when approving games for release?
			{'G', "Germany"},

			{'I', "Italy"},
			{'S', "Spain"},
			{'e', "Europe (e)"},

			{'1', "Japan"},
			{'5', "Japan + USA"}, //This one's not really seen that often because most games would just use Japan and USA together, but the odd 32X game uses this combination... but then just to be confusing, this is also used in Samsung Pico games and some Taiwanese game, and Magic School Bus which wasn't even released in Japan, so I guess it could also mean all of NTSC
	
			//There's a 2 in the Multi-Mega BIOS, not sure what it means, as far as I can tell
			//that BIOS is just for Europe which it also has as a country code
			//Puggsy protoype has "NOV" in this field which seems to be misused
		};

		public readonly static IDictionary<string, int> MONTH_ABBREVIATIONS = new Dictionary<string, int> {
			{"JAN", 1},
			{"FEB", 2},
			{"MAR", 3},
			{"APR", 4},
			{"APL", 4},
			{"MAY", 5},
			{"JUN", 6},
			{"JUL", 7},
			{"JUR", 7},
			{"JLY", 7},
			{"AUG", 8},
			{"08", 8},
			{"SEP", 9},
			{"SEPT", 9},
			{"OCT", 10},
			{"NOV", 11},
			{"DEC", 12},
		};

		public static WrappedInputStream decodeSMD(WrappedInputStream s) {
			s.Seek(512, SeekOrigin.Current);
			//Should only need this much to read the header. If I was actually converting
			//the ROM I'd need to use the SMD header to know how many blocks there are
			//and read multiple blocks and whatnot
			byte[] block = s.read(16384);

			byte[] buf = new byte[16386];
			//Yes, the below starting points are correct. It makes no goddamned
			//sense but if I use even = 0 and odd = 1 as the starting points
			//it goes off by one so I don't know anymore
			int buf_even = 1;
			int buf_odd = 2;

			int midpoint = 8192;
			for (int i = 0; i < block.Length; ++i) {
				if (i <= midpoint) {
					buf[buf_even] = block[i];
					buf_even += 2;
				} else {
					buf[buf_odd] = block[i];
					buf_odd += 2;
				}
			}

			byte[] buf2 = new byte[buf_odd];
			Array.Copy(buf, buf2, buf_odd);
			return new WrappedInputStream(new MemoryStream(buf2));
		}

		public static bool isSMD(WrappedInputStream s) {
			long origPos = s.Position;
			try {
				s.Position = 8;
				int b8 = s.read();
				int b9 = s.read();

				s.Position = 0x280;
				string str = s.read(4, Encoding.ASCII);

				return b8 == 0xaa && b9 == 0xbb && (String.Equals(str, "EAMG") || String.Equals(str, "EAGN"));
			} finally {
				s.Position = origPos;
			}
		}

		public static int calcChecksum(WrappedInputStream s) {
			long pos = s.Position;
			long len = s.Length;
			try {
				s.Position = 0x200;
				int checksum = 0;
				while (s.Position < len) {
					checksum = (checksum + s.readShortBE()) & 0xffff;
				}
				return checksum;
			} finally {
				s.Position = pos;
			}
		}

		static string getPlatformFromConsoleName(string consoleName, bool isCD) {
			if (consoleName.StartsWith("SEGA 32X")) {
				if (isCD) {
					return "Sega CD/Mega CD + 32X";
				} else {
					return "Sega 32X";
				}
			} else if (consoleName.Equals("SAMSUNG PICO")) {
				return "Samsung Pico";
			} else if (consoleName.Equals("SEGA PICO") || consoleName.Equals("SEGATOYS PICO") || consoleName.Equals("SEGA TOYS PICO")
				|| consoleName.Equals("IMA IKUNOJYUKU") || consoleName.Equals("IMA IKUNOUJYUKU")) {
				//I... have zero idea what those last two are about, but they're a thing in some Sega Pico titles
				return "Sega Pico";
			} else {
				bool isUSA = consoleName.StartsWith("SEGA GENESIS");
				bool isMD = consoleName.StartsWith("SEGA MEGA DRIVE") || consoleName.StartsWith("SEGA MEGADRIVE");
				if (isCD) {
					if (isUSA) {
						return "Sega CD";
					} else if (isMD) {
						return "Mega CD";
					} else {
						return "Sega CD/Mega CD";
					}
				} else {
					if (isUSA) {
						return "Sega Genesis";
					} else if (isMD) {
						return "Sega Mega Drive";
					} else {
						return "Sega Genesis/Mega Drive";
					}
				}
			}
		}

		private static readonly Regex copyrightRegex = new Regex(@"\(C\)(\S{4}.)(\d{4})\.(.{3})");
		public static void parseMegadriveROM(ROMInfo info, WrappedInputStream s, bool isCD = false) {
			s.Position = 0x100;

			//While we're here, it should be noted that some allegedly official licensed and final don't have correct header information
			//of any kind, and as such you will see plain garbage all across here. Even the console name, which results in them not being
			//able to boot on real Model 2 and Model 3 hardware. Point at them and laugh.
			//These games are: Zany Golf (rev 0) and Budokan: The Martial Spirit, among others which aren't actually licensed.
			//Anyway, I just thought that was interesting enough to comment on. I found it fascinating. Am I just kinda weird? Oh well.

			string consoleName = s.read(16, Encoding.ASCII).TrimEnd('\0', ' ');
			info.addInfo("Console name", consoleName);
			info.addInfo("Platform", getPlatformFromConsoleName(consoleName.TrimStart(' '), isCD));

			string copyright = s.read(16, Encoding.ASCII).TrimEnd('\0', ' ');
			info.addInfo("Copyright", copyright);
			var matches = copyrightRegex.Match(copyright);
			if (matches.Success) {
				string maker = matches.Groups[1].Value?.Trim().TrimEnd(',');
				maker = Regex.Replace(maker, "^T-0", "T-");
				maker = Regex.Replace(maker, "^T(?!-)", "T-");

				info.addInfo("Publisher", maker, SegaCommon.LICENSEES);
				info.addInfo("Year", matches.Groups[2].Value);
				if (MONTH_ABBREVIATIONS.TryGetValue(matches.Groups[3].Value?.ToUpper(), out int month)) {
					info.addInfo("Month", System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(month));
				} else {
					info.addInfo("Month", String.Format("Unknown ({0})", matches.Groups[3].Value));
				}
			}

			string domesticName = s.read(48, MainProgram.shiftJIS).TrimEnd('\0', ' ');
			info.addInfo("Internal name", domesticName);
			string overseasName = s.read(48, MainProgram.shiftJIS).TrimEnd('\0', ' ');
			info.addInfo("Overseas name", overseasName);
			string productType = s.read(2, Encoding.ASCII);
			info.addInfo("Type", productType, PRODUCT_TYPES);

			s.read(); //Space for padding
			string serialNumber = s.read(8, Encoding.ASCII).TrimEnd('\0', ' ');
			info.addInfo("Product code", serialNumber);
			s.read(); //- for padding
			string version = s.read(2, Encoding.ASCII);
			info.addInfo("Version", version);

			ushort checksum = (ushort)s.readShortBE();
			info.addInfo("Checksum", checksum, ROMInfo.FormatMode.HEX, true);
			if (!isCD) {
				int calculatedChecksum = calcChecksum(s);
				info.addInfo("Calculated checksum", calculatedChecksum, ROMInfo.FormatMode.HEX, true);
				info.addInfo("Checksum valid?", checksum == calculatedChecksum);
			}

			char[] ioSupportList = s.read(16, Encoding.ASCII).ToCharArray().Where((c) => c != ' ' && c != '\0').ToArray();
			info.addInfo("Compatible peripherals", ioSupportList, IO_SUPPORT);

			int romStart = s.readIntBE();
			info.addInfo("ROM start", romStart, ROMInfo.FormatMode.HEX, true);
			int romEnd = s.readIntBE();
			info.addInfo("ROM end", romEnd, ROMInfo.FormatMode.HEX, true);
			info.addInfo("ROM size", romEnd - romStart, ROMInfo.FormatMode.SIZE);
			int ramStart = s.readIntBE();
			info.addInfo("RAM start", ramStart, ROMInfo.FormatMode.HEX, true);
			int ramEnd = s.readIntBE();
			info.addInfo("RAM end", ramEnd, ROMInfo.FormatMode.HEX, true);
			info.addInfo("RAM size", ramEnd - ramStart, ROMInfo.FormatMode.SIZE);
			byte[] backupRamID = s.read(4);
			info.addInfo("Backup RAM ID", backupRamID);
			int backupRamStart = s.readIntBE();
			info.addInfo("Backup RAM start", backupRamStart, ROMInfo.FormatMode.HEX, true);
			int backupRamEnd = s.readIntBE();
			info.addInfo("Backup RAM end", backupRamEnd, ROMInfo.FormatMode.HEX, true);
			info.addInfo("Save size", backupRamEnd - backupRamStart, ROMInfo.FormatMode.SIZE);

			string modemData = s.read(12, Encoding.ASCII).TrimEnd(' ');
			info.addInfo("Modem data", modemData);
			//If entirely spaces, modem not supported
			//Should be an ASCII string in the format MO<company><modem no#>.<version>, padding with spaces as needed
			//Not really seen often, only seen so far in:
			//Advanced Daisenryaku: Deutsch Dengeki Sakusen Mar 7 1991 prototype: "MOSEGA05,010"
			//Sorcer Kingdom Nov 8 1991 prototype: "MO(C)T-25"
			//Ma Jiang Qing Ren: Ji Ma Jiang Zhi (bootleg): Quite literally "MOxxxxyy.z"
			//Game Toshokan, MegaMind: "MOSEGA03.000"

			string memo = s.read(40, Encoding.ASCII).TrimEnd('\0', ' ');
			info.addInfo("Memo", memo);
			char[] countries = s.read(3, Encoding.ASCII).ToCharArray().Where((c) => c != ' ' && c != '\0').ToArray();
			info.addInfo("Country", countries, COUNTRY);
		}

		public override void addROMInfo(ROMInfo info, ROMFile file) {
			if (isSMD(file.stream)) {
				info.addInfo("Detected format", "Super Magic Drive interleaved");
				parseMegadriveROM(info, decodeSMD(file.stream));
			} else {
				info.addInfo("Detected format", "Plain");
				parseMegadriveROM(info, file.stream);
			}
		}
	}
}
