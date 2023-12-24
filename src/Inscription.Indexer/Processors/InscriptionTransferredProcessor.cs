using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Forest.Contracts.Inscription;
using Microsoft.Extensions.Logging;

namespace Inscription.Indexer.Processors;

public class InscriptionTransferredProcessor : InscriptionProcessorBase<InscriptionTransferred>
{
    public InscriptionTransferredProcessor(
        ILogger<AElfLogEventProcessorBase<InscriptionTransferred, LogEventInfo>> logger) : base(logger)
    {
    }

    protected override async Task HandleEventAsync(InscriptionTransferred eventValue, LogEventContext context)
    {
        await AddInscriptionTransferAsync(context, "Transfer", eventValue.From.ToBase58(), eventValue.To.ToBase58(),
            eventValue.Tick,
            eventValue.Amt, eventValue.InscriptionInfo);

        await AddInscriptionHolderAsync(context, eventValue.Tick, eventValue.To.ToBase58(), eventValue.Amt);
        
        var inscriptionId = IdGenerateHelper.GetId(context.ChainId, eventValue.Tick);
        var inscription = await IssuedInscriptionRepository.GetFromBlockStateSetAsync(inscriptionId, context.ChainId);
        inscription.MintedAmt += eventValue.Amt;
        
        if (inscription.MintedAmt == inscription.Amt)
        {
            inscription.Progress = 100;
            inscription.IsCompleted= true;
        }
        else
        {
            inscription.Progress = inscription.MintedAmt * 100 / (float)inscription.Amt;
        }

        ObjectMapper.Map(context, inscription);
        await IssuedInscriptionRepository.AddOrUpdateAsync(inscription);
    }
}