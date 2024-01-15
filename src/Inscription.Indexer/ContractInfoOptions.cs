namespace Inscription.Indexer;

public class ContractInfoOptions
{
    public Dictionary<string, ContractInfo> ContractInfos { get; set; }
}

public class ContractInfo
{
    public string InscriptionContractAddress { get; set; }
}