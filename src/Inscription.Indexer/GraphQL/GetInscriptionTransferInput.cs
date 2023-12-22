namespace Inscription.Indexer.GraphQL;

public class GetInscriptionTransferInput : PagedResultQueryInput
{
    public string ChainId { get; set; }
}