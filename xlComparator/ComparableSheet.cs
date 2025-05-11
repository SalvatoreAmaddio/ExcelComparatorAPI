using DiffPlex.DiffBuilder.Model;
using DiffPlex.DiffBuilder;

namespace ExcelComparatorAPI.xlComparator;

public class ComparableSheet(string name, string oldContent, string newContent)
{
    public string Name { get; } = name;
    public string OldContent { get; } = oldContent;
    public string NewContent { get; } = newContent;

    public bool HasChanges { get; private set; }

    public DiffPaneModel? InlineResult { get; private set; }
    public SideBySideDiffModel? SideBySideResult { get; private set; }

    public async Task BuildAsync()
    {
        Task<SideBySideDiffModel> sideBySideTask = Task.Run(() => SideBySideDiffBuilder.Diff(OldContent, NewContent, true, false));

        Task<DiffPaneModel> inlineTask = Task.Run(() =>
        {
            return InlineDiffBuilder.Diff(OldContent, NewContent, true, false);
        });

        await Task.WhenAll(sideBySideTask, inlineTask);

        SideBySideResult = sideBySideTask.Result;
        InlineResult = inlineTask.Result;

        HasChanges = InlineResult.HasDifferences;
    }

    public override bool Equals(object? obj)
    {
        return obj is ComparableSheet model &&
               Name == model.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }

    public override string? ToString()
    {
        return Name;
    }
}