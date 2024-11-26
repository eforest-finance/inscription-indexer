using AeFinder.Sdk.Logging;
using AeFinder.Sdk.Processor;
using Forest.Contracts.Inscription;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;

namespace Inscription.Indexer.Processors;

public class InscriptionTransferredProcessor : LogEventProcessorBase<InscriptionTransferred>
{
    private readonly IObjectMapper _objectMapper;
    public InscriptionTransferredProcessor(IObjectMapper objectMapper)
    {
        _objectMapper = objectMapper;
    }
    
    
    public override string GetContractAddress(string chainId)
    {
        return ContractInfoHelper.GetInscriptionContractAddress(chainId);
    }

    public async override Task ProcessAsync(InscriptionTransferred eventValue, LogEventContext context)
    {
        if (eventValue.From == null || eventValue.From.Value.Length == 0)
        {  
            Logger.LogError("eventValue.From is null");
            return;
        }
        if (eventValue.To == null || eventValue.To.Value.Length == 0)
        {
            Logger.LogError("eventValue.To is null");
            return;
        }
        if (eventValue.Amt == null)
        {
            Logger.LogError("eventValue.Amt is null");
            return;
        } 
        if (eventValue.Tick.IsNullOrEmpty())
        {
            Logger.LogError("eventValue.Tick is null");
            return;
        }
        if (eventValue.Amt == 0)
        {
            Logger.LogError("eventValue.Amt is 0");
            return;
        }
        await AddInscriptionTransferAsync(context, "Transfer", eventValue.From.ToBase58(), eventValue.To.ToBase58(),
            eventValue.Tick,
            eventValue.Amt, eventValue.InscriptionInfo);
        await AddInscriptionHolderAsync(context, eventValue.Tick, eventValue.To.ToBase58(), eventValue.Amt);

        var inscriptionId = IdGenerateHelper.GetId(context.ChainId, eventValue.Tick);
        var inscription = await GetEntityAsync<Entities.IssuedInscription>(inscriptionId);
        if (inscription == null)
        {
            Logger.LogError("InscriptionTransferredProcessor IssuedInscription == null inscriptionId={A}",inscriptionId);
            return;
        }
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

        _objectMapper.Map(context, inscription);
        await SaveEntityAsync(inscription);
    }
    
    protected async Task AddInscriptionTransferAsync(LogEventContext context, string method, string from, string to, string tick, long amt, string inscriptionInfo)
    {
        var inscriptionId = IdGenerateHelper.GetId(context.ChainId, tick);
        var inscription = await GetEntityAsync<Entities.IssuedInscription>(inscriptionId);
        if (inscription == null)
        {
            Logger.LogError("AddInscriptionTransferAsync IssuedInscription == null inscriptionId={A}",inscriptionId);
            return;
        }
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
    
    private async Task AddInscriptionHolderAsync(LogEventContext context, string tick, string address, long amt)
    {
        var id = IdGenerateHelper.GetId(context.ChainId, tick, address);
        var inscriptionHolder = await GetEntityAsync<Entities.InscriptionHolder>(id);
        if (inscriptionHolder == null)
        {
            var inscriptionId = IdGenerateHelper.GetId(context.ChainId, tick);
            var inscription = await GetEntityAsync<Entities.IssuedInscription>(inscriptionId);
            if (inscription == null)
            { 
                Logger.LogError("AddInscriptionHolderAsync IssuedInscription == null inscriptionId={A}",inscriptionId);
                return;
            }
            inscription.HolderCount += 1;
            _objectMapper.Map(context, inscription);
            await SaveEntityAsync(inscription);
            
            inscriptionHolder = new Entities.InscriptionHolder
            {
                Id = id,
                Tick = tick,
                Address = address
            };
        }
        
        inscriptionHolder.Amt += amt;

        _objectMapper.Map(context, inscriptionHolder);
        
        await SaveEntityAsync(inscriptionHolder);
    }
}