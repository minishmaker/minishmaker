using System;
using System.Linq;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class RoomChangeBase : Change
    {

        public RoomChangeBase(int areaId, int roomId, DataType dataType, int identifier = -1) : base(areaId, roomId, dataType, identifier)
        {
        }

        public override string GetFolderLocation()
        {
            return $"/Areas/Area {StringUtil.AsStringHex2(areaId)}/Room {StringUtil.AsStringHex2(roomId)}";
        }

        public override string GetEAString(out byte[] binDat)
        {
            throw new NotImplementedException();
        }

        public override string GetJSONString()
        {
            throw new NotImplementedException();
        }

        public override void EnsureLoaded()
        {
            MapManager.Instance.GetRoom(areaId, roomId).LoadRoom();
        }

        public override bool Compare(Change change)
        {
            return change.changeType == changeType 
            && change.areaId == areaId 
            && change.roomId == roomId 
            && change.identifier == identifier;
        }
    }
}
