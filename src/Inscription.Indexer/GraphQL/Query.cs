using AElfIndexer.Client;
using AElfIndexer.Client.Providers;
using AElfIndexer.Grains;
using AElfIndexer.Grains.Grain.Client;
using AElfIndexer.Grains.State.Client;
using GraphQL;
using Microsoft.Extensions.Options;
using Nest;
using Orleans;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectMapping;

namespace Inscription.Indexer.GraphQL;

public class Query
{
    private const string MainChainId = "AELF";
    private const string InscriptionImageKey = "inscription_image";

    public static async Task<List<InscriptionDto>> Inscription(
        [FromServices] IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> repository,
        [FromServices] IObjectMapper objectMapper, GetInscriptionInput input)
    {
        input.Validate();

        var mustQuery = new List<Func<QueryContainerDescriptor<Entities.Inscription>, QueryContainer>>();
        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)));
        }
        
        if (!input.Tick.IsNullOrWhiteSpace())
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.Tick).Value(input.Tick)));
        }

        if (input.BeginBlockHeight.HasValue)
        {
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).GreaterThanOrEquals(input.BeginBlockHeight.Value)));
        }
        
        if (input.EndBlockHeight.HasValue)
        {
            mustQuery.Add(q => q.Range(i => i.Field(f => f.BlockHeight).LessThanOrEquals(input.EndBlockHeight.Value)));
        }

        QueryContainer Filter(QueryContainerDescriptor<Entities.Inscription> f) => f.Bool(b => b.Must(mustQuery));

        var result = await repository.GetListAsync(Filter, sortExp: k => k.BlockHeight,
            sortType: SortOrder.Ascending, limit: input.MaxResultCount.Value, skip: input.SkipCount.Value);
        return objectMapper.Map<List<Entities.Inscription>, List<InscriptionDto>>(result.Item2);
    }

    public static async Task<PagedResultDto<IssuedInscriptionDto>> IssuedInscription(
        [FromServices] IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo> issuedInscriptionRepository,
        [FromServices] IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> inscriptionRepository,
        [FromServices] IOptionsSnapshot<InscriptionOptions> inscriptionOptions,
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
        
        if (inscriptionOptions.Value.IgnoreInscription.Count > 0)
        {
            var mustNotQuery = new List<Func<QueryContainerDescriptor<Entities.IssuedInscription>, QueryContainer>>();
            foreach (var ignoreTick in inscriptionOptions.Value.IgnoreInscription)
            {
                mustNotQuery.Add(q => q.Term(i => i.Field(f => f.Tick).Value(ignoreTick)));
            }
            mustQuery.Add(q=>q.Bool(b=>b.MustNot(mustNotQuery)));
        }

        QueryContainer Filter(QueryContainerDescriptor<Entities.IssuedInscription> f) => f.Bool(b => b.Must(mustQuery));
        
        var issuedInscriptions = await issuedInscriptionRepository.GetListAsync(Filter, sortExp: k => k.HolderCount,
            sortType: SortOrder.Descending, limit: input.MaxResultCount.Value, skip: input.SkipCount.Value);
        var totalCount = 0L;
        var issuedInscriptionDtos =
            objectMapper.Map<List<Entities.IssuedInscription>, List<IssuedInscriptionDto>>(issuedInscriptions.Item2);
        
        if (issuedInscriptions.Item2.Count != 0)
        {
            totalCount = (await issuedInscriptionRepository.CountAsync(Filter)).Count;
            
            var images = await GetInscriptionImageAsync(inscriptionRepository, issuedInscriptions.Item2.Select(i => i.Tick).ToList());
            foreach (var issuedInscriptionDto in issuedInscriptionDtos)
            {
                if (images.TryGetValue(issuedInscriptionDto.Tick, out var image))
                {
                    issuedInscriptionDto.Image = image;
                }
            }
        }
        
        return new PagedResultDto<IssuedInscriptionDto>(totalCount, issuedInscriptionDtos);
    }

    public static async Task<List<InscriptionTransferDto>> InscriptionTransfer(
        [FromServices] IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo> transferRepository,
        [FromServices] IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> inscriptionRepository,
        [FromServices] IOptionsSnapshot<InscriptionOptions> inscriptionOptions,
        [FromServices] IObjectMapper objectMapper, GetInscriptionTransferInput input)
    {
        input.Validate();

        var mustQuery = new List<Func<QueryContainerDescriptor<Entities.InscriptionTransfer>, QueryContainer>>();
        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            mustQuery.Add(q => q.Term(i => i.Field(f => f.ChainId).Value(input.ChainId)));
        }
        
        if (inscriptionOptions.Value.IgnoreInscription.Count > 0)
        {
            var mustNotQuery = new List<Func<QueryContainerDescriptor<Entities.InscriptionTransfer>, QueryContainer>>();
            foreach (var ignoreTick in inscriptionOptions.Value.IgnoreInscription)
            {
                mustNotQuery.Add(q => q.Term(i => i.Field(f => f.Tick).Value(ignoreTick)));
            }
            mustQuery.Add(q=>q.Bool(b=>b.MustNot(mustNotQuery)));
        }

        QueryContainer Filter(QueryContainerDescriptor<Entities.InscriptionTransfer> f) =>
            f.Bool(b => b.Must(mustQuery));

        var transfers = await transferRepository.GetListAsync(Filter, sortExp: k => k.BlockTime,
            sortType: SortOrder.Descending, limit: input.MaxResultCount.Value, skip: input.SkipCount.Value);

        var inscriptionTransferDtos =
            objectMapper.Map<List<Entities.InscriptionTransfer>, List<InscriptionTransferDto>>(transfers.Item2);
        if (inscriptionTransferDtos.Count > 0)
        {
            var images = await GetInscriptionImageAsync(inscriptionRepository,
                inscriptionTransferDtos.Select(i => i.Tick).ToList());
            foreach (var inscriptionTransferDto in inscriptionTransferDtos)
            {
                if (images.TryGetValue(inscriptionTransferDto.Tick, out var image))
                {
                    inscriptionTransferDto.InscriptionImage = image;
                }
            }
        }

        return inscriptionTransferDtos;
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
    
    private static async Task<Dictionary<string,string>> GetInscriptionImageAsync(
        IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> inscriptionRepository,
        List<string> ticks)
    {
        var inscriptionMustQuery = new List<Func<QueryContainerDescriptor<Entities.Inscription>, QueryContainer>>();
        inscriptionMustQuery.Add(q => q.Ids(i => i.Values(ticks.Select(i => IdGenerateHelper.GetId(MainChainId, i)))));
        QueryContainer inscriptionFilter(QueryContainerDescriptor<Entities.Inscription> f) => f.Bool(b => b.Must(inscriptionMustQuery));
        var inscriptions = await inscriptionRepository.GetListAsync(inscriptionFilter);
        var result = new Dictionary<string, string>();
        foreach (var inscription in inscriptions.Item2)
        {
            if (inscription.CollectionExternalInfo.TryGetValue(InscriptionImageKey, out var image))
            {
                result.Add(inscription.Tick, image);
            }
        }

        return result;
    }
}