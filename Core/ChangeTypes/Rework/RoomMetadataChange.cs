using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes.Rework
{
    public class RoomMetadataChange : RoomChangeBase
    {
        public RoomMetadataChange(int areaId, int roomId) : base(areaId, roomId, Core.Rework.DataType.roomMetaData)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {

            var room = Utilities.Rework.MapManager.Get().GetRoom(areaId, roomId);
            var pointerLoc = room.GetPointerLoc(this);

            var sb = new StringBuilder();
            byte[] data = new byte[10];
            room.GetSaveData(ref data, this);

            sb.AppendLine("PUSH"); //save cursor location
            sb.AppendLine("ORG " + pointerLoc); //go to the area info
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\""); //write the area info bytes
            sb.AppendLine("POP"); //return to cursor location
            binDat = data;

            return sb.ToString();
        }
    }
}
