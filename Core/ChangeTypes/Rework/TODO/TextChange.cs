using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinishMaker.Core.ChangeTypes.Rework.TODO
{
    public class TextChange : GlobalChangeBase
    {
        public TextChange() : base(Core.Rework.DataType.textData)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            var sb = new StringBuilder();
            int[] data = LanguageManager.Get().GetAsData(out binDat);
            var pointerLoc = ROM.Instance.headers.languageTableLoc;
            sb.AppendLine("PUSH");
            sb.AppendLine("ORG " + pointerLoc);
            for (int i = 0; i<data.Length; i++)
            {
                sb.AppendLine("DWORD langStart+" + data[i]);
            }
            sb.AppendLine("POP");
            sb.AppendLine("ALIGN 4");
            sb.AppendLine("langStart:");
            sb.AppendLine("#incbin \"./" + changeType.ToString() + identifier + "Dat.bin\"");
            return sb.ToString();
        }
    }
}
