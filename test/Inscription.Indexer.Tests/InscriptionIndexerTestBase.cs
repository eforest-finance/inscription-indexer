using AElf.Contracts.MultiToken;
using AElf.CSharp.Core;
using AElf.CSharp.Core.Extension;
using AElf.Types;
using AElfIndexer.Client;
using AElfIndexer.Client.Handlers;
using AElfIndexer.Client.Providers;
using AElfIndexer.Grains;
using AElfIndexer.Grains.State.Client;
using Inscription.Indexer.TestBase;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Threading;

namespace Inscription.Indexer;

public abstract class InscriptionIndexerTestBase: AElfIndexerInscriptionTestBase<InscriptionIndexerTestModule>
{
    
    private readonly IAElfIndexerClientInfoProvider _indexerClientInfoProvider;
    private readonly IBlockStateSetProvider<LogEventInfo> _blockStateSetProvider;
    private readonly IDAppDataProvider _dAppDataProvider;
    private readonly IDAppDataIndexManagerProvider _dAppDataIndexManagerProvider;
    protected readonly IObjectMapper ObjectMapper;
    
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
}