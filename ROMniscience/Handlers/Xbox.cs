﻿using ROMniscience.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMniscience.Handlers {
	class Xbox : Handler {
		//http://www.caustik.com/cxbx/download/xbe.htm
		//http://xboxdevwiki.net/Xbe
		public override IDictionary<string, string> filetypeMap => new Dictionary<string, string>() {
			{"xbe", "Microsoft Xbox executable"},
			{"iso", "Microsoft Xbox disc"}
		};

		public override string name => "Xbox";

		

		[Flags]
		enum XboxRegions: uint {
			NorthAmerica = 1,
			Japan = 2,
			RestOfWorld = 4,
			Manufacturing = 0x80000000,
		}

		public static DateTime convertWindowsDate(int date) {
			DateTime delta = new DateTime(1969, 12, 31, 16, 0, 0, DateTimeKind.Utc);
			return delta.AddSeconds(date);
		}

		public static void parseXBE(ROMInfo info, WrappedInputStream s) {
			string magic = s.read(4, Encoding.ASCII);
			if (!"XBEH".Equals(magic)) {
				info.addInfo("Detected format", "Unknown");
				return;
			}

			info.addInfo("Detected format", "XBE");

			byte[] signature = s.read(256);
			info.addInfo("Signed", signature.Any(b => b != 0));

			s.Position = 0x104;
			int baseAddress = s.readIntLE();
			info.addInfo("Base address", baseAddress, ROMInfo.FormatMode.HEX, true);

			int headerSize = s.readIntLE();
			int imageSize = s.readIntLE();
			int imageHeaderSize = s.readIntLE();
			info.addInfo("Header size", headerSize, true);
			info.addInfo("Image size", imageSize, true);
			info.addInfo("Image header size", imageHeaderSize, true);

			DateTime xbeDate = convertWindowsDate(s.readIntLE());
			//Presumably, this is the same format as the timestamp in Windows PE executables
			info.addInfo("XBE date", xbeDate);
			info.addInfo("XBE year", xbeDate.Year);
			info.addInfo("XBE month", System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(xbeDate.Month));
			info.addInfo("XBE day", xbeDate.Day);

			int certificateOffset = s.readIntLE() - baseAddress;
			info.addInfo("Certificate offset", certificateOffset, ROMInfo.FormatMode.HEX, true);

			int sectionCount = s.readIntLE();
			int sectionsOffset = s.readIntLE() - baseAddress;
			info.addInfo("Number of sections", sectionCount, true);
			info.addInfo("Address of sections", sectionsOffset, ROMInfo.FormatMode.HEX, true);

			int initFlags = s.readIntLE();
			info.addInfo("Initialization flags", initFlags, true);

			int entryPoint = s.readIntLE();
			uint debugEntryPoint = (uint)(entryPoint ^ 0x94859d4b) - (uint)baseAddress;
			if(debugEntryPoint <= s.Length) {
				info.addInfo("Entry point", debugEntryPoint, ROMInfo.FormatMode.HEX, true);
				info.addInfo("Is debug", true);
			} else {
				//I'm gonna be real, all I have on me is homebrew and prototypes, so this could be all completely wrong
				uint retailEntryPoint = (uint)(entryPoint ^ 0xa8fc57ab) - (uint)baseAddress;
				info.addInfo("Entry point", retailEntryPoint, ROMInfo.FormatMode.HEX, true);
				info.addInfo("Is debug", false);
			}

			if(certificateOffset > 0 && certificateOffset < s.Length) {
				s.Position = certificateOffset;
				int certificateSize = s.readIntLE();
				info.addInfo("Certificate size", certificateSize, true);

				DateTime certDate = convertWindowsDate(s.readIntLE());
				info.addInfo("Date", certDate);
				info.addInfo("Year", certDate.Year);
				info.addInfo("Month", System.Globalization.DateTimeFormatInfo.CurrentInfo.GetMonthName(certDate.Month));
				info.addInfo("Day", certDate.Day);
				//Not sure what the difference is between this and the XBE date, but sometimes it's different, most of the time not
				//Anyway, when it is different it's always later, so let's go with this as the definitive date

				//Supposedly, this stuff is printed onto retail discs
				short titleID = s.readShortLE();
				info.addInfo("Title ID", titleID); //Could be a product code? I don't know, to be honest
				string maker = new string(s.read(2, Encoding.ASCII).ToCharArray().Reverse().ToArray()); //It's... backwards. Don't ask me why. Endian weirdness most likely
				info.addInfo("Publisher", maker, MicrosoftCommon.LICENSEE_CODES);

				string name = s.read(80, Encoding.Unicode);
				info.addInfo("Internal name", name);

				byte[] altTitleIDS = s.read(64);
				info.addInfo("Alt IDs", altTitleIDS, true); //Usually blank

				int allowedMedia = s.readIntLE();
				info.addInfo("Allowed on hard disk", (allowedMedia & 1) > 0, true);
				info.addInfo("Allowed on DVD X2", (allowedMedia & 2) > 0, true);
				info.addInfo("Allowed on DVD CD", (allowedMedia & 4) > 0, true);
				info.addInfo("Allowed on CD", (allowedMedia & 8) > 0, true);
				info.addInfo("Allowed on DVD", (allowedMedia & 16) > 0, true);
				info.addInfo("Allowed on DVD DL", (allowedMedia & 32) > 0, true);
				info.addInfo("Allowed on DVD-RW", (allowedMedia & 64) > 0, true);
				info.addInfo("Allowed on DVD-RW DL", (allowedMedia & 128) > 0, true);
				info.addInfo("Allowed on dongle", (allowedMedia & 256) > 0, true);
				info.addInfo("Allowed on media board", (allowedMedia & 512) > 0, true);

				int region = s.readIntLE();
				//TODO Make this look much nicer
				info.addInfo("Region", Enum.ToObject(typeof(XboxRegions), region).ToString());

				byte[] ratings = s.read(4);
				info.addInfo("Ratings", ratings, true);
				//This is where it'd be useful if I could dump a physical disk..
				//Metal Arms prototype has 30-00-00-00 here
				//farbrausch (demo by Limp Ninja) has 40-00-00-00 here
				//The rest I have are just prototypes with either 00-00-00-00 or FF-FF-FF-FF

				int discNumber = s.readIntLE();
				info.addInfo("Disc number", discNumber);

				int version = s.readIntLE();
				info.addInfo("Version", version);
			}

		}

		public override void addROMInfo(ROMInfo info, ROMFile file) {
			info.addInfo("Platform", "Xbox");
			if ("xbe".Equals(file.extension)) {
				parseXBE(info, file.stream);
			}
		}
	}
}
