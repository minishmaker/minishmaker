using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	public class Bg1DataChange :Change
	{
		public Bg1DataChange(int areaId, int roomId):base(areaId, roomId, DataType.bg1Data)
		{
		}

		public override string GetFolderLocation()
		{
			return "/Area "+StringUtil.AsStringHex2(areaId)+"/Room " + StringUtil.AsStringHex2(roomId);
		}

		public override string GetEAString(out byte[] binDat)
		{
			
			var sb = new StringBuilder();
			var gfxOffset = ROM.Instance.headers.gfxSourceBase;
			var room = MapManager.Instance.FindRoom(areaId,roomId);
			var pointerLoc = room.GetPointerLoc(changeType, areaId);
			byte[] data = null;
			var size = MapManager.Instance.FindRoom(areaId,roomId).GetSaveData(ref data, changeType);

			sb.AppendLine("PUSH");	//save cursor location
			sb.AppendLine("ORG "+pointerLoc);	//go to pointer location
			sb.AppendLine("POIN "+changeType+"x"+areaId.Hex()+"x"+roomId.Hex()+"-"+gfxOffset);	//write label location to position - constant
			sb.AppendLine("ORG currentoffset+4");//move over dest
			sb.AppendLine("WORD "+size);	//write size
			sb.AppendLine("POP");	//go back to cursor location

			sb.AppendLine("ALIGN 4");	//align to avoid a mess
			sb.AppendLine(changeType+"x"+areaId.Hex()+"x"+roomId.Hex()+":"); //create label,  wont need to supply a new position like this (if it has this functionallity like some other patcher)
			sb.AppendLine("#incbin \"./Areas"+GetFolderLocation()+"/"+changeType.ToString()+"Dat.bin\"");
			binDat = data;

			return sb.ToString();
		}

		public override bool Compare( Change change )
		{
			return change.changeType == changeType && change.areaId==areaId && change.roomId==roomId;
		}
	}
}
