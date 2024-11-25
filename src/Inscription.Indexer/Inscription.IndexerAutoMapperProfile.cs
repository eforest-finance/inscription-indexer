using AeFinder.Sdk.Processor;
using AElf.Types;
using Inscription.Indexer.GraphQL;
using AutoMapper;
using Forest.Contracts.Inscription;

namespace Inscription.Indexer;

public class InscriptionIndexerAutoMapperProfile : Profile
{
    public InscriptionIndexerAutoMapperProfile()
    {
        CreateMap<Hash, string>().ConvertUsing(s => s == null ? string.Empty : s.ToHex());
        CreateMap<Address, string>().ConvertUsing(s => s == null ? string.Empty : s.ToBase58());

        CreateMap<LogEventContext, Entities.Inscription>()
            .ForMember(destination => destination.BlockHeight,
                opt => opt.MapFrom(source => source.Block.BlockHeight))
            .ForMember(destination => destination.BlockHash,
                opt => opt.MapFrom(source => source.Block.BlockHash));;
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
        
        CreateMap<LogEventContext, Entities.IssuedInscription>()
            .ForMember(destination => destination.BlockHeight,
                opt => opt.MapFrom(source => source.Block.BlockHeight))
            .ForMember(destination => destination.BlockHash,
                opt => opt.MapFrom(source => source.Block.BlockHash));;
        CreateMap<InscriptionIssued, Entities.IssuedInscription>();
        CreateMap<Entities.IssuedInscription, IssuedInscriptionDto>();
        
        CreateMap<LogEventContext, Entities.InscriptionTransfer>()
            .ForMember(destination => destination.BlockHeight,
                opt => opt.MapFrom(source => source.Block.BlockHeight))
            .ForMember(destination => destination.BlockHash,
                opt => opt.MapFrom(source => source.Block.BlockHash));;
        CreateMap<Entities.InscriptionTransfer, InscriptionTransferDto>();
        
        CreateMap<LogEventContext, Entities.InscriptionHolder>();
    }
}