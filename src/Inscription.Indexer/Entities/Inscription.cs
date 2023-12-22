using AElf.Indexing.Elasticsearch;
using Nest;

namespace Inscription.Indexer.Entities;

public class Inscription : InscriptionIndexerEntity<string>, IIndexBuild
{
    [Keyword]
    public string CollectionSymbol { get; set; }
    [Keyword]
    public string ItemSymbol { get; set; }
    [Keyword]
    public string Tick { get; set; }
    public long TotalSupply { get; set; }
    public int Decimals { get; set; }
    [Keyword]
    public string Issuer { get; set; }
    public bool IsBurnable { get; set; }
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
}

public class ExternalInfo
{
    public string Key { get; set; }
    public string Value { get; set; }
}