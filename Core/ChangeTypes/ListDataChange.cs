using System;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class ListDataChange : RoomChangeBase
    {
        public ListDataChange(int areaId, int roomId, int listNum) : base(areaId, roomId, DataType.listData, listNum)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            var sb = new StringBuilder();
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            var room = MapManager.Instance.GetRoom(areaId, roomId);

            int areaEntityTableAddrLoc = header.ListTableRoot + (areaId << 2);
            int areaEntityTableAddr = r.ReadAddr(areaEntityTableAddrLoc);

            int roomEntityTableAddrLoc = areaEntityTableAddr + (roomId << 2);
            int roomEntityTableAddr = r.ReadAddr(roomEntityTableAddrLoc);

            var offset = identifier * 4;
            var pointerLoc = roomEntityTableAddr + offset;

            byte[] data;
            byte linkId;
            var addr = room.MetaData.BytifyListInformation(out data, out linkId, identifier);
            binDat = data;

            sb.AppendLine("PUSH");  //save cursor location
            sb.AppendLine("ORG " + pointerLoc); //go to pointer location
            if (linkId != 0xFF)
            {
                sb.AppendLine("#ifdef " + changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + "x" + linkId);
                sb.AppendLine("POIN " + changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + "x" + linkId);  //write other list label location to position
                sb.AppendLine("#else");
                sb.AppendLine("POIN 0x" + addr.Hex()); //if its vanilla data, use direct address
                sb.AppendLine("#endif");
                sb.AppendLine("POP");   //go back to cursor location
                return sb.ToString();
            }
            sb.AppendLine("POIN " + changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + "x" + identifier);  //write other list label location to position
            sb.AppendLine("POP");   //go back to cursor location

            sb.AppendLine("ALIGN 4");   //align to avoid a mess
            sb.AppendLine(changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + "x" + identifier+ ":"); //create label,  wont need to supply a new position like this (if it has this functionallity like some other patcher)
            sb.AppendLine("#incbin \"./" + changeType.ToString() + identifier.Hex(2) + "Dat.bin\"");

            return sb.ToString();
        }

        public override string GetJSONString()
        {
            var room = MapManager.Instance.GetRoom(areaId, roomId);
            return room.MetaData.SerializeList(this);
        }
        //case1: vanilla rom, change list that has links to it, also calls a change to the linked lists, everything is fine
        //case2: vanilla rom, create new list, set as linked to other, everything is fine
        //case3: above link is to a link, jump ahead? everything is fine as long as you dont loop
        //case4: prev version and change list, addr still gets loaded, link set correctly on other rooms, gets a change if a file is found
    }
}
