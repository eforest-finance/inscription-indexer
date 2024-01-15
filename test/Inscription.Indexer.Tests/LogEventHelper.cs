using AElf.Types;
using AElfIndexer.Grains.State.Client;

namespace Inscription.Indexer;

public static class LogEventHelper
{
    public static LogEventInfo ToLogEventInfo(this LogEvent logEvent)
    {
        var logEventInfo = new LogEventInfo
        {
            ExtraProperties = new Dictionary<string, string>
            {
                {"Indexed", logEvent.Indexed.ToString()},
                {"NonIndexed", logEvent.NonIndexed.ToBase64()}
            }
        };
        return logEventInfo;
    }
}