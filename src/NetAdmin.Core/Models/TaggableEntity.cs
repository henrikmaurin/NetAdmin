public interface ITaggableEntity
{
    string Id { get; }
    string Name { get; }
    string TagsCsv { get; }
}

public static class TaggableExtensions
{
    public static IReadOnlyList<string> GetTags(this ITaggableEntity entity) =>
        string.IsNullOrWhiteSpace(entity.TagsCsv)
            ? []
            : entity.TagsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}