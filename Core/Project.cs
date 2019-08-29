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
        public bool Loaded { get; private set; }

        //public MapManager mapManager;
        public string projectName { get; private set; }
        public string projectPath { get; private set; }

        private List<Change> loadedChanges;
		private StreamWriter mainWriter;
        private ROM ROM_;

        // Create mode
        public Project(string name, string baseRom, string projectFolder)
        {
            Instance = this;

            projectName = name;
            projectPath = projectFolder;

            // Double check directory
            if (!Directory.Exists(projectPath))
            {
                Directory.CreateDirectory(projectPath);
            }

            // Copy ROM
            byte[] copy = File.ReadAllBytes(baseRom);
            File.WriteAllBytes(projectPath + "/baserom.gba", copy);

            // Create project file data
            var lines = new String[2];
            lines[0] = "projectName=" + projectName;
            lines[1] = "baseROM=" + "baserom.gba";
            File.WriteAllLines(projectPath + "/" + projectName + ".mmproj", lines);

            ROM_ = new ROM(projectPath + "/baserom.gba");

            loadedChanges = new List<Change>();
            LoadProject();
        }

        public Project(string projectFile)
        {
            Instance = this;

            // Load mode
            if (!File.Exists(projectFile))
                throw new FileNotFoundException("Project file not found.");

            var settings = File.ReadLines(projectFile).ToList();

            projectName = settings.Single(x => x.Contains("projectName")).Split('=')[1];

            projectPath = Path.GetDirectoryName(projectFile);

            ROM_ = new ROM(projectPath + "/baserom.gba");

            loadedChanges = new List<Change>();
            LoadProject();
        }

        /*public Project()
        {
			loadedChanges = new List<Change>();
            Instance = this;
            //projectPath = "";

            //sourcePath = baseRom.path;
            //projectPath = Path.GetDirectoryName(sourcePath);
            //exportPath = projectPath + "/mc-hack.gba";
            //mapManager = manager;
            //LoadProject();
        }*/

		public byte[] GetSavedData(string path, bool compressed, int size = 0x2000)
		{
			byte[] data = null;
			if (File.Exists(path))
            {
				data = new byte[size];
				byte[] savedData = File.ReadAllBytes(path);
				if(compressed) {
					using (MemoryStream os = new MemoryStream(data))
					{
						using (MemoryStream ms = new MemoryStream(savedData))
						{
							Reader r = new Reader(ms);
							DataHelper.Lz77Decompress(r, os);
						}
					}
				}
			}
			return data;
		}

		public void CreateProject(string sourceROM, string directory)
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
            /*
			var exeFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
			var lines = new string[2];
			lines[0] = "romFile="+sourcePath;
			lines[1] = "projectFolder="+projectPath;
			File.WriteAllLines(exeFolder+"\\Settings.cfg",lines);
            */

			loadedChanges.Clear();
			var pos = ROM.Instance.romData.Length-1;

			while(ROM.Instance.romData[pos]==0xFF)
			{
				pos--;
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
			
			mainSets = mainSets.Select(s => s.Substring(11).TrimEnd('\"').Replace('/','\\')).ToList();
			
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
							var entry = mainSets.SingleOrDefault(x=>file.Contains(x));
							if(entry !=null)
							{
								mainSets.Remove(file);
							}
							else
							{ 
								mainWriter.WriteLine("#include \"./Areas"+change.GetFolderLocation()+"/"+change.changeType.ToString()+".event\"");
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
									mainWriter.WriteLine("#include \"./Areas"+change.GetFolderLocation()+"/"+change.changeType.ToString()+".event\"");
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

            Loaded = true;
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


		private Change CreateChange(DataType type, int area, int room)
		{
			switch (type)
			{
				case DataType.areaInfo:
					return new AreaInfoChange( area );
				case DataType.roomMetaData:
					return new RoomMetadataChange( area, room );
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
				mainWriter.WriteLine("#include \"./Areas"+change.GetFolderLocation()+"/"+fileName+"\"");
        }

        public bool BuildProject()
        {
            // Double check
            if (!File.Exists(projectPath+"/baserom.gba") || !Directory.Exists(projectPath))
                return false;

            string outputROM = projectPath + "/" + projectName + ".gba";

            // Set up new copy of ROM
            byte[] copy = File.ReadAllBytes(projectPath+"/baserom.gba");
            File.WriteAllBytes(outputROM, copy);


            // TODO better integration to colorzcore
            String[] args = new[] {"A", "FE8", "-input:" + projectPath + "/Main.event", "-output:" + outputROM};
            int exitcode = ColorzCore.Program.Main(args);

            if (exitcode == 0)
                return true;
            else
                return false;
        }
    }
}
