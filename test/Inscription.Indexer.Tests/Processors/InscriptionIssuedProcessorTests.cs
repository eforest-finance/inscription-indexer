using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Forest.Contracts.Inscription;
using Inscription.Indexer.GraphQL;
using Shouldly;
using Xunit;

namespace Inscription.Indexer.Processors;

public class InscriptionIssuedProcessorTests : InscriptionIndexerTestBase
{
    public InscriptionIssuedProcessorTests()
    {
    }

    [Fact]
    public async Task Test()
    {
        var tick = "Tick";
        await CreateInscriptionAsync(tick);
        await IssueInscriptionAsync(tick);

        var inscription = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository,
            InscriptionOptions, ObjectMapper, new GetIssuedInscriptionInput()
            {
                ChainId = ChainId,
                Tick = tick
            });
        inscription.TotalCount.ShouldBe(1);
        inscription.Items[0].Tick.ShouldBe(tick);
        inscription.Items[0].IssuedToAddress.ShouldBe(TestAddress.ToBase58());
        inscription.Items[0].Amt.ShouldBe(1000);
        inscription.Items[0].TransactionCount.ShouldBe(1);
        inscription.Items[0].Image.ShouldBe("inscriptionimage");

        var inscriptionTransfer = await Query.InscriptionTransfer(TransferRepository, InscriptionRepository,
            InscriptionOptions, ObjectMapper, new GetInscriptionTransferInput()
            {
                ChainId = ChainId,
            });
        inscriptionTransfer[0].Tick.ShouldBe(tick);
        inscriptionTransfer[0].FromAddress.ShouldBe(TestAddress.ToBase58());
        inscriptionTransfer[0].ToAddress.ShouldBe(TestAddress.ToBase58());
        inscriptionTransfer[0].Amt.ShouldBe(1000);
        inscriptionTransfer[0].InscriptionInfo.ShouldBe("InscriptionInfo");
        inscriptionTransfer[0].Method.ShouldBe("Deploy");
        inscriptionTransfer[0].InscriptionImage.ShouldBe("inscriptionimage");
        inscriptionTransfer[0].Number.ShouldBe(0);
    }
}