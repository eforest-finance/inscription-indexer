namespace Inscription.Indexer.GraphQL;

public class GetInscriptionInput : PagedResultQueryInput
{
    public string? ChainId { get; set; }
    public string? Tick { get; set; }
    public long? BeginBlockHeight { get; set; }
    public long? EndBlockHeight { get; set; }
}