using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Forest.Inscription;
using Microsoft.Extensions.Logging;

namespace Inscription.Indexer.Processors;

public class InscriptionCreatedProcessor : InscriptionProcessorBase<InscriptionCreated>
{

    public InscriptionCreatedProcessor(
        ILogger<AElfLogEventProcessorBase<InscriptionCreated, LogEventInfo>> logger) : base(logger)
    {
    }

    protected override async Task HandleEventAsync(InscriptionCreated eventValue, LogEventContext context)
    {
        var id = IdGenerateHelper.GetId(context.ChainId, eventValue.Tick);
        var inscription = new Entities.Inscription
        {
            Id = id
        };
        
        ObjectMapper.Map(context, inscription);
        ObjectMapper.Map(eventValue, inscription);
        
        await InscriptionRepository.AddOrUpdateAsync(inscription);
    }
}