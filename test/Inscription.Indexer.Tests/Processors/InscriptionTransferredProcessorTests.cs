using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Forest.Contracts.Inscription;
using Inscription.Indexer.GraphQL;
using Shouldly;
using Xunit;

namespace Inscription.Indexer.Processors;

public class InscriptionTransferredProcessorTests : InscriptionIndexerTestBase
{
    private readonly InscriptionIssuedProcessor _inscriptionIssuedProcessor;
    private readonly InscriptionTransferredProcessor _inscriptionTransferredProcessor;
    private readonly InscriptionCreatedProcessor _inscriptionCreatedProcessor;

    private readonly IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo>
        _issuedInscriptionRepository;
    private readonly IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo> _transferRepository;
    private readonly IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> _inscriptionRepository;

    public InscriptionTransferredProcessorTests()
    {
        _inscriptionIssuedProcessor = GetRequiredService<InscriptionIssuedProcessor>();
        _inscriptionTransferredProcessor = GetRequiredService<InscriptionTransferredProcessor>();
        _inscriptionCreatedProcessor = GetRequiredService<InscriptionCreatedProcessor>();
        _issuedInscriptionRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo>>();
        _transferRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo>>();
        _inscriptionRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo>>();
    }

    [Fact]
    public async Task Test()
    {
        var inscriptionCreated = new InscriptionCreated
        {
            Tick = "Tick",
            Deployer = TestAddress,
            Issuer = TestAddress,
            Limit = 10,
            Owner = TestAddress,
            TotalSupply = 100000,
            IssueChainId = 1,
            CollectionExternalInfo = new ExternalInfos
            {
                Value = { {"inscription_image","inscriptionimage"} }
            },
            ItemExternalInfo = new ExternalInfos
            {
                Value = { {"inscription_image","inscription_image"} }
            }
        };

        var logEventInfo = GenerateLogEventInfo(inscriptionCreated);
        var logEventContext = GenerateLogEventContext();

        await _inscriptionCreatedProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();
        
        var inscriptionIssued = new InscriptionIssued
        {
            Tick = "Tick",
            To = TestAddress,
            Amt = 1000,
            InscriptionInfo = "InscriptionInfo"
        };

        logEventInfo = GenerateLogEventInfo(inscriptionIssued);
        logEventContext = GenerateLogEventContext();

        await _inscriptionIssuedProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();

        var inscription = await Query.IssuedInscription(_issuedInscriptionRepository, _inscriptionRepository,
            ObjectMapper, new GetIssuedInscriptionInput()
            {
                ChainId = ChainId,
                Tick = inscriptionIssued.Tick
            });
        inscription.Items[0].Tick.ShouldBe(inscriptionIssued.Tick);
        inscription.Items[0].IssuedToAddress.ShouldBe(inscriptionIssued.To.ToBase58());
        inscription.Items[0].Amt.ShouldBe(inscriptionIssued.Amt);

        var inscriptionTransferred = new InscriptionTransferred
        {
            Tick = "Tick",
            From = TestAddress,
            To = TestAddress,
            Amt = 100,
            InscriptionInfo = "InscriptionInfo"
        };

        logEventInfo = GenerateLogEventInfo(inscriptionTransferred);
        logEventContext = GenerateLogEventContext();

        await _inscriptionTransferredProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();

        var inscriptionTransfer = await Query.InscriptionTransfer(_transferRepository, _inscriptionRepository,
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

        inscription = await Query.IssuedInscription(_issuedInscriptionRepository, _inscriptionRepository, ObjectMapper,
            new GetIssuedInscriptionInput()
            {
                ChainId = ChainId,
                Tick = inscriptionIssued.Tick
            });
        inscription.Items[0].TransactionCount.ShouldBe(2);
        inscription.Items[0].HolderCount.ShouldBe(1);
    }
}