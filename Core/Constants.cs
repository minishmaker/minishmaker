namespace MinishMaker.Core
{
    public enum RegionVersion
    {
        EU,
        JP,
        US,
        None
    }

    public struct HeaderData
    {
        public int MapHeaderBase;
        // Name on this is gonna have to change sometime
        public int AreaMetadataBase;

        public HeaderData(int map, int area)
        {
            this.MapHeaderBase = map;
            this.AreaMetadataBase = area;
        }
    }

    public class Header
    {
        // Will fill out when relevant, only need EU for now
        private readonly HeaderData[] headerTable = new HeaderData[]
        {   //             MAP     , AREA
            new HeaderData(0x11D95C, 0x0D4828),
            new HeaderData(0, 0), 
            new HeaderData(0x11E214, 0)
        };

        public HeaderData GetHeaderAddresses(RegionVersion region)
        {
            return headerTable[(int)region];
        }
    }
}
