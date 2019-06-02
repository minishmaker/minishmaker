using System;
using System.Diagnostics;
using MinishMaker.Utilities;
using System.IO;

namespace MinishMaker.Core
{
    public class ROM
    {
        public static ROM Instance { get; private set; }
        public readonly string path;

        public readonly byte[] romData;
        public readonly Reader reader;

        public RegionVersion version { get; private set;  } = RegionVersion.None;
        public HeaderData headers { get; private set; }


        public ROM(string filePath)
        {
            Instance = this;
            path = filePath;
            byte[] smallData = File.ReadAllBytes(filePath);
            if (smallData.Length >= 0x01000000)
            {
                romData = smallData;
            }
            else
            {
                romData = new byte[0x1000000];
                smallData.CopyTo(romData, 0);
            }

            Stream stream = Stream.Synchronized(new MemoryStream(romData));
            reader = new Reader(stream);
            Debug.WriteLine("Read " + stream.Length + " bytes.");

            SetupRom();
        }

        public void WriteData(int newSource, long pointerAddress, byte[] data, DataType type, long size = 0)
        {
            using (MemoryStream m = new MemoryStream(romData))
            {
                Writer w = new Writer(m);
                w.SetPosition(newSource); //actually write the data somewhere
                w.WriteBytes(data);

                switch (type)
                {
                    case DataType.bg1Data:
                    case DataType.bg2Data:
                        newSource = (newSource - ROM.Instance.headers.gfxSourceBase);
                        w.SetPosition(pointerAddress);
                        w.WriteUInt32((uint)newSource | 0x80000000);//byte 1-4 is source, high bit was removed before

                        w.SetPosition(w.Position + 4);//byte 5-8 is dest, skip
                        w.WriteUInt32((uint)size | 0x80000000);//byte 9-12 is size and compressed
                        break;
                    default:
                        if (size != 0)
                        {
                            w.SetPosition(pointerAddress);
                            w.WriteAddr(newSource);
                        }
                        else
                        {
                            w.SetPosition(pointerAddress);
                            w.WriteUInt32(0x00000000);
                        }
                        break;
                }
            }
        }

        private void SetupRom()
        {
            // Determine game region and if valid ROM
            byte[] regionBytes = reader.ReadBytes(4, 0xAC);
            string region = System.Text.Encoding.UTF8.GetString(regionBytes);
            Debug.WriteLine("Region detected: "+region);

            if (region == "BZMP")
            {
                version = RegionVersion.EU;
            }

            if (region == "BZMJ")
            {
                version = RegionVersion.JP;
            }

            if (region == "BZME")
            {
                version = RegionVersion.US;
            }

            if (version != RegionVersion.None)
            {
                headers = new Header().GetHeaderAddresses(version);
            }
        }
    }
}
