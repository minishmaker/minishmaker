using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes.Rework
{
    public class WarpDataChange : RoomChangeBase
    {
        public WarpDataChange(int areaId, int roomId) : base(areaId, roomId, Core.Rework.DataType.warpData)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            var sb = new StringBuilder();
            var room = Utilities.Rework.MapManager.Get().GetRoom(areaId, roomId);
            var pointerLoc = room.GetPointerLoc(this);
            byte[] data = null;
            var size = room.GetSaveData(ref data, this);

            sb.AppendLine("PUSH");  //save cursor location
            sb.AppendLine("ORG " + pointerLoc); //go to pointer location
            sb.AppendLine("POIN " + changeType + "x" + areaId.Hex() + "x" + roomId.Hex());  //write label location to position
            sb.AppendLine("POP");   //go back to cursor location

            sb.AppendLine("ALIGN 4");   //align to avoid a mess
            sb.AppendLine(changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + ":"); //create label,  wont need to supply a new position like this (if it has this functionallity like some other patcher)
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\"");
            binDat = data;

            return sb.ToString();
        }
    }
}
