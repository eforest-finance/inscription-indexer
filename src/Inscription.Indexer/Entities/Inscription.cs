using AeFinder.Sdk.Entities;
using Nest;

namespace Inscription.Indexer.Entities;

public class Inscription : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    
    [Keyword]
    public string Tick { get; set; }
    public long TotalSupply { get; set; }
    [Keyword]
    public string Issuer { get; set; }
    public int IssueChainId { get; set; }
    public Dictionary<string,string> CollectionExternalInfo { get; set; } = new();
    public Dictionary<string,string> ItemExternalInfo { get; set; } = new();
    [Keyword]
    public string Owner { get; set; }
    public long Limit { get; set; }
    [Keyword]
    public string Deployer { get; set; }
    [Keyword]
    public string TransactionId { get; set; }
    
    public DateTime BlockTime { get; set; }
    
    [Keyword]
    public string ChainId { get; set; }

    [Keyword]
    public string BlockHash { get; set; }

    public long BlockHeight { get; set; }

    [Keyword]
    public string PreviousBlockHash { get; set; }

    public bool IsDeleted { get; set; }
}