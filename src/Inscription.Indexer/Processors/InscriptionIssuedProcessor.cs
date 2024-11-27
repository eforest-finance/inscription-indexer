using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Forest.Contracts.Inscription;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;

namespace Inscription.Indexer.Processors;

public class InscriptionIssuedProcessor : LogEventProcessorBase<InscriptionIssued>
{
    private readonly IObjectMapper _objectMapper;
    public InscriptionIssuedProcessor(IObjectMapper objectMapper)
    {
        _objectMapper = objectMapper;
    }
    
    public override string GetContractAddress(string chainId)
    {
        return ContractInfoHelper.GetInscriptionContractAddress(chainId);
    }
    
    public async override Task ProcessAsync(InscriptionIssued eventValue, LogEventContext context)
    { 
        var id = IdGenerateHelper.GetId(context.ChainId, eventValue.Tick);
        if (eventValue.To == null || eventValue.To.Value.Length == 0)
        { 
            Logger.LogError("eventValue.To is null");
            Logger.LogError("InscriptionIssuedProcessor eventValue={A}",JsonConvert.SerializeObject(eventValue));
            return;
        }
        var inscription = new Entities.IssuedInscription
        {
            Id = id,
            IssuedTransactionId = context.Transaction.TransactionId,
            IssuedTime = context.Block.BlockTime,
            IssuedToAddress = eventValue.To.ToBase58()
        };
        
        _objectMapper.Map(context, inscription);
        _objectMapper.Map(eventValue, inscription);
        
        await SaveEntityAsync(inscription);
        
        await AddInscriptionTransferAsync(context, "Deploy", eventValue.To.ToBase58(), eventValue.To.ToBase58(),
            eventValue.Tick,
            eventValue.Amt, eventValue.InscriptionInfo);
    }
    
    private async Task AddInscriptionTransferAsync(LogEventContext context, string method, string from, string to, string tick, long amt, string inscriptionInfo)
    {
        var inscriptionId = IdGenerateHelper.GetId(context.ChainId, tick);
        var inscription = await GetEntityAsync<Entities.IssuedInscription>(inscriptionId);
        
        inscription.TransactionCount += 1;
        _objectMapper.Map(context, inscription);
        await SaveEntityAsync(inscription);
        
        var id = IdGenerateHelper.GetId(context.ChainId, context.Transaction.TransactionId);
        var inscriptionTransfer = new Entities.InscriptionTransfer
        {
            Id = id,
            Method = method,
            Tick = tick,
            FromAddress = from,
            ToAddress = to,
            Amt = amt,
            InscriptionInfo = inscriptionInfo,
            Number = inscription.TransactionCount - 1
        };
        _objectMapper.Map(context, inscriptionTransfer);
        
        await SaveEntityAsync(inscriptionTransfer);
    }
}