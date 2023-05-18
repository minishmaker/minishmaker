using MinishMaker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes
{
	public class PaletteChange: GlobalChangeBase
	{
		public PaletteChange(int pSetId) : base(DataType.palette, pSetId){}

		public override string GetEAString(out byte[] binDat)
		{
			var sb = new StringBuilder();
            var r = ROM.Instance.reader;
            var header = ROM.Instance.headers;
            int paletteSetTableLoc = header.paletteSetTableLoc;
            int tileOffset = header.tileOffset;

            // location of another id = constant + id * 4
            // location of pal data = constant + other id * 0x20;
            // id (2b) start (1b) amount (1b)
            int idLocId = identifier; //id to use with palettesettableLoc
            int idLoc = r.ReadAddr(paletteSetTableLoc + (idLocId * 4)); //pointer to a 2 byte id, and 2 bytes of data
            int palId = r.ReadUInt16(idLoc);//id to use with tileOffset
            int pointerLoc = (int)(tileOffset + (palId << 5)); //0x20 aligned 0xFFFF * 0x20 = 0x1FFFE0 + 0x5A23D0 = 0x7A23B0 max
            var gfxOffset = ROM.Instance.headers.gfxSourceBase;
			byte[] data = PaletteSetManager.Get().GetData(identifier);

			sb.AppendLine("PUSH");	//save cursor location
			sb.AppendLine("ORG "+pointerLoc);	//go to pointer location
            sb.AppendLine("#incbin \"./" + changeType.ToString() + identifier.Hex(2) + "Dat.bin\"");
            sb.AppendLine("POP");
			binDat = data;
			
			return sb.ToString();
		}

        public override string GetJSONString()
        {
            var psm = PaletteSetManager.Get();
            var data = psm.GetData(identifier);
            var json = DataHelper.ByteArrayToFormattedJSON(data, 16, 2);
            return json;
        }

        public override void EnsureLoaded()
        {
            PaletteSetManager.Get().GetSet(identifier);
        }
    }
}
