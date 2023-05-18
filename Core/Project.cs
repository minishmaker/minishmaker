using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using MinishMaker.Core.ChangeTypes;
using MinishMaker.Properties;
using MinishMaker.UI;
using MinishMaker.Utilities;

namespace MinishMaker.Core
{
    public enum DataType
    {
        bgData,
        roomMetaData,
        metaTileset, //Swapped in old version
        metaTileType,
        tileset, //Swapped in old version
        chestData,
        areaInfo,
        listData,
        warpData,
        palette,
        textData,
        roomAddition,
    }

    /// <summary>
    /// Stores configuration for a Minish Maker project
    /// </summary>
    public class Project
    {
        public static Project Instance { get; private set; }

        //public MapManager mapManager;
        public string ProjectName { get; private set; }
        public string ProjectPath { get; private set; }
        public string BaseRomPath { get; private set; }
        public int ProjectVersion { get; private set; } = 0;
        private int LatestVersion { get { return 1; } }


        private List<Change> pendingRomChanges = new List<Change>(); //not yet saved
        private List<Change> storedChanges = new List<Change>(); //not yet built
        private List<Change> writtenChanges = new List<Change>(); //built
        private StreamWriter mainWriter;
        public Dictionary<Tuple<int, int>, string> customNames { get; private set; } //custom only, dont want standard in the project file anymore
        public Dictionary<Tuple<int, int>, string> roomNames { get; private set; }

        #region creation
        public Project(string name, string baseRom, string projectFolder)
        {
            Instance = this;

            ProjectName = name;
            ProjectPath = projectFolder;
            BaseRomPath = baseRom;
            ProjectVersion = LatestVersion;
            customNames = new Dictionary<Tuple<int, int>, string>();
            roomNames = new Dictionary<Tuple<int, int>, string>();

            // Double check directory
            if (!Directory.Exists(ProjectPath))
            {
                Directory.CreateDirectory(ProjectPath);
            }

            File.Copy(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6) + "/Resources/TextConvert.xml", ProjectPath + "/TextConvert.xml");
            writtenChanges = new List<Change>();
            storedChanges = new List<Change>();
            pendingRomChanges = new List<Change>();

            LoadProject(); //load first so rom is loaded

            BaseRomPath = $"{Directory.GetParent(ProjectPath).FullName}/baserom_{ROM.Instance.version}.gba";

            // Create project file data
            var sb = new StringBuilder();
            sb.AppendLine($"projectName={ProjectName}");
            sb.AppendLine($"version={ProjectVersion}");
            sb.AppendLine($"baseROM={BaseRomPath}");

            // Copy ROM
            if (!File.Exists(BaseRomPath))
            {
                byte[] copy = File.ReadAllBytes(baseRom);
                File.WriteAllBytes(BaseRomPath, copy);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            var lines = new string[0];

            using (Stream stream = assembly.GetManifestResourceStream("MinishMaker.Resources.StandardNames.txt"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContents = reader.ReadToEnd();
                    // Each line is a different location, split regardless of return form
                    lines = fileContents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }
            }

            var baseNames = new Dictionary<string, string>();
            var seperator = new char[] { '=' };
            foreach (var line in lines)
            {
                if (line.Length == 0) continue;

                var kvArray = line.Split(seperator, 2);
                baseNames.Add(kvArray[0], kvArray[1]);
            }

            LoadRoomNames(baseNames, true);

            File.WriteAllText(ProjectPath + "/" + ProjectName + ".mmproj", sb.ToString());
        }

        public void CreateProjectFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("projectName=" + ProjectName);
            sb.AppendLine("baseROM=" + BaseRomPath);
            sb.AppendLine("version=" + ProjectVersion);

            foreach (KeyValuePair<Tuple<int, int>, string> set in customNames)
            {
                var area = set.Key.Item1.Hex();
                var room = set.Key.Item2.Hex();
                if(set.Key.Item2 == -1)
                {
                    room = "-1";
                }
                var name = set.Value;

                sb.AppendLine($"roomName[{area},{room}]={name}");
            }

            File.WriteAllText(ProjectPath + "/" + ProjectName + ".mmproj", sb.ToString());
        }
        #endregion

        #region loading
        public Project(string projectFile)
        {
            // Load mode
            if (!File.Exists(projectFile))
                throw new FileNotFoundException("Project file not found.");

            Instance = this;
            var settings = new Dictionary<string, string>();
            var settingLines = File.ReadLines(projectFile).ToList();
            //do projectversion the old way to check the need for the below
            var versionString = settingLines.Where(l => l.StartsWith("version")).SingleOrDefault();
            if (versionString != null)
            {
                ProjectVersion = int.Parse(versionString.Split('=')[1]);
            }
            settingLines = ProjectFileCheck(settingLines);
            
            
            var seperator = new char[] { '=' };
            foreach (var line in settingLines)
            {
                if (line.Length == 0) continue;

                var kvArray = line.Split(seperator, 2);
                settings.Add(kvArray[0], kvArray[1]);
            }

            ProjectName = settings["projectName"];
            BaseRomPath = settings["baseROM"];
            ProjectPath = Path.GetDirectoryName(projectFile);

            roomNames = new Dictionary<Tuple<int, int>, string>();

            LoadRoomNames(settings, false);
            customNames = roomNames.ToDictionary(entry => new Tuple<int, int>(entry.Key.Item1, entry.Key.Item2), entry => entry.Value);
            Assembly assembly = Assembly.GetExecutingAssembly();
            var lines = new string[0];

            using (Stream stream = assembly.GetManifestResourceStream("MinishMaker.Resources.StandardNames.txt"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContents = reader.ReadToEnd();
                    // Each line is a different location, split regardless of return form
                    lines = fileContents.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                }
            }

            var baseNames = new Dictionary<string, string>();
            int i = 0;
            foreach (var line in lines)
            {
                if (line.Length == 0) continue;

                var kvArray = line.Split(seperator, 2);
                baseNames.Add(kvArray[0], kvArray[1]);
                i++;
            }

            LoadRoomNames(baseNames, true);

            writtenChanges = new List<Change>();
            storedChanges = new List<Change>();
            pendingRomChanges = new List<Change>();
            //VersionCheck();
            LoadProject();
        }

        public void LoadProject()
        {
            storedChanges.Clear();
            var rom = new ROM(BaseRomPath); //sets instance
            var pos = rom.romData.Length - 1;

            while (rom.romData[pos] == 0xFF)
            {
                pos--;
            }

            if (!File.Exists(ProjectPath + "/Main.event"))
            {
                var file = File.Create(ProjectPath + "/Main.event");
                using (StreamWriter s = new StreamWriter(file))
                {
                    s.WriteLine("ORG " + (pos + 1));
                    s.WriteLine("#include \"./Patches.event\"");
                }
                file.Dispose();
            }

            if (!File.Exists(ProjectPath + "/Patches.event"))
            {
                var file = File.Create(ProjectPath + "/Patches.event");
                using (StreamWriter s = new StreamWriter(file))
                {
                    s.WriteLine("//dummy");
                }
                file.Dispose();
            }

            var mainSets = File.ReadAllLines(ProjectPath + "/Main.event").ToList();
            mainSets[0] = "ORG " + (pos + 1);
            mainSets[1] = "#include \"./Patches.event\"";
            File.WriteAllLines(ProjectPath + "/Main.event", mainSets);
            mainSets.RemoveAt(0);
            mainSets.RemoveAt(0); //2nd time to remove 2 lines
            //StartSave();

            //mainSets = mainSets.Select(s => s.Substring(11).TrimEnd('\"').Replace('/', '\\')).ToList();

            LoadGlobalChanges(); //palettes, text
            LoadChanges(ProjectPath + "\\Areas", -1);
            #region comment
            /*
            if (Directory.Exists(ProjectPath + "\\Areas"))
            {
                string[] areaDirectories = Directory.GetDirectories(ProjectPath + "\\Areas");

                foreach (string areaDirectory in areaDirectories)
                {
                    int areaIndex = Convert.ToInt32(areaDirectory.Substring(areaDirectory.Length - 2), 16);

                    string[] areaFiles = Directory.GetFiles(areaDirectory, "*.json");
                    foreach (var file in areaFiles)
                    {
                        DataType type;
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        var fileNumbers = new String(fileName.Where(char.IsDigit).ToArray());
                        var fileNumberless = new String(fileName.Where(c => !char.IsDigit(c)).ToArray());
                        var success = Enum.TryParse(fileNumberless, out type);

                        var identifier = -1;
                        if (fileNumbers.Length != 0)
                        {
                            identifier = int.Parse(fileNumbers);
                        }


                        if (success)
                        {
                            var change = CreateChange(type, areaIndex, 0, identifier);
                            var entry = mainSets.SingleOrDefault(x => file.Contains(x));
                            var changeNameBase = change.changeType.ToString() + (change.identifier != -1 ? change.identifier + "" : "");
                            if (entry != null)
                            {
                                mainSets.Remove(entry);
                            }
                            else
                            {
                                mainWriter.WriteLine("#include \"./Areas" + change.GetFolderLocation() + "/" + changeNameBase + ".event\"");
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
                            var fileName = Path.GetFileNameWithoutExtension(file);
                            var fileNumbers = new String(fileName.Where(char.IsDigit).ToArray());
                            var fileNumberless = new String(fileName.Where(c => !char.IsDigit(c)).ToArray());
                            var success = Enum.TryParse(Path.GetFileNameWithoutExtension(fileNumberless), out type);

                            var identifier = -1;
                            if (fileNumbers.Length != 0)
                            {
                                identifier = int.Parse(fileNumbers);
                            }

                            if (success)
                            {
                                var change = CreateChange(type, areaIndex, roomIndex, identifier);
                                var entry = mainSets.SingleOrDefault(x => file.Contains(x));
                                var changeNameBase = change.changeType.ToString() + (change.identifier != -1 ? change.identifier + "" : "");
                                if (entry != null)
                                {
                                    mainSets.Remove(entry);
                                }
                                else
                                {
                                    mainWriter.WriteLine("#include \"./Areas" + change.GetFolderLocation() + "/" + changeNameBase + ".event\"");
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
            */
            #endregion
            MainWindow.instance.RunTests();
        }

        private void LoadGlobalChanges()
        {
            var globalPath = ProjectPath + "\\Global";
            if (!Directory.Exists(globalPath))
            {
                return;
            }
            string[] directories = Directory.GetDirectories(globalPath);
            foreach (string directory in directories)
            {
                DataType currentType;

                if(!Enum.TryParse(Path.GetFileName(directory), out currentType))
                {
                    Debug.WriteLine("directory does not match any DataType: " + directory);
                    continue;
                }
                string[] files = Directory.GetFiles(directory, "*.json");
                foreach (var file in files)
                {
                    var fileNumbered = Path.GetFileNameWithoutExtension(file);
                    var stringIdentifier = fileNumbered.Substring(fileNumbered.Length - 2);
                    int identifier = -1;
                    if (stringIdentifier.Length != 0)
                    {
                        identifier = int.Parse(stringIdentifier, NumberStyles.HexNumber);
                    }
                    var change = CreateChange(currentType, -1, -1, identifier);
                    var eventFileLoc = $"{ProjectPath}/Build{change.GetFolderLocation()}/{currentType}{(identifier != -1 ? identifier.Hex(2) : "")}.event";
                    
                    if (File.Exists(eventFileLoc))
                    {
                        writtenChanges.Add(change); //event file exists
                    }

                    if (!File.Exists(eventFileLoc) || File.GetLastWriteTime(file) > File.GetLastWriteTime(eventFileLoc))
                    {
                        storedChanges.Add(change); //no event file or event file is older than json
                    }
                }
            }
        }

        private void LoadChanges(string path, int originalAreaIndex)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            var areaIndex = originalAreaIndex;
            string[] directories = Directory.GetDirectories(path);

            foreach (string directory in directories)
            {
                //TODO: looks fishy
                int currentIndex = Convert.ToInt32(directory.Substring(directory.Length - 2), 16);
                
                if (originalAreaIndex == -1)
                {
                    areaIndex = currentIndex;
                    currentIndex = -1;
                }

                string[] files = Directory.GetFiles(directory, "*.json");
                foreach (var file in files)
                {
                    DataType type;
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var fileNumbers = new String(fileName.Where(char.IsDigit).ToArray());
                    var fileNumberless = new String(fileName.Where(c => !char.IsDigit(c)).ToArray());

                    if (!Enum.TryParse(fileNumberless, out type))
                    {
                        Debug.WriteLine("unknown file found: " + file);
                        continue;
                    }

                    var identifier = -1;
                    if (fileNumbers.Length != 0)
                    {
                        identifier = int.Parse(fileNumbers);
                    }

                    var change = CreateChange(type, areaIndex, currentIndex, identifier);
                    var eventFileLoc = $"{ProjectPath}/Build/{change.GetFolderLocation()}/{type}{(identifier != -1 ? identifier.Hex(2) : "")}.event";

                    if (File.Exists(eventFileLoc))
                    {
                        writtenChanges.Add(change); //event file exists
                    }

                    if (!File.Exists(eventFileLoc) || File.GetCreationTime(file) > File.GetCreationTime(eventFileLoc))
                    {
                        storedChanges.Add(change); //no event file or event file is older than json
                    }
                }

                if (originalAreaIndex == -1)
                {
                    LoadChanges(directory, areaIndex);
                }
            }
        }

        private void LoadRoomNames(Dictionary<string,string> settings, bool ignoreDuplicate)
        {
            var separator = new char[] { ',' };
            foreach(var key in settings.Keys)
            {
                if(!key.StartsWith("roomName["))
                {
                    continue;
                }
                //Console.Write(key + " : ");
                //Console.WriteLine(settings[key]);
                //roomName[---,---]
                var idParts = key.Substring(9, key.Length - 10).Split(separator, 2);
                var roomId = -1;
                var areaId = -1;
                var roomName = settings[key];
                var success = false;

                success = int.TryParse(idParts[0].TrimStart(' ').TrimEnd(' '), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out areaId);
                if (!success)
                {
                    throw new Exception("Invalid area value for: " + key);
                }
                var roomval = idParts[1].TrimStart(' ').TrimEnd(' ');
                var neg = false;

                if (roomval.Contains('-')) //because none of the parsers can do negatives on hex value conversion
                {
                    neg = true;
                    roomval = roomval.TrimStart('-');
                }

                success = int.TryParse(roomval, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out roomId);
                if (!success)
                {
                    throw new Exception("Invalid room value for: " + key);
                }

                if (neg)
                {
                    roomId *= -1;
                }

                var keyTuple = new Tuple<int, int>(areaId, roomId);

                if (roomNames.ContainsKey(keyTuple))
                {
                    if (ignoreDuplicate)
                    {
                        continue;
                    }
                    throw new Exception("A name for this room already exists: 1:" + roomNames[keyTuple] + " | 2:" + roomName);
                }
                else
                {
                    roomNames.Add(keyTuple, roomName);
                }
            }
        }

        private void LoadRoomNames(string[] nameSets, bool ignoreDuplicate)
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

                var keyTuple = new Tuple<int, int>(areaId, roomId);

                if (roomNames.ContainsKey(keyTuple))
                {
                    if (ignoreDuplicate)
                    {
                        continue;
                    }
                    throw new Exception("A name for this room already exists: 1:" + roomNames[keyTuple] + " | 2:" + roomName);
                }
                else
                {
                    roomNames.Add(keyTuple, roomName);
                }
            }
        }

        private Change CreateChange(DataType type, int area, int room, int identifier)
        {
            switch (type)
            {
                case DataType.areaInfo:
                    return new AreaInfoChange(area);
                case DataType.roomMetaData:
                    return new RoomMetadataChange(area, room);
                case DataType.bgData:
                    return new BgDataChange(area, room, identifier);
                case DataType.tileset:
                    return new TilesetChange(area, identifier);
                case DataType.metaTileset:
                    return new MetaTilesetChange(area, identifier);
                case DataType.listData:
                    return new ListDataChange(area, room, identifier);
                case DataType.warpData:
                    return new WarpDataChange(area, room);
                case DataType.metaTileType:
                    return new MetaTileTypeChange(area, identifier);
                case DataType.palette:
                    return new PaletteChange(identifier);
                case DataType.textData:
                    return new TextChange(identifier);
                default:
                    Debug.WriteLine("unknown file found of type: " + type);
                    return null;
            }
        }

        //TODO: might rebuild the file on build
        private void CleanIncludes(List<string> remaining)
        {
            var text = File.ReadAllText(ProjectPath + "/Main.event");

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

                File.WriteAllText(ProjectPath + "/Main.event", text);
            }
        }
        #endregion

        public void AddPendingChange(Change change)
        {
            if (!pendingRomChanges.Any(x => x.Compare(change))) //change does not yet exist
            {
                pendingRomChanges.Add(change);
            }
        }

        #region saving

        public void Save()
        {
            while (pendingRomChanges.Count > 0)
            {
                Change data = pendingRomChanges.ElementAt(0);
                SaveChange(data);
                pendingRomChanges.RemoveAt(0);
                storedChanges.Add(data);
            }
        }

        public void SaveChange(Change change)
        {
            var folderLoc = ProjectPath + change.GetFolderLocation();
            var changeNameBase = change.changeType.ToString() + (change.identifier != -1 ? change.identifier.Hex(2) : "");
            var fileName = changeNameBase + ".json";
            var content = change.GetJSONString();
            if (content == "") return;
            Directory.CreateDirectory(folderLoc);
            File.WriteAllText(folderLoc + "/" + fileName, content);
            if(!storedChanges.Any(c => change.Compare(c)))
            {
                storedChanges.Add(change);
            }
        }

        public void BuildChange(Change change)
        {
            var changeNameBase = $"{change.GetFolderLocation()}/{change.changeType}{(change.identifier != -1 ? change.identifier.Hex(2) : "")}";

            var eventFilePathBase = $"{ProjectPath}/Build{changeNameBase}";
            var datFilePath = $"{eventFilePathBase}Dat.bin";
            var eventFilePath = $"{eventFilePathBase}.event";
            var jsonFilePath = $"{ProjectPath}{changeNameBase}.json";

            var fileName = changeNameBase  + ".event";
            //if the event is newer than the json file, that means that its already up to date
            if (File.Exists(eventFilePath) && File.GetLastWriteTime(eventFilePath) > File.GetLastWriteTime(jsonFilePath)) {
                return;
            }

            byte[] binData;
            var content = change.GetEAString(out binData);
            Directory.CreateDirectory(Directory.GetParent(eventFilePath).FullName);
            File.WriteAllText(eventFilePath, content);

            if (binData != null)
            {
                File.WriteAllBytes(datFilePath, binData);
            }

            if (!writtenChanges.Any(x => x.Compare(change))) //change not yet already written
            {
                mainWriter.WriteLine("#include \"./Build" + fileName + "\"");
                writtenChanges.Add(change);
            }
        }
        #endregion

        public bool BuildProject()
        {
            // Double check
            if (!File.Exists(BaseRomPath) || !Directory.Exists(ProjectPath))
                return false;

            mainWriter = File.AppendText(ProjectPath + "/Main.event");
            while (storedChanges.Count > 0)
            {
                Change data = storedChanges.ElementAt(0);
                BuildChange(data);
                storedChanges.RemoveAt(0);
            }
            mainWriter.Dispose();

            Directory.CreateDirectory ($"{Directory.GetParent(ProjectPath).FullName}/Builds/");
            string outputROM = $"{Directory.GetParent(ProjectPath).FullName}/Builds/{ProjectName}.gba";

            // Set up new copy of ROM
            byte[] copy = File.ReadAllBytes(BaseRomPath);
            File.WriteAllBytes(outputROM, copy);


            // TODO better integration to colorzcore
            string[] args = new[] { "A", "FE8", "-input:" + ProjectPath + "/Main.event", "-output:" + outputROM };
            int exitcode = ColorzCore.Program.Main(args);

            if (exitcode == 0)
                return true;
            else
                return false;
        }


        private List<string> ProjectFileCheck(List<string> lines)
        {
            if (ProjectVersion < 1)
            {

                for (int i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];
                    if (!line.StartsWith("roomname="))
                    {
                        continue;
                    }
                    //roomname=1,1,name
                    var parts = line.Substring(9).Split(',');
                    if (parts[1] == "FFFFFFFF")
                    {
                        parts[1] = "-1";
                    }
                    lines[i] = $"roomname[{parts[0]},{parts[1]}]={parts[2]}";
                }
            }
            return lines;
        }
        private void VersionCheck()
        {
            if (ProjectVersion == LatestVersion)
            {
                return;
            }

            if (ProjectVersion < 1)
            {
                V1Update();
            }
            
            ProjectVersion = LatestVersion;
        }

        private void V1Update()
        {
            if (!File.Exists(ProjectPath + "\\TextConvert.xml"))
            {
                File.Copy(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6) + "\\Resources\\TextConvert.xml", ProjectPath + "\\TextConvert.xml");
            }

            if (!Directory.Exists(ProjectPath + "\\Areas"))
            {
                return;
            }

            string[] areaDirectories = Directory.GetDirectories(ProjectPath + "\\Areas");
            //TODO: JSONIFY
            foreach (string areaDirectory in areaDirectories)
            {
                int areaIndex = Convert.ToInt32(areaDirectory.Substring(areaDirectory.Length - 2), 16);
                string[] areaFiles = Directory.GetFiles(areaDirectory, "*.event");
                foreach (var file in areaFiles)
                {
                    OldEnum type;
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var fileNumbers = new String(fileName.Where(char.IsDigit).ToArray());
                    var success = Enum.TryParse(fileName, out type); //get the old type

                    var identifier = -1;
                    if (fileNumbers.Length != 0)
                    {
                        identifier = int.Parse(fileNumbers);
                    }

                    if (!success)
                    {
                        continue;
                    }

                    string json = "";
                    var dataPath = fileName + "Dat.bin";
                    byte[] data = File.ReadAllBytes(dataPath);

                    DataType newType;
                    switch (type)
                    {
                        case (OldEnum.bg1TileSet):
                        case (OldEnum.bg2TileSet):
                        case (OldEnum.commonTileSet):
                            if (identifier == -1)
                            {
                                identifier = 1; //common is 1 but has no number in its filename
                            }
                            if (identifier == 1)
                            {
                                identifier = 0; //bg1 is 0 but has a 1 in its filename
                            }
                            data = DataHelper.GetFromSavedData(dataPath, true, 0x4000);
                            newType = DataType.tileset;
                            break;
                            //room only
                        /*case (OldEnum.bg1Data):
                        case (OldEnum.bg2Data):
                            newType = DataType.bgData;
                            break;
                        case (OldEnum.bg1MetaTileSet):
                        case (OldEnum.bg2MetaTileSet):
                            newType = DataType.bgMetaTileSet;
                            break;
                        case (OldEnum.bg1MetaTileType):
                        case (OldEnum.bg2MetaTileType):
                            newType = DataType.bgMetaTileType;
                            break;
                        case (OldEnum.list1Data):
                        case (OldEnum.list2Data):
                        case (OldEnum.list3Data):
                        case (OldEnum.chestData):
                            identifier -= 1;
                            newType = DataType.listData;
                            if (identifier <= -1)
                            {
                                identifier = 3;
                            }

                            json = Newtonsoft.Json.JsonConvert.SerializeObject(new RoomMetaData.ListStruct(list, GetLinkFor(identifier)));
                            break;*/
                        default:
                            throw new NotImplementedException("you forgot to check for this dum dum");
                    }

                    string idString = identifier != -1 ? "" + identifier : "";
                    File.Move(file, "build\\" + areaDirectory + "\\" + newType + idString + ".event");
                    //TODO: LOAD DATA
                    Change tempChange = CreateChange(newType, areaIndex, 0, identifier);
                    File.WriteAllText(areaDirectory + "\\" + newType + idString + ".json", tempChange.GetJSONString());
                }

                string[] roomDirectories = Directory.GetDirectories(areaDirectory);

                foreach (string roomDirectory in roomDirectories)
                {
                    int roomIndex = Convert.ToInt32(roomDirectory.Substring(roomDirectory.Length - 2), 16);

                    string[] roomFiles = Directory.GetFiles(roomDirectory, "*.event");
                    foreach (var file in roomFiles)
                    {
                        OldEnum type;
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        var fileNumbers = new String(fileName.Where(char.IsDigit).ToArray());
                        var success = Enum.TryParse(fileName, out type); //get the old type

                        var identifier = -1;
                        if (fileNumbers.Length != 0)
                        {
                            identifier = int.Parse(fileNumbers);
                        }

                        if (!success)
                        {
                            continue;
                        }

                        string json = "";
                        var dataPath = fileName + "Dat.bin";
                        byte[] data = File.ReadAllBytes(dataPath);

                        DataType newType;
                        switch (type)
                        {
                            case (OldEnum.bg1TileSet):
                            case (OldEnum.bg2TileSet):
                            case (OldEnum.commonTileSet):
                                if (identifier == -1)
                                {
                                    identifier = 1; //common is 1 but has no number in its filename
                                }
                                if (identifier == 1)
                                {
                                    identifier = 0; //bg1 is 0 but has a 1 in its filename
                                }
                                newType = DataType.tileset;
                                break;
                            case (OldEnum.bg1Data):
                            case (OldEnum.bg2Data):
                                newType = DataType.bgData;
                                var possibleHeight = 10;
                                data = DataHelper.GetFromSavedData(dataPath, true, 0x8000);

                                var tileTotal = data.Length / 2;
                                while (tileTotal % possibleHeight != 0 && tileTotal / possibleHeight > 63)
                                {
                                    possibleHeight++;
                                    if (possibleHeight > 63 || tileTotal / possibleHeight < 15)
                                    {
                                        throw new ArgumentOutOfRangeException($"no divisor found for area 0x{areaIndex.Hex(2)} room 0x{roomIndex.Hex(2)}");
                                    }
                                }
                                
                                json = DataHelper.ByteArrayToFormattedJSON(data, tileTotal/possibleHeight, 2);
                                break;
                            case (OldEnum.bg1MetaTileSet):
                            case (OldEnum.bg2MetaTileSet):
                                newType = DataType.metaTileset;
                                data = DataHelper.GetFromSavedData(dataPath, true, 0x8000);
                                json = DataHelper.ByteArrayToFormattedJSON(data, 16, 8);
                                break;
                            case (OldEnum.bg1MetaTileType):
                            case (OldEnum.bg2MetaTileType):
                                newType = DataType.metaTileType;
                                data = DataHelper.GetFromSavedData(dataPath, true, 0x8000);
                                json = DataHelper.ByteArrayToFormattedJSON(data, 16, 2);
                                break;
                            case (OldEnum.list1Data):
                            case (OldEnum.list2Data):
                            case (OldEnum.list3Data):
                            case (OldEnum.chestData):
                                newType = DataType.listData;
                                if (identifier == -1)
                                {
                                    identifier = 4;
                                }

                                List<byte> dataList = data.ToList();
                                var dataPos = 0;
                                var list = new List<List<byte>>();
                                int dataSize;
                                while (dataPos < dataList.Count)
                                {
                                    ObjectDefinitionParser.FilterData(out dataSize, dataList, identifier);
                                    var dat = new List<byte>(data.Skip(dataPos).Take(dataSize).ToArray());
                                    list.Add(dat);
                                }
                                json = Newtonsoft.Json.JsonConvert.SerializeObject(new RoomMetaData.ListStruct(list, 0xFF));
                                break;
                            case (OldEnum.roomMetaData):
                                newType = DataType.roomMetaData;
                                var mapX = (data[0] + (data[1] << 8));
                                var mapY = (data[2] + (data[3] << 8));
                                int sizeX;
                                int sizeY;
                                int tilesetOffset;
                                if (data.Length == 4) //backwards compatibility because I did not save it at first
                                {
                                    var r = ROM.Instance.reader;
                                    var header = ROM.Instance.headers;

                                    int mapInfoArea = r.ReadAddr(header.MapInfoRoot + (areaIndex << 2));
                                    int mapInfoRoomLoc = mapInfoArea + (roomIndex * 0x0A);
                                    r.SetPosition(mapInfoRoomLoc + 4);
                                    sizeX = r.ReadUInt16();
                                    sizeY = r.ReadUInt16();
                                    tilesetOffset = r.ReadUInt16();
                                }
                                else
                                {
                                    sizeX = (data[4] + (data[5] << 8));
                                    sizeY = (data[6] + (data[7] << 8));
                                    tilesetOffset = (data[8] + (data[9] << 8));
                                }
                                json = Newtonsoft.Json.JsonConvert.SerializeObject(new RoomMetaData.RoomMetaDataStruct(sizeX, sizeY, mapX, mapY, tilesetOffset));
                                break;
                            default:
                                throw new NotImplementedException("you forgot to check for this dum dum");
                        }
                        string idString = identifier != -1 ? "" + identifier : "";
                        File.Move(file, "build\\" + roomDirectory + "\\" + newType + idString + ".event");
                        //TODO: LOAD DATA
                        File.WriteAllText(areaDirectory + "\\" + newType + idString + ".json", json);
                    }
                }
            }
        }
        
        private enum OldEnum
        {
            bg1Data,
            bg2Data,
            bg1TileSet,
            bg2TileSet,
            commonTileSet,
            bg1MetaTileSet,
            bg2MetaTileSet,
            chestData,
            list1Data,
            list2Data,
            list3Data,
            bg1MetaTileType,
            bg2MetaTileType,
            roomMetaData,

        }
    }
}
