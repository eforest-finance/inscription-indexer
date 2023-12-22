using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Inscription.Indexer.GraphQL;
using Inscription.Indexer.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;

namespace Inscription.Indexer;

[DependsOn(typeof(AElfIndexerClientModule))]
public class InscriptionIndexerModule : AElfIndexerClientPluginBaseModule<InscriptionIndexerModule,
    InscriptionIndexerSchema, Query>
{
    protected override string ClientId { get; } = "AElfIndexer_Inscription";
    protected override string Version { get; } = "71d48a7804674c8a84e90db08c4903d1";

    protected override void ConfigureServices(IServiceCollection serviceCollection)
    {
        var configuration = serviceCollection.GetConfiguration();
        
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, InscriptionCreatedProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, InscriptionIssuedProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, InscriptionTransferredProcessor>();

        Configure<ContractInfoOptions>(configuration.GetSection("ContractInfo"));
    }
}