using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	class ChestDataChange : Change
	{
		public ChestDataChange( int areaId, int roomId ) : base( areaId, roomId, DataType.chestData, false )
		{
		}

		public override string GetFolderLocation()
		{
			return "/Area "+StringUtil.AsStringHex2(areaId)+"/Room " + StringUtil.AsStringHex2(roomId);
		}

		public override string GetEAString()
		{
			var sb = new StringBuilder();
			var room = MapManager.Instance.FindRoom(areaId,roomId);
			var pointerLoc = room.GetPointerLoc(changeType, areaId);
			var chestData = MapManager.Instance.FindRoom(areaId,roomId).GetChestData();

			sb.AppendLine("PUSH");//save cursor location
			sb.AppendLine("ORG "+pointerLoc);//go to pointer location
			//write label location to position
			sb.AppendLine("POP");//go back to cursor location
			sb.AppendLine("ALIGN 4");
			sb.AppendLine(changeType+"x"+areaId.Hex()+"x"+roomId.Hex()+":"); //create label
			
			foreach(var chest in chestData)
			{
				var b5 = chest.chestLocation & 0xFF; //first 8
				var b6 = chest.chestLocation >> 8; //last 8
				var b7 = chest.unknown & 0xFF; //first 8
				var b8 = chest.unknown >>8; //last 8
				sb.AppendLine("BYTE "+chest.type+" "+chest.chestId+" "+chest.itemId+" "+chest.itemSubNumber+" "+b5+" "+b6+" "+b7+" "+b8);
			}

			return sb.ToString();
		}

		public override bool Compare( Change change )
		{
			return change.changeType == changeType && change.areaId==areaId && change.roomId==roomId;
		}
	}
}
