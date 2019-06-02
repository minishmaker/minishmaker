using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
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
        metaTileSet,
        chestData
    }

    /// <summary>
    /// Stores configuration for a minishmaker project
    /// </summary>
    public class Project
    {
        public static Project Instance;

        public MapManager mapManager;

        public string sourcePath;
        public string exportPath;
        public string projectPath;
        public List<ModifiedData> modifiedData;

        public struct SpecialData
        {
            public DataType dataType;
            public string path;

            public SpecialData(DataType type, string filepath)
            {
                dataType = type;
                path = filepath;
            }
        }

        public struct ModifiedData
        {
            public int areaIndex;
            public int roomIndex;
            public DataType dataType;

            public ModifiedData(int areaIndex, int roomIndex, DataType type)
            {
                this.areaIndex = areaIndex;
                this.roomIndex = roomIndex;
                this.dataType = type;
            }
        }

        public Project(ROM baseRom, MapManager manager)
        {
            Instance = this;
            projectPath = "";

            sourcePath = baseRom.path;
            projectPath = Path.GetDirectoryName(sourcePath);
            exportPath = projectPath + "/mc-hack.gba";
            modifiedData = new List<ModifiedData>();
            mapManager = manager;
            LoadProject();
        }

        public void LoadProject()
        {
            if (Directory.Exists(projectPath + "/Areas"))
            {
                string[] areaDirectories = Directory.GetDirectories(projectPath + "/Areas");

                foreach (string areaDirectory in areaDirectories)
                {
                    int areaIndex = Convert.ToInt32(areaDirectory.Substring(areaDirectory.Length - 2), 16);

                    string[] roomDirectories = Directory.GetDirectories(areaDirectory);

                    foreach (string roomDirectory in roomDirectories)
                    {
                        int roomIndex = Convert.ToInt32(roomDirectory.Substring(roomDirectory.Length - 2), 16);

                        string[] roomModifications = Directory.GetFiles(roomDirectory);

                        //Room room = mapManager.FindRoom(areaIndex, roomIndex);

                        foreach (string modification in roomModifications)
                        {
                            string filename = modification.Split('\\').Last();
                            ModifiedData newData = new ModifiedData(areaIndex, roomIndex, (DataType)Convert.ToInt32(filename));
                            modifiedData.Add(newData);
                        }
                    }
                }
            }
        }

        public void SaveProject()
        {
            modifiedData = modifiedData.Distinct().ToList();

            if (modifiedData.Count > 0 && !Directory.Exists(projectPath + "/Areas")) 
            {
                Directory.CreateDirectory(projectPath + "/Areas");
            }

            foreach (ModifiedData modification in modifiedData)
            {
                string areaDirectory = projectPath + "/Areas/Area " + StringUtil.AsStringHex2(modification.areaIndex);

                if (!Directory.Exists(areaDirectory))
                {
                    Directory.CreateDirectory(areaDirectory);
                }

                string roomDirectory = areaDirectory + "/Room " + StringUtil.AsStringHex2(modification.roomIndex);

                if (!Directory.Exists(roomDirectory))
                {
                    Directory.CreateDirectory(roomDirectory);
                }

                Room room = mapManager.FindRoom(modification.areaIndex, modification.roomIndex);

                if (!room.Loaded)
                {
                    room.LoadRoom(modification.areaIndex);
                }

                byte[] data = null;
                long size = room.GetSaveData(ref data, modification.dataType);

                string name = ((int)modification.dataType).ToString();

                File.WriteAllBytes(roomDirectory + "/" + name, data);
            }
        }

        public void AddChange(int areaIndex, int roomIndex, DataType type)
        {
            modifiedData.Add(new ModifiedData(areaIndex, roomIndex, type));
        }

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
    }
}
