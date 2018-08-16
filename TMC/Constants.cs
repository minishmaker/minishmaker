namespace MinishMaker.TMC
{
    public enum RegionVersion
    {
        EU,
        JP,
        US
    }

    public enum Constants : int
    {
        MapHeaderBase = 0x11D95C
    }

    enum USConstants : int
    {
        MapHeaderBase = 0x11E214
    }
}
