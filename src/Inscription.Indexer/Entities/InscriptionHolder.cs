using AElf.Indexing.Elasticsearch;
using Nest;

namespace Inscription.Indexer.Entities;

public class InscriptionHolder : InscriptionIndexerEntity<string>, IIndexBuild
{
    [Keyword]
    public string Address { get; set; }
    [Keyword]
    public string Tick { get; set; }
    public long Amt { get; set; }
}