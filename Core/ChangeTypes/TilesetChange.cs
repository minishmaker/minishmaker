using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MinishMaker.Core.Tileset;

namespace MinishMaker.Core.ChangeTypes
{
    public class TilesetChange : AreaChangeBase
    {
        public TilesetChange(int areaId, int tsetIdAndType) : base(areaId, DataType.tileset, tsetIdAndType)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            var sb = new StringBuilder();
            var area = MapManager.Instance.GetArea(areaId);
            var pointerLoc = area.GetParsedPointerLoc(this);
            var gfxOffset = ROM.Instance.headers.gfxSourceBase;
            byte[] data = null;
            int type = identifier % 10;
            int id = identifier / 10;
            var size = area.Tilesets[id].GetCompressedTilesetData(ref data, (TilesetDataType)type);
            var bitSet = ROM.Instance.reader.ReadByte(pointerLoc + 3) == 0x80;

            //TODO: pointerloc ok? change not replicating

            sb.AppendLine("PUSH");  //save cursor location
            sb.AppendLine("ORG " + pointerLoc); //go to pointer location
            Console.WriteLine(pointerLoc.Hex());
            sb.AppendLine("WORD " + changeType + "x" + areaId.Hex() + "x" + identifier + "-" + gfxOffset + "+$" + (bitSet ? "80000000" : "0"));    //write label location to position - constant
            sb.AppendLine("ORG currentoffset+4"); //move over dest
            sb.AppendLine("WORD $" + size.Hex()); //write size
            sb.AppendLine("POP");   //go back to cursor location

            sb.AppendLine("ALIGN 4");   //align to avoid a mess
            sb.AppendLine(changeType + "x" + areaId.Hex() + "x" + identifier + ":"); //create label,  wont need to supply a new position like this
            sb.AppendLine("#incbin \"./" + changeType.ToString() + identifier.Hex(2) + "Dat.bin\"");
            binDat = data;

            return sb.ToString();
        }

        public override string GetJSONString()
        {
            var area = MapManager.Instance.GetArea(areaId);
            int type = identifier % 10;
            int id = identifier / 10;
            var data = area.Tilesets[id].GetTilesetData((TilesetDataType)type);
            var json = DataHelper.ByteArrayToFormattedJSON2(data, 32, 32);
            return json;
        }
    }
}
