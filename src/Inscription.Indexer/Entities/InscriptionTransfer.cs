using AElf.Indexing.Elasticsearch;
using Nest;

namespace Inscription.Indexer.Entities;

public class InscriptionTransfer : InscriptionIndexerEntity<string>, IIndexBuild
{
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
}