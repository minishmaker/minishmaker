using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MinishMaker.Core;

namespace MinishMaker.Utilities
{
    public class PaletteSetManager
    {
        private static PaletteSetManager instance;
        private Dictionary<int, PaletteSet> paletteSets = new Dictionary<int, PaletteSet>();
        private Dictionary<int, List<int>> areaIdUsages = new Dictionary<int, List<int>>();
        private Reader r = ROM.Instance.reader;

        private PaletteSetManager() { }
        public static PaletteSetManager Get()
        {
            if (instance == null)
            {
                instance = new PaletteSetManager();
            }

            return instance;
        }


        public PaletteSet GetSet(int id)
        {
            if (!paletteSets.ContainsKey(id))
            {
                paletteSets.Add(id, new PaletteSet(id));
            }

            return paletteSets[id];
        }

        public byte[] GetData(int id)
        {
            if (!paletteSets.ContainsKey(id))
            {
                paletteSets.Add(id, new PaletteSet(id));
            }

            var ps = paletteSets[id];
            var data = new byte[ps.amount * 16 * 2];
            for(int i = 0; i < ps.amount * 16; i++)
            {
                var color = ps.Colors[ps.start * 0x10 + i];
                //int red = (pd & 0x1F) << 3;
                //int green = ((pd >> 5) & 0x1F) << 3;
                //int blue = ((pd >> 10) & 0x1F) << 3;

                ushort pd = (ushort)((color.R >> 3) + ((color.G >> 3) << 5) + ((color.B >> 3) << 10));
                data[i * 2] = (byte)(pd & 0xFF);
                data[i * 2 + 1] = (byte)(pd >> 8); 
            }

            return data;
        }

        public void ReportUsage(int areaId, int paletteSetId)
        {
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            int paletteSetTableLoc = header.paletteSetTableLoc;
            int addr = r.ReadAddr(paletteSetTableLoc + (paletteSetId * 4)); //4 byte entries
            int palAddr = r.ReadUInt16(addr);

            if (!areaIdUsages.ContainsKey(paletteSetId))
            {
                areaIdUsages.Add(paletteSetId, new List<int>());
            }
            areaIdUsages[paletteSetId].Add(areaId);
        }

        public List<int> GetIdUsage(int id)
        {
            return areaIdUsages[id];
        }

        public void ChangeUsage(int areaId,int oldId, int newId)
        {
            areaIdUsages[oldId].Remove(areaId);

            if (!areaIdUsages.ContainsKey(newId))
            {
                areaIdUsages.Add(newId, new List<int>());
            }
            areaIdUsages[newId].Add(areaId);

        }

        public void Clear()
        {
            areaIdUsages.Clear();
            paletteSets.Clear();
        }
    }
}
