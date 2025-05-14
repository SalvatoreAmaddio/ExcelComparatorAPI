using DiffPlex.DiffBuilder.Model;

namespace ExcelComparatorAPI.xlComparator;

public static class DiffPieceExtetions
{
    public static List<KeyValuePair<string, string?>> GetSubPiecesInfo(this DiffPiece line, bool isOld)
    {
        List<KeyValuePair<string, string?>> details = [];

        foreach (DiffPiece? piece in line.SubPieces)
        {
            if (string.IsNullOrEmpty(piece?.Text))
                continue;

            ChangeType subType = piece.Type switch
            {
                ChangeType.Modified => isOld ? ChangeType.Deleted : ChangeType.Inserted,
                ChangeType.Inserted => ChangeType.Inserted,
                ChangeType.Deleted => ChangeType.Deleted,
                ChangeType.Unchanged => ChangeType.Unchanged,
                _ => ChangeType.Imaginary
            };

            string? subTypeStr = subType != ChangeType.Imaginary ? subType.ToString() : null;

            if (details.Count > 0)
            {
                KeyValuePair<string, string?> last = details[details.Count - 1];

                if (string.Equals(last.Value, subTypeStr, StringComparison.InvariantCulture))
                {
                    details[^1] = new KeyValuePair<string, string?>(last.Key + piece.Text, subTypeStr);
                    continue;
                }
            }

            details.Add(new KeyValuePair<string, string?>(piece.Text, subTypeStr));
        }

        return details;
    }
}