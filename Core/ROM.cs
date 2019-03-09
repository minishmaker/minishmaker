using System;
using MinishMaker.Utilities;
using System.IO;

namespace MinishMaker.Core
{
    public class ROM
    {
        public static ROM Instance { get; private set; }

        public readonly byte[] romData;
        public readonly Reader reader;

        public RegionVersion version { get; private set;  } = RegionVersion.None;
        public HeaderData headers { get; private set; }


        public ROM(string filePath)
        {
            Instance = this;
            romData = File.ReadAllBytes(filePath);
            Stream stream = Stream.Synchronized(new MemoryStream(romData));
            reader = new Reader(stream);

            SetupRom();
        }

        private void SetupRom()
        {
            // Determine game region and if valid ROM
            byte[] regionBytes = reader.ReadBytes(4, 0xAC);
            string region = System.Text.Encoding.UTF8.GetString(regionBytes);
            Console.WriteLine(region);

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