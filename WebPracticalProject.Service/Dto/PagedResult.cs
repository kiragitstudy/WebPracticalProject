namespace WebPracticalProject.Service.Dto;

public sealed class PagedResult<T>
{
    public required int Page { get; init; }
    public required int Size { get; init; }
    public required int Total { get; init; }
    public required IReadOnlyList<T> Items { get; set; }
}