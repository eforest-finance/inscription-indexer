namespace Inscription.Indexer.GraphQL;

public class GetInscriptionInput
{
    public string ChainId { get; set; }
    public string Tick { get; set; }
    public long BeginBlockHeight { get; set; }
    public long EndBlockHeight { get; set; }
}