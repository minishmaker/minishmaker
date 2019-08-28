using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
	public class RoomMetaData
	{
		private int width, height;
		public int mapPosX, mapPosY;

		public int PixelWidth
		{
			get
			{
				return width * 16;
			}
		}

		public int PixelHeight
		{
			get
			{
				return height * 16;
			}
		}

		public int TileWidth
		{
			get
			{
				return width;
			}
		}

		public int TileHeight
		{
			get
			{
				return height;
			}
		}
		
        private string roomPath;
		private string areaPath;

		private int paletteSetID;
		private List<AddrData> tileSetAddrs = new List<AddrData>();

		private List<ChestData> chestInformation = new List<ChestData>();
		public List<ChestData> ChestInfo
		{
			get { return chestInformation;}
		}

		private List<EnemyData> enemyPlacementInformation = new List<EnemyData>();
		public List<EnemyData> EnemyPlacementInfo
		{
			get { return enemyPlacementInformation;}
		}

		private List<WarpData> warpInformation = new List<WarpData>();
		public List<WarpData> WarpInformation
		{
			get { return warpInformation;}
		}

		private AddrData? bg2RoomDataAddr;
		private AddrData bg2MetaTilesAddr;

		private AddrData? bg1RoomDataAddr;
		private AddrData bg1MetaTilesAddr;

		private bool chestDataLarger = false;
		public bool ChestDataLarger
		{
			get { return chestDataLarger;}
		}

		private bool bg1Use20344B0 = false;
		public bool Bg1Use20344B0
		{
			get
			{
				return bg1Use20344B0;
			}
		}

		public struct AddrData
		{
			public int src;
			public int dest;
			public int size; // in words (2x bytes)
			public bool compressed;

			public AddrData( int src, int dest, int size, bool compressed )
			{
				this.src = src;
				this.dest = dest;
				this.size = size;
				this.compressed = compressed;
			}
		}

		public struct ChestData
		{
			public byte type;
            public byte chestId;
            public byte itemId;
            public byte itemSubNumber;
            public ushort chestLocation;
            public ushort unknown;

			public ChestData(byte type, byte chestId, byte itemId, byte itemSubNumber, ushort chestLocation, ushort other)
			{
				this.type = type;
				this.chestId = chestId;
				this.itemId = itemId;
				this.itemSubNumber = itemSubNumber;
				this.chestLocation = chestLocation;
				this.unknown = other;
			}
		}


		public struct WarpData
		{
			public ushort warpType;	//2
			public ushort warpXPixel;//4
			public ushort warpYPixel;//6
			public ushort destXPixel;//8
			public ushort destYPixel;//10 A
			public byte warpVar;//11 B
			public byte destArea;//12 C
			public byte destRoom;//13 D
			public byte exitHeight;//14 E
			public byte transitionType;//15 F
			public byte facing;//16 10
			public ushort soundId;//18 12
			//2 byte padding 20 14
			public WarpData(byte[] data, int offset)
			{
				warpType =		(ushort)(data[0+offset]+(data[1+offset]<<8));
				warpXPixel =	(ushort)(data[2+offset]+(data[3+offset]<<8));
				warpYPixel =	(ushort)(data[4+offset]+(data[5+offset]<<8));
				destXPixel =	(ushort)(data[6+offset]+(data[7+offset]<<8));
				destYPixel =	(ushort)(data[8+offset]+(data[9+offset]<<8));
				warpVar =				data[10+offset];
				destArea =				data[11+offset];
				destRoom =				data[12+offset];
				exitHeight =			data[13+offset];
				transitionType =		data[14+offset];
				facing =				data[15+offset];
				soundId =		(ushort)(data[16+offset]+(data[17+offset]<<8));

			}
		}

		public struct EnemyData
		{
			public byte objectType; 
			public byte objectSub; //?
			public byte id;
			public byte subId;
			public ushort unknown1; //?
			public ushort unknown2; //?
			public ushort xpos;
			public ushort ypos;
			public ushort unknown3; //?
			public ushort unknown4; //?

			public EnemyData(byte oType, byte oSub, byte id, byte subId, ushort u1, ushort u2, ushort xpos, ushort ypos, ushort u3, ushort u4)
			{
				this.objectType= oType;
				this.objectSub = oSub;
				this.id = id;
				this.subId = subId;
				this.unknown1 = u1;
				this.unknown2 = u2;
				this.xpos = xpos;
				this.ypos = ypos;
				this.unknown3 = u3;
				this.unknown4 = u4;
			}

		}

		public RoomMetaData( int areaIndex, int roomIndex )
		{
			LoadMetaData( areaIndex, roomIndex );
		}

		private void LoadMetaData( int areaIndex, int roomIndex )
		{
			areaPath = Project.Instance.projectPath + "/Areas/Area " + StringUtil.AsStringHex2(areaIndex);
            roomPath = areaPath + "/Room " + StringUtil.AsStringHex2(roomIndex);

            var r = ROM.Instance.reader;
			var header = ROM.Instance.headers;

			int areaRMDTableLoc = r.ReadAddr( header.MapHeaderBase + (areaIndex << 2) );
			int roomMetaDataTableLoc = areaRMDTableLoc + (roomIndex * 0x0A);
			
			if(File.Exists(roomPath+ "/" + DataType.roomMetaData +"Dat.bin"))
			{
				var data = File.ReadAllBytes(roomPath+ "/" + DataType.roomMetaData +"Dat.bin");
				this.mapPosX = (data[0]+(data[1]<<8))>>4;
				this.mapPosY = (data[2]+(data[3]<<8))>>4;
				r.SetPosition(roomMetaDataTableLoc+4);
			}
			else
			{
				this.mapPosX = r.ReadUInt16( roomMetaDataTableLoc )>>4;
				this.mapPosY = r.ReadUInt16()>>4;
			}
			
			this.width = r.ReadUInt16() >> 4; //bytes 5+6 pixels/16 = tiles
			this.height = r.ReadUInt16() >> 4;                          //bytes 7+8 pixels/16 = tiles

			//get addr of TPA data
			int tileSetOffset = r.ReadUInt16() << 2;                    //bytes 9+10

			int areaTileSetTableLoc = r.ReadAddr( header.globalTileSetTableLoc + (areaIndex << 2) );
			int roomTileSetLoc = r.ReadAddr( areaTileSetTableLoc + tileSetOffset );

			r.SetPosition( roomTileSetLoc );

			ParseData( r, Set1 );

			//metatiles
			int metaTileSetsLoc = r.ReadAddr( header.globalMetaTileSetTableLoc + (areaIndex << 2) );

			r.SetPosition( metaTileSetsLoc );

			ParseData( r, Set2 );

			//get addr of room data 
			int areaTileDataTableLoc = r.ReadAddr( header.globalTileDataTableLoc + (areaIndex << 2) );
			int tileDataLoc = r.ReadAddr( areaTileDataTableLoc + (roomIndex << 2) );
			r.SetPosition( tileDataLoc );

			ParseData( r, Set3 );

			//attempt at obtaining chest data (+various)
			int areaEntityTableAddrLoc = header.AreaMetadataBase + (areaIndex << 2);
            int areaEntityTableAddr = r.ReadAddr(areaEntityTableAddrLoc);

            int roomEntityTableAddrLoc = areaEntityTableAddr + (roomIndex << 2);
            int roomEntityTableAddr =r.ReadAddr(roomEntityTableAddrLoc);

            //4 byte chunks, 1-2 are unknown use, 3rd seems to be mostly enemies. 4th seems to be mostly chests
			//Enemies
			string enemyDataPath = roomPath + "/" + DataType.enemyPlacementData +"Dat.bin";
			if (File.Exists(enemyDataPath))
            {
                byte[] data = File.ReadAllBytes(enemyDataPath);
                int index = 0;
                while (index < data.Length && (TileEntityType)data[index] != TileEntityType.None && data[index]!=0xFF)
                {
                    var objectType = data[index];
                    var objectSub = data[index + 1];
                    var id = data[index + 2];
                    var subid = data[index + 3];
					var unknown1 = (ushort)(data[index + 4] | (data[index+5]<<8));
					var unknown2 = (ushort)(data[index + 6] | (data[index+7]<<8));
					var locX = (ushort)(data[index + 8] | (data[index+9]<<8));
					var locY = (ushort)(data[index + 10] | (data[index+11]<<8));
					var unknown3 = (ushort)(data[index + 12] | (data[index+13]<<8));
					var unknown4 = (ushort)(data[index + 14] | (data[index+15]<<8));
                    enemyPlacementInformation.Add(new EnemyData(objectType,objectSub,id,subid,unknown1,unknown2,locX,locY,unknown3,unknown4));
                    index += 16;
                }
            } 
            else
            {
                int chestTableAddr = r.ReadAddr(roomEntityTableAddr+8);

                var data = r.ReadBytes(16, chestTableAddr);

                while ((TileEntityType)data[0] != TileEntityType.None && data[0]!=0xFF) //ends on type 0
                {
                    var objectType = data[0];
                    var objectSub = data[ 1];
                    var id = data[2];
                    var subid = data[3];
					var unknown1 = (ushort)(data[4] | (data[5]<<8));
					var unknown2 = (ushort)(data[6] | (data[7]<<8));
					var locX = (ushort)(data[8] | (data[9]<<8));
					var locY = (ushort)(data[10] | (data[11]<<8));
					var unknown3 = (ushort)(data[12] | (data[13]<<8));
					var unknown4 = (ushort)(data[14] | (data[15]<<8));
                    enemyPlacementInformation.Add(new EnemyData(objectType,objectSub,id,subid,unknown1,unknown2,locX,locY,unknown3,unknown4));
                    data = r.ReadBytes(16);
                }
            }

			//Chests
            string chestDataPath = roomPath + "/" + DataType.chestData +"Dat.bin";
            if (File.Exists(chestDataPath))
            {
                byte[] data = File.ReadAllBytes(chestDataPath);
                int index = 0;
                while (index < data.Length && (TileEntityType)data[index] != TileEntityType.None)
                {
					
                    var type = data[index];
                    var id = data[index + 1];
                    var item = data[index + 2];
                    var subNum = data[index + 3];
                    ushort loc = (ushort)(data[index + 4] | (data[index + 5] << 8));
                    ushort other = (ushort)(data[index + 6] | (data[index + 7] << 8));
                    chestInformation.Add(new ChestData(type, id, item, subNum, loc, other));
                    index += 8;
                }
            } 
            else
            {
                int chestTableAddr = r.ReadAddr(roomEntityTableAddr+12);

                var data = r.ReadBytes(8, chestTableAddr);

                while ((TileEntityType)data[0] != TileEntityType.None) //ends on type 0
                {
                    var type = data[0];
                    var id = data[1];
                    var item = data[2];
                    var subNum = data[3];
                    ushort loc = (ushort)(data[4] | (data[5] << 8));
                    ushort other = (ushort)(data[6] | (data[7] << 8));
                    chestInformation.Add(new ChestData(type, id, item, subNum, loc, other));
                    data = r.ReadBytes(8);
                }
            }

			//Warps
			string warpDataPath = roomPath + "/" + DataType.warpData +"Dat.bin";
			if (File.Exists(warpDataPath))
            {
                byte[] data = File.ReadAllBytes(warpDataPath);
                int index = 0;
                while (index < data.Length && (data[index]+(data[index+1]<<8))!=0xFFFF)
                {
                    warpInformation.Add(new WarpData(data, index));
                    index += 20;
                }
            } 
            else
            {
				int areaWarpTableAddrLoc = header.warpInformationTableLoc + (areaIndex << 2);
				int areaWarpTableAddr = r.ReadAddr(areaWarpTableAddrLoc);

				int roomWarpTableAddrLoc = areaWarpTableAddr + (roomIndex << 2);
				int roomWarpTableAddr =r.ReadAddr(roomWarpTableAddrLoc);

                var data = r.ReadBytes(20, roomWarpTableAddr);

                while (data[0]!=0xFF) //ends on type FFFF
                {
                    warpInformation.Add(new WarpData(data,0));
                    data = r.ReadBytes(20);
                }
            }


		}

		public TileSet GetTileSet()
		{
            string tilesetPath = roomPath + "/" + (int)DataType.tileSet;
            if (File.Exists(tilesetPath))
            {
                return new TileSet(File.ReadAllBytes(tilesetPath));
            }
            else
            {
                return new TileSet(tileSetAddrs);
            }
		}

		public PaletteSet GetPaletteSet()
		{
            return new PaletteSet(paletteSetID);
		}

		public bool GetBG2Data( ref byte[] bg2RoomData, ref MetaTileSet bg2MetaTiles )
		{
			if( bg2RoomDataAddr != null )
			{
				bg2MetaTiles = new MetaTileSet( bg2MetaTilesAddr, false, areaPath+"/"+DataType.bg2MetaTileSet+"Dat.bin" );

                byte[] data = null;
                string bg2Path = roomPath + "/" + DataType.bg2Data+"Dat.bin";

				data = Project.Instance.GetSavedData(bg2Path,true);
                if(data==null)
                {
                    data = DataHelper.GetData((AddrData)bg2RoomDataAddr);
                }
				data.CopyTo( bg2RoomData, 0 );

				return true;
			}
			return false;
		}

		public bool GetBG1Data( ref byte[] bg1RoomData, ref MetaTileSet bg1MetaTiles )
		{
			if( bg1RoomDataAddr != null )
			{
                byte[] data = null;
                string bg1Path = roomPath + "/" + DataType.bg1Data+"Dat.bin";

				data = Project.Instance.GetSavedData(bg1Path, true);
                if(data == null)
                {
                    data = DataHelper.GetData((AddrData)bg1RoomDataAddr);
                }

				if( !bg1Use20344B0 )
                {
					bg1MetaTiles = new MetaTileSet( bg1MetaTilesAddr , true, areaPath+"/"+DataType.bg1MetaTileSet+"Dat.bin" );
				}

                data.CopyTo(bg1RoomData, 0);
                return !bg1Use20344B0;
			}
			return false;
		}

		private void ParseData( Reader r, Func<AddrData, bool> postFunc )
		{
			var header = ROM.Instance.headers;
			bool cont = true;
			while( cont )
			{
				UInt32 data = r.ReadUInt32();
				UInt32 data2 = r.ReadUInt32();
				UInt32 data3 = r.ReadUInt32();

				

				if( data2 == 0 )
				{ //palette
					this.paletteSetID = (int)(data & 0x7FFFFFFF); //mask off high bit
				}
				else
				{
					int source = (int)((data & 0x7FFFFFFF) + header.gfxSourceBase); //08324AE4 is tile gfx base
					int dest = (int)(data2 & 0x7FFFFFFF);
					bool compressed = (data3 & 0x80000000) != 0; //high bit of size determines LZ or DMA
					int size = (int)(data3 & 0x7FFFFFFF);

					cont = postFunc( new AddrData( source, dest, size, compressed ) );
				}
				if(cont == true)
				{
					cont = (data & 0x80000000) != 0; //high bit determines if more to load
				}
			}
		}

		public long CompressBG1(ref byte[] outdata, byte[] bg1data)
		{
			var compressed = new byte[bg1data.Length];
			long totalSize = 0;
			MemoryStream ous = new MemoryStream( compressed );
			totalSize = DataHelper.Compress(bg1data, ous, false);

			outdata = new byte[totalSize];
			Array.Copy(compressed,outdata,totalSize);
            //var sizeDifference = totalSize - bg1RoomDataAddr.Value.size;

            totalSize |= 0x80000000;

			return totalSize;
		}

		public long CompressBG2(ref byte[] outdata,byte[] bg2data)
		{
			var compressed = new byte[bg2data.Length];
			long totalSize = 0;
			MemoryStream ous = new MemoryStream( compressed );
			totalSize = DataHelper.Compress(bg2data, ous, false);
			
			outdata = new byte[totalSize];
			Array.Copy(compressed,outdata,totalSize);
            //var sizeDifference = totalSize - bg2RoomDataAddr.Value.size;

            totalSize |= 0x80000000;

            return totalSize;
		}

		public long GetChestData(ref byte[] outdata )
		{
            outdata = new byte[chestInformation.Count*8+8];

			for(int i = 0; i< chestInformation.Count; i++)
			{
				var index = i*8;
				var data = chestInformation[i];
				outdata[index] = data.type;
				outdata[index+1] = data.chestId;
				outdata[index+2] = data.itemId;
				outdata[index+3] = data.itemSubNumber;
				byte high = (byte)(data.chestLocation>>8);
				byte low = (byte)(data.chestLocation-(high<<8));
				outdata[index+4] = low;
				outdata[index+5] = high;
				high = (byte)(data.unknown>>8);
				low = (byte)(data.unknown-(high<<8));
				outdata[index+6] = low;
				outdata[index+7] = high;

				if(i == chestInformation.Count-1)// add ending 0's
				{
					for(int j= 0; j<8;j++)
						outdata[index+8+j]=0;
				}
			}

            return outdata.Length;
		}

		public void AddChestData(ChestData data)
		{
			chestDataLarger = true; //larger so should be moved
			chestInformation.Add(data);
		}

        public void RemoveChestData(ChestData data)
        {
            chestInformation.Remove(data);
        }

		public long GetEnemyPlacementData(ref byte[] data)
		{
			var outdata = new byte[enemyPlacementInformation.Count*16+16];

			for(int i = 0; i< enemyPlacementInformation.Count; i++)
			{
				var index = i*16;
				var currentData = enemyPlacementInformation[i];
				outdata[index] = currentData.objectType;
				outdata[index+1] = currentData.objectSub;
				outdata[index+2] = currentData.id;
				outdata[index+3] = currentData.subId;
				outdata[index+4] = (byte)(currentData.unknown1 & 0xff);
				outdata[index+5] = (byte)(currentData.unknown1 >> 8);
				outdata[index+6] = (byte)(currentData.unknown2 & 0xff);
				outdata[index+7] = (byte)(currentData.unknown2 >> 8);
				outdata[index+8] = (byte)(currentData.xpos & 0xff);
				outdata[index+9] = (byte)(currentData.xpos >> 8);
				outdata[index+10] = (byte)(currentData.ypos & 0xff);
				outdata[index+11] = (byte)(currentData.ypos >> 8);
				outdata[index+12] = (byte)(currentData.unknown3 & 0xff);
				outdata[index+13] = (byte)(currentData.unknown3 >> 8);
				outdata[index+14] = (byte)(currentData.unknown4 & 0xff);
				outdata[index+15] = (byte)(currentData.unknown4 >> 8);

				if(i == enemyPlacementInformation.Count-1)// add ending 0's
				{
					outdata[index+16] = 0xFF;
					for(int j= 1; j<16;j++)
						outdata[index+16+j]=0;
				}
			}
			data = outdata;
			return outdata.Length;
		}

		public long GetWarpData(ref byte[] data)
		{
			var outdata = new byte[warpInformation.Count*20+20];

			for(int i = 0; i< warpInformation.Count; i++)
			{
				var index = i*20;
				var currentData = warpInformation[i];
				outdata[index+0] = (byte)(currentData.warpType & 0xff);
				outdata[index+1] = (byte)(currentData.warpType >> 8);
				outdata[index+2] = (byte)(currentData.warpXPixel & 0xff);
				outdata[index+3] = (byte)(currentData.warpXPixel >> 8);
				outdata[index+4] = (byte)(currentData.warpYPixel & 0xff);
				outdata[index+5] = (byte)(currentData.warpYPixel >> 8);
				outdata[index+6] = (byte)(currentData.destXPixel & 0xff);
				outdata[index+7] = (byte)(currentData.destXPixel >> 8);
				outdata[index+8] = (byte)(currentData.destYPixel & 0xff);
				outdata[index+9] = (byte)(currentData.destYPixel >> 8);
				outdata[index+10] = currentData.warpVar;
				outdata[index+11] = currentData.destArea;
				outdata[index+12] = currentData.destRoom;
				outdata[index+13] = currentData.exitHeight;
				outdata[index+14] = currentData.transitionType;
				outdata[index+15] = currentData.facing;
				outdata[index+16] = (byte)(currentData.soundId & 0xff);
				outdata[index+17] = (byte)(currentData.soundId >> 8);
				outdata[index+18] = 0;
				outdata[index+19] = 0;//padding

				if(i == warpInformation.Count-1)// add ending 0's
				{
					outdata[index+16] = 0xFF;
					outdata[index+17] = 0xFF;
					for(int j= 2; j<20;j++)
						outdata[index+20+j]=0;
				}
			}
			data = outdata;
			return outdata.Length;
		}

        //To be changed as actual data gets changed and tested
        public int GetPointerLoc(DataType type, int areaIndex, int roomIndex)
		{
			var r = ROM.Instance.reader;
			var header = ROM.Instance.headers;
			int retAddr = 0;
			int areaRMDTableLoc = r.ReadAddr( header.MapHeaderBase + (areaIndex << 2) );
			int roomMetaDataTableLoc = areaRMDTableLoc + (roomIndex * 0x0A);

			switch(type)
			{
				case DataType.roomMetaData:
					retAddr = roomMetaDataTableLoc;
					break;

				case DataType.tileSet:
					//get addr of TPA data
					int tileSetOffset = r.ReadUInt16(roomMetaDataTableLoc+8) << 2;                    //bytes 9+10

					int areaTileSetTableLoc = r.ReadAddr( header.globalTileSetTableLoc + (areaIndex << 2) );
					int roomTileSetAddrLoc = areaTileSetTableLoc + tileSetOffset;
					retAddr = roomTileSetAddrLoc;
					break;

				case DataType.bg1MetaTileSet:
				case DataType.bg2MetaTileSet:
					int metaTileSetsAddrLoc = r.ReadAddr( header.globalMetaTileSetTableLoc + (areaIndex << 2) );
					//retAddr = metaTileSetsAddrLoc;
					r.SetPosition(metaTileSetsAddrLoc);
					if(type == DataType.bg1MetaTileSet)
					{
						ParseData(r, Meta1Check);
					}
					if(type == DataType.bg2MetaTileSet)
					{
						ParseData(r, Meta2Check);
					}
					retAddr = (int)r.Position-12; //step back 12 bytes as the bg was found after reading
					break;

				case DataType.bg1Data:
				case DataType.bg2Data:
					int areaTileDataTableLoc = r.ReadAddr( header.globalTileDataTableLoc + (areaIndex << 2) );
					int tileDataLoc = r.ReadAddr( areaTileDataTableLoc + (roomIndex << 2) );
					r.SetPosition( tileDataLoc );

					if(type == DataType.bg1Data)
					{
						ParseData(r,Bg1Check);
					}
					else //not bg1 so has to be bg2
					{
						ParseData(r,Bg2Check);
					}
					retAddr = (int)r.Position-12; //step back 12 bytes as the bg was found after reading
					break;

				case DataType.chestData:
				case DataType.enemyPlacementData:
					int areaEntityTableAddrLoc = header.AreaMetadataBase + (areaIndex << 2);
					int areaEntityTableAddr = r.ReadAddr(areaEntityTableAddrLoc);

					int roomEntityTableAddrLoc = areaEntityTableAddr + (roomIndex << 2);
					int roomEntityTableAddr = r.ReadAddr(roomEntityTableAddrLoc);

					//4 byte chunks, 1-3 are unknown use, 4th seems to be chests
					var offset = 0;
					if(type == DataType.chestData)
						offset = 0x0C;
					if(type == DataType.enemyPlacementData)
						offset = 0x08;
					retAddr = roomEntityTableAddr + offset;

                    Console.WriteLine(retAddr);
					break;

				case DataType.warpData:
					int areaWarpTableAddrLoc = header.warpInformationTableLoc + (areaIndex << 2);
					int areaWarpTableAddr = r.ReadAddr(areaWarpTableAddrLoc);

					int roomWarpTableAddrLoc = areaWarpTableAddr + (roomIndex << 2);
					retAddr = roomWarpTableAddrLoc;
					break;

				default:
					break;
			}

			return retAddr;
		}

		//dont have any good names for these 3
		private bool Set1( AddrData data )
		{
			if( (data.dest & 0xF000000) != 0x6000000 )
			{ //not valid tile data addr
				Console.WriteLine( "Unhandled tile data destination address: " + data.dest.Hex() + " Source:" + data.src.Hex() + " Compressed:" + data.compressed + " Size:" + data.size.Hex() );
				return false;
			}

			data.dest = data.dest & 0xFFFFFF;
			this.tileSetAddrs.Add( data );
			return true;
		}

		private bool Set2( AddrData data )
		{
			//use a switch in case this data is out of order
			switch( data.dest )
			{
				case 0x0202CEB4:
					this.bg2MetaTilesAddr = data;
					Debug.WriteLine( data.src.Hex() + " bg2" );
					break;
				case 0x02012654:
					this.bg1MetaTilesAddr = data;
					Debug.WriteLine( data.src.Hex() + " bg1" );
					break;
				//case 0x0202AEB4:
				//ret[8] = source; //not handled
				//break;
				//case 0x02010654:
				//ret[9] = source; //not handled
				//break;
				default:
					Debug.Write( "Unhandled metatile addr: " );
					Debug.Write( data.src.Hex() + "->" + data.dest.Hex() );
					Debug.WriteLine( data.compressed ? " (compressed)" : "" );
					break;
			}
			return true;
		}

		private bool Set3( AddrData data )
		{
			switch( data.dest )
			{
				case 0x02025EB4:
					this.bg2RoomDataAddr = data;
					break;
				case 0x0200B654:
					this.bg1RoomDataAddr = data;
					break;
				case 0x02002F00:
					this.bg1RoomDataAddr = data;
					this.bg1Use20344B0 = true;
					break;
				default:
					Debug.Write( "Unhandled room data addr: " );
					Debug.Write( data.src.Hex() + "->" + data.dest.Hex() );
					Debug.WriteLine( data.compressed ? " (compressed)" : "" );
					break;
			}
			return true;
		}
		
		private bool Bg1Check(AddrData data)
		{
			switch(data.dest)
			{
				case 0x0200B654:
					return false;
				case 0x2002F00:
					return false;
				default:
					break;
			}
			return true;
		}

		private bool Bg2Check(AddrData data)
		{
			switch(data.dest)
			{
				case 0x02025EB4:
					return false;
				default:
					break;
			}
			return true;
		}

		private bool Meta1Check(AddrData data)
		{
			switch(data.dest)
			{
				case 0x02012654:
					return false;
				default:
					break;
			}
			return true;
		}

		private bool Meta2Check(AddrData data)
		{
			switch(data.dest)
			{
				case 0x0202CEB4:
					return false;
				default:
					break;
			}
			return true;
		}
	}
}
