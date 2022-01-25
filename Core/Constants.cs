﻿namespace MinishMaker.Core
{
    public class Constants
    {
        public static readonly string _listData = "listData";
    }

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

        //added for now, change names to whatever you want
        public int tileOffset;
        public int paletteSetTableLoc;
        public int chunk0TableLoc;
        public int area1Chunk0TableLoc;
        public int chunk1TableLoc;
        public int chunk2TableLoc;
        public int swapBase;
        public int paletteChangeBase;
        public int area1SwapBase;
        public int globalTileSetTableLoc;
        public int gfxSourceBase;
        public int globalMetaTileSetTableLoc;
        public int globalTileDataTableLoc;
        public int areaInformationTableLoc;
        public int warpInformationTableLoc;
        public int languageTableLoc;

        public HeaderData(int map, int area, int tileOffset, int paletteSetTableLoc, int c0TableLoc, int a1C0TableLoc, int c1TableLoc, int c2TableLoc, int swapBase, int paletteChangeBase, int area1SwapBase, int globalTileSetTableLoc, int gfxSourceBase, int globalMetaTileSetTableLoc, int globalTileDataTableLoc, int areaInformationTableLoc, int warpInformationTableLoc, int languageTableLoc)
        {
            this.MapHeaderBase = map;
            this.AreaMetadataBase = area;
            this.tileOffset = tileOffset;
            this.paletteSetTableLoc = paletteSetTableLoc;
            this.chunk0TableLoc = c0TableLoc; //looks like each next chunkTable is a 0x16 further (eu and us), not adding because of possible jp
            this.area1Chunk0TableLoc = a1C0TableLoc;
            this.chunk1TableLoc = c1TableLoc; //c0+ 0x16
            this.chunk2TableLoc = c2TableLoc; //c0+ 0x32
            this.swapBase = swapBase;
            this.paletteChangeBase = paletteChangeBase;
            this.area1SwapBase = area1SwapBase; //above -0x140?
            this.globalTileSetTableLoc = globalTileSetTableLoc;
            this.gfxSourceBase = gfxSourceBase;
            this.globalMetaTileSetTableLoc = globalMetaTileSetTableLoc;
            this.globalTileDataTableLoc = globalTileDataTableLoc;
            this.areaInformationTableLoc = areaInformationTableLoc;
            this.warpInformationTableLoc = warpInformationTableLoc;
            this.languageTableLoc = languageTableLoc;
        }
    }

    public enum TileEntityType
    {
        None = 0x00,
        TestA = 0x01,
        Chest = 0x02,
        BigChest = 0x03,
        TestB = 0x04,
        TestC = 0x05,

    }

    public enum EnemyTypes
    {
        Octorok = 0x00,
        Chuchu = 0x01,
        Leever = 0x02,
        Peahat = 0x03,
        Rollobite = 0x04,
        Darknut = 0x05,
        MinishNut = 0x06,
        Beetle = 0x07,
        Keese = 0x08,
        DoorMimic = 0x09,
        RockChuchu = 0x0A,
        SpinyChuchu = 0x0B,
        Chick = 0x0C,
        Moldorm = 0x0D,
        MoldWorm = 0x0F,
        Sluggula = 0x10,
        Pesto = 0x11,
        PuffStool = 0x12,
        BigGreenChu = 0x13,
        LikeLike = 0x14,
        SpearMoblin = 0x15,
        DekuScrub = 0x16,
        RupeeLike = 0x17,
        Madderpillar = 0x18,
        GiantRaindrop = 0x19,
        Wallmaster = 0x1A,
        BombPeahat = 0x1B,
        Spark = 0x1C,
        Chaser = 0x1D,
        SpikedBeetle = 0x1E,
        BlueTrap = 0x1F,
        Helmasaur = 0x20,
        Boulder = 0x21,
        BobOmb = 0x22,
        Floormaster = 0x23,
        Gleerok = 0x24,
        VaatiHandEyes = 0x25,
        Tektite = 0x26,
        NormalWizrobe = 0x27,
        FireWizrobe = 0x28,
        IceWizrobe = 0x29,
        Armos = 0x2A,
        Eyegore = 0x2B,
        Rope = 0x2C,
        Smallpesto = 0x2D,
        AcroBandits = 0x2E,
        GreenTrap = 0x2F,
        Keaton = 0x30,
        Crow = 0x31,
        Mulldozer = 0x32,
        Bombarossa = 0x33,
        Bubble = 0x34,
        SpinyBeetle = 0x35,
        Mazaal = 0x36,
        MazaalPillar = 0x37,
        BigOctorok = 0x39,
        FlyingPot = 0x3A,
        Gibdo = 0x3B,
        GoldOctorock = 0x3C,
        GoldTektite = 0x3D,
        GoldRope = 0x3E,
        CloudPiranha = 0x3F,
        ScissorsBeetle = 0x40,
        Cucco = 0x41,
        Stalfos = 0x42,
        Skull = 0x43,
        RedCrow = 0x45,
        BowMoblin = 0x46,
        Lakitu = 0x47,
        BallAndChainSoldier = 0x4C,
        Ghini = 0x4E,
        BonkTree = 0x65,
    }

    public enum KinstoneType
    {
        UnTyped,

        YellowTornadoProng = 0x65,
        YellowTornadoSpike = 0x66,
        YellowTornadoChaotic = 0x67,
        //68 and 69 are repeats of above

        YellowTotemProng = 0x6A,
        YellowTotemWave = 0x6B,
        YellowTotemZigZag = 0x6C,

        YellowCrown = 0x6D,

        RedSpike = 0x6E,
        RedCrack = 0x6F,
        RedProng = 0x70,

        BlueL = 0x71,
        BlueS = 0x72,

        GreenSpike = 0x73,
        GreenSquare = 0x74,
        GreenSplit = 0x75,
    }

    public enum ItemType
    {
        Untyped,
        SmithSword = 0x01,
        GreenSword = 0x02,
        RedSword = 0x03,
        BlueSword = 0x04,
        //      UnusedSword = 0x05,
        FourSword = 0x06,
        Bombs = 0x07,
        RemoteBombs = 0x08,
        Bow = 0x09,
        LightArrow = 0x0A,
        Boomerang = 0x0B,
        MagicBoomerang = 0x0C,
        Shield = 0x0D,
        MirrorShield = 0x0E,
        LanternOff = 0x0F,

        GustJar = 0x11,
        PacciCane = 0x12,
        MoleMitts = 0x13,
        RocsCape = 0x14,
        PegasusBoots = 0x15,
        FireRod = 0x16,
        Ocarina = 0x17,
        Bottle1 = 0x1C,
        Bottle2 = 0x1D,
        Bottle3 = 0x1E,
        Bottle4 = 0x1F,
        BottleEmpty = 0x20,
        BottleButter = 0x21,
        BottleMilk = 0x22,
        BottleHalfMilk = 0x23,
        BottleRedPotion = 0x24,
        BottleBluePotion = 0x25,
        BottleWater = 0x26,
        BottleMineralWater = 0x27,
        BottleFairy = 0x28,
        BottlePicolyteRed = 0x29,
        BottlePicolyteOrange = 0x2A,
        BottlePicolyteYellow = 0x2B,
        BottlePiclolyteGreen = 0x2C,
        BottlePicolyteBlue = 0x2D,
        BottlePicolyteWhite = 0x2E,
        BottleCharmNayru = 0x2F,
        BottleCharmFarore = 0x30,
        BottleCharmDin = 0x31,


        SmithSwordQuest = 0x34,
        BrokenPicoriBlade = 0x35,
        DogFoodBottle = 0x36,
        LonLonKey = 0x37,
        WakeUpMushroom = 0x38,
        HyruleanBestiary = 0x39,
        PicoriLegend = 0x3A,
        MaskHistory = 0x3B,
        GraveyardKey = 0x3C,
        TingleTrophy = 0x3D,
        CarlovMedal = 0x3E,
        ShellsX = 0x3F,
        EarthElement = 0x40,
        FireElement = 0x41,
        WaterElement = 0x42,
        WindElement = 0x43,
        GripRing = 0x44,
        PowerBracelets = 0x45,
        Flippers = 0x46,
        HyruleMap = 0x47,
        SpinAttack = 0x48,
        RollAttack = 0x49,
        DashAttack = 0x4A,
        RockBreaker = 0x4B,
        SwordBeam = 0x4C,
        GreatSpin = 0x4D,
        DownThrust = 0x4E,
        PerilBeam = 0x4F,
        DungeonMap = 0x50,
        Compass = 0x51,
        BigKey = 0x52,
        SmallKey = 0x53,
        Rupee1 = 0x54,
        Rupee5 = 0x55,
        Rupee20 = 0x56,
        Rupee50 = 0x57,
        Rupee100 = 0x58,
        Rupee200 = 0x59,

        JabberNut = 0x5B,
        KinstoneX = 0x5C,
        Bombs5 = 0x5D,
        Arrows5 = 0x5E,
        SmallHeart = 0x5F,
        Fairy = 0x60,
        Shells30 = 0x61,
        HeartContainer = 0x62,
        PieceOfHeart = 0x63,
        Wallet = 0x64,
        BombBag = 0x65,
        LargeQuiver = 0x66,
        KinstoneBag = 0x67,
        Brioche = 0x68,
        Croissant = 0x69,
        PieSlice = 0x6A,
        CakeSlice = 0x6B,
        Bombs10 = 0x6C,
        Bombs30 = 0x6D,
        Arrows10 = 0x6E,
        Arrows30 = 0x6F,
        ArrowButterfly = 0x70,
        DigButterfly = 0x71,
        SwimButterfly = 0x72,
        FastSpin = 0x73,
        FastSplit = 0x74,
        LongSpin = 0x75
    }

    public enum Object6Types
    {
        Item = 0x00,
        EnemyDeathEffect = 0x01,
        SaleItem = 0x02,
        PressureSwitch = 0x03,
        //0x04,
        Pot = 0x05,
        //EzloShrinkAnimation - 0x06,
        PushRock = 0x07,
        DungeonDoor = 0x08,
        //0x09,
        KeyBlock = 0x0A,
        //0x0B,
        //SecretAnimation = 0x0C,
        Skull = 0x0D,
        FallingTile = 0x0E,
        SpecialEffect = 0x0F,
        //WarpEffect = 0x10,
        //VanishingRock = 0x11,
        BigRockDoor = 0x12,
        MinecartTrack = 0x13,
        Lilypad = 0x14,
        //weird sprite = 0x15,
        MovingPlatform = 0x16,
        //TinyWhitePuff = 0x17,
        BlueMask = 0x18,
        //Crash = 0x19,////
        BouncingItem = 0x1A,
        //GreatFairyWings = 0x1B,
        MailSign = 0x1C,
        //0x1D,
        DiggingDirt = 0x1E,
        //VanishingBush = 0x1F,
        //Explosion2 = 0x20,
        YellowDust = 0x21,
        GachaMachines = 0x22,
        ArrowEye = 0x23,
        ClonePressureSwitch = 0x24,
        RotatingBarrel = 0x25,
        BarrelBackground = 0x26,
        PushableStatue = 0x27,
        //0x28,
        AmbientClouds = 0x29,
        ArmosFlame = 0x2A,
        //Dot = 0x2B,
        GrowingVine = 0x2C,
        SmithChimney = 0x2D,
        PushableBoulder = 0x2E,
        Lever = 0x2F,
        //floating item? = 0x30,
        FrozeDecoration = 0x31,
        Mushroom = 0x32,
        WaterBarrier = 0x33,
        WarpTile = 0x34,
        //gacha animation = 0x35,
        //0x36,
        //0x37,
        MinishPot = 0x38,
        BigKeyDoor = 0x39,
        //triangle transition = 0x3A,
        //minish portal sprites = 0x3B,
        //link falling = 0x3C,
        //dark transition = 0x3D,
        //minish town+ portal = 0x3E,
        BigLeaf = 0x3F,
        FlyingFairy = 0x40, //Form = 60
        Ladder = 0x41,
        //0x42,
        //0x43,
        //0x44,
        //0x45,
        BookLadder = 0x46,
        HeartContainer = 0x47,
        //0x48,
        //0x49,
        BackgroundCloud = 0x4A,
        //chuchu animation = 0x4B,
        PushableFurniture = 0x4C,
        NonPushableFurniture = 0x4D,
        MinishDoors = 0x4E,  
        Archways = 0x4F,
        BigRockDecoration = 0x50,
        BigRock = 0x51,
        //0x52,
        //bush thrown = 0x53,
        PullHandle = 0x54,
        Minecart = 0x55,
        //bubble? = 0x56,
        HiddenDownLadder = 0x57,
        GentariCurtains = 0x58,
        LavaPlatform = 0x59,
        Paper = 0x5A,
        //sleep sprites = 0x5B,
        WallMask = 0x5C,
        Door = 0x5D,
        WhirlWind = 0x5E,
        PushStaircase = 0x5F,
        SwordsmanNewsletter = 0x60,
        //0x61,
        Twig = 0x62,
        //rupee/heart item drops = 0x63,
        //lightning = 0x64,
        ShelfLadderHole = 0x65,
        //0x66,
        //lava stuff = 0x67,
        //bottle spill = 0x68,
        Cutscene = 0x69,
        //some items? = 0x6A,
        CrenelBean = 0x6B,
        MinecartDoor = 0x6C,
        RockPillar = 0x6D,
        MineralWaterSource = 0x6E,
        MinishHouseDoor = 0x6F,
        //swamp splash = 0x70,
        Tombstone = 0x71,
        StoneTablet = 0x72,
        MinishLilypad = 0x73,
        //0x74,
        //minish transform sprites = 0x75,
        //0x76,
        Bell = 0x77,
        BigFlower = 0x78,
        //floating minish letters = 0x79,
        FlamesSteam = 0x7A,
        PushableLever = 0x7B,
        BigShoe = 0x7C,
        //breaking animation = 0x7D,
        //7E,
        PictoBloom = 0x7F,
        Board = 0x80,
        //small fence = 0x81,
        BigTornado = 0x82,
        BigPushableLever = 0x83,
        SmallIceBlock = 0x84,
        BigIceBlock = 0x85,
        CloverTrapdoor = 0x86,
        //fireball = 0x87,
        GiantBook = 0x88,
        MazaalBase = 0x89,
        MayorLakeFurniture = 0x8A,
        DoubleBookshelf = 0x8B,
        Book = 0x8C,
        Fireplace = 0x8D,
        //black screen earthquake = 0x8E,
        WaterElementIceBlock = 0x8F,
        //water element without ice = 0x90,
        //0x91,
        BreadFurnace = 0x92,
        Lamp = 0x93,
        WindySign = 0x94,
        Birds = 0x95,
        //gold key stuff = 0x96,
        // ^ = 0x97,
        //0x98,
        //0x99,
        GiantAcorn = 0x9A,
        //0x9B,
        PegasusTree = 0x9C,
        ToggleSwitch = 0x9D,
        TreeThorns = 0x9E,
        SmallWindTurbine = 0x9F,
        AngryStatue = 0xA0,
        Door2 = 0xA1,
        //bounce noise = 0xA2,
        //secret opening = 0xA3,
        GlowingDecor = 0xA4,
        FireballChain = 0xA5,
        //0xA6,
        //cloneswitch2? = 0xA7,
        //0xA8,
        //spawn animation = 0xA9,
        //waterfall cave opening = 0xAA,
        VaatiTrapdoor = 0xAB,
        FourElements = 0xAC,
        //bg3 stuff = 0xAD,
        FloatingBlocks = 0xAE,
        //0xAF,
        ElementalMetalDoor = 0xB0,
        Jailbars = 0xB1,
        //0xB2,
        //kinstone shiny animation = 0xB3,
        //japanese titlescreen subs = 0xB4,
        //move screen to pos = 0xB5,
        //0xB6,
        WellDrop = 0xB7,
        SkyWarp = 0xB8,
        //0xB9,
        //0xBA,
        WindCrestStone = 0xBB,
        LightInDarkness = 0xBC,
        //title screen = 0xBD,
        Pinwheel = 0xBE,
        //0xBF,
        EnemyCarryItem = 0xC0,
        //link item get anim = 0xC1,
        //0xC2,
        //0xC3,
        Fog = 0xC4
    }

    public enum NPCTypes
    {
        Gentari = 0x01,
        Festari = 0x02,
        ForestMinish = 0x03,
        Postman = 0x04,
        //Companion = 0x05,
        People = 0x06,
        People2 = 0x07,
        Soldier = 0x08,
        //broken = 0x09,
        Stamp = 0x0A,
        RedServant = 0x0B,
        Marcy = 0x0C,
        Wheaton = 0x0D,
        Pita = 0x0E,
        MinishEzlo = 0x0F,
        Postbox = 0x10,
        Beedle = 0x11,
        Brocco = 0x12,
        People3 = 0x13,
        Pina = 0x14,
        Soldier2 = 0x15,
        BlueServant = 0x16,
        Din = 0x17,
        Nayru = 0x18,
        Farore = 0x19,
        Sturgeon = 0x1A,
        Tinglets = 0x1B,
        Stockwell = 0x1C,
        Talon = 0x1D,
        Malon = 0x1E,
        Epona = 0x1F,
        EponaWagon = 0x20,
        Ghosts = 0x21,
        Smith = 0x22,
        //broken = 0x23
        King = 0x24,
        Minister = 0x25,
        Soldier3 = 0x26,
        //NSFWHero = 0x27,
        Zelda = 0x28,
        Mutoh = 0x29,
        Mack = 0x2A,
        CastorTotem = 0x2B,
        Cats = 0x2C,
        MountainMinish = 0x2D,
        ZeldaCompanion = 0x2E,
        Melari = 0x2F,
        BladeBrothers = 0x30,
        Cow = 0x31,
        Goron = 0x32,
        GoronMerchant = 0x33,
        Gorman = 0x34,
        Dogs = 0x35,
        Syrup = 0x36,
        Rem = 0x37,
        TownMinish = 0x38,
        Librari = 0x39,
        Percy = 0x3A,
        VaatiReborn = 0x3B,
        MoblinLady = 0x3C,
        Clerks = 0x3D,
        Farmers = 0x3E,
        Rlovs = 0x3F,
        Dampe = 0x40,
        DrLeft = 0x41,
        GhostKing = 0x42,
        Gina = 0x43,
        Simon = 0x44,
        Anju = 0x45,
        Mama = 0x46,
        Emma = 0x47,
        Teachers = 0x48,
        WindTribe = 0x49,
        Gregal = 0x4A,
        Mayor = 0x4B,
        Biggoron = 0x4C,
        Ezlo = 0x4D,
        //0x4E,
        DryingRacks = 0x4F,
        PicolyteBottles = 0x50,
        SmallTownMinish = 0x52,
        HurdyGurdy = 0x53,
        Cucco = 0x54,
        Chick = 0x55,
        //0x56,
        Phonograph = 0x57,
    }


    public enum ObjectCategories
    {
        Enemy = 0x03,
        Object = 0x06,
        NPC = 0x07,
        Event = 0x09,
        Enemy2 = 0x13,
        Object2 = 0x16
    }

    public class Header
    {
        // Will fill out when relevant, only need EU for now
        private readonly HeaderData[] headerTable =
        {   //             MAP     , ENTITY?,	TILEOFFSET	PALETTESET	CHUNK0		CHUNK0AREA1	CHUNK1		CHUNK2		SWAP		PALETTECHANGE	AREA1SWAP	TILESET		GFXSOURCE	METATILE	TILEDATA	AREADATA	WARP		LANGUAGE
            new HeaderData(0x11D95C, 0x0D4828,  0x5A23D0,   0xFED88,    0x107AEC,   0x1077AC,   0x107B02,   0x107B18,   0x107B5C,   0x107940,       0x107800,   0x101BC8,   0x323FEC,   0x1027F8,   0x1070E4,   0x127468,   0x139EDC,   0x108968),
            new HeaderData(0x11DED8, 0x0D4E9C,  0x5A2B20,   0xFF500,    0x10805C,   0x107D18,   0x108072,   0x108088,   0x1080CC,   0x107EAC,       0x107D6C,   0x102134,   0x324710,   0x102D64,   0x107650,   0x1279F4,   0x13A41C,   0x0),
            new HeaderData(0x11E214, 0x0D50FC,  0x5A2E80,   0xFF850,    0x108398,   0x108050,   0x1083AE,   0x1083C4,   0x108408,   0x1081E4,       0x1080A4,   0x10246C,   0x324AE4,   0x10309C,   0x107988,   0x127D30,   0x13A7F0,   0x0)
        };

        public HeaderData GetHeaderAddresses(RegionVersion region)
        {
            return headerTable[(int)region];
        }
    }
}
