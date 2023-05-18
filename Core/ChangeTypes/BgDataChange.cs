using System.Text;
using System;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class BgDataChange : RoomChangeBase
    {
        public BgDataChange(int areaId, int roomId, int bgNum) : base(areaId, roomId, DataType.bgData, bgNum)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            var sb = new StringBuilder();
            var gfxOffset = ROM.Instance.headers.gfxSourceBase;
            var room = MapManager.Instance.GetRoom(areaId, roomId);
            var pointerLoc = room.MetaData.GetBGPointerLoc(this);
            byte[] data = null;
            var size = room.GetBGData(ref data, this.identifier);
            var bitSet = ROM.Instance.reader.ReadByte(pointerLoc + 3) == 0x80;

            sb.AppendLine("PUSH");  //save cursor location
            if (pointerLoc == -1)
            {
                sb.AppendLine("ORG " + DataType.roomAddition  + "x" + areaId.Hex() + "x" + roomId.Hex() + "x" + changeType);
            }
            else
            {
                sb.AppendLine("ORG " + pointerLoc); //go to pointer location
            }
            sb.AppendLine("WORD " + changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + "x" + identifier + "-" + gfxOffset + "+$" + (bitSet ? "80000000" : "0"));   //write label location to position - constant
            sb.AppendLine("ORG currentoffset+4");//move over dest
            sb.AppendLine("WORD $" + size.Hex());   //write size
            sb.AppendLine("POP");   //go back to cursor location

            sb.AppendLine("ALIGN 4");   //align to avoid a mess
            sb.AppendLine(changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + "x" + identifier + ":"); //create label,  wont need to supply a new position like this (if it has this functionallity like some other patcher)
            sb.AppendLine("#incbin \"./" + changeType.ToString() + identifier.Hex(2) + "Dat.bin\"");
            binDat = data;

            return sb.ToString();
        }

        public override string GetJSONString()
        {
            var room = MapManager.Instance.GetRoom(areaId, roomId);
            return room.GetBGJSON(this);
        }
    }
}
