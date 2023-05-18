using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class RoomMetadataChange : RoomChangeBase
    {
        public RoomMetadataChange(int areaId, int roomId) : base(areaId, roomId, DataType.roomMetaData)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {

            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            int mapInfoArea = r.ReadAddr(header.MapInfoRoot + (areaId << 2));
            int pointerLoc = mapInfoArea + (roomId * 0x0A);

            var room = MapManager.Instance.GetRoom(areaId, roomId);
            var sb = new StringBuilder();
            var data = room.MetaData.GetMetadataBytes();

            sb.AppendLine("PUSH"); //save cursor location
            sb.AppendLine("ORG " + pointerLoc); //go to the area info
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\""); //write the area info bytes
            sb.AppendLine("POP"); //return to cursor location
            binDat = data;

            return sb.ToString();
        }

        public override string GetJSONString()
        {
            return MapManager.Instance.GetRoom(areaId, roomId).MetaData.SerializeMetaData();
        }
    }
}
