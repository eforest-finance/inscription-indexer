using AeFinder.Sdk.Entities;
using Nest;

namespace Inscription.Indexer.Entities;

public class InscriptionHolder : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    
    [Keyword]
    public string Address { get; set; }
    [Keyword]
    public string Tick { get; set; }
    public long Amt { get; set; }
    
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