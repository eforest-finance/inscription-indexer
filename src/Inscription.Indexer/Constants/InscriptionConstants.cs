namespace Inscription.Indexer.Constants;

public static partial class InscriptionIndexerConstants
{
    public const string AELF = "AELF";
    public const string TDVV = "tDVV";
    public const string TDVW = "tDVW";

    //Prod
    public const string InscriptionContractAddressAELF = "5coTDCKk8v8e9XksU9aPJpLAJZrQ6SxNfxifYViwGZNVj4Wng";
    //Test
    //public const string InscriptionContractAddressAELF = "2mHpqC8YzBuFfRLmDaRFnKHDYSfREAir128rd5byenDPXPAz2q";

    public const string InscriptionContractAddressTDVV = "5coTDCKk8v8e9XksU9aPJpLAJZrQ6SxNfxifYViwGZNVj4Wng";


    public const string InscriptionContractAddressTDVW = "2mHpqC8YzBuFfRLmDaRFnKHDYSfREAir128rd5byenDPXPAz2q";

    public static List<string> IgnoreInscription { get; set; }
        = new List<string>() { };
}