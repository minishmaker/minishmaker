using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	public abstract class Change
	{
		public int areaId;
		public int roomId;
		public DataType changeType; //used in comparison

		public Change(int areaId, int roomId, DataType changeType)
		{
			this.areaId = areaId;
			this.roomId = roomId;
			this.changeType = changeType;
		}

		public abstract string GetFolderLocation(); //where does the file that is written need to go

		public abstract bool Compare( Change change ); //test if a change is the same as another

		public abstract string GetEAString(out byte[] binDat); //Get EA string to be written
	}
}
