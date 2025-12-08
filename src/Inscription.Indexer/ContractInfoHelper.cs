
using Inscription.Indexer.Constants;

namespace Inscription.Indexer;

public class ContractInfoHelper
{
    public static string GetInscriptionContractAddress(string chainId)
    {
        return chainId switch
        {
            InscriptionIndexerConstants.AELF => InscriptionIndexerConstants.InscriptionContractAddressAELF,
            InscriptionIndexerConstants.TDVV => InscriptionIndexerConstants.InscriptionContractAddressTDVV,
            InscriptionIndexerConstants.TDVW => InscriptionIndexerConstants.InscriptionContractAddressTDVW,
            _ => ""
        };
    }
}