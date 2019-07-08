using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	public class AreaInfoChange: PendingChange
	{
		public AreaInfoChange(int areaId):base(areaId, 0, DataType.areaInfo, true)
		{
		}

		public override int GetPointerLoc()
		{
			throw new NotImplementedException(); //shouldnt need to use this as the pointer going to this should not be changed because its being overwritten
		}

		public override int GetOldLocation()
		{
			return ROM.Instance.headers.areaInformationTableLoc + (4 * areaId);
		}

		public override string FolderLocation()
		{
			return "/Area "+StringUtil.AsStringHex2(areaId);
		}

		public override string GetEAString()
		{
			var sb = new StringBuilder();
			var infoLoc = GetOldLocation();
			var areaBytes =ROM.Instance.reader.ReadBytes(4, infoLoc);
			sb.AppendLine("PUSH"); //save cursor location
			sb.AppendLine("ORG "+infoLoc); //go to the area info
			sb.AppendLine("BYTE "+areaBytes[0]+" "+areaBytes[1]+" "+areaBytes[2]+" "+areaBytes[3]); //write the area info bytes
			sb.AppendLine("POP"); //return to cursor location

			return sb.ToString();
		}


		public override bool Compare( PendingChange change )
		{
			return change.changeType == changeType && change.areaId == areaId;
		}
	}
}
