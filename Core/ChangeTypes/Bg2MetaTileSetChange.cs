using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	public class Bg2MetaTileSetChange: PendingChange
	{
		public Bg2MetaTileSetChange(int areaId):base(areaId, 0, DataType.bg2MetaTileSet, false)
		{
		}

		public override int GetPointerLoc()
		{
			var room = MapManager.Instance.MapAreas.Single(a=>a.Index == areaId).Rooms.First();
			return room.GetPointerLoc(changeType,areaId);
		}

		public override int GetOldLocation()
		{
			throw new NotImplementedException();
		}

		public override string FolderLocation()
		{
			return "/Area "+StringUtil.AsStringHex2(areaId);
		}

		public override string GetEAString()
		{
			var sb = new StringBuilder();
			var pointerLoc = GetPointerLoc();
			var gfxOffset = ROM.Instance.headers.gfxSourceBase;
			byte[] data = null;
			var size = MapManager.Instance.MapAreas.Single(a=>a.Index == areaId).Rooms.First().GetSaveData(ref data, changeType);
			
			sb.AppendLine("PUSH");	//save cursor location
			sb.AppendLine("ORG "+pointerLoc);	//go to pointer location
			sb.AppendLine("POIN "+changeType+"x"+areaId.Hex()+"-"+gfxOffset.Hex());	//write label location to position - constant
			sb.AppendLine("ORG currentoffset+4"); //move over dest
			sb.AppendLine("WORD "+size); //write size
			sb.AppendLine("POP");	//go back to cursor location

			sb.AppendLine("ALIGN 4");	//align to avoid a mess
			sb.AppendLine(changeType+"x"+areaId.Hex()+":"); //create label,  wont need to supply a new position like this
			sb.Append("BYTE ");
			foreach(var dbyte in data) //write all bytes
			{
				sb.AppendLine(dbyte+" ");
			}
			
			return sb.ToString();
		}


		public override bool Compare( PendingChange change )
		{
			return change.changeType == changeType && change.areaId == areaId;
		}
	}
}
