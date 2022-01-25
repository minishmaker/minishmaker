using System;
using System.Linq;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes.Rework
{
    public class GlobalChangeBase : Change
    {

        public GlobalChangeBase(Core.Rework.DataType dataType, int identifier = -1) : base(-1, -1, dataType, identifier)
        {
        }

        public override string GetFolderLocation()
        {
            return "";
        }

        public override string GetEAString(out byte[] binDat)
        {
            throw new NotImplementedException();
        }


        public override bool Compare(Change change)
        {
            return change.changeType == changeType;
        }
    }
}
