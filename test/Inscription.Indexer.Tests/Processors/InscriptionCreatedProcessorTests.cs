using Inscription.Indexer.GraphQL;
using Shouldly;
using Xunit;

namespace Inscription.Indexer.Processors;

public class InscriptionCreatedProcessorTests : InscriptionIndexerTestBase
{

    public InscriptionCreatedProcessorTests()
    {
    }

    [Fact]
    public async Task Test()
    {
        var tick = "Tick";
        await CreateInscriptionAsync(tick);

        var inscription = await Query.Inscription(InscriptionRepository, ObjectMapper, new GetInscriptionInput()
        {
            ChainId = ChainId,
            Tick = tick
        });
        inscription[0].Tick.ShouldBe(tick);
        inscription[0].Deployer.ShouldBe(TestAddress.ToBase58());
        inscription[0].Limit.ShouldBe(10);
        inscription[0].Owner.ShouldBe(TestAddress.ToBase58());
        inscription[0].TotalSupply.ShouldBe(100000);
        inscription[0].CollectionExternalInfo[0].Key.ShouldBe("inscription_image");
        inscription[0].CollectionExternalInfo[0].Value.ShouldBe("inscriptionimage");
        inscription[0].ItemExternalInfo[0].Key.ShouldBe("key2");
        inscription[0].ItemExternalInfo[0].Value.ShouldBe("value2");
        inscription[0].IssueChainId.ShouldBe(1);
        inscription[0].Issuer.ShouldBe(TestAddress.ToBase58());
    }
}