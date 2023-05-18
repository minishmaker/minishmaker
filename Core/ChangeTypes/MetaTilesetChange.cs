using System;
using System.Linq;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class MetaTilesetChange : AreaChangeBase
    {
        public MetaTilesetChange(int areaId, int bgNum) : base(areaId, DataType.metaTileset, bgNum)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            var sb = new StringBuilder();
            var area = MapManager.Instance.GetArea(areaId);
            Console.WriteLine("Checking Set");
            var pointerLoc = area.GetParsedPointerLoc(this);
            var gfxOffset = ROM.Instance.headers.gfxSourceBase;
            byte[] data = null;
            var size = area.GetCompressedMetaTiles(ref data, this.identifier);
            var bitSet = ROM.Instance.reader.ReadByte(pointerLoc + 3) == 0x80;

            sb.AppendLine("PUSH");  //save cursor location
            sb.AppendLine("ORG " + pointerLoc); //go to pointer location
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
            if(identifier == 1)
            {
                return area.Bg1MetaTileset.SaveSetToJSON();
            }
            return area.Bg2MetaTileset.SaveSetToJSON();
        }
    }
}
