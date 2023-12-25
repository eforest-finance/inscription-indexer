using AElf.CSharp.Core;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Client.Providers;
using AElfIndexer.Grains;
using AElfIndexer.Grains.State.Client;
using Forest.Contracts.Inscription;
using Inscription.Indexer.Processors;
using Inscription.Indexer.TestBase;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Threading;

namespace Inscription.Indexer;

public abstract class InscriptionIndexerTestBase: InscriptionIndexerTestBase<InscriptionIndexerTestModule>
{
    
    private readonly IAElfIndexerClientInfoProvider _indexerClientInfoProvider;
    private readonly IBlockStateSetProvider<LogEventInfo> _blockStateSetProvider;
    private readonly IDAppDataProvider _dAppDataProvider;
    private readonly IDAppDataIndexManagerProvider _dAppDataIndexManagerProvider;
    protected readonly IObjectMapper ObjectMapper;
    
    protected readonly InscriptionIssuedProcessor InscriptionIssuedProcessor;
    protected readonly InscriptionTransferredProcessor InscriptionTransferredProcessor;
    protected readonly InscriptionCreatedProcessor InscriptionCreatedProcessor;
    protected readonly IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo> IssuedInscriptionRepository;
    protected readonly IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo> TransferRepository;
    protected readonly IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo> InscriptionRepository;
    
    protected Address TestAddress = Address.FromBase58("ooCSxQ7zPw1d4rhQPBqGKB6myvuWbicCiw3jdcoWEMMpa54ea");
    protected string ChainId = "AELF";
    protected string BlockHash = "dac5cd67a2783d0a3d843426c2d45f1178f4d052235a907a0d796ae4659103b1";
    protected string PreviousBlockHash = "e38c4fb1cf6af05878657cb3f7b5fc8a5fcfb2eec19cd76b73abb831973fbf4e";
    protected string TransactionId = "c1e625d135171c766999274a00a7003abed24cfe59a7215aabf1472ef20a2da2";
    protected long BlockHeight = 100;
    protected string BlockStateSetKey;
    
    public InscriptionIndexerTestBase()
    {
        _indexerClientInfoProvider = GetRequiredService<IAElfIndexerClientInfoProvider>();
        _blockStateSetProvider = GetRequiredService<IBlockStateSetProvider<LogEventInfo>>();
        _dAppDataProvider = GetRequiredService<IDAppDataProvider>();
        _dAppDataIndexManagerProvider = GetRequiredService<IDAppDataIndexManagerProvider>();
        ObjectMapper = GetRequiredService<IObjectMapper>();
        
        InscriptionIssuedProcessor = GetRequiredService<InscriptionIssuedProcessor>();
        InscriptionTransferredProcessor = GetRequiredService<InscriptionTransferredProcessor>();
        InscriptionCreatedProcessor = GetRequiredService<InscriptionCreatedProcessor>();
        IssuedInscriptionRepository = GetRequiredService<IAElfIndexerClientEntityRepository<Entities.IssuedInscription, LogEventInfo>>();
        TransferRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<Entities.InscriptionTransfer, LogEventInfo>>();
        InscriptionRepository =
            GetRequiredService<IAElfIndexerClientEntityRepository<Entities.Inscription, LogEventInfo>>();
       

        BlockStateSetKey = AsyncHelper.RunSync(async () => await InitializeBlockStateSetAsync(
            new BlockStateSet<LogEventInfo>
            {
                BlockHash = BlockHash,
                BlockHeight = BlockHeight,
                Confirmed = true,
                PreviousBlockHash = PreviousBlockHash,
            }, ChainId));
    }

    protected async Task<string> InitializeBlockStateSetAsync(BlockStateSet<LogEventInfo> blockStateSet,string chainId)
    {
        var key = GrainIdHelper.GenerateGrainId("BlockStateSets", _indexerClientInfoProvider.GetClientId(), chainId,
            _indexerClientInfoProvider.GetVersion());
        
        await _blockStateSetProvider.SetBlockStateSetAsync(key,blockStateSet);
        await _blockStateSetProvider.SetCurrentBlockStateSetAsync(key, blockStateSet);
        await _blockStateSetProvider.SetLongestChainBlockStateSetAsync(key,blockStateSet.BlockHash);
        
        return key;
    }

    protected LogEventInfo GenerateLogEventInfo<T>(T eventData) where T : IEvent<T>
    {
        var logEventInfo = eventData.ToLogEvent().ToLogEventInfo();
        logEventInfo.BlockHeight = BlockHeight;
        logEventInfo.ChainId = ChainId;
        logEventInfo.BlockHash = BlockHash;
        logEventInfo.TransactionId = TransactionId;

        return logEventInfo;
    }
    
    protected LogEventContext GenerateLogEventContext()
    {
        return new LogEventContext
        {
            ChainId = ChainId,
            BlockHeight = BlockHeight,
            BlockHash = BlockHash,
            PreviousBlockHash = PreviousBlockHash,
            TransactionId = Guid.NewGuid().ToString("N"),
            BlockTime = DateTime.UtcNow
        };
    }

    protected async Task SaveDataAsync()
    {
        await _dAppDataProvider.SaveDataAsync();
        await _dAppDataIndexManagerProvider.SavaDataAsync();
        await _blockStateSetProvider.SaveDataAsync(BlockStateSetKey);
    }
    
    protected async Task CreateInscriptionAsync(string tick)
    {
        var inscriptionCreated = new InscriptionCreated
        {
            Tick = tick,
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
                Value = { {"key2","value2"} }
            }
        };

        var logEventInfo = GenerateLogEventInfo(inscriptionCreated);
        var logEventContext = GenerateLogEventContext();

        await InscriptionCreatedProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();
    }

    protected async Task IssueInscriptionAsync(string tick)
    {
        var inscriptionIssued = new InscriptionIssued
        {
            Tick = tick,
            To = TestAddress,
            Amt = 1000,
            InscriptionInfo = "InscriptionInfo"
        };

        var logEventInfo = GenerateLogEventInfo(inscriptionIssued);
        var logEventContext = GenerateLogEventContext();

        await InscriptionIssuedProcessor.HandleEventAsync(logEventInfo, logEventContext);
        await SaveDataAsync();
    }
}