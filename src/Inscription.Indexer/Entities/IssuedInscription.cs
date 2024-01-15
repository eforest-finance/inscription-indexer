using AElf.Indexing.Elasticsearch;
using Nest;

namespace Inscription.Indexer.Entities;

public class IssuedInscription : InscriptionIndexerEntity<string>, IIndexBuild
{
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
}