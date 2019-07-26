using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	public class AreaInfoChange: Change
	{
		public AreaInfoChange(int areaId):base(areaId, 0, DataType.areaInfo)
		{
		}

		public override string GetFolderLocation()
		{
			return "/Area "+StringUtil.AsStringHex2(areaId);
		}

		public override string GetEAString(out byte[] binDat)
		{
			var infoLoc = ROM.Instance.headers.areaInformationTableLoc + (4 * areaId);
			var sb = new StringBuilder();
			var areaBytes = MapManager.Instance.MapAreas.Single(a=>a.Index==this.areaId).areaInfo;
			sb.AppendLine("PUSH"); //save cursor location
			sb.AppendLine("ORG "+infoLoc); //go to the area info
			sb.AppendLine("#incbin \"./"+changeType.ToString()+"Dat.bin\"");
			sb.AppendLine("POP"); //return to cursor location
			binDat = areaBytes;
			return sb.ToString();
		}


		public override bool Compare( Change change )
		{
			return change.changeType == changeType && change.areaId == areaId;
		}
	}
}
