using AElfIndexer.Client.GraphQL;

namespace Inscription.Indexer.GraphQL;

public class InscriptionIndexerSchema : AElfIndexerClientSchema<Query>
{
    public InscriptionIndexerSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}