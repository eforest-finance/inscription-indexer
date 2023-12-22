namespace Inscription.Indexer.GraphQL;

public class InscriptionHolderDto : GraphQLDto
{
    public string Address { get; set; }
    public string Tick { get; set; }
    public long Amt { get; set; }
}