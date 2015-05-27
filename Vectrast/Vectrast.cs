using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.IO;

namespace vectrast {
	class VectRast {
		ArrayList polygons;
		ArrayList objects;
		bool printProgressOn;
		bool printWarningsOn;
		public bool someWarning;
		int xmin;
		int xmax;
		int ymin;
		int ymax;

		public VectRast(bool printProgressOn, bool printWarningsOn) {
			this.printProgressOn = printProgressOn;
			this.printWarningsOn = printWarningsOn;
			someWarning = false;
			polygons = new ArrayList();
			objects = new ArrayList();
		}

		protected SortedList createVectors(byte[,] pixelOn, Bitmap bmp) {
			SortedList vectors = new SortedList(bmp.Width * bmp.Height / 32);
			Hashtable vectorsReverse = new Hashtable(bmp.Width * bmp.Height / 32);
			int steps = 0;
			for (int j = 0; j < bmp.Height + 1; j++)
				for (int i = 0; i < bmp.Width + 1; i++) {
					printProgress(ref steps, 400000);
					int type = 
						(pixelOn[i, j] & 1) + 
						((pixelOn[i + 1, j] & 1) << 1) + 
						((pixelOn[i, j + 1] & 1) << 2) +
						((pixelOn[i + 1, j + 1] & 1) << 3);
					if (type == 1 + 8 || type == 2 + 4) { // get rid of illegal situations
						pixelOn[i, j] |= 1;
						pixelOn[i + 1, j] |= 1;
						pixelOn[i, j + 1] |= 1;
						pixelOn[i + 1, j + 1] |= 1;
						printWarning("\nillegal pixel configuration at [" + i + ", " + j + "]");
					}
				}
			for (int j = 0; j < bmp.Height; j++)
				for (int i = 0; i < bmp.Width; i++) {
					printProgress(ref steps, 800000);
					if ((pixelOn[i + 1, j + 1] & 1) == 1) {
						int type1 = 
							(pixelOn[i + 1, j] & 1) + 
							(pixelOn[i + 1, j + 2] & 1);
						int type2 = 
							(pixelOn[i, j + 1] & 1) + 
							(pixelOn[i + 2, j + 1] & 1);
						if (type1 == 2 && type2 == 0 || type1 == 0 && type2 == 2 ) { // get rid of illegal situations
							pixelOn[i + 2, j + 1] |= 1;
							pixelOn[i + 1, j + 2] |= 1;
							printWarning("\nillegal pixel configuration at [" + i + ", " + j + "]");
						}
					}
				}
			for (int j = -1; j < bmp.Height; j++)
				for (int i = -1; i < bmp.Width; i++) {
					printProgress(ref steps, 400000);
					int type = 
						(pixelOn[i + 1, j + 1] & 1) + 
						((pixelOn[i + 2, j + 1] & 1) << 1) + 
						((pixelOn[i + 1, j + 2] & 1) << 2) + 
						((pixelOn[i + 2, j + 2] & 1) << 3);
					IntVector2 fromPnt = new IntVector2();
					IntVector2 toPnt = new IntVector2();
					switch (type) { // create horizontal and vertical vectors between adjacent pixels
						case 3: 
							// xx
							// --
							fromPnt = new IntVector2(i, j);
							toPnt = new IntVector2(i + 1, j);
							break;
						case 12: 
							// --
							// xx
							fromPnt = new IntVector2(i + 1, j + 1);
							toPnt = new IntVector2(i, j + 1);
							break;
						case 5: 
							// x-
							// x-
							fromPnt = new IntVector2(i, j + 1);
							toPnt = new IntVector2(i, j);
							break;
						case 10: 
							// -x
							// -x
							fromPnt = new IntVector2(i + 1, j);
							toPnt = new IntVector2(i + 1, j + 1);
							break;
						case 14: 
							// -x
							// xx
							fromPnt = new IntVector2(i + 1, j);
							toPnt = new IntVector2(i, j + 1);
							break;
						case 13: 
							// x-
							// xx
							fromPnt = new IntVector2(i + 1, j + 1);
							toPnt = new IntVector2(i, j);
							break;
						case 11: 
							// xx
							// -x
							fromPnt = new IntVector2(i, j);
							toPnt = new IntVector2(i + 1, j + 1);
							break;
						case 7: 
							// xx
							// x-
							fromPnt = new IntVector2(i, j + 1);
							toPnt = new IntVector2(i + 1, j);
							break;
					}
					if (fromPnt.defined) {
						if (vectorsReverse.Contains(fromPnt)) {
							VectorPixel oldVP = (VectorPixel)vectorsReverse[fromPnt];
							if ((toPnt - fromPnt).extension(oldVP.toPnt - oldVP.fromPnt))  {
								vectors.Remove(oldVP.fromPnt);
								vectorsReverse.Remove(oldVP.toPnt);
								fromPnt = oldVP.fromPnt;
							}
						}
						if (vectors.Contains(toPnt)) {
							VectorPixel oldVP = (VectorPixel)vectors[toPnt];
							if ((toPnt - fromPnt).extension(oldVP.toPnt - oldVP.fromPnt))  {
								vectors.Remove(oldVP.fromPnt);
								vectorsReverse.Remove(oldVP.toPnt);
								toPnt = oldVP.toPnt;
							}
						}
						if (!fromPnt.Equals(toPnt)) { // do not add null vectors, they ugly :/
							VectorPixel newVP = new VectorPixel(fromPnt, toPnt);
							if (vectors.Contains(newVP.fromPnt))
								throw new Exception("illegal edge configuration at pixel [" + newVP.fromPnt.x + ", " + newVP.fromPnt.y + "]");
							else
								vectors.Add(newVP.fromPnt, newVP);
							if (vectorsReverse.Contains(newVP.toPnt))
								throw new Exception("illegal edge configuration at pixel [" + newVP.toPnt.x + ", " + newVP.toPnt.y + "]");
							else
								vectorsReverse.Add(newVP.toPnt, newVP);
						}
					}
				}
			return vectors;
		}
		protected void collapseVectors(SortedList vectors) {
			ArrayList vertices;
			int steps = 0;
			while (vectors.Count > 0) {
				VectorPixel vectorOld = (VectorPixel)vectors.GetByIndex(0); // ensures we begin at start/end of some line (as opposed to an inner segment)
				IntVector2 startPnt = vectorOld.fromPnt;
				VectorPixel vectorNow;
				VectorPixel vectorNew;
				VectorPixel vectorPrev = null;
				Line line = new Line(vectorOld);
				vertices = new ArrayList(1000);
				do {
					printProgress(ref steps, 2000);
					vectorNow = (VectorPixel)vectors[vectorOld.toPnt];
					vectorNew = (VectorPixel)vectors[vectorNow.toPnt];
					if (!vectorNow.fromPnt.Equals(startPnt) && !vectorNow.toPnt.Equals(startPnt) && vectorNow.linkVector() && line.sameDir(vectorNow) && !vectorNew.linkVector() && line.sameDir(vectorNew) ) {
						if (line.satisfiesInner(vectorNew)) {
							// new segment ok, let's try the next one
							line.update(vectorNew);
							vectors.Remove(vectorNow.fromPnt);
							vectorOld = vectorNew;
						} else if (line.satisfiesOuter(vectorNew)) {
							// vectorNow can be an outer segment
							line.update(vectorNew);

							vertices.Add(new DoubleVector2(line.toPnt));
							vectors.Remove(vectorNew.fromPnt);
							vectors.Remove(vectorNow.fromPnt);
							vectorOld = (VectorPixel)vectors[vectorNew.toPnt];
							line = new Line(vectorOld);
						} else {
							// new segment off, but link is ok as an outer segment
							line.update(vectorNow);
							vertices.Add(new DoubleVector2(line.toPnt));
							vectors.Remove(vectorNow.fromPnt);
							vectorOld = vectorNew;
							line = new Line(vectorOld);
						}
					} else if (!vectorNow.fromPnt.Equals(startPnt) && line.sameDir(vectorNow)) {
						// vectorNow is not a simple link between two segments
						if (line.satisfiesInner(vectorNow)) {
							// new segment ok, let's try the next one
							line.update(vectorNow);
							vectorOld = vectorNow;
						} else if (line.satisfiesOuter(vectorNow)) {
							// vectorNow can be an outer segment
							line.update(vectorNow);
							vertices.Add(new DoubleVector2(line.toPnt));
							vectors.Remove(vectorNow.fromPnt);
							vectorOld = vectorNew;
							line = new Line(vectorOld);
						} else {
							// vectorNow just won't fit - process it separately
							vertices.Add(new DoubleVector2(line.toPnt));
							vectorOld = vectorNow;
							line = new Line(vectorOld);
						}
					} else {
						// vectorNow just won't fit - process it separately
						vertices.Add(new DoubleVector2(line.toPnt));
						vectorOld = vectorNow;
						line = new Line(vectorOld);
					}
					if (vectorPrev != null)
						vectors.Remove(vectorPrev.fromPnt);
					vectorPrev = vectorOld;
				} while (!vectorOld.fromPnt.Equals(startPnt));
				vectors.Remove(startPnt);
				vertices.TrimToSize();
				polygons.Add(vertices);
			}
		}

		protected void transformVectors(Matrix2D matrix) {
			Random randGen = new Random();
			for (int j = 0; j < polygons.Count; j++) {
				ArrayList vertices = (ArrayList)polygons[j];
				for (int i = 0; i < vertices.Count; i++) {
					DoubleVector2 vertexOld = (DoubleVector2)vertices[i];
					vertices.RemoveAt(i);
					DoubleVector2 vertexNew = vertexOld * matrix;
					// add a little random jitter so as to remove the 'horizontal line' bug
					vertexNew.x += randGen.NextDouble() / 100000.0;
					vertexNew.y += randGen.NextDouble() / 100000.0;
					vertices.Insert(i, vertexNew);
				}
			}
			for (int i = 0; i < objects.Count; i++) {
				ElmaObject objectNow = (ElmaObject)objects[i];
				DoubleVector2 vertexOld = new DoubleVector2(objectNow.x, objectNow.y);
				objects.RemoveAt(i);
				DoubleVector2 vertexNew = vertexOld * matrix;
				objectNow.x = vertexNew.x;
				objectNow.y = vertexNew.y;
				objects.Insert(i, objectNow);
			}
			getMinMax();
		}
		
		protected void saveAsLev(String fileName, String levelName, String LGRName, String groundName, String skyName) {
			if (polygons.Count == 0)
				throw new Exception("there must be at least one polygon!");
			if (xmax - xmin > 170 || ymax - ymin > 141)
				throw new Exception("too large polygons; use different scale");
			int numPolys = polygons.Count;
			if (numPolys > 300)
				throw new Exception("too many polygons; max 300 polygons allowed by elma");
			Random randGen = new Random();
			byte[] randomNum = new byte[4];
			randGen.NextBytes(randomNum);
			byte[] POT14 = {(byte)'P', (byte)'O', (byte)'T', (byte)'1', (byte)'4'};
			double PSUM = 0;
			for (int j = 0; j < polygons.Count; j++) {
				ArrayList vertices = (ArrayList)polygons[j];
				for (int i = 0; i < vertices.Count; i++) {
					DoubleVector2 vertex = (DoubleVector2)vertices[i];
					PSUM += vertex.x + vertex.y;
				}
			}
			double OSUM = 0;
			IEnumerator objectNow = objects.GetEnumerator();
			while (objectNow.MoveNext()) {
				ElmaObject elmaObject = (ElmaObject)objectNow.Current;
				OSUM += elmaObject.x + elmaObject.y + elmaObject.type;
			}
			double PICSUM = 0;
			double SUM = (PSUM + OSUM + PICSUM) * 3247.764325643;
			double integrity1 = SUM;
			double integrity2 = ((double) (randGen.Next() % 5871)) + 11877 - SUM;
			double integrity3 = ((double) (randGen.Next() % 5871)) + 11877 - SUM;
			bool unknown = false;
			if (unknown)
				integrity3 = ((double) (randGen.Next() % 4982)) + 20961 - SUM;
			double integrity4 = ((double) (randGen.Next() % 6102)) + 12112 - SUM;
			bool locked = false;
			if (locked)
				integrity4 = ((double) (randGen.Next() % 6310)) + 23090 - SUM;
			Int32 endOfData = 0x0067103A;
			byte[] emptyTables = {
									 21,5,106,183,137,237,89,196,72,255,143,115,118,188,112,192,223,87,180,
									 47,13,158,7,188,99,8,111,138,9,40,173,56,224,115,249,160,128,0,0,
									 138,55,111,105,96,23,9,119,65,172,140,38,52,172,2,167,152,201,165,254,
									 128,223,192,151,180,80,215,29,202,68,65,251,71,247,216,96,56,132,203,76,
									 130,49,36,154,145,118,201,14,107,204,117,252,204,248,165,116,152,142,2,187,
									 104,68,203,76,189,153,196,52,244,139,122,76,104,88,83,154,112,205,79,43,
									 204,209,26,224,128,46,143,115,52,231,14,212,70,29,189,2,154,191,51,39,
									 132,6,88,17,164,16,228,29,117,42,176,120,47,230,6,134,239,164,213,249,
									 252,145,46,84,116,93,248,145,177,246,162,85,249,35,251,71,142,15,69,244,
									 231,93,84,57,140,209,46,143,56,191,143,187,104,160,56,211,200,137,73,145,
									 59,189,2,3,136,229,44,127,156,181,147,95,87,160,161,228,75,17,92,253,
									 107,132,52,198,252,145,177,23,9,145,59,150,165,116,152,63,189,94,125,166,
									 137,132,39,224,82,100,167,165,162,144,64,79,115,52,152,4,144,31,205,184,
									 165,129,67,136,196,216,201,204,25,45,30,151,101,70,252,53,88,50,215,42,
									 45,253,74,22,152,129,218,132,157,25,91,140,176,133,21,85,190,56,165,254,
									 148,149,203,227,218,86,34,105,4,177,141,7,70,147,108,166,52,93,84,208,
									 248,132,144,31,225,195,182,103,237,56,152,96,246,96,200,91,2,95,28,194,
									 167,93,163,126,207,36,167,27,127,123,222,66,167,211,246,175,171,204,163,100,
									 3,44,35,21,190,174,64,79,69,244,80,5,126,233,247,203,253,140,255,143,
									 82,251,71,83,29,51,118,188,171,191,64,184,224,23,239,0,13,66,62,102,
									 19,246,83,246,142,28,181,180,80,202,186,30,184,27,2,16,77,230,137,132,
									 236,225,241,3,221,235,158,184,191,169,50,254,207,128,33,195,44,127,38,78,
									 192,72,104,173,227,21,236,186,17,72,78,212,116,113,36,154,40,173,181,180,
									 152,4,26,86,211,226,190,148,208,18,251,176,87,101,188,171,86,119,6,226,
									 59,91,245,62,69,1,21,229,175,105,4,111,33,228,49,213,52,172,48,15,
									 36,213,6,167,60,145,92,181,42,104,206,107,132,65,54,96,200,19,180,185,
									 252,171,99,159,179,59,25,196,216,17,164,29,163,8,183,137,204,235,158,0,
									 0,243,203,30,197,231,119,124,40,160,128,72,137,73,27,212,136,196,85,6,
									 206,166,170,255,143,187,45,56,145,177,187,163,139,89,183,196,236,48,74,232,
									 160,220,122,227,185,160,56,237,194,213,190,56,40,22,152,109,207,115,85,19,
									 75,76,25,242,169,4,157,235,66,141,33,215,75,43,7,155,219,35,80,110,
									 130,213,216,4,52,21,190,56,237,181,16,182,83,121,245,88,50,136,32,233,
									 214,152,234,222,151,42,150,106,163,93,12,248,47,197,172,107,86,198,121,81,
									 210,41,37,103,237,89,255,156,56,211,167,132,203,4,72,111,197,34,151,193,
									 54,175,20,195,149,216,96,233,76
								 };
			Int32 endOfFile = 0x00845D52;

			BinaryWriter levWriter = new BinaryWriter((Stream)File.Create(fileName));
			levWriter.Write(POT14);
			levWriter.Write(randomNum, 2, 2);
			levWriter.Write(randomNum);
			levWriter.Write(integrity1);
			levWriter.Write(integrity2);
			levWriter.Write(integrity3);
			levWriter.Write(integrity4);
			for (int i = 0; i < 51; i++)
				levWriter.Write(i < levelName.Length ? (byte)levelName[i] : (byte)0);
			for (int i = 0; i < 16; i++)
				levWriter.Write(i < LGRName.Length ? (byte)LGRName[i] : (byte)0);
			for (int i = 0; i < 10; i++)
				levWriter.Write(i < groundName.Length ? (byte)groundName[i] : (byte)0);
			for (int i = 0; i < 10; i++)
				levWriter.Write(i < skyName.Length ? (byte)skyName[i] : (byte)0);
			levWriter.Write(numPolys + 0.4643643);
			for (int j = 0; j < numPolys; j++) {
				ArrayList vertices = (ArrayList)polygons[j];
				levWriter.Write((Int32)0);
				levWriter.Write((Int32)vertices.Count);
				for (int i = 0; i < vertices.Count; i++) {
					DoubleVector2 vertex = (DoubleVector2)vertices[i];
					levWriter.Write(vertex.x);
					levWriter.Write(vertex.y);
				}
			}
			levWriter.Write(objects.Count + 0.4643643);
			for (int i = 0; i < objects.Count; i++) {
				ElmaObject elmaObject = (ElmaObject)objects[i];
				levWriter.Write(elmaObject.x);
				levWriter.Write(elmaObject.y);
				levWriter.Write((Int32)elmaObject.type);
				levWriter.Write((Int32)elmaObject.typeOfFood);
				levWriter.Write((Int32)elmaObject.animationNumber);
			}
			levWriter.Write((double) 0.2345672);
			levWriter.Write(endOfData);
			levWriter.Write(emptyTables);
			levWriter.Write(endOfFile);
			levWriter.Close();
		}
		
		protected void loadAsLev(String fileName) {
			BinaryReader levReader = new BinaryReader((Stream)File.OpenRead(fileName));
			levReader.BaseStream.Seek(130, SeekOrigin.Begin);
			int numPolys = (int)Math.Round(levReader.ReadDouble() - 0.4643643);
			for (int j = 0; j < numPolys; j++) {
				int grassPoly = levReader.ReadInt32();
				int numVertices = levReader.ReadInt32();
				if (grassPoly == 0) {
					ArrayList vertices = new ArrayList(numVertices);
					for (int i = 0; i < numVertices; i++) {
						DoubleVector2 vertex = new DoubleVector2(levReader.ReadDouble(), levReader.ReadDouble());
						vertices.Add(vertex);
					}
					polygons.Add(vertices);
				} else
					levReader.BaseStream.Seek(16 * numVertices, SeekOrigin.Current);
			}
			int numObjects = (int)Math.Round(levReader.ReadDouble() - 0.4643643);
			objects = new ArrayList(numObjects);
			for (int j = 0; j < numObjects; j++) {
				ElmaObject elmaObject = new ElmaObject(levReader.ReadDouble(), levReader.ReadDouble(), levReader.ReadUInt32(), levReader.ReadUInt32(), levReader.ReadUInt32());
				objects.Add(elmaObject);
			}
			levReader.Close();
			if (polygons.Count == 0)
				throw new Exception("there must be at least one polygon!");
			getMinMax();
		}
		
		protected void loadAsBmp(String fileName, out Bitmap bmp, out byte[,] pixelOn, double zoom, byte merging, int numFlowers) {
			bmp = new Bitmap(fileName);
			pixelOn = new byte[bmp.Width + 2, bmp.Height + 2];
			int steps = 0;
			for (int j = -1; j <= bmp.Height; j++)
				for (int i = -1; i <= bmp.Width; i++) {
					printProgress(ref steps, 200000);
					if (j < 0 || j >= bmp.Height || i < 0 || i >= bmp.Width)
						pixelOn[i + 1, j + 1] = merging;
					else {
						Color col = bmp.GetPixel(i, j);
						if (col.R < 250 || col.G < 250 || col.B < 250) {
							pixelOn[i + 1, j + 1] = 1;
						}
					}
				}
			objects = new ArrayList();
			for (int i = 0; i < numFlowers; i++)
				objects.Add(new ElmaObject(
					bmp.Width / 2 + (2 + 6 * Math.Cos(i * Math.PI / numFlowers)) / zoom,
					bmp.Height / 2 - 6 * Math.Sin(i * Math.PI / numFlowers) / zoom,
					1, 0, 0));
			objects.Add(new ElmaObject(bmp.Width / 2, bmp.Height / 2, 4, 0, 0));
		}

		protected void saveAsBmp(String fileName) {
			if (polygons.Count == 0)
				throw new Exception("there must be at least one polygon!");
			ArrayList[] inPnt = new ArrayList[ymax - ymin + 1];
			for (int i = 0; i < inPnt.Length; i++)
				inPnt[i] = new ArrayList(20);
			for (int j = 0; j < polygons.Count; j++) {
				ArrayList vertices = (ArrayList)polygons[j];
				for (int i = 0; i < vertices.Count; i++) {
					DoubleVector2 vertex = (DoubleVector2)vertices[i];
					DoubleVector2 vertexNext = i + 1 < vertices.Count ? (DoubleVector2)vertices[i + 1] : (DoubleVector2)vertices[0];
					IntVector2 pointFrom = new IntVector2((int)vertex.x - xmin, (int)vertex.y - ymin);
					IntVector2 pointTo = new IntVector2((int)vertexNext.x - xmin, (int)vertexNext.y - ymin);
					VectorPixel vect = new VectorPixel(pointFrom, pointTo);
					if (vect.dy != 1) {
						IntVector2 pointNow;
						for (int k = 0; k < vect.dy; k++) {
							if (vect.dx > vect.dy) {
								if (vect.sx * vect.sy == 1)
									pointNow = new IntVector2(pointFrom.x + vect.sx * (2 * vect.dx * k + vect.dy - 1) / (2 * vect.dy), pointFrom.y + vect.sy * k);
								else
									pointNow = new IntVector2(pointFrom.x + vect.sx * ((2 * vect.dx * (k + 1) + vect.dy - 1) / (2 * vect.dy) - 1), pointFrom.y + vect.sy * k);
							} else
								pointNow = new IntVector2(pointFrom.x + vect.sx * (2 * (vect.dx - 1) * k + vect.dy - 1) / (2 * (vect.dy - 1)), pointFrom.y + vect.sy * k);
							inPnt[pointNow.y].Add(new FromToInt(pointNow.x, vect.sx, vect.sy, pointFrom.x + pointTo.x));
						}
					}
				}
			}
			Bitmap bmp = new Bitmap(xmax - xmin + 3, ymax - ymin + 3, PixelFormat.Format32bppRgb);
			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
			int stride = bmpData.Stride;
			Int64 startPtr = bmpData.Scan0.ToInt64();
			if (stride < 0) {
				startPtr += stride * (bmpData.Height - 1);
				stride = Math.Abs(stride);
			}
			byte[] rowBytes = new byte[bmpData.Stride];
			for (int i = 0; i < bmpData.Height; i++) // make everything black first
				System.Runtime.InteropServices.Marshal.Copy(new IntPtr(startPtr + stride * i), rowBytes, 0, bmpData.Stride);
			for (int i = 0; i < bmpData.Stride; i++)
				rowBytes[i] = 255;
			for (int i = 0; i < inPnt.Length; i++) {
				inPnt[i].Sort();
				for (int j = 1; j < inPnt[i].Count; j++) {
					FromToInt f = (FromToInt)inPnt[i][j];
					FromToInt fprev = (FromToInt)inPnt[i][j - 1];
					if (f.sy > 0 && fprev.sy <= 0 && f.x - (fprev.x + 1) > 0) // insert empty space where appropriate
						System.Runtime.InteropServices.Marshal.Copy(rowBytes, 0, new IntPtr(startPtr + stride * i + 4 * (fprev.x + 1)), 4 * (f.x - (fprev.x + 1)));
					f = (FromToInt)inPnt[i][j];
					fprev = (FromToInt)inPnt[i][j - 1];
				}
			}
			bmp.UnlockBits(bmpData);
			bmp.Save(fileName);
		}
		void getMinMax() {
			xmin = Int32.MaxValue;
			xmax = Int32.MinValue;
			ymin = Int32.MaxValue;
			ymax = Int32.MinValue;
			for (int j = 0; j < polygons.Count; j++) {
				ArrayList vertices = (ArrayList)polygons[j];
				for (int i = 0; i < vertices.Count; i++) {
					DoubleVector2 vertex = (DoubleVector2)vertices[i];
					xmin = Math.Min(xmin, (int)vertex.x);
					xmax = Math.Max(xmax, (int)vertex.x);
					ymin = Math.Min(ymin, (int)vertex.y);
					ymax = Math.Max(ymax, (int)vertex.y);
				}
			}
			for (int i = 0; i < objects.Count; i++) {
				DoubleVector2 vertex = new DoubleVector2(((ElmaObject)objects[i]).x, ((ElmaObject)objects[i]).y);
				xmin = Math.Min(xmin, (int)vertex.x);
				xmax = Math.Max(xmax, (int)vertex.x);
				ymin = Math.Min(ymin, (int)vertex.y);
				ymax = Math.Max(ymax, (int)vertex.y);
			}
		}

		void printProgress(ref int steps, int max) {
			if (printProgressOn)
				if (steps-- == 0) {
					steps = max;
					Console.Write(".");
				}
		}
		void printWarning(string warning) {
			someWarning = true;
			if (printWarningsOn) {
				Console.WriteLine(warning);
			}
		}
		static Int32 Main(string[] args) {
			#region init defaults
			const double ONEPIXEL = 1.0 / 47.0;
			bool printProgressOn = true;
			bool printWarningsOn = false;
			IOType load_type = IOType.None;
			IOType save_type = IOType.None;
			String loadLevFileName = null;
			String saveLevFileName = null;
			String loadBmpFileName = null;
			String saveBmpFileName = null;
			int numFlowers = 1;
			Matrix2D transformMatrix = Matrix2D.identityM();
			#endregion
			#region arguments parse
			int arg_num = 0;
			try {
				while (arg_num < args.Length) {
					String arg_now = args[arg_num++];
					switch (arg_now) {
						case "-loadbmp":
							// initialize from bitmap
							load_type = IOType.Bitmap;
							loadBmpFileName = args[arg_num++];
							break;
						case "-loadlev":
							// initialize from level
							load_type = IOType.Level;
							loadLevFileName = args[arg_num++];
							break;
						case "-savebmp":
							// save to bitmap
							save_type = IOType.Bitmap;
							saveBmpFileName = args[arg_num++];
							break;
						case "-savelev":
							// save to level
							save_type = IOType.Level;
							saveLevFileName = args[arg_num++];
							break;
						case "-loadlevbmp":
							load_type = IOType.LevelBitmap;
							loadLevFileName = args[arg_num++];
							loadBmpFileName = args[arg_num++];
							break;
						case "-translate":
							// offset by tx, ty
							double tx = Double.Parse(args[arg_num++]);
							double ty = Double.Parse(args[arg_num++]);
							transformMatrix = transformMatrix * Matrix2D.translationM(tx, ty);
							break;
						case "-rotate":
							// rotate by angle (in degrees)
							double ang = Double.Parse(args[arg_num++]);
							transformMatrix = transformMatrix * Matrix2D.rotationM(ang);
							break;
						case "-scale":
							// scale by factor sx, sy
							double sx = Double.Parse(args[arg_num++]) * ONEPIXEL;
							double sy = Double.Parse(args[arg_num++]) * ONEPIXEL;
							transformMatrix = transformMatrix * Matrix2D.scaleM(sx, sy);
							break;
						case "-flowers":
							// number of flowers
							numFlowers = Int32.Parse(args[arg_num++]);
							break;
						case "-progress":
							// show progress continually
							printProgressOn = Boolean.Parse(args[arg_num++]);
							break;
						case "-warnings":
							// print warnings
							printWarningsOn = Boolean.Parse(args[arg_num++]);
							break;
						default:
							throw new Exception("unknown parameter '" + arg_now + "'");
					}
				}
			} catch (IndexOutOfRangeException) {
				Console.WriteLine("\nexpected value(s) after the switch");
				return 7;
			} catch (Exception e) {
				Console.WriteLine("\nerror parsing command line: " + e.Message);
				return 8;
			}
			if (load_type == IOType.LevelBitmap && save_type == IOType.Bitmap) {
				Console.WriteLine("savebmp not allowed after loadlevbmp");
				return 3;
			}
			#endregion
			#region load and transform
			VectRast vr = new VectRast(printProgressOn, printWarningsOn);
			if (load_type == IOType.Bitmap || load_type == IOType.LevelBitmap) {
				byte[,] pixelOn;
				Bitmap bmp;
				try {
					Console.Write("\nloading in bitmap {0}", loadBmpFileName);
					vr.loadAsBmp(loadBmpFileName, out bmp, out pixelOn, Math.Abs(transformMatrix.elements[0,0]) + Math.Abs(transformMatrix.elements[1,1]), load_type == IOType.LevelBitmap ? (byte)0 : (byte)1, numFlowers);
				} catch (Exception e) {
					Console.WriteLine("\nerror loading bitmap {0}: " + e.Message, loadBmpFileName);
					return 1;
				}
				try {
					Console.Write("\ncreating polygons");
					vr.collapseVectors(vr.createVectors(pixelOn, bmp));
				} catch (Exception e) {
					Console.WriteLine("\nerror vectorizing the bitmap: " + e.Message);
					return 2;
				}
				transformMatrix = Matrix2D.translationM(-bmp.Width / 2.0, -bmp.Height / 2.0) * transformMatrix;
				bmp.Dispose();
			}
			if (load_type == IOType.LevelBitmap)
				try {
					Console.Write("\ntransforming vectors");
					vr.transformVectors(transformMatrix);
				} catch (Exception e) {
					Console.WriteLine("\nerror transforming vectors: " + e.Message);
					return 4;
				}
			if (load_type == IOType.Level || load_type == IOType.LevelBitmap)
				try {
					Console.Write("\nloading in level {0}", loadLevFileName);
					vr.loadAsLev(loadLevFileName);
				} catch (Exception e) {
					Console.WriteLine("\nerror loading level {0}: " + e.Message, loadLevFileName);
					return 5;
				}
			if (load_type != IOType.LevelBitmap)
				try {
					Console.Write("\ntransforming vectors");
					vr.transformVectors(transformMatrix);
				} catch (Exception e) {
					Console.WriteLine("\nerror transforming vectors: " + e.Message);
					return 6;
				}
			#endregion
			#region save
			switch (save_type) {
				case IOType.None:
					break;
				case IOType.Bitmap:
					try {
						Console.Write("\nsaving bitmap {0}", saveBmpFileName);
						vr.saveAsBmp(saveBmpFileName);
					} catch (Exception e) {
						Console.WriteLine("\nerror saving bitmap {0}: " + e.Message, saveBmpFileName);
						return 5;
					}
					break;
				case IOType.Level:
					try {
						Console.Write("\nsaving level {0}", saveLevFileName);
						vr.saveAsLev(saveLevFileName, "autogenerated on " + DateTime.Now.ToString("g"), "DEFAULT", "ground", "sky");
					} catch (Exception e) {
						Console.WriteLine("\nerror saving level {0}: " + e.Message, saveLevFileName);
						return 6;
					}
					break;
			}
			if (vr.someWarning)
				Console.WriteLine("\ndone, but there were some warnings; set '-warnings true' to view them\n");
			else
				Console.WriteLine("\ndone\n");
			return 0;
			#endregion
		}
	}
}
