namespace Inscription.Indexer.GraphQL;

public class GetIssuedInscriptionInput : PagedResultQueryInput
{
    public string? ChainId { get; set; }
    public string? Tick { get; set; }
    public bool? IsCompleted { get; set; }
}