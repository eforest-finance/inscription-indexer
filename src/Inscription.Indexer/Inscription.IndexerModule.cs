using GraphQL.Types;
using Inscription.Indexer.Processors;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

using AeFinder.Sdk.Processor;
using Inscription.Indexer.GraphQL;

namespace Inscription.Indexer;

public class InscriptionIndexerModule : AbpModule
{

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<InscriptionIndexerModule>(); });
        context.Services.AddSingleton<ISchema, InscriptionIndexerSchema>();
        
        context.Services.AddTransient<ILogEventProcessor, InscriptionCreatedProcessor>();
        
        context.Services.AddTransient<ILogEventProcessor, InscriptionIssuedProcessor>();
        
        context.Services.AddTransient<ILogEventProcessor, InscriptionTransferredProcessor>();
    }
}