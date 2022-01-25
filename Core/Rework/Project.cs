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

namespace MinishMaker.Core.Rework
{
    public enum DataType
    {
        bgData,
        roomMetaData,
        tileSet,
        bgMetaTileSet,
        chestData,
        areaInfo,
        listData,
        warpData,
        bgMetaTileType,
        palette,
        textData
    }

    public enum LayerDataType
    {

    }

    /// <summary>
    /// Stores configuration for a Minish Maker project
    /// </summary>
    public class Project
    {
        public static Project Instance { get; private set; }
        public bool Loaded { get; private set; }

        //public MapManager mapManager;
        public string ProjectName { get; private set; }
        public string ProjectPath { get; private set; }
        public int ProjectVersion { get; private set; }
        private int LatestVersion { get { return 2; } }

        private List<ChangeTypes.Rework.Change> loadedChanges;
        private List<ChangeTypes.Rework.Change> pendingRomChanges;
        private StreamWriter mainWriter;
        public Dictionary<Tuple<int, int>, string> customNames { get; private set; } //custom only, dont want standard in the project file anymore
        public Dictionary<Tuple<int, int>, string> roomNames { get; private set; }

        #region creation
        public Project(string name, string baseRom, string projectFolder)
        {
            Instance = this;

            ProjectName = name;
            ProjectPath = projectFolder;
            ProjectVersion = LatestVersion;
            roomNames = new Dictionary<Tuple<int, int>, string>();

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

            LoadRoomNames(baseNames, true);

            // Double check directory
            if (!Directory.Exists(ProjectPath))
            {
                Directory.CreateDirectory(ProjectPath);
            }

            // Copy ROM
            byte[] copy = File.ReadAllBytes(baseRom);
            File.WriteAllBytes(ProjectPath + "/baserom.gba", copy);

            // Create project file data
            var sb = new StringBuilder();
            sb.AppendLine("projectName=" + ProjectName);
            sb.AppendLine("baseROM=" + "baserom.gba");
            sb.AppendLine("version=" + ProjectVersion);

            foreach (string line in baseNames)
            {
                sb.AppendLine(line);
            }

            File.WriteAllText(ProjectPath + "/" + ProjectName + ".mmproj", sb.ToString());

            loadedChanges = new List<ChangeTypes.Rework.Change>();
            pendingRomChanges = new List<ChangeTypes.Rework.Change>();
            LoadProject();
        }

        public void CreateProjectFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("projectName=" + ProjectName);
            sb.AppendLine("baseROM=" + "baserom.gba");
            sb.AppendLine("version=" + ProjectVersion);

            foreach (KeyValuePair<Tuple<int, int>, string> set in customNames)
            {
                var area = set.Key.Item1.Hex();
                var room = set.Key.Item2.Hex();
                var name = set.Value;

                sb.AppendLine("roomName=" + area + "," + room + "," + name);
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

            var settings = File.ReadLines(projectFile).ToList();

            ProjectName = settings.Single(x => x.Contains("projectName")).Split('=')[1];
            var versionElement = settings.SingleOrDefault(x => x.Contains("projectVersion"));
            if(versionElement != null) {
                ProjectVersion = int.Parse(versionElement.Split('=')[1]);
            }
            ProjectPath = Path.GetDirectoryName(projectFile);

            roomNames = new Dictionary<Tuple<int, int>, string>();

            var nameSets = settings.Where(x => x.Split('=')[0] == "roomName").ToArray();

            LoadRoomNames(nameSets, false);
            customNames = roomNames.ToDictionary(entry => new Tuple<int, int>(entry.Key.Item1, entry.Key.Item2), entry => entry.Value);

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

            LoadRoomNames(baseNames, true);

            loadedChanges = new List<ChangeTypes.Rework.Change>();
            pendingRomChanges = new List<ChangeTypes.Rework.Change>();
            LoadProject();
        }

        public void LoadProject()
        {
            AttemptFileTransition();

            loadedChanges.Clear();
            var rom = new ROM(ProjectPath + "/baserom.gba"); //sets instance
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
            StartSave();

            mainSets = mainSets.Select(s => s.Substring(11).TrimEnd('\"').Replace('/', '\\')).ToList();

            if (Directory.Exists(ProjectPath + "\\Areas"))
            {
                string[] areaDirectories = Directory.GetDirectories(ProjectPath + "\\Areas");

                foreach (string areaDirectory in areaDirectories)
                {
                    int areaIndex = Convert.ToInt32(areaDirectory.Substring(areaDirectory.Length - 2), 16);

                    string[] areaFiles = Directory.GetFiles(areaDirectory, "*.event");
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

            Loaded = true;
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

                var keyTupple = new Tuple<int, int>(areaId, roomId);

                if (roomNames.ContainsKey(keyTupple))
                {
                    if (ignoreDuplicate)
                    {
                        continue;
                    }
                    throw new Exception("A name for this room already exists: 1:" + roomNames[keyTupple] + " | 2:" + roomName);
                }
                else
                {
                    roomNames.Add(keyTupple, roomName);
                }
            }
        }

        private ChangeTypes.Rework.Change CreateChange(DataType type, int area, int room, int identifier)
        {
            switch (type)
            {
                case DataType.areaInfo:
                    return new ChangeTypes.Rework.AreaInfoChange(area);
                case DataType.roomMetaData:
                    return new ChangeTypes.Rework.RoomMetadataChange(area, room);
                case DataType.bgData:
                    return new ChangeTypes.Rework.BgDataChange(area, room, identifier);
                case DataType.bgMetaTileSet:
                    return new ChangeTypes.Rework.BgMetaTileSetChange(area, identifier);
                case DataType.tileSet:
                    return new ChangeTypes.Rework.TileSetChange(area, identifier);
                case DataType.listData:
                    return new ChangeTypes.Rework.ListDataChange(area, room, identifier);
                case DataType.warpData:
                    return new ChangeTypes.Rework.WarpDataChange(area, room);
                case DataType.bgMetaTileType:
                    return new ChangeTypes.Rework.BgMetaTileTypeChange(area, identifier);
                case DataType.palette:
                    return new ChangeTypes.Rework.PaletteChange(area);
                default:
                    Debug.WriteLine("unknown file found of type: " + type);
                    return null;
            }
        }

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

        public void AddPendingChange(ChangeTypes.Rework.Change change)
        {
            if (!pendingRomChanges.Any(x => x.Compare(change))) //change does not yet exist
            {
                pendingRomChanges.Add(change);
            }
        }

        #region saving
        public void StartSave()
        {
            mainWriter = File.AppendText(ProjectPath + "/Main.event");
        }

        public void Save()
        {
            while (pendingRomChanges.Count > 0)
            {
                ChangeTypes.Rework.Change data = pendingRomChanges.ElementAt(0);
                SaveChange(data);
                pendingRomChanges.RemoveAt(0);
            }
        }

        public void EndSave()
        {
            mainWriter.Dispose();
        }

        public void SaveChange(ChangeTypes.Rework.Change change)
        {
            var folderLoc = ProjectPath + "/Areas" + change.GetFolderLocation();
            var changeNameBase = change.changeType.ToString() + (change.identifier != -1 ? change.identifier + "" : "");
            var fileName = changeNameBase  + ".event";
            byte[] binData;
            var content = change.GetEAString(out binData);
            Directory.CreateDirectory(folderLoc);
            File.WriteAllText(folderLoc + "/" + fileName, content);

            if (binData != null)
            {
                File.WriteAllBytes(folderLoc + "/" + changeNameBase + "Dat.bin", binData);
            }

            if (!loadedChanges.Any(x => x.Compare(change))) //change not yet already written
            {
                mainWriter.WriteLine("#include \"./Areas" + change.GetFolderLocation() + "/" + fileName + "\"");
                loadedChanges.Add(change);
            }
        }
        #endregion

        public bool BuildProject()
        {
            // Double check
            if (!File.Exists(ProjectPath + "/baserom.gba") || !Directory.Exists(ProjectPath))
                return false;

            string outputROM = ProjectPath + "/" + ProjectName + ".gba";

            // Set up new copy of ROM
            byte[] copy = File.ReadAllBytes(ProjectPath + "/baserom.gba");
            File.WriteAllBytes(outputROM, copy);


            // TODO better integration to colorzcore
            string[] args = new[] { "A", "FE8", "-input:" + ProjectPath + "/Main.event", "-output:" + outputROM };
            int exitcode = ColorzCore.Program.Main(args);

            if (exitcode == 0)
                return true;
            else
                return false;
        }

        private void AttemptFileTransition()
        {
            if (ProjectVersion == LatestVersion)
            {
                return;
            }

            if (!Directory.Exists(ProjectPath + "\\Areas"))
            {
                return;
            }

            string[] areaDirectories = Directory.GetDirectories(ProjectPath + "\\Areas");

            foreach (string areaDirectory in areaDirectories)
            {
                int areaIndex = Convert.ToInt32(areaDirectory.Substring(areaDirectory.Length - 2), 16);
                string[] areaFiles = Directory.GetFiles(areaDirectory, "*.event");
                foreach (var file in areaFiles)
                {
                    OldEnum type;
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var fileNumbers =new String(fileName.Where(char.IsDigit).ToArray());
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
                            newType = DataType.tileSet;
                            break;
                        case (OldEnum.bg1Data):
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
                            newType = DataType.listData;
                            if (identifier == -1)
                            {
                                identifier = 4;
                            }
                            break;
                        default:
                            throw new NotImplementedException("you forgot to check for this dum dum");
                    }

                    string idString = identifier != -1 ? "" + identifier : "";
                    File.Move(file, areaDirectory + "\\" + newType + idString + ".event");

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
                                newType = DataType.tileSet;
                                break;
                            case (OldEnum.bg1Data):
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
                                newType = DataType.listData;
                                if (identifier == -1)
                                {
                                    identifier = 4;
                                }
                                break;
                            default:
                                throw new NotImplementedException("you forgot to check for this dum dum");
                        }
                        string idString = identifier != -1 ? "" + identifier : "";
                        File.Move(file, roomDirectory + "\\" + newType + idString + ".event");
                    }
                }
            }
            ProjectVersion = LatestVersion;
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
        }
    }
}
