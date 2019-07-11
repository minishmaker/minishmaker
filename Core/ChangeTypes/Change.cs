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
		public bool overwriting; //does it require a new spot in ROM or does it overwrite its existing spot

		public Change(int areaId, int roomId, DataType changeType, bool overwriting)
		{
			this.areaId = areaId;
			this.roomId = roomId;
			this.changeType = changeType;
			this.overwriting = overwriting;
		}

		public abstract string GetFolderLocation(); //where does the file that is written need to go

		public abstract bool Compare( Change change ); //test if a change is the same as another

		public abstract string GetEAString(); //Get EA string to be written
	}
}
