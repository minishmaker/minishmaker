using System;

namespace MinishMaker.Core.Rework
{
    public class MetaTileSet
    {
        private byte[] metaTileSetData;
        private byte[] metaTileTypeData;
        public bool IsBg1 { get; private set; }
        public MetaTileSet(Core.AddrData addrData, Core.AddrData metaTypes, bool isBg1, string filePath)
        {
            var setPath = filePath;
            var typePath = filePath;
            if (isBg1)
            {
                setPath += "/" + DataType.bgMetaTileSet + "1Dat.bin";
                typePath += "/" + DataType.bgMetaTileType + "1Dat.bin";
            }
            else
            {
                setPath += "/" + DataType.bgMetaTileSet + "2Dat.bin";
                typePath += "/" + DataType.bgMetaTileType + "2Dat.bin";
            }

            metaTileSetData = DataHelper.GetFromSavedData(setPath, true, addrData.size);
            metaTileTypeData = DataHelper.GetFromSavedData(typePath, true, metaTypes.size);

            if (metaTileSetData == null)
            {
                metaTileSetData = DataHelper.GetData(addrData);
            }
            if (metaTileTypeData == null)
            {
                metaTileTypeData = DataHelper.GetData(metaTypes);
            }
            Console.WriteLine("types" + metaTileTypeData.Length);
            Console.WriteLine("tiles" + metaTileSetData.Length);
            this.IsBg1 = isBg1;
        }

        public byte[] GetTileImageInfo(int tileNum)
        {
            try
            {
                byte[] tileData = new byte[8];
                //write 8 bytes from tileId *8 to tileData
                Array.Copy(metaTileSetData, tileNum << 3, tileData, 0, 8);
                return tileData;
            }
            catch
            {
                return null;
            }
        }

        public void SetTileImageInfo(byte[] tileInfo, int tileNum)
        {
            if (tileInfo.Length != 8)
                return;

            var tileStart = tileNum << 3;
            metaTileSetData[tileStart] = tileInfo[0];
            metaTileSetData[tileStart + 1] = tileInfo[1];
            metaTileSetData[tileStart + 2] = tileInfo[2];
            metaTileSetData[tileStart + 3] = tileInfo[3];
            metaTileSetData[tileStart + 4] = tileInfo[4];
            metaTileSetData[tileStart + 5] = tileInfo[5];
            metaTileSetData[tileStart + 6] = tileInfo[6];
            metaTileSetData[tileStart + 7] = tileInfo[7];
        }

        public byte[] GetTileTypeInfo(int tileNum)
        {
            try
            {
                byte[] tileData = new byte[2];
                Array.Copy(metaTileTypeData, tileNum << 1, tileData, 0, 2);
                return tileData;
            }
            catch
            {
                return null;
            }
        }

        public void SetTileTypeInfo(byte[] tileType, int tileNum)
        {
            if (tileType.Length != 2)
                return;

            var tileStart = tileNum << 1;
            metaTileTypeData[tileStart] = tileType[0];
            metaTileTypeData[tileStart + 1] = tileType[1];
        }
        
        public long CompressMetaTileSet(ref byte[] outdata)
        {
            return DataHelper.CompressData(ref outdata, metaTileSetData);
        }

        public long CompressMetaTileTypes(ref byte[] outdata)
        {
            return DataHelper.CompressData(ref outdata, metaTileTypeData);
        }
    }
}
