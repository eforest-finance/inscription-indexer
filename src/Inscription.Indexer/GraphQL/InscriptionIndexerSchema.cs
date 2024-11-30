using AeFinder.Sdk;

namespace Inscription.Indexer.GraphQL;

public class InscriptionIndexerSchema : AppSchema<Query>
{
    public InscriptionIndexerSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}