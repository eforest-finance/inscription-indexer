using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Forest.Inscription;
using Inscription.Indexer.GraphQL;
using Shouldly;
using Xunit;

namespace Inscription.Indexer.Processors;

public class InscriptionCreatedProcessorTests : InscriptionIndexerTestBase
{
    private readonly InscriptionCreatedProcessor _inscriptionCreatedProcessor;
    private readonly IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> _repository;

    public InscriptionCreatedProcessorTests()
    {
        _inscriptionCreatedProcessor = GetRequiredService<InscriptionCreatedProcessor>();
        _repository = GetRequiredService<IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo>>();
    }

    [Fact]
    public async Task Test()
    {
        var inscriptionCreated = new InscriptionCreated
        {
            Tick = "Tick",
            Decimals = 0,
            Deployer = TestAddress,
            Issuer = TestAddress,
            Limit = 10,
            Owner = TestAddress,
            CollectionSymbol = "CollectionSymbol",
            IsBurnable = false,
            ItemSymbol = "ItemSymbol",
            TotalSupply = 100000,
            IssueChainId = 1,
            CollectionExternalInfo = new ExternalInfos
            {
                Value = { {"key1","value1"} }
            },
            ItemExternalInfo = new ExternalInfos
            {
                Value = { {"key2","value2"} }
            }
        };

        var logEventInfo = GenerateLogEventInfo(inscriptionCreated);
        var logEventContext = GenerateLogEventContext();

        await _inscriptionCreatedProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();

        var inscription = await Query.Inscription(_repository, ObjectMapper, new GetInscriptionInput()
        {
            ChainId = ChainId,
            Tick = inscriptionCreated.Tick
        });
        inscription[0].Tick.ShouldBe(inscriptionCreated.Tick);
        inscription[0].Decimals.ShouldBe(inscriptionCreated.Decimals);
        inscription[0].Deployer.ShouldBe(inscriptionCreated.Deployer.ToBase58());
        inscription[0].Issuer.ShouldBe(inscriptionCreated.Issuer.ToBase58());
        inscription[0].Limit.ShouldBe(inscriptionCreated.Limit);
        inscription[0].Owner.ShouldBe(inscriptionCreated.Owner.ToBase58());
        inscription[0].CollectionSymbol.ShouldBe(inscriptionCreated.CollectionSymbol);
        inscription[0].IsBurnable.ShouldBe(inscriptionCreated.IsBurnable);
        inscription[0].ItemSymbol.ShouldBe(inscriptionCreated.ItemSymbol);
        inscription[0].TotalSupply.ShouldBe(inscriptionCreated.TotalSupply);
        inscription[0].IssueChainId.ShouldBe(inscriptionCreated.IssueChainId);
        inscription[0].CollectionExternalInfo[0].Key.ShouldBe("key1");
        inscription[0].CollectionExternalInfo[0].Value.ShouldBe("value1");
        inscription[0].ItemExternalInfo[0].Key.ShouldBe("key2");
        inscription[0].ItemExternalInfo[0].Value.ShouldBe("value2");
    }
}