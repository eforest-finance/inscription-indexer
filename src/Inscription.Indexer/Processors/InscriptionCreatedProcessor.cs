using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Forest.Contracts.Inscription;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;

namespace Inscription.Indexer.Processors;

public class InscriptionCreatedProcessor : LogEventProcessorBase<InscriptionCreated>
{

    private readonly IObjectMapper _objectMapper;
    public InscriptionCreatedProcessor(IObjectMapper objectMapper)
    {
        _objectMapper = objectMapper;
    }

    public override string GetContractAddress(string chainId)
    {
        return ContractInfoHelper.GetInscriptionContractAddress(chainId);
    }
    
    public async override Task ProcessAsync(InscriptionCreated eventValue, LogEventContext context)
    {
        Logger.LogDebug("InscriptionCreated eventValue {A}",JsonConvert.SerializeObject(eventValue));
        var id = IdGenerateHelper.GetId(context.ChainId, eventValue.Tick);
        var inscription = new Entities.Inscription
        {
            Id = id
        };
        
        _objectMapper.Map(context, inscription);
        _objectMapper.Map(eventValue, inscription);
        await SaveEntityAsync(inscription);
    }
}