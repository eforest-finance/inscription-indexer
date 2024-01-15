using AElfIndexer;

namespace Inscription.Indexer.GraphQL;

public class GetSyncStateDto
{
    public string ChainId { get; set; }
    public BlockFilterType FilterType { get; set; }
}