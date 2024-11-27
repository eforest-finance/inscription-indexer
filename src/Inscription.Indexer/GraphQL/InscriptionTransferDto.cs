namespace Inscription.Indexer.GraphQL;

public class InscriptionTransferDto : GraphQLDto
{
    public string TransactionId { get; set; }
    public string Method { get; set; }
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public string InscriptionInfo { get; set; }
    public string Tick { get; set; }
    public string? InscriptionImage { get; set; }
    public long Amt { get; set; }
    public int Number { get; set; }
}