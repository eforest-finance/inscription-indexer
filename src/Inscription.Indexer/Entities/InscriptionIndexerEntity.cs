using AElfIndexer.Client;

namespace Inscription.Indexer.Entities;

public class InscriptionIndexerEntity<T> : AElfIndexerClientEntity<T>
{
    public DateTime BlockTime { get; set; }
}