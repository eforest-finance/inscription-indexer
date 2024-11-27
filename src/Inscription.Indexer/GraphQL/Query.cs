using AeFinder.Sdk;
using GraphQL;
using Inscription.Indexer.Constants;
using Volo.Abp.ObjectMapping;

namespace Inscription.Indexer.GraphQL;

public class Query
{
    private const string MainChainId = "AELF";
    private const string MainChainIdPre = "AELF-";
    private const string InscriptionImageKey = "inscription_image";

    public static async Task<List<InscriptionDto>> Inscription(
        [FromServices] IReadOnlyRepository<Entities.Inscription> repository,
        [FromServices] IObjectMapper objectMapper, GetInscriptionInput input)
    {
        input.Validate();

        var queryable = await repository.GetQueryableAsync();
        
        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(i => i.ChainId == input.ChainId);
        }
        
        if (!input.Tick.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(i => i.Tick == input.Tick);
        }

        if (input.BeginBlockHeight.HasValue)
        {
            queryable = queryable.Where(i => i.BlockHeight >= input.BeginBlockHeight.Value);
        }
        
        if (input.EndBlockHeight.HasValue)
        {
            queryable = queryable.Where(i => i.BlockHeight <= input.EndBlockHeight.Value);
        }

        var result = queryable
            .OrderBy(i => i.BlockHeight)
            .Skip(input.SkipCount.Value)
            .Take(input.MaxResultCount.Value)
            .ToList();
       
        return objectMapper.Map<List<Entities.Inscription>, List<InscriptionDto>>(result);
    }

    public static async Task<PagedResultDto<IssuedInscriptionDto>> IssuedInscription(
        [FromServices] IReadOnlyRepository<Entities.IssuedInscription> issuedInscriptionRepository,
        [FromServices] IReadOnlyRepository<Entities.Inscription> inscriptionRepository,
        [FromServices] IObjectMapper objectMapper, GetIssuedInscriptionInput input)
    {
        input.Validate();
        var issuedInscriptionQueryable = await issuedInscriptionRepository.GetQueryableAsync();

        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            issuedInscriptionQueryable = issuedInscriptionQueryable.Where(i => i.ChainId == input.ChainId);
        }
        
        if (!input.Tick.IsNullOrWhiteSpace())
        {
            issuedInscriptionQueryable = issuedInscriptionQueryable.Where(i => i.Tick == input.Tick);
        }
        
        if (input.IsCompleted.HasValue)
        {
            issuedInscriptionQueryable = issuedInscriptionQueryable.Where(i => i.IsCompleted == input.IsCompleted.Value);
        }

        if (InscriptionIndexerConstants.IgnoreInscription.Count > 0)
        {
            for (int i = 0; i < InscriptionIndexerConstants.IgnoreInscription.Count; i++)
            {
                var ignoreTick = InscriptionIndexerConstants.IgnoreInscription[i];
                issuedInscriptionQueryable = issuedInscriptionQueryable.Where(i => i.Tick !=ignoreTick);
            }
        }


        var issuedInscriptions =  issuedInscriptionQueryable
            .OrderByDescending(i => i.HolderCount)
            .Skip(input.SkipCount.Value)
            .Take(input.MaxResultCount.Value)
            .ToList();
        var totalCount = 0L;
        var issuedInscriptionDtos =
            objectMapper.Map<List<Entities.IssuedInscription>, List<IssuedInscriptionDto>>(issuedInscriptions);
        
        if (issuedInscriptions.Count != 0)
        {
            totalCount = issuedInscriptionQueryable.Count();

            var images = await GetInscriptionImageAsync(inscriptionRepository,
                issuedInscriptions.Select(i => i.Tick).ToList());
            for (var i = 0; i < issuedInscriptionDtos.Count; i++)
            {
                var issuedInscriptionDto = issuedInscriptionDtos[i];
                if (images.TryGetValue(issuedInscriptionDto.Tick, out var image))
                {
                    issuedInscriptionDto.Image = image;
                }
            }
        }
        
        return new PagedResultDto<IssuedInscriptionDto>(totalCount, issuedInscriptionDtos);
    }

    public static async Task<List<InscriptionTransferDto>> InscriptionTransfer(
        [FromServices] IReadOnlyRepository<Entities.InscriptionTransfer> transferRepository,
        [FromServices] IReadOnlyRepository<Entities.Inscription> inscriptionRepository,
        [FromServices] IObjectMapper objectMapper, GetInscriptionTransferInput input)
    {
        
        input.Validate();

        var transferQueryable = await transferRepository.GetQueryableAsync();
        
        
        if (!input.ChainId.IsNullOrWhiteSpace())
        {
            transferQueryable = transferQueryable.Where(i => i.ChainId == input.ChainId);
        }

        if (InscriptionIndexerConstants.IgnoreInscription.Count > 0)
        {
            for (var i = 0; i < InscriptionIndexerConstants.IgnoreInscription.Count; i++)
            {
                var ignoreTick = InscriptionIndexerConstants.IgnoreInscription[i];
                transferQueryable = transferQueryable.Where(i => i.Tick != ignoreTick);
            }
        }

        var transfers = transferQueryable.OrderByDescending(i => i.BlockTime)
            .Skip(input.SkipCount.Value)
            .Take(input.MaxResultCount.Value)
            .ToList();

        var inscriptionTransferDtos =
            objectMapper.Map<List<Entities.InscriptionTransfer>, List<InscriptionTransferDto>>(transfers);

        if (inscriptionTransferDtos.Count > 0)
        {
            var images = await GetInscriptionImageAsync(inscriptionRepository,
                inscriptionTransferDtos.Select(i => i.Tick).ToList());
            for (var i = 0; i < inscriptionTransferDtos.Count(); i++)
            {
                var inscriptionTransferDto = inscriptionTransferDtos[i];
                if (images.TryGetValue(inscriptionTransferDto.Tick, out var image))
                {
                    inscriptionTransferDto.InscriptionImage = image;
                }
            }
        }

        return inscriptionTransferDtos;
    }

    // public static async Task<SyncStateDto> SyncState(
    //     [FromServices] IClusterClient clusterClient, [FromServices] IAElfIndexerClientInfoProvider clientInfoProvider,
    //     [FromServices] IObjectMapper objectMapper, GetSyncStateDto input)
    // {
    //     var version = clientInfoProvider.GetVersion();
    //     var clientId = clientInfoProvider.GetClientId();
    //     var blockStateSetInfoGrain =
    //         clusterClient.GetGrain<IBlockStateSetInfoGrain>(
    //             GrainIdHelper.GenerateGrainId("BlockStateSetInfo", clientId, input.ChainId, version));
    //     var confirmedHeight = await blockStateSetInfoGrain.GetConfirmedBlockHeight(input.FilterType);
    //     return new SyncStateDto
    //     {
    //         ConfirmedBlockHeight = confirmedHeight
    //     };
    // }
    
    private static async Task<Dictionary<string,string>> GetInscriptionImageAsync(
        IReadOnlyRepository<Entities.Inscription> inscriptionRepository,
        List<string> ticksBefore)
    {
        var ticks = ticksBefore
            .Where(t => t.StartsWith(MainChainIdPre))
            .Select(t => t.Substring(MainChainIdPre.Length))
            .ToList();
        var inscriptionQueryable = await inscriptionRepository.GetQueryableAsync();

        inscriptionQueryable = inscriptionQueryable.Where(i => ticks.Contains(i.Id));
        var inscriptions = inscriptionQueryable.ToList();
        var result = new Dictionary<string, string>();
        for (int i = 0; i < inscriptions.Count(); i++)
        {
            var inscription = inscriptions[i];
            if (inscription.CollectionExternalInfo.TryGetValue(InscriptionImageKey, out var image))
            {
                result.Add(inscription.Tick, image);
            }
        }

        return result;
    }
}