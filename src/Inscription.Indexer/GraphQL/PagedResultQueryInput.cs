using Volo.Abp.Application.Dtos;

namespace Inscription.Indexer.GraphQL;

public class PagedResultQueryInput : PagedAndSortedResultRequestDto
{
    public void Validate()
    {
        if (MaxResultCount > MaxMaxResultCount)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxResultCount),
                $"Max allowed value for {nameof(MaxResultCount)} is {MaxMaxResultCount}.");
        }
    }
}