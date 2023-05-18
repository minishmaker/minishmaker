using System.Linq;
using System;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class AreaChangeBase : Change
    {
        public AreaChangeBase(int areaId, DataType dataType, int identifier = -1) : base(areaId, 0, dataType, identifier)
        {
        }

        public override string GetFolderLocation()
        {
            return $"/Areas/Area {StringUtil.AsStringHex2(areaId)}";
        }

        public override string GetEAString(out byte[] binDat)
        {
            throw new NotImplementedException();
        }

        public override bool Compare(Change change)
        {
            return change.changeType == changeType 
            && change.areaId == areaId 
            && change.identifier == identifier;
        }

        public override string GetJSONString()
        {
            throw new NotImplementedException();
        }

        public override void EnsureLoaded()
        {
            //just getting the area in most cases loads everything needed, might need to change that if load times get long on startup
        }
    }
}
