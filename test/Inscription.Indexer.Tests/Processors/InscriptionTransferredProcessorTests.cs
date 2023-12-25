using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Forest.Contracts.Inscription;
using Inscription.Indexer.GraphQL;
using Nest;
using Shouldly;
using Xunit;

namespace Inscription.Indexer.Processors;

public class InscriptionTransferredProcessorTests : InscriptionIndexerTestBase
{
    private readonly IAElfIndexerClientEntityRepository<Entities.InscriptionHolder, LogEventInfo> _holderRepository;


    public InscriptionTransferredProcessorTests()
    {
        _holderRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<Entities.InscriptionHolder, LogEventInfo>>();
    }

    [Fact]
    public async Task Test()
    {
        var tick = "Tick";
        await CreateInscriptionAsync(tick);
        await IssueInscriptionAsync(tick);

        var inscription = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository,
            ObjectMapper, new GetIssuedInscriptionInput()
            {
                ChainId = ChainId,
                Tick = tick
            });
        inscription.Items[0].Tick.ShouldBe(tick);
        inscription.Items[0].IssuedToAddress.ShouldBe(TestAddress.ToBase58());
        inscription.Items[0].Amt.ShouldBe(1000);

        {
            var inscriptionTransferred = new InscriptionTransferred
            {
                Tick = "Tick",
                From = TestAddress,
                To = TestAddress,
                Amt = 100,
                InscriptionInfo = "InscriptionInfo"
            };

            var logEventInfo = GenerateLogEventInfo(inscriptionTransferred);
            var logEventContext = GenerateLogEventContext();

            await InscriptionTransferredProcessor.HandleEventAsync(logEventInfo, logEventContext);
            await SaveDataAsync();

            var inscriptionTransfer = await Query.InscriptionTransfer(TransferRepository, InscriptionRepository,
                ObjectMapper, new GetInscriptionTransferInput()
                {
                    ChainId = ChainId,
                });
            inscriptionTransfer.Count.ShouldBe(2);
            inscriptionTransfer[0].Tick.ShouldBe(inscriptionTransferred.Tick);
            inscriptionTransfer[0].FromAddress.ShouldBe(inscriptionTransferred.From.ToBase58());
            inscriptionTransfer[0].ToAddress.ShouldBe(inscriptionTransferred.To.ToBase58());
            inscriptionTransfer[0].Amt.ShouldBe(inscriptionTransferred.Amt);
            inscriptionTransfer[0].InscriptionInfo.ShouldBe(inscriptionTransferred.InscriptionInfo);
            inscriptionTransfer[0].Method.ShouldBe("Transfer");
            inscriptionTransfer[0].Number.ShouldBe(1);
            inscriptionTransfer[0].InscriptionImage.ShouldBe("inscriptionimage");

            inscription = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository,
                ObjectMapper,
                new GetIssuedInscriptionInput()
                {
                    ChainId = ChainId,
                    Tick = tick
                });
            inscription.Items[0].TransactionCount.ShouldBe(2);
            inscription.Items[0].HolderCount.ShouldBe(1);
            inscription.Items[0].IsCompleted.ShouldBeFalse();
            inscription.Items[0].Progress.ShouldBe(10);
            inscription.Items[0].MintedAmt.ShouldBe(100);

            var holder = await GetHolderAsync(ChainId, tick, TestAddress.ToBase58());
            holder.Amt.ShouldBe(100);
        }

        {
            var inscriptionTransferred = new InscriptionTransferred
            {
                Tick = "Tick",
                From = TestAddress,
                To = TestAddress,
                Amt = 50,
                InscriptionInfo = "InscriptionInfo"
            };

            var logEventInfo = GenerateLogEventInfo(inscriptionTransferred);
            var logEventContext = GenerateLogEventContext();

            await InscriptionTransferredProcessor.HandleEventAsync(logEventInfo, logEventContext);
            await SaveDataAsync();

            var inscriptionTransfer = await Query.InscriptionTransfer(TransferRepository, InscriptionRepository,
                ObjectMapper, new GetInscriptionTransferInput()
                {
                    ChainId = ChainId,
                });
            inscriptionTransfer.Count.ShouldBe(3);
            inscriptionTransfer[0].Tick.ShouldBe(inscriptionTransferred.Tick);
            inscriptionTransfer[0].FromAddress.ShouldBe(inscriptionTransferred.From.ToBase58());
            inscriptionTransfer[0].ToAddress.ShouldBe(inscriptionTransferred.To.ToBase58());
            inscriptionTransfer[0].Amt.ShouldBe(inscriptionTransferred.Amt);
            inscriptionTransfer[0].InscriptionInfo.ShouldBe(inscriptionTransferred.InscriptionInfo);
            inscriptionTransfer[0].Method.ShouldBe("Transfer");
            inscriptionTransfer[0].Number.ShouldBe(2);
            inscriptionTransfer[0].InscriptionImage.ShouldBe("inscriptionimage");

            inscription = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository,
                ObjectMapper,
                new GetIssuedInscriptionInput()
                {
                    ChainId = ChainId,
                    Tick = tick
                });
            inscription.Items[0].TransactionCount.ShouldBe(3);
            inscription.Items[0].HolderCount.ShouldBe(1);
            inscription.Items[0].IsCompleted.ShouldBeFalse();
            inscription.Items[0].Progress.ShouldBe(15);
            inscription.Items[0].MintedAmt.ShouldBe(150);

            var holder = await GetHolderAsync(ChainId, tick, TestAddress.ToBase58());
            holder.Amt.ShouldBe(150);
        }

        {
            var inscriptionTransferred = new InscriptionTransferred
            {
                Tick = "Tick",
                From = TestAddress,
                To = Address.FromBase58("2oF6i8qmydFaEmBGo2c7kpUHPiPdYeQb4b7w2nVZugkbsBd4Ng"),
                Amt = 850,
                InscriptionInfo = "InscriptionInfo"
            };

            var logEventInfo = GenerateLogEventInfo(inscriptionTransferred);
            var logEventContext = GenerateLogEventContext();

            await InscriptionTransferredProcessor.HandleEventAsync(logEventInfo, logEventContext);
            await SaveDataAsync();

            var inscriptionTransfer = await Query.InscriptionTransfer(TransferRepository, InscriptionRepository,
                ObjectMapper, new GetInscriptionTransferInput()
                {
                    ChainId = ChainId,
                });
            inscriptionTransfer.Count.ShouldBe(4);
            inscriptionTransfer[0].Tick.ShouldBe(inscriptionTransferred.Tick);
            inscriptionTransfer[0].FromAddress.ShouldBe(inscriptionTransferred.From.ToBase58());
            inscriptionTransfer[0].ToAddress.ShouldBe(inscriptionTransferred.To.ToBase58());
            inscriptionTransfer[0].Amt.ShouldBe(inscriptionTransferred.Amt);
            inscriptionTransfer[0].InscriptionInfo.ShouldBe(inscriptionTransferred.InscriptionInfo);
            inscriptionTransfer[0].Method.ShouldBe("Transfer");
            inscriptionTransfer[0].Number.ShouldBe(3);
            inscriptionTransfer[0].InscriptionImage.ShouldBe("inscriptionimage");

            inscription = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository,
                ObjectMapper,
                new GetIssuedInscriptionInput()
                {
                    ChainId = ChainId,
                    Tick = tick
                });
            inscription.Items[0].TransactionCount.ShouldBe(4);
            inscription.Items[0].HolderCount.ShouldBe(2);
            inscription.Items[0].IsCompleted.ShouldBeTrue();
            inscription.Items[0].Progress.ShouldBe(100);
            inscription.Items[0].MintedAmt.ShouldBe(1000);

            var holder = await GetHolderAsync(ChainId, tick, inscriptionTransferred.To.ToBase58());
            holder.Amt.ShouldBe(850);
        }
    }
    
    private async Task<Entities.InscriptionHolder> GetHolderAsync(string chainId, string tick, string address)
    {
        var mustQuery = new List<Func<QueryContainerDescriptor<Entities.InscriptionHolder>, QueryContainer>>();
        mustQuery.Add(q => q.Term(i => i.Field(f => f.ChainId).Value(chainId)));
        mustQuery.Add(q => q.Term(i => i.Field(f => f.Tick).Value(tick)));
        mustQuery.Add(q => q.Term(i => i.Field(f => f.Address).Value(address)));

        QueryContainer Filter(QueryContainerDescriptor<Entities.InscriptionHolder> f) => f.Bool(b => b.Must(mustQuery));
        return await _holderRepository.GetAsync(Filter);

    }
}