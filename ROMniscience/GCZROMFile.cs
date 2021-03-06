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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROMniscience {
	class GCZROMFile: ROMFile {

		FileInfo fi;
		GCZInputStream gcz;

		public GCZROMFile(FileInfo path) {
			fi = path;
			gcz = new GCZInputStream(path.OpenRead());
		}

		public override bool compressed => true;

		public override long compressedLength => (long)gcz.compressedSize;

		public override FileInfo path => fi;

		public override long length => (long)gcz.uncompressedSize;

		public override WrappedInputStream stream => gcz;

		//This kinda sucks but it's better than pretending the uncompressed file is called .gcz
		public override string name => fi.Name.Replace(".gcz", ".iso");

		public override void Dispose() {
			((IDisposable)gcz).Dispose();
		}

		public override WrappedInputStream getSiblingFile(string filename) {
			string path = Path.Combine(fi.DirectoryName, filename);
			return new WrappedInputStream(File.OpenRead(path));
		}

		public override bool hasSiblingFile(string filename) {
			return new FileInfo(Path.Combine(fi.DirectoryName, filename)).Exists;
		}
	}
}
