using Forest.Contracts.Inscription;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Identity;
using Xunit;

namespace Inscription.Indexer.GraphQL;

public class IssuedInscriptionQueryTests : InscriptionIndexerTestBase
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
            Amt = 1000,
            InscriptionInfo = "InscriptionInfo"
        };

        var logEventInfo = GenerateLogEventInfo(inscriptionTransferred);
        var logEventContext = GenerateLogEventContext();

        await InscriptionTransferredProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();
        
        await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository, ObjectMapper, new GetIssuedInscriptionInput()
        {
            ChainId = ChainId,
            SkipCount = 0,
            MaxResultCount = 1001
        }).ShouldThrowAsync<ArgumentOutOfRangeException>();
        
        var list = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository, ObjectMapper, new GetIssuedInscriptionInput()
        {
            ChainId = ChainId,
            Tick = "Tick1"
        });
        list.TotalCount.ShouldBe(1);
        list.Items[0].Tick.ShouldBe(tick1);
        
        list = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository, ObjectMapper, new GetIssuedInscriptionInput()
        {
            ChainId = ChainId,
        });
        list.TotalCount.ShouldBe(2);
        list.Items[0].Tick.ShouldBe(tick2);
        
        list = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository, ObjectMapper, new GetIssuedInscriptionInput()
        {
            ChainId = ChainId,
            IsCompleted = false
        });
        list.TotalCount.ShouldBe(1);
        list.Items[0].Tick.ShouldBe(tick1);
        
        list = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository, ObjectMapper, new GetIssuedInscriptionInput()
        {
            ChainId = ChainId,
            IsCompleted = true
        });
        list.TotalCount.ShouldBe(1);
        list.Items[0].Tick.ShouldBe(tick2);
        
        list = await Query.IssuedInscription(IssuedInscriptionRepository, InscriptionRepository, ObjectMapper, new GetIssuedInscriptionInput()
        {
            ChainId = ChainId,
            MaxResultCount = 1
        });
        list.TotalCount.ShouldBe(2);
        list.Items.Count.ShouldBe(1);
        list.Items[0].Tick.ShouldBe(tick2);
    }   
}