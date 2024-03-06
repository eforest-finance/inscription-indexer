using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Grains.State.Client;
using Inscription.Indexer.GraphQL;
using Inscription.Indexer.Processors;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Inscription.Indexer;

[DependsOn(typeof(AElfIndexerClientModule))]
public class InscriptionIndexerModule : AElfIndexerClientPluginBaseModule<InscriptionIndexerModule,
    InscriptionIndexerSchema, Query>
{
    protected override string ClientId { get; } = "AElfIndexer_Inscription";
    protected override string Version { get; } = "eb2c712e46eb49168d7d27ae793029ab";

    protected override void ConfigureServices(IServiceCollection serviceCollection)
    {
        var configuration = serviceCollection.GetConfiguration();
        
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, InscriptionCreatedProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, InscriptionIssuedProcessor>();
        serviceCollection.AddSingleton<IAElfLogEventProcessor<LogEventInfo>, InscriptionTransferredProcessor>();

        Configure<ContractInfoOptions>(configuration.GetSection("ContractInfo"));
        Configure<InscriptionOptions>(configuration.GetSection("Inscription"));
    }
}