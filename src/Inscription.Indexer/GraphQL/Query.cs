using System.Linq.Expressions;
using AElfIndexer.Client;
using AElfIndexer.Client.Providers;
using AElfIndexer.Grains;
using AElfIndexer.Grains.Grain.Client;
using AElfIndexer.Grains.State.Client;
using GraphQL;
using Nest;
using Orleans;
using Volo.Abp.ObjectMapping;

namespace Inscription.Indexer.GraphQL;

public class Query
{
    public static async Task<List<InscriptionDto>> Inscription(
        [FromServices] IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper, GetInscriptionInput input)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<Entities.Inscription>, QueryContainer>>();
        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)));
        }
        
        if (!input.Tick.IsNullOrWhiteSpace())
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.Tick).Value(input.Tick)));
        }

        if (input.BeginBlockHeight != 0)
        {
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.BeginBlockHeight)));
        }
        
        if (input.EndBlockHeight != 0)
        {
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).LessThanOrEquals(input.EndBlockHeight)));
        }

        QueryContainer Filter(QueryContainerDescriptor<Entities.Inscription> f) => f.Bool(b => b.Must(mustQuery));
        
        var result = await repository.GetListAsync(Filter, sortExp: k => k.BlockHeight,
            sortType: SortOrder.Ascending);
        return objectMapper.Map<List<Entities.Inscription>, List<InscriptionDto>>(result.Item2);
    }

    public static async Task<List<IssuedInscriptionDto>> IssuedInscription(
        [FromServices] IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper, GetIssuedInscriptionInput input)
    {
        input.Validate();
        
        var mustQuery = new List<Func<QueryContainerDescriptor<Entities.IssuedInscription>, QueryContainer>>();
        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)));
        }
        
        if (!input.Tick.IsNullOrWhiteSpace())
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.Tick).Value(input.Tick)));
        }
        
        if (input.IsCompleted.HasValue)
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.IsCompleted).Value(input.IsCompleted.Value)));
        }
        
        QueryContainer Filter(QueryContainerDescriptor<Entities.IssuedInscription> f) => f.Bool(b => b.Must(mustQuery));
        
        var result = await repository.GetListAsync(Filter, sortExp: k => k.HolderCount,
            sortType: SortOrder.Descending, limit: input.MaxResultCount, skip: input.SkipCount);
        return objectMapper.Map<List<Entities.IssuedInscription>, List<IssuedInscriptionDto>>(result.Item2);
    }

    public static async Task<List<InscriptionTransferDto>> InscriptionTransfer(
        [FromServices] IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper, GetInscriptionTransferInput input)
    {
        input.Validate();
        
        var mustQuery = new List<Func<QueryContainerDescriptor<Entities.InscriptionTransfer>, QueryContainer>>();
        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)));
        }

        QueryContainer Filter(QueryContainerDescriptor<Entities.InscriptionTransfer> f) =>
            f.Bool(b => b.Must(mustQuery));

        var result = await repository.GetListAsync(Filter, sortExp: k => k.BlockTime,
            sortType: SortOrder.Descending, limit: input.MaxResultCount, skip: input.SkipCount);
        return objectMapper.Map<List<Entities.InscriptionTransfer>, List<InscriptionTransferDto>>(result.Item2);
    }
    
    public static async Task<SyncStateDto> SyncState(
        [FromServices] IClusterClient clusterClient, [FromServices] IAElfIndexerClientInfoProvider clientInfoProvider,
        [FromServices] IObjectMapper objectMapper, GetSyncStateDto input)
    {
        var version = clientInfoProvider.GetVersion();
        var clientId = clientInfoProvider.GetClientId();
        var blockStateSetInfoGrain =
            clusterClient.GetGrain<IBlockStateSetInfoGrain>(
                GrainIdHelper.GenerateGrainId("BlockStateSetInfo", clientId, input.ChainId, version));
        var confirmedHeight = await blockStateSetInfoGrain.GetConfirmedBlockHeight(input.FilterType);
        return new SyncStateDto
        {
            ConfirmedBlockHeight = confirmedHeight
        };
    }
}