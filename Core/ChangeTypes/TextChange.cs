using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinishMaker.Utilities;

namespace MinishMaker.Core.ChangeTypes
{
    public class TextChange : GlobalChangeBase
    {
        public TextChange(int identifier) : base(DataType.textData, identifier)
        {
        }

        public override string GetEAString(out byte[] binDat)
        {
            //get single bank
            //check if theres a 0th entry
            //check if it fits in the original spot
            //if not add the next entry to be written after
            //repeat check with next entry until it fits

            var lm = LanguageManager.Get();


            var sb = new StringBuilder();
            //int[] data = 
            EnsureLoaded();
            var langStart = ROM.Instance.headers.languageTableLoc;
            var data = LanguageManager.Get().GetAsData2(identifier);
            var combinedText = new List<byte>();
            sb.AppendLine("PUSH");
            foreach(var langBankData in data)
            {
                combinedText.AddRange(langBankData.bankText);
                var prevId = -1;
                for(int i = 0; i < langBankData.entryIds.Length; i++)
                {
                    var entryId = langBankData.entryIds[i];
                    if(entryId == 0)
                    {
                        continue;
                    }

                    if(i == 0 || prevId != entryId-1)
                    {
                        sb.AppendLine("ORG " + (langBankData.bankStart + entryId * 4)); //jump to the next spot if theres a gap
                    }
                    //var distanceModifier = langBankData.entryOffsets[i] - langBankData.bankStart - (entryId * 4); 
                    sb.AppendLine("WORD dataStartx" + identifier + "+" + (langBankData.entryOffsets[i] - langBankData.bankStart));

                    prevId = entryId;
                }

                var zeroEntryPos = Array.IndexOf(langBankData.entryIds, 0);
                if (zeroEntryPos != -1)
                {
                    sb.AppendLine("ORG " + (langBankData.bankStart + langBankData.entryOffsets[zeroEntryPos])); //location of the first entry's text
                    sb.Append("BYTE"); //start writing the 0 entry string for this language
                    var ze = langBankData.zeroEntry;
                    for(int i = 0; i < ze.Length; i++)
                    {
                        sb.Append(" 0x" + ze[i].Hex());
                    }
                    sb.AppendLine();
                }

            }

            sb.AppendLine("POP");
            binDat = combinedText.ToArray();
            sb.AppendLine("dataStartx"+identifier+":");
            sb.AppendLine("#incbin \"./" + changeType.ToString() + identifier.Hex(2) + "Dat.bin\"");
            return sb.ToString();
        }

        public override string GetJSONString()
        {
            return LanguageManager.Get().GetAsJSON(identifier);
        }

        public override void EnsureLoaded()
        {
            LanguageManager.Get().LoadFromJSON(identifier);
        }
    }
}
