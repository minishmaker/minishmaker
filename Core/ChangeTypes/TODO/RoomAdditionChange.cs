using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class RoomAdditionChange : RoomChangeBase
    {
        public RoomAdditionChange(int areaId, int roomId) : base(areaId, roomId, DataType.roomAddition)
        {
        }

        public override string GetFolderLocation()
        {
            return "/Area " + StringUtil.AsStringHex2(areaId) + "/Room " + StringUtil.AsStringHex2(roomId);
        }

        public override string GetEAString(out byte[] binDat)
        {
            var headers = ROM.Instance.headers;
            var r = ROM.Instance.reader;
            int areaRMDTableLoc = r.ReadAddr(headers.MapInfoRoot + (areaId << 2));
            int roomMetaDataTableLoc = areaRMDTableLoc + (roomId * 0x0A);

            int areaTilesetTableLoc = r.ReadAddr(headers.metaTilesetRoot + (areaId << 2));
            int roomTilesetLoc = r.ReadAddr(areaTilesetTableLoc);

            int areaTileDataTableLoc = r.ReadAddr(headers.TileDataRoot + (areaId << 2));
            int tileDataLoc = r.ReadAddr(areaTileDataTableLoc + (roomId << 2));

            int areaEntityTableAddrLoc = headers.ListTableRoot + (areaId << 2);
            int areaEntityTableAddr = r.ReadAddr(areaEntityTableAddrLoc);

            int roomEntityTableAddrLoc = areaEntityTableAddr + (roomId << 2);
            int roomEntityTableAddr = r.ReadAddr(roomEntityTableAddrLoc);

            int areaWarpTableAddrLoc = headers.WarpTableRoot + (areaId << 2);
            int areaWarpTableAddr = r.ReadAddr(areaWarpTableAddrLoc);

            int roomWarpTableAddrLoc = areaWarpTableAddr + (roomId << 2);


            var sb = new StringBuilder();
            var area = Utilities.MapManager.Instance.GetArea(areaId);
            byte[] data = null;

            //TODO: make chart of what data is inside a room to help visualise what i need to do here
            sb.AppendLine("ALIGN 4");
            sb.AppendLine("");
            /*if (pointerLoc != -1)
            {
                sb.AppendLine("PUSH");  //save cursor location
                sb.AppendLine("ORG " + pointerLoc); //go to pointer location
                sb.AppendLine("POIN " + changeType + "x" + areaId.Hex() + "x" + roomId.Hex());  //write label location to position
                sb.AppendLine("POP");   //go back to cursor location
            }*/

            sb.AppendLine("ALIGN 4");   //align to avoid a mess
            sb.AppendLine(changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + ":"); //create label,  wont need to supply a new position like this (if it has this functionallity like some other patcher)
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\"");
            binDat = data;

            return sb.ToString();
        }

        public override bool Compare(Change change)
        {
            return change.changeType == changeType && change.areaId == areaId && change.roomId == roomId;
        }
    }
}
