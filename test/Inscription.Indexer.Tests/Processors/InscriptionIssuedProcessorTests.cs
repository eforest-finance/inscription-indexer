using AElfIndexer.Client;
using AElfIndexer.Grains.State.Client;
using Forest.Inscription;
using Inscription.Indexer.GraphQL;
using Shouldly;
using Xunit;

namespace Inscription.Indexer.Processors;

public class InscriptionIssuedProcessorTests: InscriptionIndexerTestBase
{
    private readonly InscriptionIssuedProcessor _inscriptionIssuedProcessor;
    private readonly InscriptionTransferredProcessor _inscriptionTransferredProcessor;
    private readonly IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo> _issuedInscriptionRepository;
    private readonly IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo> _transferRepository;

    public InscriptionIssuedProcessorTests()
    {
        _inscriptionIssuedProcessor = GetRequiredService<InscriptionIssuedProcessor>();
        _inscriptionTransferredProcessor = GetRequiredService<InscriptionTransferredProcessor>();
        _issuedInscriptionRepository = GetRequiredService<IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo>>();
        _transferRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo>>();
    }

    [Fact]
    public async Task Test()
    {
        var inscriptionIssued = new InscriptionIssued
        {
            Tick = "Tick",
            To = TestAddress,
            Symbol = "Symbol",
            Amt = 1000,
            InscriptionInfo = "InscriptionInfo"
        };

        var logEventInfo = GenerateLogEventInfo(inscriptionIssued);
        var logEventContext = GenerateLogEventContext();

        await _inscriptionIssuedProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();

        var inscription = await Query.IssuedInscription(_issuedInscriptionRepository, ObjectMapper, new GetIssuedInscriptionInput()
        {
            ChainId = ChainId,
            Tick = inscriptionIssued.Tick
        });
        inscription[0].Tick.ShouldBe(inscriptionIssued.Tick);
        inscription[0].IssuedToAddress.ShouldBe(inscriptionIssued.To.ToBase58());
        inscription[0].Symbol.ShouldBe(inscriptionIssued.Symbol);
        inscription[0].Amt.ShouldBe(inscriptionIssued.Amt);
        inscription[0].TransactionCount.ShouldBe(1);
        
        var inscriptionTransfer = await Query.InscriptionTransfer(_transferRepository, ObjectMapper, new GetInscriptionTransferInput()
        {
            ChainId = ChainId,
        });
        inscriptionTransfer[0].Tick.ShouldBe(inscriptionIssued.Tick);
        inscriptionTransfer[0].FromAddress.ShouldBe(inscriptionIssued.To.ToBase58());
        inscriptionTransfer[0].ToAddress.ShouldBe(inscriptionIssued.To.ToBase58());
        inscriptionTransfer[0].Symbol.ShouldBe(inscriptionIssued.Symbol);
        inscriptionTransfer[0].Amt.ShouldBe(inscriptionIssued.Amt);
        inscriptionTransfer[0].InscriptionInfo.ShouldBe(inscriptionIssued.InscriptionInfo);
        inscriptionTransfer[0].Method.ShouldBe("Deploy");
        
        
    }
}