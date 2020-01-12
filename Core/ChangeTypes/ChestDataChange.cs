using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
    class ChestDataChange : Change
    {
        public ChestDataChange(int areaId, int roomId) : base(areaId, roomId, DataType.chestData)
        {
        }

        public override string GetFolderLocation()
        {
            return "/Area " + StringUtil.AsStringHex2(areaId) + "/Room " + StringUtil.AsStringHex2(roomId);
        }

        public override string GetEAString(out byte[] binDat)
        {
            var sb = new StringBuilder();
            var room = MapManager.Instance.FindRoom(areaId, roomId);
            var pointerLoc = room.GetPointerLoc(changeType, areaId);
            var chestData = MapManager.Instance.FindRoom(areaId, roomId).GetChestData();

            sb.AppendLine("PUSH");//save cursor location
            sb.AppendLine("ORG " + pointerLoc);//go to pointer location
            sb.AppendLine("POIN " + changeType + "x" + areaId.Hex() + "x" + roomId.Hex()); //write label location to position
            sb.AppendLine("POP");//go back to cursor location
            sb.AppendLine("ALIGN 4");
            sb.AppendLine(changeType + "x" + areaId.Hex() + "x" + roomId.Hex() + ":"); //create label
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\"");

            var index = 0;
            var data = new byte[chestData.Count * 8 + 8];
            foreach (var chest in chestData)
            {
                data[0 + index] = chest.type;
                data[1 + index] = chest.chestId;
                data[2 + index] = chest.itemId;
                data[3 + index] = chest.itemSubNumber;
                data[4 + index] = (byte)(chest.chestLocation & 0xFF);
                data[5 + index] = (byte)(chest.chestLocation >> 8);
                data[6 + index] = (byte)(chest.unknown & 0xFF);
                data[7 + index] = (byte)(chest.unknown >> 8);
                index += 8;
            }

            data[0 + index] = 0; //terminator
            data[1 + index] = 0;
            data[2 + index] = 0;
            data[3 + index] = 0;
            data[4 + index] = 0;
            data[5 + index] = 0;
            data[6 + index] = 0;
            data[7 + index] = 0;

            binDat = data;
            return sb.ToString();
        }

        public override bool Compare(Change change)
        {
            return change.changeType == changeType && change.areaId == areaId && change.roomId == roomId;
        }
    }
}
