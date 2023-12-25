using Inscription.Indexer.TestBase;
using Orleans.TestingHost;
using Volo.Abp.Modularity;

namespace Inscription.Indexer.Orleans.TestBase;

public abstract class InscriptionIndexerOrleansTestBase<TStartupModule>:InscriptionIndexerTestBase<TStartupModule> 
    where TStartupModule : IAbpModule
{
    protected readonly TestCluster Cluster;

    public InscriptionIndexerOrleansTestBase()
    {
        Cluster = GetRequiredService<ClusterFixture>().Cluster;
    }
}