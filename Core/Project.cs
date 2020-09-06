using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
    public enum DataType
    {
        bg1Data,
        bg2Data,
        roomMetaData,
        bg1TileSet,
        bg2TileSet,
        commonTileSet,
        bg1MetaTileSet,
        bg2MetaTileSet,
        chestData,
        areaInfo,
        listData,
        list1Data,
        list2Data,
        list3Data,
        warpData,
        bg1MetaTileType,
        bg2MetaTileType,
        palette,
    }

    /// <summary>
    /// Stores configuration for a Minish Maker project
    /// </summary>
    public class Project
    {
        public static Project Instance;
        public bool Loaded { get; private set; }

        //public MapManager mapManager;
        public string projectName { get; private set; }
        public string projectPath { get; private set; }

        private List<Change> loadedChanges;
        private List<Change> pendingRomChanges;
        private StreamWriter mainWriter;
        private ROM ROM_;
        public Dictionary<Tuple<int, int>, string> roomNames = new Dictionary<Tuple<int, int>, string>();

        // Create mode
        public Project(string name, string baseRom, string projectFolder)
        {
            Instance = this;

            projectName = name;
            projectPath = projectFolder;
            var exeFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);

            roomNames.Clear();
            Assembly assembly = Assembly.GetExecutingAssembly();
            var baseNames = new string[0];
            using (Stream stream = assembly.GetManifestResourceStream("MinishMaker.Resources.StandardNames.txt")) 
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContents = reader.ReadToEnd();
                    // Each line is a different location, split regardless of return form
                    baseNames = fileContents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }
            }

            LoadRoomNames(baseNames);

            // Double check directory
            if (!Directory.Exists(projectPath))
            {
                Directory.CreateDirectory(projectPath);
            }

            // Copy ROM
            byte[] copy = File.ReadAllBytes(baseRom);
            File.WriteAllBytes(projectPath + "/baserom.gba", copy);

            // Create project file data
            var sb = new StringBuilder();
            sb.AppendLine("projectName=" + projectName);
            sb.AppendLine("baseROM=" + "baserom.gba");

            foreach (string line in baseNames)
            {
                sb.AppendLine(line);
            }



            File.WriteAllText(projectPath + "/" + projectName + ".mmproj", sb.ToString());

            ROM_ = new ROM(projectPath + "/baserom.gba");

            loadedChanges = new List<Change>();
            pendingRomChanges = new List<Change>();
            LoadProject();
        }


        public void CreateProjectFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("projectName=" + projectName);
            sb.AppendLine("baseROM=" + "baserom.gba");

            foreach (KeyValuePair<Tuple<int, int>, string> set in roomNames)
            {
                var area = set.Key.Item1.Hex();
                var room = set.Key.Item2.Hex();
                var name = set.Value;

                sb.AppendLine("roomName=" + area + "," + room + "," + name);
            }

            File.WriteAllText(projectPath + "/" + projectName + ".mmproj", sb.ToString());
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

            var nameSets = settings.Where(x => x.Split('=')[0] == "roomName").ToArray();
            if (nameSets.Length == 0)
            {
                var exeFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6);
                nameSets = File.ReadAllLines(exeFolder + "\\Resources\\StandardNames.txt");
            }

            LoadRoomNames(nameSets);

            ROM_ = new ROM(projectPath + "/baserom.gba");

            loadedChanges = new List<Change>();
            pendingRomChanges = new List<Change>();
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
                if (compressed)
                {
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
            if (!File.Exists(projectPath + "/Main.event"))
            {
                var file = File.Create(projectPath + "/Main.event");
                file.Dispose();
            }
        }

        public void RecheckProject()
        {
            var mainSets = File.ReadAllLines(projectPath + "/Main.event").ToList();

            mainSets = mainSets.Select(s => s.Substring(4)).ToList();
        }

        private void AddLoadedChange(Change change)
        {
            if (!loadedChanges.Any(x => x.Compare(change)))
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
            var pos = ROM.Instance.romData.Length - 1;

            while (ROM.Instance.romData[pos] == 0xFF)
            {
                pos--;
            }

            if (!File.Exists(projectPath + "/Main.event"))
            {
                var file = File.Create(projectPath + "/Main.event");
                using (StreamWriter s = new StreamWriter(file))
                {
                    s.WriteLine("ORG " + (pos + 1));
                    s.WriteLine("#include \"./Patches.event\"");
                }
                file.Dispose();
            }

            if (!File.Exists(projectPath + "/Patches.event"))
            {
                var file = File.Create(projectPath + "/Patches.event");
                using (StreamWriter s = new StreamWriter(file))
                {
                    s.WriteLine("//dummy");
                }
                file.Dispose();
            }

            var mainSets = File.ReadAllLines(projectPath + "/Main.event").ToList();
            mainSets[0] = "ORG " + (pos + 1);
            mainSets[1] = "#include \"./Patches.event\"";
            File.WriteAllLines(projectPath + "/Main.event", mainSets);
            mainSets.RemoveAt(0);
            mainSets.RemoveAt(0); //2nd time to remove 2 lines
            StartSave();

            mainSets = mainSets.Select(s => s.Substring(11).TrimEnd('\"').Replace('/', '\\')).ToList();

            if (Directory.Exists(projectPath + "\\Areas"))
            {
                string[] areaDirectories = Directory.GetDirectories(projectPath + "\\Areas");

                foreach (string areaDirectory in areaDirectories)
                {
                    int areaIndex = Convert.ToInt32(areaDirectory.Substring(areaDirectory.Length - 2), 16);

                    string[] areaFiles = Directory.GetFiles(areaDirectory, "*.event");
                    foreach (var file in areaFiles)
                    {
                        DataType type;
                        var success = Enum.TryParse(Path.GetFileNameWithoutExtension(file), out type);

                        if (success)
                        {
                            var change = CreateChange(type, areaIndex, 0);
                            var entry = mainSets.SingleOrDefault(x => file.Contains(x));
                            if (entry != null)
                            {
                                mainSets.Remove(entry);
                            }
                            else
                            {
                                mainWriter.WriteLine("#include \"./Areas" + change.GetFolderLocation() + "/" + change.changeType.ToString() + ".event\"");
                            }
                            loadedChanges.Add(change);
                        }
                        else
                        {
                            Debug.WriteLine("unknown file found: " + file);
                        }
                    }

                    string[] roomDirectories = Directory.GetDirectories(areaDirectory);

                    foreach (string roomDirectory in roomDirectories)
                    {
                        int roomIndex = Convert.ToInt32(roomDirectory.Substring(roomDirectory.Length - 2), 16);

                        string[] roomFiles = Directory.GetFiles(roomDirectory, "*.event");
                        foreach (var file in roomFiles)
                        {
                            DataType type;
                            var success = Enum.TryParse(Path.GetFileNameWithoutExtension(file), out type);

                            if (success)
                            {
                                var change = CreateChange(type, areaIndex, roomIndex);
                                var entry = mainSets.SingleOrDefault(x => file.Contains(x));
                                if (entry != null)
                                {
                                    mainSets.Remove(entry);
                                }
                                else
                                {
                                    mainWriter.WriteLine("#include \"./Areas" + change.GetFolderLocation() + "/" + change.changeType.ToString() + ".event\"");
                                }
                                loadedChanges.Add(change);
                            }
                            else
                            {
                                Debug.WriteLine("unknown file found: " + file);
                            }
                        }
                    }
                }
            }
            EndSave();

            if (mainSets.Count != 0)
            {
                CleanIncludes(mainSets);
            }

            Loaded = true;
        }

        private void CleanIncludes(List<string> remaining)
        {
            var text = File.ReadAllText(projectPath + "/Main.event");

            foreach (var line in remaining)
            {
                var newline = line.Replace("\\", "/");
                var pos = text.IndexOf(newline);
                if (pos == -1)
                {
                    Debug.WriteLine("Didnt find include line: " + line);
                }
                else
                {
                    var length = newline.Length + 14; //#include(8) + space(1) + ""(2) + \r\n(4)
                    text = text.Remove(pos - 13, length);
                }

                File.WriteAllText(projectPath + "/Main.event", text);
            }
        }

        private Change CreateChange(DataType type, int area, int room)
        {
            switch (type)
            {
                case DataType.areaInfo:
                    return new AreaInfoChange(area);
                case DataType.roomMetaData:
                    return new RoomMetadataChange(area, room);
                case DataType.bg1Data:
                    return new Bg1DataChange(area, room);
                case DataType.bg1MetaTileSet:
                    return new Bg1MetaTileSetChange(area);
                case DataType.bg1TileSet:
                    return new Bg1TileSetChange(area);
                case DataType.bg2Data:
                    return new Bg2DataChange(area, room);
                case DataType.bg2MetaTileSet:
                    return new Bg2MetaTileSetChange(area);
                case DataType.bg2TileSet:
                    return new Bg2TileSetChange(area);
                case DataType.commonTileSet:
                    return new CommonTileSetChange(area);
                case DataType.chestData:
                    return new ChestDataChange(area, room);
                case DataType.list1Data:
                    return new List1DataChange(area, room);
                case DataType.list2Data:
                    return new List2DataChange(area, room);
                case DataType.list3Data:
                    return new List3DataChange(area, room);
                case DataType.warpData:
                    return new WarpDataChange(area, room);
                case DataType.bg1MetaTileType:
                    return new Bg1MetaTileTypeChange(area);
                case DataType.bg2MetaTileType:
                    return new Bg2MetaTileTypeChange(area);
                case DataType.palette:
                    return new PaletteChange(area);
                default:
                    Debug.WriteLine("unknown file found of type: " + type);
                    return null;
            }
        }

        public void AddPendingChange(Change change)
        {
            if (!pendingRomChanges.Any(x => x.Compare(change))) //change does not yet exist
            {
                pendingRomChanges.Add(change);
            }
        }

        public void StartSave()
        {
            mainWriter = File.AppendText(projectPath + "/Main.event");
        }

        public void Save()
        {
            while (pendingRomChanges.Count > 0)
            {
                Change data = pendingRomChanges.ElementAt(0);
                SaveChange(data);
                pendingRomChanges.RemoveAt(0);
            }
        }

        public void EndSave()
        {
            mainWriter.Dispose();
        }

        public void SaveChange(Change change)
        {
            var folderLoc = projectPath + "/Areas" + change.GetFolderLocation();
            var fileName = change.changeType.ToString() + ".event";
            byte[] binData;
            var content = change.GetEAString(out binData);
            Directory.CreateDirectory(folderLoc);
            File.WriteAllText(folderLoc + "/" + fileName, content);

            if (binData != null)
            {
                File.WriteAllBytes(folderLoc + "/" + change.changeType.ToString() + "Dat.bin", binData);
            }

            if (!loadedChanges.Any(x => x.Compare(change))) //change not yet already written
            {
                mainWriter.WriteLine("#include \"./Areas" + change.GetFolderLocation() + "/" + fileName + "\"");
                loadedChanges.Add(change);
            }
        }

        public bool BuildProject()
        {
            // Double check
            if (!File.Exists(projectPath + "/baserom.gba") || !Directory.Exists(projectPath))
                return false;

            string outputROM = projectPath + "/" + projectName + ".gba";

            // Set up new copy of ROM
            byte[] copy = File.ReadAllBytes(projectPath + "/baserom.gba");
            File.WriteAllBytes(outputROM, copy);


            // TODO better integration to colorzcore
            String[] args = new[] { "A", "FE8", "-input:" + projectPath + "/Main.event", "-output:" + outputROM };
            int exitcode = ColorzCore.Program.Main(args);

            if (exitcode == 0)
                return true;
            else
                return false;
        }

        private void LoadRoomNames(string[] nameSets)
        {
            foreach (string nameSet in nameSets)
            {
                var parts = nameSet.Split('=');
                var areaId = -1;
                var roomId = -1;
                var roomName = "";
                var valParts = parts[1].Split(',');

                if (parts.Length != 2)
                {
                    throw new Exception("Invalid line: " + nameSet);
                }

                if (valParts.Length == 3)
                {
                    var success = false;

                    success = Int32.TryParse(valParts[0].TrimStart(' ').TrimEnd(' '), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out areaId);
                    if (!success)
                    {
                        throw new Exception("Invalid area value on line: " + nameSet);
                    }
                    var roomval = valParts[1].TrimStart(' ').TrimEnd(' ');
                    var neg = false;

                    if (roomval.Contains('-')) //because none of the parsers can do negatives on hex value conversion
                    {
                        neg = true;
                        roomval = roomval.TrimStart('-');
                    }

                    success = Int32.TryParse(roomval, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out roomId);
                    if (!success)
                    {
                        throw new Exception("Invalid room value on line: " + nameSet);
                    }

                    if (neg)
                    {
                        roomId *= -1;
                    }
                    roomName = valParts[2].TrimStart(' ').TrimEnd(' ');
                }
                else if (valParts.Length > 3)
                {
                    throw new Exception("Too many values on line: " + nameSet);
                }
                else
                {
                    throw new Exception("Too few values on line: " + nameSet);
                }

                var keyTupple = new Tuple<int, int>(areaId, roomId);

                if (roomNames.ContainsKey(keyTupple))
                {
                    throw new Exception("A name for this room already exists: 1:" + roomNames[keyTupple] + " | 2:" + roomName);
                }

                roomNames.Add(keyTupple, roomName);
            }
        }
    }
}
