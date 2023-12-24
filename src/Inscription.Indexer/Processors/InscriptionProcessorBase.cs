using AElf.CSharp.Core;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;

namespace Inscription.Indexer.Processors;

public class InscriptionProcessorBase<TEvent> : AElfLogEventProcessorBase<TEvent, LogEventInfo>
    where TEvent : IEvent<TEvent>, new()
{
    public InscriptionProcessorBase(ILogger<AElfLogEventProcessorBase<TEvent, LogEventInfo>> logger) : base(logger)
    {
    }

    public IAbpLazyServiceProvider LazyServiceProvider { get; set; }

    protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();

    protected ContractInfoOptions ContractInfoOptions =>
        LazyServiceProvider.LazyGetRequiredService<IOptionsSnapshot<ContractInfoOptions>>().Value;

    protected IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> InscriptionRepository => LazyServiceProvider
        .LazyGetRequiredService<IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo>>();
    
    protected IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo> IssuedInscriptionRepository => LazyServiceProvider
        .LazyGetRequiredService<IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo>>();
    
    protected IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo> InscriptionTransferRepository => LazyServiceProvider
        .LazyGetRequiredService<IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo>>();
    
    protected IAElfIndexerClientEntityRepository<Entities.InscriptionHolder, LogEventInfo> InscriptionHolderRepository => LazyServiceProvider
        .LazyGetRequiredService<IAElfIndexerClientEntityRepository<Entities.InscriptionHolder, LogEventInfo>>();
    

    public override string GetContractAddress(string chainId)
    {
        return ContractInfoOptions.ContractInfos[chainId].InscriptionContractAddress;
    }
    
    protected async Task AddInscriptionTransferAsync(LogEventContext context, string method, string from, string to, string tick, long amt, string inscriptionInfo)
    {
        var inscriptionId = IdGenerateHelper.GetId(context.ChainId, tick);
        var inscription = await IssuedInscriptionRepository.GetFromBlockStateSetAsync(inscriptionId, context.ChainId);
        inscription.TransactionCount += 1;
        ObjectMapper.Map(context, inscription);
        await IssuedInscriptionRepository.AddOrUpdateAsync(inscription);
        
        var id = IdGenerateHelper.GetId(context.ChainId, context.TransactionId);
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
        ObjectMapper.Map(context, inscriptionTransfer);
        
        await InscriptionTransferRepository.AddOrUpdateAsync(inscriptionTransfer);
    }
    
    protected async Task AddInscriptionHolderAsync(LogEventContext context, string tick, string address, long amt)
    {
        var id = IdGenerateHelper.GetId(context.ChainId, tick, address);
        var inscriptionHolder = await InscriptionHolderRepository.GetFromBlockStateSetAsync(id, context.ChainId);
        if (inscriptionHolder == null)
        {
            var inscriptionId = IdGenerateHelper.GetId(context.ChainId, tick);
            var inscription = await IssuedInscriptionRepository.GetFromBlockStateSetAsync(inscriptionId, context.ChainId);
            inscription.HolderCount += 1;
            ObjectMapper.Map(context, inscription);
            await IssuedInscriptionRepository.AddOrUpdateAsync(inscription);
            
            inscriptionHolder = new Entities.InscriptionHolder
            {
                Id = id,
                Tick = tick,
                Address = address
            };
        }
        
        inscriptionHolder.Amt += amt;

        ObjectMapper.Map(context, inscriptionHolder);
        
        await InscriptionHolderRepository.AddOrUpdateAsync(inscriptionHolder);
    }
}