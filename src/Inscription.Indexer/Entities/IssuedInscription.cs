using AeFinder.Sdk.Entities;
using Nest;

namespace Inscription.Indexer.Entities;

public class IssuedInscription : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    
    public long Amt { get; set; }
    [Keyword]
    public string Tick { get; set; }
    [Keyword]
    public string IssuedToAddress { get; set; }
    public DateTime IssuedTime { get; set; }
    public long MintedAmt { get; set; }
    public float Progress { get; set; }
    public int HolderCount { get; set; }
    public int TransactionCount { get; set; }
    public bool IsCompleted { get; set; }
    [Keyword]
    public string IssuedTransactionId { get; set; }
    
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