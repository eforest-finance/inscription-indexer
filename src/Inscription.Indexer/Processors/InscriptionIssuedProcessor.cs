using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Forest.Contracts.Inscription;
using Microsoft.Extensions.Logging;

namespace Inscription.Indexer.Processors;

public class InscriptionIssuedProcessor : InscriptionProcessorBase<InscriptionIssued>
{
    public InscriptionIssuedProcessor(
        ILogger<AElfLogEventProcessorBase<InscriptionIssued, LogEventInfo>> logger) : base(logger)
    {
    }

    protected override async Task HandleEventAsync(InscriptionIssued eventValue, LogEventContext context)
    {
        var id = IdGenerateHelper.GetId(context.ChainId, eventValue.Tick);
        var inscription = new Entities.IssuedInscription
        {
            Id = id,
            IssuedTransactionId = context.TransactionId,
            IssuedTime = context.BlockTime,
            IssuedToAddress = eventValue.To.ToBase58()
        };
        
        ObjectMapper.Map(context, inscription);
        ObjectMapper.Map(eventValue, inscription);
        
        await IssuedInscriptionRepository.AddOrUpdateAsync(inscription);
        
        await AddInscriptionTransferAsync(context, "Deploy", eventValue.To.ToBase58(), eventValue.To.ToBase58(),
            eventValue.Tick,
            eventValue.Amt, eventValue.InscriptionInfo);
    }
}