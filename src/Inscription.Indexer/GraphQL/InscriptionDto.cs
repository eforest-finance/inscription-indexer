namespace Inscription.Indexer.GraphQL;

public class InscriptionDto : GraphQLDto
{
    public string Tick { get; set; }
    public long TotalSupply { get; set; }
    public string Issuer { get; set; }
    public int IssueChainId { get; set; }
    public List<ExternalInfoDto> CollectionExternalInfo { get; set; } = new();
    public List<ExternalInfoDto> ItemExternalInfo { get; set; } = new();
    public string Owner { get; set; }
    public long Limit { get; set; }
    public string Deployer { get; set; }
    public string TransactionId { get; set; }
}

public class ExternalInfoDto
{
    public string Key { get; set; }
    public string Value { get; set; }
}