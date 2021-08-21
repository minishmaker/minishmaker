using System.Linq;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes.Rework
{
    public class AreaInfoChange : AreaChangeBase
    {
        public AreaInfoChange(int areaId) : base(areaId, Core.Rework.DataType.areaInfo)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            var infoLoc = ROM.Instance.headers.areaInformationTableLoc + (4 * areaId);
            var sb = new StringBuilder(); 
            var areaBytes = Utilities.Rework.MapManager.Get().GetArea(areaId).GetInfoBytes();
            sb.AppendLine("PUSH"); //save cursor location
            sb.AppendLine("ORG " + infoLoc); //go to the area info
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\"");
            sb.AppendLine("POP"); //return to cursor location
            binDat = areaBytes;
            return sb.ToString();
        }
    }
}
