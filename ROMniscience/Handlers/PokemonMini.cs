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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ROMniscience.IO;

namespace ROMniscience.Handlers {
	class PokemonMini: Handler {
		//Adapted mostly from http://www.pokemon-mini.net/documentation/cartridge/

		public static readonly IDictionary<char, string> GAME_TYPES = new Dictionary<char, string> {
			{'M', "Game"}, //That's all that's used for the few official games released
			{'K', "Prototype"}, //There's a prototype that's been seen that includes a cart that hasn't been dumped, but it has MIN-KCFO-01 as the serial
		};

		public override IDictionary<string, string> filetypeMap => new Dictionary<string, string>() {
			{"min", "Pokémon Mini ROM"}
		};

		public override string name => "Pokemon Mini";

		public override void addROMInfo(ROMInfo info, ROMFile file) {
			info.addInfo("Platform", "Pokemon Mini");

			WrappedInputStream s = file.stream;
			s.Position = 0x2100;

			string marker = s.read(2, Encoding.ASCII);
			info.addInfo("Marker", marker, true);

			info.addInfo("Entry point", s.read(6), ROMInfo.FormatMode.HEX, true);
			//What the heck is all this
			info.addInfo("PRC frame copy IRQ", s.read(6), true);
			info.addInfo("PRC render IRQ", s.read(6), true);
			info.addInfo("Timer 2 underflow upper IRQ", s.read(6), true);
			info.addInfo("Timer 2 underflow lower IRQ", s.read(6), true);
			info.addInfo("Timer 1 underflow upper IRQ", s.read(6), true);
			info.addInfo("Timer 1 underflow lower IRQ", s.read(6), true);
			info.addInfo("Timer 3 underflow upper IRQ", s.read(6), true);
			info.addInfo("Timer 3 comparator IRQ", s.read(6), true);
			info.addInfo("32Hz timer IRQ", s.read(6), true);
			info.addInfo("8Hz timer IRQ", s.read(6), true);
			info.addInfo("2Hz timer IRQ", s.read(6), true);
			info.addInfo("1Hz timer IRQ", s.read(6), true);
			info.addInfo("IR receiver IRQ", s.read(6), true);
			info.addInfo("Shake sensor IRQ", s.read(6), true);
			info.addInfo("Power key IRQ", s.read(6), true);
			info.addInfo("Right key IRQ", s.read(6), true);
			info.addInfo("Left key IRQ", s.read(6), true);
			info.addInfo("Down key IRQ", s.read(6), true);
			info.addInfo("Up key IRQ", s.read(6), true);
			info.addInfo("C key IRQ", s.read(6), true);
			info.addInfo("B key IRQ", s.read(6), true);
			info.addInfo("A key IRQ", s.read(6), true);
			info.addInfo("Unknown IRQ 1", s.read(6), true);
			info.addInfo("Unknown IRQ 2", s.read(6), true);
			info.addInfo("Unknown IRQ 3", s.read(6), true);
			info.addInfo("Cartridge IRQ", s.read(6), true);

			string headerMagic = s.read("NINTENDO".Length, Encoding.ASCII);
			info.addInfo("Magic", headerMagic, true); //Should be "NINTENDO"

			string productCode = s.read(4, Encoding.ASCII);
			info.addInfo("Product code", productCode);

			char gameType = productCode[0];
			info.addInfo("Type", gameType, GAME_TYPES);
			string shortTitle = productCode.Substring(1, 2);
			info.addInfo("Short title", shortTitle);
			char country = productCode[3];
			info.addInfo("Country", country, NintendoCommon.COUNTRIES);

			//All the Japanese exclusive games use some kind of JIS (maybe the Japanese versions of worldwide games do too)
			string title = s.read(12, MainProgram.shiftJIS).TrimEnd('\0', ' ');
			info.addInfo("Internal name", title);

			string manufacturer = s.read(2, Encoding.ASCII);
			//This will always be 2P (The Pokemon Company) but still
			info.addInfo("Publisher", manufacturer, NintendoCommon.LICENSEE_CODES);

			byte[] reserved = s.read(18);
			info.addInfo("Reserved", reserved, true); //Should be 0 filled
		}
	}
}
