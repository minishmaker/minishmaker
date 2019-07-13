using MinishMaker.Core.ChangeTypes;
using MinishMaker.UI;
using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core
{
    public enum DataType
    {
        bg1Data,
        bg2Data,
        roomMetaData,
        tileSet,
        bg1MetaTileSet,
		bg2MetaTileSet,
        chestData,
		areaInfo
	}

    /// <summary>
    /// Stores configuration for a minishmaker project
    /// </summary>
    public class Project
    {
        public static Project Instance;

        //public MapManager mapManager;

        public string sourcePath;
        public string exportPath;
        public string projectPath;

		private List<Change> loadedChanges;
		private StreamWriter mainWriter;

        public Project()
        {
			loadedChanges = new List<Change>();
            Instance = this;
            //projectPath = "";

            //sourcePath = baseRom.path;
            //projectPath = Path.GetDirectoryName(sourcePath);
            //exportPath = projectPath + "/mc-hack.gba";
            //mapManager = manager;
            //LoadProject();
        }

		public void CreateProject(string directory)
		{
			if(!File.Exists(projectPath+"/Main.event"))
			{
				var file = File.Create(projectPath+"/Main.event");
				file.Dispose();
			}
		}

		public void RecheckProject()
		{
			var mainSets = File.ReadAllLines(projectPath+"/Main.event").ToList();

			mainSets = mainSets.Select(s => s.Substring(4)).ToList();
		}



		private void AddLoadedChange(Change change)
		{
			if(!loadedChanges.Any(x=>x.Compare(change)))
				loadedChanges.Add(change);
		}



        public void LoadProject()
        {
			var exeFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
			var lines = new string[2];
			lines[0] = "romFile="+sourcePath;
			lines[1] = "projectFolder="+projectPath;
			File.WriteAllLines(exeFolder+"\\Settings.cfg",lines);

			loadedChanges.Clear();
			var pos = ROM.Instance.romData.Length-1;

			while(ROM.Instance.romData[pos]==0xFF)
			{
				pos--;
			}

            if (!Directory.Exists(projectPath))
            {
                Directory.CreateDirectory(projectPath);
            }

			if(!File.Exists(projectPath+"/Main.event"))
			{
				var file = File.Create(projectPath+"/Main.event");
				using( StreamWriter s = new StreamWriter( file ) )
				{
					s.WriteLine("ORG "+(pos+1));
				}
				file.Dispose();
			}

			var mainSets = File.ReadAllLines(projectPath+"/Main.event").ToList();
			mainSets[0]= "ORG "+(pos+1);
			File.WriteAllLines(projectPath+"/Main.event",mainSets);
			mainSets.RemoveAt(0);
			StartSave();
			
			mainSets = mainSets.Select(s => s.Substring(10).TrimEnd('\"').Replace('/','\\')).ToList();
			
			if (Directory.Exists(projectPath + "\\Areas"))
            {
                string[] areaDirectories = Directory.GetDirectories(projectPath + "\\Areas");

                foreach (string areaDirectory in areaDirectories)
                {
                    int areaIndex = Convert.ToInt32(areaDirectory.Substring(areaDirectory.Length - 2), 16);

					string[] areaFiles = Directory.GetFiles(areaDirectory,"*.event");
					foreach (var file in areaFiles)
					{
						DataType type;
						var success = Enum.TryParse(Path.GetFileNameWithoutExtension(file), out type);

						if(success)
						{ 
							var change = CreateChange(type ,areaIndex,0);

							if(mainSets.Contains(file))
							{
								mainSets.Remove(file);
							}
							else
							{ 
								mainWriter.WriteLine("#include \"/Areas"+change.GetFolderLocation()+"/"+change.changeType.ToString()+".event\"");
							}
							loadedChanges.Add(change);
						}
						else
						{
							Debug.WriteLine("unknown file found: "+file);
						}
					}

                    string[] roomDirectories = Directory.GetDirectories(areaDirectory);

                    foreach (string roomDirectory in roomDirectories)
                    {
                        int roomIndex = Convert.ToInt32(roomDirectory.Substring(roomDirectory.Length - 2), 16);

                        string[] roomFiles = Directory.GetFiles(roomDirectory,"*.event");
						foreach (var file in roomFiles)
						{
							DataType type;
							var success = Enum.TryParse(Path.GetFileNameWithoutExtension(file), out type);
	
							if(success)
							{ 
								var change = CreateChange(type, areaIndex, roomIndex);
								var entry = mainSets.SingleOrDefault(x=>file.Contains(x));
								if(entry!=null)
								{
									mainSets.Remove(file);
								}
								else
								{ 
									mainWriter.WriteLine("#include \"/Areas"+change.GetFolderLocation()+"/"+change.changeType.ToString()+".event\"");
								}
								loadedChanges.Add(change);
							}
							else
							{
								Debug.WriteLine("unknown file found: "+file);
							}
						}
                    }
                }
            }
			EndSave();

			if(mainSets.Count!=0)
			{
				CleanIncludes(mainSets);
			}
			ApplyMainEventPatch();

        }

		private void CleanIncludes(List<string> remaining)
		{
			var text = File.ReadAllText(projectPath+"/Main.event");

			foreach(var line in remaining)
			{
				var pos = text.IndexOf(line);
				if(pos == -1)
				{
					Debug.WriteLine("Didnt find include line: "+ line);
				}
				else
				{
					var length = line.Length+9; //#include + space
					text.Remove(pos-9,length);
				}

				File.WriteAllText(projectPath+"/Main.event",text);
			}
		}

		private void ApplyMainEventPatch()
		{
			//TODO: add EA integration to modify the ROM byte[] to show changes already made
		}


		private Change CreateChange(DataType type, int area, int room)
		{
			switch (type)
			{
				case DataType.areaInfo:
					return new AreaInfoChange( area );
				case DataType.bg1Data:
					return new Bg1DataChange( area, room );
				case DataType.bg1MetaTileSet:
					return new Bg1MetaTileSetChange( area );
				case DataType.bg2Data:
					return new Bg2DataChange( area, room );
				case DataType.bg2MetaTileSet:
					return new Bg2MetaTileSetChange( area );
				case DataType.chestData:
					return new ChestDataChange( area, room );
				default:
					Debug.WriteLine("unknown file found of type: "+type);
					return null;
			}
		}
		public void StartSave()
		{
			mainWriter = File.AppendText(projectPath+"/Main.event");
		}

		public void EndSave()
		{
			mainWriter.Dispose();
		}

        public void SaveChange(Change change)
        {
            var folderLoc = projectPath+"/Areas"+change.GetFolderLocation();
			var fileName = change.changeType.ToString() +".event";
			byte[] binData;
			var content = change.GetEAString(out binData);
			Directory.CreateDirectory(folderLoc);
			File.WriteAllText(folderLoc+"/"+fileName, content);

			if(binData!=null)
			{
				File.WriteAllBytes(folderLoc+"/"+change.changeType.ToString()+"Dat.bin", binData);
			}

			if(!loadedChanges.Any(x=>x.Compare(change))) //change not yet already written
				mainWriter.WriteLine("#include \"/Areas"+change.GetFolderLocation()+"/"+fileName+"\"");
        }


		//TO BE REMOVED?
		/*
        public bool ExportToRom()
        {
            int newSource = 0xEF3340;

            foreach (ModifiedData data in modifiedData)
            {
                var room = mapManager.FindRoom(data.areaIndex, data.roomIndex);

                if (!room.Loaded)
                {
                    room.LoadRoom(data.areaIndex);
                }

                byte[] saveData = null;
                long pointerAddress = room.GetPointerLoc(data.dataType, data.areaIndex);
                long size = room.GetSaveData(ref saveData, data.dataType);
                ROM.Instance.WriteData(newSource, pointerAddress, saveData, data.dataType, size);
                newSource += (int)size & 0x7FFFFFFF;
            }

            try
            {
                File.WriteAllBytes(exportPath, ROM.Instance.romData);
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }
		*/
    }
}
