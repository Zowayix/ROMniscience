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
 *
 * TODO: Does that restrict anyone from using the various dictionaries as documentation (I'd rather it not), and not just the actual code? I hate that I'm not a lawyer
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMniscience.Handlers {
	class NintendoCommon {

		public static readonly IDictionary<char, string> COUNTRIES = new Dictionary<char, string> {
			//Used by GBC, GBA, Gamecube/Wii, Pokemon Mini, SNES (with product codes, not the other one in the ROM header), Virtual Boy, N64, DS, and WiiWare
			{'A', "Worldwide"}, //Sometimes means Japan + USA in the case of N64 and maybe some GBC games, but generally means something among the lines of "more than one country", and in newer systems means region-free
			{'B', "Brazil"},
			{'C', "China"},
			{'D', "Germany"},
			{'E', "USA"}, //Also includes Latin America
			{'F', "France"},
			{'G', "Greece"},
			{'H', "Netherlands"},
			{'I', "Italy"},
			{'J', "Japan"},
			{'K', "Korea"},
			{'L', "Canada"},
			{'M', "Sweden"},
			{'N', "Norway"},
			{'O', "International"}, //Basically every region except Japan (or is it except China?); only seen on DSi (and Pokemon gen 5 games at that). Maybe it's actually just USA + Europe + Australia (but those games are completely region free, except maybe not on the iQue thingy)
			{'P', "Europe"}, //Also includes UK and Australia; though for DSi it seems to mean "Europe and not Australia", since Australia is separate there
			{'Q', "Denmark"},
			{'R', "Russia"},
			{'S', "Spain"},
			{'T', "USA + Australia"}, //DSi only
			{'U', "Australia"}, //Usually P or X is used to mean Europe + Australia, but there are a few exclusives
			{'V', "Europe + Australia"}, //DSi only, otherwise it would just be P/X/Y/Z
			{'W', "Taiwan"},
			{'X', "Europe (X)"}, //Hmm... some Canada and Latin America Wii releases have this. It might just mean "region not specified", but generally means Europe
			{'Y', "Europe (Y)"}, //Not seen very often... might mean "the part of Europe that doesn't speak English", as it seems to show up in European releases of games where English is not one of the languages
			{'Z', "Europe (Z)"}, //Seen even less often, and even more confusing. Only seen in obscure Wii releases and DS promotional carts (Europe/Australia Pokemon distributions, basically)
		};

		public static IDictionary<char, string> DISC_TYPES => new Dictionary<char, string> {
			//https://wiki.dolphin-emu.org/index.php?title=GameIDs
			{'G', "GameCube game"},
			{'C', "Commodore 64 Virtual Console"},
			{'D', "GameCube demo disc"}, //Also OoT: Master Quest
			{'E', "Arcade/Neo Geo Virtual Console"},
			{'F', "NES Virtual Console"},
			{'H', "Wii channel"},
			{'J', "SNES Virtual Console"},
			{'L', "Sega Master System Virtual Console"},
			{'M', "Megadrive Virtual Console"},
			{'N', "N64 Virtual Console"},
			{'P', "GameCube promo or PC Engine Virtual Console"},
			{'Q', "TurboGrafx-CD Virtual Console"},
			{'R', "Wii game"},
			{'S', "Wii game (newer)"},
			{'U', "GameCube utility"}, //Used for GBA Player startup disc
			{'W', "WiiWare game"},
			{'X', "WiiWare demo or MSX Virtual Console"},
		};

		public static readonly IDictionary<int, string> REGIONS = new Dictionary<int, string>() {
			{0, "NTSC-J"},
			{1, "NTSC"},
			{2, "PAL"},
			{3, "Region free"},
			{4, "NTSC-K"},
		};

		public static readonly IDictionary<String, String> LICENSEE_CODES = new Dictionary<String, String>() {
			//Big list! Used in every Nintendo system except: NES (but FDS does use it), N64 (but 64DD does use it), e-Reader, and depending on who you ask Pokemon Mini either has 2P for every game (i.e. if you ask me) or it doesn't use this. Well, I presume Switch does in some way; 3DS seems to be a bit weird, and Wii U uses 4 characters so I guess they ran out of codes with all the indie publishers (most just use this list prepended with 00)
			//Preferred format that I probably don't always use consistently:
			//Company without Co, Inc, Ltd, etc
			//Company (Brand name used by company)
			//Company A / Company B which purchased A or A otherwise got absorbed into
			//Company A / New name of Company A
			//[Company B that merged with company A but kept A's licensee code] Company A e.g. [Bandai] Namco (Namco used the code AF before the merger)

			//These are invalid, honestly, but eh
			{"00", "Nobody"}, //Probably homebrew; although... some 3DS games use this? What the heck, I don't think Smash 4 was published by nobody
			{"  ", "Nobody"},
			{"\0\0", "Nobody"},
			{"##", "Nobody"},
			{"??", "Nobody"},

			//These are definitely what they are, appears in multiple games that are definitely published by that thing
			{"01", "Nintendo"},
			{"08", "Capcom"},
			{"0A", "Jaleco"},
			{"0B", "Coconuts Japan"},
			{"13", "Electronic Arts Victor"}, //This only appears in Japanese games by EA, like Sommelier DS, or the Japanese version of Harry Potter and the Philosopher's Stone for GBC; every other EA game uses the sex number. Could this be that EA/Square joint thing in modern times?
			{"18", "Hudson Soft"},
			{"1F", "Virgin Japan"}, //Unsure of exact name but it's Virgin but Japanese versions of games. Also found in the Muhammad Ali Heavyweight Boxing prototype (SNES)
			{"1M", "Micro Cabin"},
			{"1Q", "TDK Core"}, //TDK's Japanese game publishing branch
			{"29", "Seta"},
			{"2L", "Tamsoft"},
			{"2N", "Smilesoft"},
			{"2P", "The Pokémon Company"},
			{"30", "Viacom"},
			{"36", "Codemasters"},
			{"39", "Event Evolution Entertainment"},
			{"41", "Ubisoft"},
			{"46", "System 3"}, //The old one
			{"49", "Irem"},
			{"4B", "Raya Systems"},
			{"4S", "Black Pearl Software"},
			{"4Y", "Rare"},
			{"4Z", "Crave Entertainment"}, //Resident Evil GBC prototype also has this, but it was developed by HotGen and would have been published by Capcom
			{"50", "Absolute Entertainment"},
			{"51", "Acclaim"},
			{"52", "Activision"},
			{"55", "Hi-Tech Expressions"},
			{"56", "LJN"},
			{"58", "Mattel"},
			{"5A", "Mindscape"},
			{"5F", "American Softworks"},
			{"5G", "Majesco"},
			{"5H", "The 3DO Company"},
			{"5K", "Hasbro"},
			{"5L", "NewKidCo"},
			{"5T", "Cryo Interactive"},
			{"5X", "Microids"},
			{"60", "Titus"},
			{"61", "Virgin"},
			{"64", "LucasArts"},
			{"67", "Ocean"},
			{"69", "Electronic Arts"}, //nice
			{"6B", "Laser Beam Entertainment"}, //Publishing arm of Beam Software
			{"6F", "Electro Brain"},
			{"6H", "BBC Multimedia"},
			{"6J", "Software 2000"},
			{"6L", "BAM! Entertainment"},
			{"6M", "System 3"}, //The new one. What the fuck? They didn't even change the spelling or anything, it really is the same company. Gee System 3! How come your mum lets you eat _two_ weiners?
			{"6S", "TDK Mediactive"}, //TDK's Not-Japan publishing branch
			{"6V", "JoWooD Entertainment"},
			{"71", "Interplay"},
			{"72", "JVC"},
			{"75", "The Sales Curve"}, //AKA Sales Curve Interactve or SCi (short for Sales Curve with lowercase interactive for some reason)
			{"78", "THQ"},
			{"79", "Accolade"},
			{"7C", "Microprose"}, //Seen in Tinhead (Europe) (Proto) (SNES) and F-15 Strike Eagle II (Unknown) (Proto) (Game Boy); which are both prototypes and so therefore unreliable, but they both are indeed by Microprose so this is probably correct
			{"7F", "Kemco"},
			{"7G", "Rage Software"},
			{"7L", "Simon & Schuster"},
			{"82", "Namcot"}, //Some division of Namco that was apparently necessary to create as a separate thing.... I don't know
			{"8B", "Bullet-Proof Software"},
			{"8E", "Character Soft"},
			{"8F", "I'Max"},
			{"8P", "Sega"},
			{"91", "Chunsoft"},
			{"93", "Bec"},
			{"95", "Varie"},
			{"97", "Kaneko"},
			{"9A", "Nichibutsu"},
			{"9B", "Tecmo"},
			{"9C", "Imagineer"},
			{"9H", "Bottom Up"},
			{"9M", "Jaguar"}, //The sewing machine company
			{"9N", "Marvelous Entertainment"}, //Before merger in 2003
			{"A0", "Telenet"}, //Sometimes known as Telenet Japan, or Nippon Telenet, but I think it's fair to just call it "Telenet"
			{"A4", "Konami"},
			{"A5", "K Amusement Leasing"},
			{"A7", "Takara"}, //Vast Fame also uses this for most of their GBC bootlegs, interestingly
			{"A8", "Royal Industries"}, //More damn sewing machine companies!
			{"AD", "Toho"},
			{"AH", "J-Wing"},
			{"AL", "Media Factory"},
			{"B0", "Acclaim Japan"},
			{"B1", "ASCII"},
			{"B2", "Bandai"},
			{"B3", "Soft Pro"},
			{"B4", "Enix"},
			{"B6", "HAL"},
			{"BA", "Culture Brain"},
			{"BB", "Sunsoft"},
			{"BC", "Toshiba EMI"},
			{"BD", "Sony Imagesoft"},
			{"BJ", "Compile"},
			{"BL", "MTO"},
			{"C0", "Taito"},
			{"C1", "Sunsoft (Chinou Game Series)"}, //I guess it's an educational branch of Sunsoft? Or it might all be published by Ask Odansha
			{"C3", "Squaresoft"},
			{"C5", "Data East"},
			{"C6", "Tonkin House"},
			{"C8", "Koei"},
			{"CB", "Vap"},
			{"CF", "Angel"},
			{"CP", "Enterbrain"},
			{"D1", "Sofel"},
			{"D9", "Banpresto"},
			{"DA", "Tomy"},
			{"DD", "Masaya"}, //Brand that Nippon Computer Systems uses to distribute games... maybe more adequately called that?
			{"DE", "Human"}, //Or Human Entertainment
			{"DF", "Altron"}, //mb2gba uses this too for som reason, as well as a few GBA bootlegs
			{"E2", "Yuutaka"}, //Or is it spelled Yutaka?
			{"E4", "T&E Soft"},
			{"E5", "Epoch"},
			{"E7", "Athena"},
			{"E9", "Natsume"},
			{"EB", "Atlus"},
			{"FJ", "Virtual Toys"},
			{"FQ", "iQue"}, //Sort of. Only WarioWare: Touched, Polarium, and Yoshi: Touch & Go use this, the other three iQue games: New Super Mario Bros, Super Mario 64, and Nintendogs (kiosk demo) use Nintendo instead. Those were all 2007 or later, the latter being on the iQue DSi, so it may be something corporate related going on there
			{"FR", "Digital Tainment Pool"}, //Or DTP Entertainment if you prefer
			{"FT", "Daiwon C&A Holdings"},
			{"GD", "Square Enix"},
			{"GL", "Gameloft"},
			{"HF", "Level5"},
			{"JS", "Digital Leisure"},
			{"KR", "Krea Medie"},
			{"KM", "Deep Silver"},
			{"RW", "RealNetworks"}, //Or GameHouse, or Real Arcade; which are sorta subsidiaries I guess
			{"WY", "WayForward"},
			{"XN", "Netflix"},

			//Mostly verified, just that companies buy each other out and change their names and things, so these are a bit weird and dependent on what year it is
			{"4F", "U.S. Gold/Eidos"}, //Eidos purchased U.S. Gold in 1996; also this appears in the Gauntlet DS proto, but that wasn't going to be published by Eidos nor did they develop it
			{"5D", "Midway/Tradewest/Williams"}, //For all intents and purposes starts off as Bally Midway (it doesn't, but let's not care about pre-1969), purchased by Williams in 1988 (but as "Bally/Midway"), albeit in 1991 started using the name Midway alone, then in 1996 some screwy stuff happened involving WMS and Time Warner, becomes independent in 1998 (but keeps the subsidiary known as "Atari Games" just to confuse me further until 2000) and then now they're sold to Warner Bros and then there's Tradeswest in there somewhere but only up to 1994? Aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
			{"70", "Infogrames/Atari, SA"}, //Infogrames renamed itself to Atari or purchased the brand name or something in 2003 because they could, but are otherwise unrelated to the OG Atari, also purchased and sold off all sorts of companies along the way (e.g. GT Interactive in 1999, Hasbro Interactive in 2001)
			{"7D", "Vivendi/Sierra"}, //This is weird, because Sierra was merged with Activision in 2010, who at the time were owned by Vivendi, but now they aren't and then they revived the Sierra brand name in 2014. So that's confusing. One of these boxes I have lying around containing a game with this licensee code says Sierra on it, and that was... 2008? So I dunno
			{"7J", "Zoo Digital Publishing"}, //AKA Zushi Games. See 20 for rambling
			{"99", "Pack-in Video/Victor Interactive/Marvelous Interactive"}, //Merged with Victor in 1997 and then with Marvelous in 2003, so Victor and Marvelous probably used different codes before then (or weren't involved with Nintendo)
			{"AF", "[Bandai] Namco"}, //Namco games have always used this, but when they merged with Bandai (Sep 2005), they kept this code (so newer games like that Code Geass DS one use this code as well), which is interesting because Square Enix doesn't reuse Squaresoft or Enix's licensee codes and gets a new one

			//Welcome to confusing town, population these
			{"20", "Destination Software"}, //This is a confusing load of fuck, and I hate this entire company. So basically you also have 7J, and at first you'd think 20 is the UK/Europe Zoo Digital Publishing (aka Zoo Games) (aka Zushi Games), and 7J is the USA Destination Software (aka DSI Games). But then that turns out to not be the case. They're sort of the same company and have been traded around and bought out and sold multiple times. Thanks I hate it.
			{"4Q", "Disney/Buena Vista Interactive"}, //What's maximum spooky is that this also shows up in the Spiderman: Friend or Foe trailer, but that was 2007 and Disney hadn't purchased Marvel yet. Disney Interactive Studios spun off Buena Vista Interactive from 2003 to 2007 but that still doesn't explain Spiderman. It's not in any of the other DS downloadable videos. What the crap?
			{"54", "Take-Two Interactive"}, //Oof this one's a really fun one. Some documentation says "Konami/GameTek" which is just garbage because Konami has nothing to do with this, but some older games using this code like Wheel of Fortune on SNES and the InfoGenius Productivity Pak on GB are indeed published under GameTek. Where it gets fun is that GameTek became Take-Two Interactive at some point, and Duke Nukem Advance uses this code too and was published under the Take-Two Interactive name.. but then Dora the Explorer: Dora's World Adventures also uses this code but was published under Global Star Software, which was a company that became 2K Play which is then a branch of 2K Games and if you've lost track of everything I don't blame you and I just spent several tens of minutes googling around for info on Dora the Explorer why do I do this to myself
			//Anyway this one just seems to cover everything that's owned by Take-Two Interactive
			{"5Z", "Classified Games"}, //Also appears in the Card Shark (SNES) proto, which as far as I can tell was developed by someone named Bonsai and involved someone named Bicycle as well. Seems like there's a mixup here with Conspiracy Entertainment? Are they the same company?
			{"AC", "Toei Animation"}, //Used in some EXTREMELY obscure "Waiwai Check" games for Satellaview, which according to the title screen of one of them, are made by Hori Electric? Well I'm confused; Tooyama no Kinsan Space Chou - Mr. Gold is actually published by Toei though (apparently)
			{"CA", "Konami (Ultra Games)"}, //Definitely Konami but might not be Ultra Games? Published Batman Returns SNES in Japan (but not elsewhere), and Parodius (Europe) for Game Boy
			{"CE", "Pony Canyon/FCI"}, //Seems to be Fujisankei Communications International (as FCI Inc) for USA releases, Pony Canyon for Japanese releases, both are owned by Fujisankei Communications Group

			//So far, only seen once (in a reliable source, i.e. not protos or anything like that)
			{"09", "Hot-B"}, //Ginga - Card & Puzzle Collection (Japan) (En,Ja)
			{"19", "Bandai (B-AI)"}, //Pingu - Sekai de 1ban Genki na Penguin (Japan)
			{"1P", "Creatures"}, //Chee-Chai Alien (Japan) (GBC)
			{"35", "Hector"}, //California Games II (USA) (might be just Hect?) (game was published by DTMC in USA and Hect in Japan, but DTMC seems to be related to Hect and may be a subdivision/branch?)
			{"47", "Spectrum Holobyte"}, //Star Trek: The Next Generation: Future's Past (USA) (SNES)
			{"4A", "Gakken"}, //Titanic Mystery - Ao no Senritsu
			{"53", "American Sammy"}, //Ys III (SNES); also Ninja Taro (USA) (Beta) (GB); dunno what the difference is between this and non-American Sammy
			{"5Q", "Lego"}, //Lego Island 2: The Brickster's Revenge
			{"6E", "Elite Systems"}, //Might and Magic II (Europe) (SNES)
			{"6P", "Ravensburger Interactive"}, //Fix & Foxi - Episode 1 - Lupo (Europe) (En,Fr,De) (GBC)
			{"6R", "Sound Source"}, //Extreme Sports with the Berenstain Bears (USA, Europe) (En,Fr,De,Es,It) (GBC)
			{"8C", "Vic Tokai"}, //Zerd no Densetsu (Japan)
			{"8N", "Success"}, //Minna no Soft Series - Tetris Advance (Japan)
			{"A6", "Kawada"}, //Othello (FDS)
			{"A9", "Technos Japan"}, //Shin Nekketsu Kouha - Kunio-tachi no Banka (Japan) [T-En by Aeon Genesis v1.00] (SNES)
			{"BF", "Sammy"}, //Gindama Oyakata no Pachinko Hisshouhou (Japan)
			{"C7", "East Cube"}, //Reflect World (FDS) (did they even make any other games?)
			{"CU", "Marvelous AQL"}, //Senran Kagura Burst (Europe) (3DS)
			{"D4", "Ask Kodansha"}, //Magna Braban - Henreki no Yuusha
			{"D6", "Naxat Soft"}, //Spriggan Powered BS - Prelude
			{"DB", "Hiro"}, //Daisenryaku (Japan) (Gameboy)
			{"E1", "Towa Chiki"}, //Taikyoku Renju
			{"E8", "Asmik"}, //Dokapon Gaiden - Honoo no Audition (Japan) (merged with Ace to become Asmik Ace in 1998)
			{"EA", "King Records"}, //Azumanga Daioh Advance
			{"EC", "Epic / Sony Records"}, //Jerry Boy (SNES)
			{"EE", "Information Global Services"}, //Super Shadow of the Beast (USA) (Proto) (SNES)
			{"F3", "Extreme Entertainment"}, //Super Solitaire (USA) (En,Fr,De,Es,It) (SNES)
			{"FE", "Davidson & Associates"}, //Math Blaster - Episode 1 (USA) (SNES)
			{"GN", "OG International"}, //Cartoon Network Punch Time Explosion XL (Europe) (Wii)
			{"GY", "The Game Factory"}, //Code Lyoko (USA) (DS) (might be "The American Game Factory", both names are used on the box so I dunno)
			{"H4", "SNK Playmore"}, //Doki Doki Majo Shinpan (DS)
			{"HL", "Frontier Developments"}, //LostWinds (USA) (WiiWare)
			{"HN", "High Voltage"}, //VIP Casino Blackjack (USA) (WiiWare)
			{"JT", "Empty Clip Studios"}, //Groovin' Blocks (USA) (WiiWare) (retail release is by Zoo)
			{"JY", "Steel Penny"}, //Bruiser and Scratch (USA) (WiiWare)
			{"NR", "Destineer"}, //Giana Sisters DS (USA)
			{"PV", "Pan Vision Games"}, //Miffy's World (Europe) (DS)
			{"RG", "Romino Games"}, //Swords & Soldiers II (Wii U)
			{"S3", "Genius Sonority"}, //The Denpa Men demo (3DS)
			{"TL", "Telltale"}, //Strong Badia the Free series (WiiWare)
			{"QU", "QubicGames"}, //AiRace Speed demo (3DS)
			{"UZ", "Circle Entertainment"}, //Sweet Memories: Blackjack (3DS)
			{"WR", "Warner Bros"}, //Scribblenauts and Super Scribblenauts (Europe) (DS) (or Warner Bros Interactive Entertainment if you prefer to be overly specific)
			{"YV", "KnapNok Games"}, //Affordable Space Adventures (Wii U)

			//So far only seen once, but probably only would ever be seen once
			{"62", "Maxis"}, //SimAnt - The Electronic Ant Colony (USA) (SNES) (only release they ever published themselves as far as Nintendo platforms are concerned)
			{"9Z", "Crunchyroll"}, //Crunchyroll app on both Wii and Wii U, so I guess that's published by Crunchyroll, well the Wii U version's metadata says so anyway
			{"WB", "Google"}, //YouTube app on Wii and Wii U
			{"WD", "Amazon"}, //Amazon Instant Video (WiiWare)
			{"XB", "Hulu"}, //Hulu Plus (WiiWare)

			//Only seen once and seems incorrect
			{"AA", "Broderbund"}, //Only seen in Rally - The Final Round of the World Rally Championship (USA) (Proto), which was by JVC although MAME software list says Europress
			{"D2", "Bothtec / Quest"}, //Only seen in Wakusei Aton Gaiden, which is... apparently published by the Japanese tax agency, so maybe this is wrong

			//Unverified, I guess I just found some other list somewhere and I forgot by now whoops. So they could be wrong, but probably aren't
			{"1A", "Yanoman"}, //Although seen in Aretha II - Ariel no Fushigi na Tabi (Japan) (Beta 1) (SNES), indeed by Yanoman
			{"1D", "Clary"},
			{"24", "PCM Complete"},
			{"33", "Ocean/Acclaim"}, //This is an important one, because if this is the old licensee code it means it uses
			//the new licensee code instead (and the extended header in SNES), and also it needs to be 33 for Super Gameboy functions to work on GB
			//Is this even valid anyway? Ocean already has 67 and probably just uses Acclaim's licensee when it got purchased
			{"38", "Hudson/Capcom"}, //Hudson was not, in fact, bought out by Capcom
			{"3E", "Gremlin"},
			{"44", "Malibu"},
			{"57", "Matchbox"},
			{"59", "Milton Bradley"},
			{"5B", "Romstar"},
			{"73", "Sculptured Software"}, //Possibly wrong as I've only seen it in Monopoly SNES and Clue SNES (only developed by them, published by Parker Bros), other games developed by them like Doom or The Simpsons: Bart's Nightmare use their publisher's code as you'd expect
			{"7A", "Triffix Entertainment"},
			{"80", "Misawa Entertainment"},
			{"83", "LOZC"},
			{"87", "Tsukuda Ori"},
			{"92", "Video System"}, //F-1 World Grand Prix (Europe) (Beta) (Game Boy) (N64 version was by Video System, not sure about this one)
			{"9F", "Nova"},
			{"A1", "Hori Electric"},
			{"A2", "Scorpion Soft"},
			{"B7", "SNK"},
			{"C9", "UFL"},
			{"CC", "Use"},
			{"CD", "Meldac"}, //Wordtris (USA) (Beta) (Game Boy) (final Wordtris was by Sphere, not Meldac)
			{"D3", "Sigma Enterprises"},
			{"D7", "Copya Systems"},
			{"F0", "A Wave"},

			//Duplicates, may be dubious but some really are used with two different codes and what the heck?
			{"C2", "Kemco (C2)"}, //Already uses 7F; not sure what's going on here. This is used by Electrician and Roger Rabbit for FDS, 7F is used by Top Gear 3000, Daikatana GBC and Virtual League Baseball, could be C2 is Kemco Japan and 7F is Kemco Not-Japan? Also used for The Sword of Hope for GBC which was published by Seika
			{"28", "Kemco (28)"}, //Used in Virtual Pro Yakyuu 98 (VB)
			{"86", "Tokuma Shoten"}, //Only seen in Madou Monogatari SNES (well the fan translation, actually... they wouldn't change it, right?)
			{"C4", "Tokuma Shoten Intermedia"}, //Possibly the more correct one, used in all the other games
			{"42", "Sunsoft (42)"}, //Only seen in Project S-11 which is indeed Sunsoft (also some cheeky bugger homebrew games that want to use 42), all other Sunsoft games use BB, even others on GBC from that year in that country

			//Duplicates that haven't even been seen anywhere (reliable) and hence are very dubious
			{"32", "Bandai (32)"}, //Only seen in the Picachu bootleg for SNES, other Bandai games use B2
			{"9D", "Banpresto (9D)"},
			{"0C", "Elite Systems (0C)"},
			{"E0", "Jaleco (E0)"}, //Already uses 0A
			{"34", "Konami (34)"}, //Already uses A4
			{"4D", "Malibu (4D)"}, //Already uses 44 (supposedly)
			{"5C", "Naxat Soft (5C)"},//Already uses D6
			{"31", "Nintendo (31)"}, //Ehhh?? I've only seen this used in Heisei Gunjin Shougi for Satellaview, which was developed by some company called
			//Carrozzeria Japan apparently, which might be some brand used by Pioneer that's mostly used for car radios? I don't even know
			{"B9", "Pony Canyon (B9)"},
			{"37", "Taito (37)"}, //Already uses C0
			{"D0", "Taito (D0)"},
			{"E3", "Varie (E3)"}, //Already uses 95

			//Are these names even correct? Probably not
			{"22", "pow"},
			{"3C", "Entertainment I"}, //ndustries?
			{"25", "san-x"},
			{"96", "Yonezawa/s''pal"},

			//FF is 100% junk; it'll basically only show up when the rest of the header is junk (unlicensed, or SNES game with incorrectly detected header)
		};

		public readonly static IDictionary<string, string> EXTENDED_LICENSEE_CODES = new Dictionary<string, string>() {
			//Seen on Wii U, which has four character licensee codes, but if they start with 00 they're just the old list of licensee codes (with the 00 at the front). So I guess they ran out of licensee codes at that point, huh? There's like, 1296 you can do with 2 alphanumeric characters. 1292 as 00, 33, FF, and ZZ would be probably invalid. So like... damn. I guess that's a thing that happened. Welp.
			{"010P", "13AM Games"},
			{"0167", "GalaxyTrail"},
		};

		//Is 0 = no rating correct for any of these, given that it's not no rating for the AGCB?
		public readonly static IDictionary<int, string> CERO_RATINGS = new Dictionary<int, string>() {
			//Used in Japan
			{0, "A"},
			{12, "B (12)"},
			{15, "C (15)"},
			{17, "D (17)"},
			{18, "Z (18)"},
			//There may be also some educational/utility/otherwise not-really-game software that has a
			//"Education/Database" rating
		};

		public readonly static IDictionary<int, string> ESRB_RATINGS = new Dictionary<int, string>() {
			//Used in USA, Canada, and Mexico
			{0, "No rating"}, //Shouldn't really exist, but it's used in Nintendo DSi Demo Video (which wasn't really sold) and Photo Channel and Wii Shop Channel (which were pre-installed)
			{3, "Early Childhood"},
			{6, "Everyone"},
			{10, "E10+"},
			{13, "Teen"},
			{17, "Mature"},
			//In theory there is 18 = Adults Only but I don't think any game like that would appear on these systems
		};

		public readonly static IDictionary<int, string> USK_RATINGS = new Dictionary<int, string>() {
			//Used in Germany
			{0, "No age restriction"},
			{6, "6+"},
			{12, "12+"},
			{16, "16+"},
			{18, "18+"},
		};

		public readonly static IDictionary<int, string> PEGI_RATINGS = new Dictionary<int, string>() {
			//Used in Europe, India, Pakistan, and Israel
			{0, "No rating"}, //In theory this doesn't exist (there isn't any PEGI rating label which corresponds to all ages including 2 and under), but Art Academy and Flipnote Studio use it?
			{3, "3+"},
			{7, "7+"},
			{12, "12+"},
			{16, "16+"},
			{18, "18+"},
		};

		public readonly static IDictionary<int, string> PEGI_PORTUGAL_RATINGS = new Dictionary<int, string>() {
			//Portugal decided to change 3 to 4 and 7 to 6 to align with the film rating system, so they end up being their own thing
			{0, "No rating"},
			{4, "4+"},
			{6, "6+"},
			{12, "12+"},
			{16, "16+"},
			{18, "18+"},
		};

		public readonly static IDictionary<int, string> PEGI_UK_RATINGS = new Dictionary<int, string>() {
			//No informational comment here, I'm just as confused as to what's going on as you are, or possibly more so
			{0, "No rating"},
			{3, "3+"},
			{4, "4+/U"},
			{7, "7+"},
			{8, "8+/PG"},
			{12, "12+"},
			{15, "15+"},
			{16, "16+"},
			{18, "18+"},
		};

		public readonly static IDictionary<int, string> AGCB_RATINGS = new Dictionary<int, string>() {
			//Used in Australia
			{0, "G"},
			{7, "PG"},
			{8, "PG (8)"}, //Seen on Wii U games
			{13, "M (13)"}, //Seen on Wii U games. I know it's definitely correct at least, because Earthbound has this and definitely says M when you start it up (which I will always find funny)
			{14, "M"},
			{15, "MA15+"},
			{18, "R"}, //Not introduced until 2013, all the Wii games released after that are not the kind of game which would earn such a rating
			//Not sure if there are games marked Exempt From Classification and how that would work
			//Back in my days there was a G8+ instead of PG
		};

		public readonly static IDictionary<int, string> GRB_RATINGS = new Dictionary<int, string>() {
			//Used in South Korea, apparently since 2006
			{0, "A"},
			{12, "12+"},
			{15, "15+"},
			{18, "18+"},
		};

		public readonly static IDictionary<int, string> FBFC_RATINGS = new Dictionary<int, string>() {
			//Used in Finland up until 2011, and then I think they started using PEGI
			//Definitely exists and used in Wii games, DSi might not be (GBATEK says the byte which would be used for FBFC is reserved)
			{0, "Not rated"},
			{3, "S"},
			{7, "K-7"},
			{11, "K-11"}, //May or may not exist, but Twilight Princess's FBFC rating is set to 11
			{12, "K-12"},
			{15, "K-15"}, //May or may not exist, but Red Steel's FBFC rating is set to 15
			{16, "K-16"},
			{18, "K-18"},
		};

		public readonly static IDictionary<int, string> CGSRR_RATINGS = new Dictionary<int, string>() {
			{0, "G"},
			{6, "Protected"},
			{12, "PG 12"},
			{15, "PG 15"},
			{18, "Restricted"},
		};

		//There are 6 unused bytes after these so those might be other countries
		//Some other countries and their rating boards in case they turn out to be used:
		//Brazil (ClassInd): L = General Audiences, 10+, 12+, 14+, 16+, 18+; "Especially recommended for children/teenagers" rating abandoned in 2009
		//Iran (ERSA): +3 (but they call it all ages), +7, +12, +15, +18
		//Argentina (INCAA): ATP = 0, 13+, 16+, 18+
		//New Zealand (OFLC, not at all like the OFLC Australia used to have): G = 0, PG = ?, M = 16+, RP13 = 13+ or with parental guidance, R13 = 13+, R15 = 15+, R16 = 16+, RP16 = 16+ or with parental guidance, R18 = 18+, RP18 (not created until April 2017) = 18+ or with parental guidance
		//There's a thing called the IARC which is an evil alliance of all the supervillains of the game rating world, and they have 3+/7+/12+/16+/18+ ratings for countries that don't have their own ratings thing, but that didn't exist since 2013

		readonly static Tuple<string, IDictionary<int, string>>[] RATING_NAMES = {
			new Tuple<string, IDictionary<int, string>>("CERO", CERO_RATINGS),
			new Tuple<string, IDictionary<int, string>>("ESRB", ESRB_RATINGS),
			new Tuple<string, IDictionary<int, string>>("<reserved>", null), //Probably BBFC, given position relative to Wii U XML stuff
			new Tuple<string, IDictionary<int, string>>("USK", USK_RATINGS),
			new Tuple<string, IDictionary<int, string>>("PEGI", PEGI_RATINGS),
			new Tuple<string, IDictionary<int, string>>("FBFC", FBFC_RATINGS), //Finland uses a different ratings board since 2011, but for Wii and DSi games it should be fine; I haven't seen this used anyway (actually, it might be Wii only and then became reserved with DSi/3DS)
			new Tuple<string, IDictionary<int, string>>("PEGI (Portgual)", PEGI_PORTUGAL_RATINGS),
			new Tuple<string, IDictionary<int, string>>("PEGI (UK)", PEGI_UK_RATINGS),
			new Tuple<string, IDictionary<int, string>>("AGCB", AGCB_RATINGS),
			new Tuple<string, IDictionary<int, string>>("GRB", GRB_RATINGS),
			new Tuple<string, IDictionary<int, string>>("CGSRR", CGSRR_RATINGS), //3DS only
		};

		public static void parseRating(ROMInfo info, int rating, string name, IDictionary<int, string> dict, bool isDSi) {
			//Bit 5 is set for ESRB on Super Smash Bros Brawl (USA v1.01), Bomberman Blast (USA), and
			//Mario Strikers Charged
			//Possibly indicates online interactivity, e.g. the specific label "Online Interactions Not Rated by the ESRB" / "Game Experience May Change During Online Play"

			//Bit 6 is set for USK in Madworld (PAL), so it possibly indicates something
			//like "banned in this country" or "refused classification"; otherwise Madworld is parsed as being all ages in Germany which is absolutely not the case
			//It's also set on Gnubox GX, VBA GX and USBLoaderCFG channel forwarders, but those are homebrew, so it might be just invalid (they also set bit 5, and without those two
			//bits the rating is read as 10, which isn't a rating category for USK)

			//On 3DS, bit 6 indicates "Rating Pending" and bit 5 indicates No Age Restriction but that can't be right for Wii and probably not DSi

			//Wii U seems to set bit 6 on every single BBFC and reserved rating so maybe it's like "not used"

			if ((rating & 0x40) > 0) {
				info.addInfo(name + " bit 6", true);
			}
			if ((rating & 0x20) > 0) {
				info.addInfo(name + " bit 5", true);
			}

			bool ratingExists;
			if (isDSi) {
				ratingExists = (rating & 0x80) != 0;
			} else {
				ratingExists = (rating & 0x80) == 0;
			}

			if (ratingExists) {
				//Actual rating is bits 0-4
				int ratingValue = rating & 0x1f;
				if (dict != null) {
					info.addInfo(name, ratingValue, dict);
				} else {
					info.addInfo(name, ratingValue);
				}
			}
		}

		public static void parseRatings(ROMInfo info, byte[] ratings, bool isDSi) {
			//Used with Wii/WiiWare and DSi, which both introduced parental controls features I guess
			//DSi seems to use bit 7 to indicate if a rating exists for a given country differently
			//To be precise: With DSi (and 3DS), bit 7 is set when a rating exists, on Wii, bit 7 is unset when a rating exists
			//Wii U has ratings too, but they're XML. Seemingly very similar, though


			for (int i = 0; i < 16; ++i) {
				int rating = ratings[i];
				string ratingName;
				if (i >= RATING_NAMES.Length) {
					ratingName = "Unknown rating " + (i - RATING_NAMES.Length);
				} else {
					ratingName = RATING_NAMES[i].Item1 + " rating";
				}

				IDictionary<int, string> ratingsDict = null;
				if (i < RATING_NAMES.Length && RATING_NAMES[i].Item2 != null) {
					ratingsDict = RATING_NAMES[i].Item2;
				}
				parseRating(info, rating, ratingName, ratingsDict, isDSi);

			}

		}
	}
}
