namespace Inscription.Indexer.GraphQL;

public class GetInscriptionHolderInput : PagedResultQueryInput
{
    public string ChainId { get; set; }
    public string Tick { get; set; }
}