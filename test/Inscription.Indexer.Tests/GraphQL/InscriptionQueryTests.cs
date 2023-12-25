using Shouldly;
using Volo.Abp;
using Xunit;

namespace Inscription.Indexer.GraphQL;

public class InscriptionQueryTests : InscriptionIndexerTestBase
{
    [Fact]
    public async Task Test()
    {
        await CreateInscriptionAsync("Tick1");
        await CreateInscriptionAsync("Tick2");

        await Query.Inscription(InscriptionRepository, ObjectMapper, new GetInscriptionInput()
        {
            ChainId = ChainId,
            Tick = "Tick1",
            BeginBlockHeight = 1000,
            EndBlockHeight = 2000
        }).ShouldThrowAsync<ArgumentOutOfRangeException>();

        var list = await Query.Inscription(InscriptionRepository, ObjectMapper, new GetInscriptionInput()
        {
            ChainId = ChainId,
            Tick = "Tick1"
        });
        list.Count.ShouldBe(1);
        list[0].Tick.ShouldBe("Tick1");

        list = await Query.Inscription(InscriptionRepository, ObjectMapper, new GetInscriptionInput()
        {
            ChainId = ChainId,
            BeginBlockHeight = BlockHeight,
            EndBlockHeight = BlockHeight
        });
        list.Count.ShouldBe(2);
        list[0].Tick.ShouldBe("Tick1");
        list[1].Tick.ShouldBe("Tick2");

        list = await Query.Inscription(InscriptionRepository, ObjectMapper, new GetInscriptionInput()
        {
            ChainId = ChainId,
            BeginBlockHeight = BlockHeight,
            EndBlockHeight = BlockHeight + 999
        });
        list.Count.ShouldBe(2);
        list[0].Tick.ShouldBe("Tick1");
        list[1].Tick.ShouldBe("Tick2");

        list = await Query.Inscription(InscriptionRepository, ObjectMapper, new GetInscriptionInput()
        {
            ChainId = ChainId,
            BeginBlockHeight = BlockHeight + 1,
            EndBlockHeight = BlockHeight + 1
        });
        list.Count.ShouldBe(0);
    }
}