using Inscription.Indexer.TestBase;
using Orleans.TestingHost;
using Volo.Abp.Modularity;

namespace Inscription.Indexer.Orleans.TestBase;

public abstract class AElfIndexerInscriptionOrleansTestBase<TStartupModule>:AElfIndexerInscriptionTestBase<TStartupModule> 
    where TStartupModule : IAbpModule
{
    protected readonly TestCluster Cluster;

    public AElfIndexerInscriptionOrleansTestBase()
    {
        Cluster = GetRequiredService<ClusterFixture>().Cluster;
    }
}