namespace Inscription.Indexer.GraphQL;

/// <summary>
/// Implements <see cref="IPagedResult{T}"/>.
/// </summary>
/// <typeparam name="T">Type of the items in the <see cref="ListResultDto{T}.Items"/> list</typeparam>
[Serializable]
public class PagedResultDto<T> : ListResultDto<T>, IPagedResult<T>
{
    /// <inheritdoc />
    public long TotalCount { get; set; } //TODO: Can be a long value..?

    /// <summary>
    /// Creates a new <see cref="PagedResultDto{T}"/> object.
    /// </summary>
    public PagedResultDto()
    {

    }

    /// <summary>
    /// Creates a new <see cref="PagedResultDto{T}"/> object.
    /// </summary>
    /// <param name="totalCount">Total count of Items</param>
    /// <param name="items">List of items in current page</param>
    public PagedResultDto(long totalCount, IReadOnlyList<T> items)
        : base(items)
    {
        TotalCount = totalCount;
    }
}

[Serializable]
public class ListResultDto<T> : IListResult<T>
{
    /// <inheritdoc />
    public IReadOnlyList<T> Items
    {
        get { return _items ?? (_items = new List<T>()); }
        set { _items = value; }
    }
    private IReadOnlyList<T> _items;

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    public ListResultDto()
    {

    }

    /// <summary>
    /// Creates a new <see cref="ListResultDto{T}"/> object.
    /// </summary>
    /// <param name="items">List of items</param>
    public ListResultDto(IReadOnlyList<T> items)
    {
        Items = items;
    }
}
public interface IPagedResult<T> : IListResult<T>, IHasTotalCount
{

}

public interface IListResult<T>
{
    /// <summary>
    /// List of items.
    /// </summary>
    IReadOnlyList<T> Items { get; set; }
}

public interface IHasTotalCount
{
    /// <summary>
    /// Total count of Items.
    /// </summary>
    long TotalCount { get; set; }
}
