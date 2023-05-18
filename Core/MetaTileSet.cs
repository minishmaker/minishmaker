using System;
using System.Diagnostics;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
    public class MetaTileset
    {
        private byte[] MetaTilesetData { get; set; }

        public int dataSize { get { return MetaTilesetData.Length; } }
        private byte[] TileTypeData { get; set; }
        public bool IsBg1 { get; private set; }
        public MetaTileset(AddrData tileImages, AddrData tileTypes, bool isBg1, string filePath)
        {
            var setPath = filePath + "/" + DataType.metaTileset;
            var typePath = filePath + "/" + DataType.metaTileType;
            if (isBg1)
            {
                setPath += "01";
                typePath += "01";
            }
            else
            {
                setPath += "02";
                typePath += "02";
            }
            setPath += ".json";
            typePath += ".json";
            //setPath += "Dat.bin";
            //typePath += "Dat.bin";

            //MetaTileSetData = DataHelper.GetFromSavedData(setPath, true, addrData.size);
            //MetaTileTypeData = DataHelper.GetFromSavedData(typePath, true, metaTypes.size);
            MetaTilesetData = DataHelper.GetByteArrayFromJSON(setPath, 8);
            TileTypeData = DataHelper.GetByteArrayFromJSON(typePath, 2);
            if (MetaTilesetData == null)
            {
                MetaTilesetData = DataHelper.GetData(tileImages);
            }
            if (TileTypeData == null)
            {
                TileTypeData = DataHelper.GetData(tileTypes);
            }
            //Console.WriteLine("types:" + TileTypeData.Length);
            //Console.WriteLine("tiles:" + MetaTilesetData.Length);
            this.IsBg1 = isBg1;
        }

        public byte[] GetMetaTileData(int tileNum)
        {
            try
            {
                byte[] tileData = new byte[8];
                //write 8 bytes from tileId *8 to tileData
                Array.Copy(MetaTilesetData, tileNum << 3, tileData, 0, 8);
                return tileData;
            }
            catch
            {
                //Debug.WriteLine("Exception fetching tile data:" + tileNum.Hex());
                return null;
            }
        }

        public void SetMetaTileData(byte[] tileInfo, int tileNum)
        {
            if (tileInfo.Length != 8)
                return;

            var tileStart = tileNum << 3;
            MetaTilesetData[tileStart] = tileInfo[0];
            MetaTilesetData[tileStart + 1] = tileInfo[1];
            MetaTilesetData[tileStart + 2] = tileInfo[2];
            MetaTilesetData[tileStart + 3] = tileInfo[3];
            MetaTilesetData[tileStart + 4] = tileInfo[4];
            MetaTilesetData[tileStart + 5] = tileInfo[5];
            MetaTilesetData[tileStart + 6] = tileInfo[6];
            MetaTilesetData[tileStart + 7] = tileInfo[7];
        }

        public byte[] GetMetaTileTypeData(int tileNum)
        {
            try
            {
                byte[] tileData = new byte[2];
                Array.Copy(TileTypeData, tileNum << 1, tileData, 0, 2);
                return tileData;
            }
            catch
            {
                return null;
            }
        }

        public void SetMetaTileTypeData(byte[] tileType, int tileNum)
        {
            if (tileType.Length != 2)
                return;

            var tileStart = tileNum << 1;
            TileTypeData[tileStart] = tileType[0];
            TileTypeData[tileStart + 1] = tileType[1];
        }
        
        public string SaveSetToJSON()
        {
            return DataHelper.ByteArrayToFormattedJSON(MetaTilesetData, 16, 8);
        }

        public string SaveTypesToJSON()
        {
            return DataHelper.ByteArrayToFormattedJSON(TileTypeData, 16, 2);
        }

        public long CompressTileset(ref byte[] outdata)
        {
            return DataHelper.CompressData(ref outdata, MetaTilesetData);
        }

        public long CompressTileTypes(ref byte[] outdata)
        {
            return DataHelper.CompressData(ref outdata, TileTypeData);
        }
    }
}
