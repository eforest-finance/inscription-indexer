using Forest.Contracts.Inscription;
using Microsoft.Extensions.Options;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Identity;
using Xunit;

namespace Inscription.Indexer.GraphQL;

public class InscriptionTransferQueryTests : InscriptionIndexerTestBase
{
    [Fact]
    public async Task Test()
    {
        var tick1 = "Tick1";
        var tick2 = "Tick2";
        await CreateInscriptionAsync(tick1);
        await IssueInscriptionAsync(tick1);
        await CreateInscriptionAsync(tick2);
        await IssueInscriptionAsync(tick2);
        
        var inscriptionTransferred = new InscriptionTransferred
        {
            Tick = tick2,
            From = TestAddress,
            To = TestAddress,
            Amt = 10,
            InscriptionInfo = "InscriptionInfo"
        };

        var logEventInfo = GenerateLogEventInfo(inscriptionTransferred);
        var logEventContext = GenerateLogEventContext();

        await InscriptionTransferredProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();
        
        await Query.InscriptionTransfer(TransferRepository, InscriptionRepository,InscriptionOptions, ObjectMapper, new GetInscriptionTransferInput()
        {
            ChainId = ChainId,
            SkipCount = 0,
            MaxResultCount = 1001
        }).ShouldThrowAsync<ArgumentOutOfRangeException>();
        
        var list = await Query.InscriptionTransfer(TransferRepository, InscriptionRepository,InscriptionOptions, ObjectMapper, new GetInscriptionTransferInput()
        {
            ChainId = ChainId,
        });
        list.Count.ShouldBe(3);
        list[0].Tick.ShouldBe(tick2);
        list[0].Method.ShouldBe("Transfer");
        list[1].Tick.ShouldBe(tick2);
        list[1].Method.ShouldBe("Deploy");
        list[2].Tick.ShouldBe(tick1);
        list[2].Method.ShouldBe("Deploy");
    }   
    
    [Fact]
    public async Task Test_Ignore()
    {
        var tick1 = "Tick1";
        var tick2 = "TickIgnore";
        await CreateInscriptionAsync(tick1);
        await IssueInscriptionAsync(tick1);
        await CreateInscriptionAsync(tick2);
        await IssueInscriptionAsync(tick2);
        
        var inscriptionTransferred = new InscriptionTransferred
        {
            Tick = tick2,
            From = TestAddress,
            To = TestAddress,
            Amt = 10,
            InscriptionInfo = "InscriptionInfo"
        };

        var logEventInfo = GenerateLogEventInfo(inscriptionTransferred);
        var logEventContext = GenerateLogEventContext();

        await InscriptionTransferredProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();
        
        var list = await Query.InscriptionTransfer(TransferRepository, InscriptionRepository,InscriptionOptions, ObjectMapper, new GetInscriptionTransferInput()
        {
            ChainId = ChainId,
        });
        list.Count.ShouldBe(1);
        list[0].Tick.ShouldBe(tick1);
        list[0].Method.ShouldBe("Deploy");
    }   
}