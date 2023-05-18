using System.Linq;
using System.Text;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class AreaInfoChange : AreaChangeBase
    {
        public AreaInfoChange(int areaId) : base(areaId, DataType.areaInfo)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            var infoLoc = ROM.Instance.headers.areaInformationTableLoc + (4 * areaId);
            var sb = new StringBuilder(); 
            var areaBytes = MapManager.Instance.GetArea(areaId).GetInfoBytes();
            sb.AppendLine("PUSH"); //save cursor location
            sb.AppendLine("ORG " + infoLoc); //go to the area info
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\"");
            sb.AppendLine("POP"); //return to cursor location
            binDat = areaBytes;
            return sb.ToString();
        }

        public override void EnsureLoaded()
        {
            MapManager.Instance.GetArea(areaId).LoadAreaInfo();
        }

        public override string GetJSONString()
        {
            return MapManager.Instance.GetArea(areaId).GetInfoJSON();
        }
    }
}
