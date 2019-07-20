using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	public class Bg1MetaTileSetChange: Change
	{
		public Bg1MetaTileSetChange(int areaId):base(areaId, 0, DataType.bg1MetaTileSet)
		{
		}

		public override string GetFolderLocation()
		{
			return "/Area "+StringUtil.AsStringHex2(areaId);
		}

		public override string GetEAString(out byte[] binDat)
		{
			var sb = new StringBuilder();
			var room = MapManager.Instance.MapAreas.Single(a=>a.Index == areaId).Rooms.First();
			var pointerLoc = room.GetPointerLoc(changeType,areaId);
			var gfxOffset = ROM.Instance.headers.gfxSourceBase;
			byte[] data = null;
			var size = room.GetSaveData(ref data, changeType);
			var bitSet = ROM.Instance.reader.ReadByte(pointerLoc+3)==0x80;

			sb.AppendLine("PUSH");	//save cursor location
			sb.AppendLine("ORG "+pointerLoc);	//go to pointer location
			sb.AppendLine("WORD "+changeType+"x"+areaId.Hex()+"-"+gfxOffset + "+$" + (bitSet?"80000000":"0"));	//write label location to position - constant
			sb.AppendLine("ORG currentoffset+4"); //move over dest
			sb.AppendLine("WORD $"+size.Hex()); //write size
			sb.AppendLine("POP");	//go back to cursor location

			sb.AppendLine("ALIGN 4");	//align to avoid a mess
			sb.AppendLine(changeType+"x"+areaId.Hex()+":"); //create label,  wont need to supply a new position like this
			sb.AppendLine("#incbin \"./"+changeType.ToString()+"Dat.bin\"");
			binDat = data;
			
			return sb.ToString();
		}


		public override bool Compare( Change change )
		{
			return change.changeType == changeType && change.areaId == areaId;
		}
	}
}
