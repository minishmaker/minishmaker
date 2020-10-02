using MinishMaker.Utilities;
using System.Linq;
using System.Text;

namespace MinishMaker.Core.ChangeTypes
{
	public class PaletteChange: Change
	{
		public PaletteChange(int areaId):base(areaId, 0, DataType.palette)
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
			var tileOffset = ROM.Instance.headers.tileOffset;
			byte[] data = null;
			var size = room.GetSaveData(ref data, changeType);
            //var junkLength = tileOffset % 32;

			sb.AppendLine("PUSH");	//save cursor location
			sb.AppendLine("ORG 0x"+pointerLoc.Hex());	//go to pointer location
            sb.AppendLine("#incbin \"./" + changeType.ToString() + "Dat.bin\""); //overwritten because limits
            //sb.AppendLine("SHORT ("+changeType+"x"+areaId.Hex()+"-"+ tileOffset + ")>> 5");	//write label location to position - constant
			sb.AppendLine("POP");	//go back to cursor location

			//sb.AppendLine("ALIGN 32");	//because thats apperantly the minimum distance (<<5)
            //for(int i = 0; i < junkLength; i++) {
            //    sb.AppendLine("BYTE 0");
            //}

			//sb.AppendLine(changeType+"x"+areaId.Hex()+":"); //create label, wont need to supply a new position like this
			//sb.AppendLine("#incbin \"./"+changeType.ToString()+"Dat.bin\"");
			binDat = data;
			
			return sb.ToString();
		}


		public override bool Compare( Change change )
		{
			return change.changeType == changeType && change.areaId == areaId;
		}
	}
}
