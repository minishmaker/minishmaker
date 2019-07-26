using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	public class RoomMetadataChange: Change
	{
		public RoomMetadataChange(int areaId, int roomId):base(areaId, roomId, DataType.roomMetaData)
		{
		}

		public override string GetFolderLocation()
		{
			return "/Area "+StringUtil.AsStringHex2(areaId)+"/Room " + StringUtil.AsStringHex2(roomId);
		}

		public override string GetEAString(out byte[] binDat)
		{
			
			var room = MapManager.Instance.FindRoom(areaId,roomId);
			var pointerLoc = room.GetPointerLoc(changeType, areaId);

			var sb = new StringBuilder();
			var rect = room.GetMapRect(areaId);
			rect.X*=16;//offset the read at >>4
			rect.Y*=16;
			var metaBytes =new byte[] { (byte)(rect.X&0xff), (byte)(rect.X>>8), (byte)(rect.Y&0xff), (byte)(rect.Y>>8)};
			sb.AppendLine("PUSH"); //save cursor location
			sb.AppendLine("ORG "+pointerLoc); //go to the area info
			sb.AppendLine("#incbin \"./"+changeType.ToString()+"Dat.bin\""); //write the area info bytes
			sb.AppendLine("POP"); //return to cursor location
			binDat = metaBytes;
			return sb.ToString();
		}


		public override bool Compare( Change change )
		{
			return change.changeType == changeType && change.areaId == areaId;
		}
	}
}
