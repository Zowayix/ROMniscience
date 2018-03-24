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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROMniscience.IO;

namespace ROMniscience.Handlers {
	//Mostly adapted from http://problemkaputt.de/gbatek.htm#gbacartridgeheader
	class GBA: Handler {
		public override IDictionary<string, string> filetypeMap => new Dictionary<string, string> {
			{"gba","Nintendo Game Boy Advance ROM" },
			{"bin","Nintendo Game Boy Advance ROM" },
			{"srl","Nintendo Game Boy Advance ROM" },
			{"mb","Nintendo Game Boy Advance multiboot ROM" },
		};
		public override string name => "Game Boy Advance";

		public static readonly IDictionary<char, string> GBA_GAME_TYPES = new Dictionary<char, string> {
			{'A', "Normal game (older)"},
			{'B', "Normal game (newer)"},
			{'C', "Normal game (unused)"}, //Why do I get the feeling it's not unused?
			{'F', "Famicom/Classic NES series"},
			{'K', "Acceleration sensor"}, //Yoshi's Universal Gravitation, Koro Koro Puzzle
			{'P', "e-Reader"},
			{'R', "Gyro sensor"}, //WarioWare: Twisted
			{'U', "Solar sensor"}, //Boktai: The Sun is in Your Hands
			{'V', "Rumble"}, //Drill Dozer
			{'M', "GBA Video"}, //Also used by mb2gba and any multiboot roms converted by it
			{'T', "Test cart"}, //AGS Aging Cartridge
			{'Z', "DS expansion"}, //Daigassou! Band-Brothers - Request Selection (it's just a slot 2 device for a DS game, but it has a
			//GBA ROM header surprisingly), also Nintendo MP3 Player which was marketed as being for the DS so maybe "DS expansion" isn't quite
			//the right name but it'll have to do
			//Have also seen J for the Pokemon Aurora Ticket distribution cart, and G for GameCube multiboot images (they just use the product code of the GameCube disc they were from usually)
		};

		public static readonly IDictionary<int, string> GBA_MULTIBOOT_MODES = new Dictionary<int, string> {
			{0, "Not multiboot"},
			{1, "Joybus"},
			{2, "Normal"},
			{3, "Multiplay"},
		};

		int calculateChecksum(InputStream f) {
			long origPos = f.Position;
			try {
				int x = 0;
				f.Position = 0xa0;
				while(f.Position <= 0xbc) {
					x = (x - f.read()) & 0xff;
				}
				return (x - 0x19) & 0xff;
			} finally {
				f.Position = origPos;
			}
		}

		readonly static byte[] SAPPY_SELECTSONG = {
			0x00, 0xB5, 0x00, 0x04, 0x07, 0x4A, 0x08, 0x49,
			0x40, 0x0B, 0x40, 0x18, 0x83, 0x88, 0x59, 0x00,
			0xC9, 0x18, 0x89, 0x00, 0x89, 0x18, 0x0A, 0x68,
			0x01, 0x68, 0x10, 0x1C, 0x00, 0xF0};

		const int GBA_LOGO_CRC32 = -0x2F414AA2;

		static bool isNintendoLogoEqual(byte[] nintendoLogo) {
			return Datfiles.CRC32.crc32(nintendoLogo) == GBA_LOGO_CRC32;
		}

		//There's no official way to detect the save type, but Nintendo's SDK ends up
		//putting these strings in the ROM according to what it uses, apparently
		readonly static byte[] EEPROM = Encoding.ASCII.GetBytes("EEPROM_V");
		readonly static byte[] SRAM = Encoding.ASCII.GetBytes("SRAM_V");
		readonly static byte[] SRAM_F = Encoding.ASCII.GetBytes("SRAM_F_V");
		readonly static byte[] FLASH = Encoding.ASCII.GetBytes("FLASH_V");
		readonly static byte[] FLASH_512 = Encoding.ASCII.GetBytes("FLASH512_V");
		readonly static byte[] FLASH_1024 = Encoding.ASCII.GetBytes("FLASH1M_V");
		//It also puts this in for games that use the real time clock
		readonly static byte[] RTC = Encoding.ASCII.GetBytes("SIIRTC_V");

		static void detectSaveType(ROMInfo info, byte[] bytes) { 
			if(ByteSearch.contains(bytes, EEPROM)) {
				info.addInfo("Save type", "EEPROM");
				//Can't tell the save size from this, it's either 512 or 8192 though
			} else if(ByteSearch.contains(bytes, SRAM) || ByteSearch.contains(bytes, SRAM_F)) {
				info.addInfo("Save type", "SRAM");
				info.addInfo("Save size", 32 * 1024, ROMInfo.FormatMode.SIZE);
			} else if(ByteSearch.contains(bytes, FLASH) || ByteSearch.contains(bytes, FLASH_512)) {
				info.addInfo("Save type", "Flash");
				info.addInfo("Save size", 64 * 1024, ROMInfo.FormatMode.SIZE);
			} else if(ByteSearch.contains(bytes, FLASH_1024)) {
				info.addInfo("Save type", "Flash");
				info.addInfo("Save size", 128 * 1024, ROMInfo.FormatMode.SIZE);
			}
		}

		public override void addROMInfo(ROMInfo info, ROMFile file) {
			info.addInfo("Platform", name);
			InputStream f = file.stream;

			byte[] entryPoint = f.read(4);
			info.addInfo("Entry point", entryPoint, true);
			byte[] nintendoLogo = f.read(156);
			info.addInfo("Nintendo logo", nintendoLogo, true);
			info.addInfo("Nintendo logo valid?", isNintendoLogoEqual(nintendoLogo));
			//TODO: Bits 2 and 7 of nintendoLogo[0x99] enable debugging functions when set (undefined instruction exceptions are sent
			//to a user handler identified using the device type)
			//0x9b bits 0 and 1 also have some crap in them but I don't even know

			string title = f.read(12, Encoding.ASCII).TrimEnd('\0');
			info.addInfo("Internal name", title);
			string gameCode = f.read(4, Encoding.ASCII);
			info.addInfo("Product code", gameCode);
			char gameType = gameCode[0];
			info.addInfo("Type", gameType, GBA_GAME_TYPES);
			string shortTitle = gameCode.Substring(1, 2);
			info.addInfo("Short title", shortTitle);
			char region = gameCode[3];
			info.addInfo("Region", region, NintendoCommon.REGIONS);

			string makerCode = f.read(2, Encoding.ASCII);
			info.addInfo("Manufacturer", makerCode, NintendoCommon.LICENSEE_CODES);
			int fixedValue = f.read();
			info.addInfo("Fixed value", fixedValue, true);
			info.addInfo("Fixed value valid?", fixedValue == 0x96);

			//This indicates the required hardware, should be 0 but it's possible that
			//some prototype/beta/multiboot/other weird ROMs have something else
			int mainUnitCode = f.read();
			info.addInfo("Main unit code", mainUnitCode);

			//If bit 7 is set, the debugging handler entry point is at 0x9fe2000 and not 0x9ffc000, normally this will be 0
			int deviceType = f.read();
			info.addInfo("Device type", deviceType);

			byte[] reserved = f.read(7); //Should be all 0
			info.addInfo("Reserved", reserved, true);
			int version = f.read();
			info.addInfo("Version", version);
			int checksum = f.read();
			info.addInfo("Checksum", checksum, true);
			int calculatedChecksum = calculateChecksum(f);
			info.addInfo("Calculated checksum", calculatedChecksum, true);
			info.addInfo("Checksum valid?", checksum == calculatedChecksum);
			byte[] reserved2 = f.read(2);
			info.addInfo("Reserved 2", reserved2, true);
			byte[] multibootEntryPoint = f.read(4);
			info.addInfo("Multiboot entry point", multibootEntryPoint, true);
			int multibootMode = f.read();
			info.addInfo("Multiboot mode", multibootMode, GBA_MULTIBOOT_MODES);
			int multibootSlaveID = f.read();
			info.addInfo("Multiboot slave ID", multibootSlaveID);
			//0xe0 contains a joybus entry point if joybus stuff is set, meh

			byte[] restOfCart = f.read((int)f.Length);
			detectSaveType(info, restOfCart);
			info.addInfo("Has RTC", ByteSearch.contains(restOfCart, RTC));
			info.addInfo("Sound driver", ByteSearch.contains(restOfCart, SAPPY_SELECTSONG) ? "Sappy" : "Unknown");
			//TODO Krawall is open source, see if we can detect that
		}
	}
}
