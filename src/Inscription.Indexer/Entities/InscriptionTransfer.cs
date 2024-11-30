using AeFinder.Sdk.Entities;
using Nest;

namespace Inscription.Indexer.Entities;

public class InscriptionTransfer : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    
    [Keyword]
    public string TransactionId { get; set; }
    [Keyword]
    public string Method { get; set; }
    [Keyword]
    public string FromAddress { get; set; }
    [Keyword]
    public string ToAddress { get; set; }
    [Keyword]
    public string InscriptionInfo { get; set; }
    [Keyword]
    public string Tick { get; set; }
    public long Amt { get; set; }
    public long Number { get; set; }
    
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