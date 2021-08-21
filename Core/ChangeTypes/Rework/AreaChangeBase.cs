using System.Linq;
using System;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes.Rework
{
    public class AreaChangeBase : Change
    {
        public AreaChangeBase(int areaId, Core.Rework.DataType dataType, int identifier = -1) : base(areaId, 0, dataType, identifier)
        {
        }

        public override string GetFolderLocation()
        {
            return "/Area " + StringUtil.AsStringHex2(areaId);
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
    }
}
