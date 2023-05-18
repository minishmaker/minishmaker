using System;
using System.Linq;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class GlobalChangeBase : Change
    {

        public GlobalChangeBase(DataType dataType, int identifier = -1) : base(-1, -1, dataType, identifier)
        {
        }

        public override string GetFolderLocation()
        {
            return $"/Global/{this.changeType}";
        }

        public override string GetEAString(out byte[] binDat)
        {
            throw new NotImplementedException();
        }


        public override bool Compare(Change change)
        {
            return change.changeType == changeType
            && change.identifier == identifier;
        }

        public override string GetJSONString()
        {
            throw new NotImplementedException();
        }

        public override void EnsureLoaded()
        {
            throw new NotImplementedException();
        }
    }
}
