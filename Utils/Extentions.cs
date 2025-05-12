using DiffPlex.DiffBuilder.Model;

namespace ExcelComparatorAPI.Utils;

public static class Extentions
{
    public static void TruncateText(this List<DiffPiece> list)
    {
        foreach (DiffPiece item in list)
        {
            List<DiffPiece> subPieces = item.SubPieces.ToList();

            int count = 0;

            for (int i = 0; i < subPieces.Count; i++)
            {
                string txt = subPieces[i].Text;
                if (!string.IsNullOrEmpty(txt) && txt.Length > 10)
                {
                    txt = txt[..10];
                    item.SubPieces[i].Text = txt;
                }

                count += item.SubPieces[i].Text.Length + 1;

                if (count > 10) 
                {                    
                    var p = item.SubPieces[i];
                    item.SubPieces.Remove(p);
                }
            }
        }
    }
}