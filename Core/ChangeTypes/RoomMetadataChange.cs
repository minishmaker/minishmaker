using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class RoomMetadataChange : Change
    {
        public RoomMetadataChange(int areaId, int roomId) : base(areaId, roomId, DataType.roomMetaData)
        {
        }

        public override string GetFolderLocation()
        {
            return "/Area " + StringUtil.AsStringHex2(areaId) + "/Room " + StringUtil.AsStringHex2(roomId);
        }

        public override string GetEAString(out byte[] binDat)
        {

            var room = MapManager.Instance.FindRoom(areaId, roomId);
            var pointerLoc = room.GetPointerLoc(changeType, areaId);

            var sb = new StringBuilder();
            byte[] data = new byte[10];
            room.GetSaveData(ref data, this.changeType);

            sb.AppendLine("PUSH"); //save cursor location
            sb.AppendLine("ORG " + pointerLoc); //go to the area info
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\""); //write the area info bytes
            sb.AppendLine("POP"); //return to cursor location
            binDat = data;

            return sb.ToString();
        }


        public override bool Compare(Change change)
        {
            return change.changeType == changeType && change.areaId == areaId;
        }
    }
}
