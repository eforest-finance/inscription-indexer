using AElf.Types;
using AElfIndexer.Client.Handlers;
using AutoMapper;
using Forest.Inscription;
using Inscription.Indexer.GraphQL;

namespace Inscription.Indexer;

public class InscriptionIndexerMapperProfile : Profile
{
    public InscriptionIndexerMapperProfile()
    {
        // Common
        CreateMap<Hash, string>().ConvertUsing(s => s == null ? string.Empty : s.ToHex());
        CreateMap<Address, string>().ConvertUsing(s => s == null ? string.Empty : s.ToBase58());

        CreateMap<LogEventContext, Entities.Inscription>();
        CreateMap<InscriptionCreated, Entities.Inscription>()
            .ForMember(d => d.CollectionExternalInfo,
                opt => opt.MapFrom(s => s.CollectionExternalInfo.Value.ToDictionary(o => o.Key, o => o.Value)))
            .ForMember(d => d.ItemExternalInfo,
                opt => opt.MapFrom(s => s.ItemExternalInfo.Value.ToDictionary(o => o.Key, o => o.Value)))
            ;
        CreateMap<Entities.Inscription, InscriptionDto>()
            .ForMember(d => d.CollectionExternalInfo,
                opt => opt.MapFrom(s => s.CollectionExternalInfo == null
                    ? new List<ExternalInfoDto>()
                    : s.CollectionExternalInfo.Select(o => new ExternalInfoDto
                    {
                        Key = o.Key,
                        Value = o.Value
                    }).ToList()))
            .ForMember(d => d.ItemExternalInfo,
                opt => opt.MapFrom(s => s.ItemExternalInfo == null
                    ? new List<ExternalInfoDto>()
                    : s.ItemExternalInfo.Select(o => new ExternalInfoDto
                    {
                        Key = o.Key,
                        Value = o.Value
                    }).ToList()));
        
        CreateMap<LogEventContext, Entities.IssuedInscription>();
        CreateMap<InscriptionIssued, Entities.IssuedInscription>();
        CreateMap<Entities.IssuedInscription, IssuedInscriptionDto>();
        
        CreateMap<LogEventContext, Entities.InscriptionTransfer>();
        CreateMap<Entities.InscriptionTransfer, InscriptionTransferDto>();
        
        CreateMap<LogEventContext, Entities.InscriptionHolder>();
        CreateMap<Entities.InscriptionHolder, InscriptionHolderDto>();
    }
}