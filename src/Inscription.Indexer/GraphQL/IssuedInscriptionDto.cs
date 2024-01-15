namespace Inscription.Indexer.GraphQL;

public class IssuedInscriptionDto : GraphQLDto
{
    public long Amt { get; set; }
    public string Tick { get; set; }
    public string IssuedToAddress { get; set; }
    public DateTime IssuedTime { get; set; }
    public long MintedAmt { get; set; }
    public float Progress { get; set; }
    public int HolderCount { get; set; }
    public int TransactionCount { get; set; }
    public bool IsCompleted { get; set; }
    public string IssuedTransactionId { get; set; }
    public string Image { get; set; }
}