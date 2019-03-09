using System.IO;

namespace MinishMaker.Core
{
    public class ROM
    {
        public static ROM Instance { get; private set; }

        public readonly byte[] romData;
        public readonly BinaryReader reader;
        public readonly BinaryWriter writer;

        public RegionVersion version { get; private set;  } = RegionVersion.None;
        public HeaderData headers { get; private set; }


        public ROM(string filePath)
        {
            Instance = this;
            romData = File.ReadAllBytes(filePath);
            Stream stream = Stream.Synchronized(new MemoryStream(romData));
            reader = new BinaryReader(stream);
            writer = new BinaryWriter(stream);

            SetupRom();
        }

        private void SetupRom()
        {
            // Determine Game region and if valid ROM (write your reader class already)
            reader.BaseStream.Position = 0xAC;
            byte[] regionBytes = reader.ReadBytes(4);
            string region = System.Text.Encoding.UTF8.GetString(regionBytes);

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