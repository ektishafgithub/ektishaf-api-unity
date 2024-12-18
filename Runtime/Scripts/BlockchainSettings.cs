using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ektishaf;

public class BlockchainSettings : ScriptableObject
{
    private const string ApiService = "https://api.ektishaf.com";
    private const string ApiVersion = "v1";

    public const string AssetPath = "Assets/Resources/BlockchainSettings.asset";

    [SerializeField]
    [Tooltip("A list of EVM compatible networks to be used for blockchain communication.")]
    public List<EktishafNetwork> Networks;

    [SerializeField]
    [Tooltip("A list of accounts to be used for development purposes. To generate, please click Ektishaf->Generate New Accounts.")]
    public List<EktishafAccount> Accounts;

    [SerializeField]
    [Range(1, 5)]
    [Tooltip("How many new accounts to create when Ektishaf->Generate New Accounts is clicked.")]
    public int MaxAccountsPerRequest;

    [SerializeField]
    [Tooltip("The password to be used for newly generated accounts. Make sure to modify (if needed) and remember this before Generate New Accounts.")]
    public string GenerateAccountsWithPassword;

    [SerializeField]
    [Tooltip("An IPFS url to access NFT's assets path on blockchain. i.e. Pinata gateway url. (Only for contract owner)")]
    public string AssetGateway;

    [SerializeField]
    [Tooltip("The metadata hash obtained when all metadatas are uploaded for the NFTs. (Only for contract owner)")]
    public string MetadataHash;

    [SerializeField]
    [Tooltip("Shows or hides logs.")]
    public bool ShowLogs;

    public static BlockchainSettings GetOrCreateSettings()
    {
        var settings = AssetDatabase.LoadAssetAtPath<BlockchainSettings>(AssetPath);
        if(settings == null)
        {
            if(!Directory.Exists(Path.GetDirectoryName(AssetPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(AssetPath));
            }

            settings = CreateInstance<BlockchainSettings>();
            settings.Networks = new List<EktishafNetwork>();
            settings.Accounts = new List<EktishafAccount>();
            settings.MaxAccountsPerRequest = 2;
            settings.ShowLogs = true;
            
            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    public static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }

    public bool HasAnyNetwork()
    {
        if (Networks.Count <= 0)
        {
            Debug.LogError($"No network found in Project Settings->Ektishaf Blockchain Settings->Networks, please add at least one network.");
            return false;
        }
        return true;
    }

    public bool IsValidNetwork(EktishafNetwork Network)
    {
        if(string.IsNullOrEmpty(Network.Rpc) || string.IsNullOrWhiteSpace(Network.Rpc) ||
           string.IsNullOrEmpty(Network.ChainId) || string.IsNullOrWhiteSpace(Network.ChainId) ||
           string.IsNullOrEmpty(Network.CurrencySymbol) || string.IsNullOrWhiteSpace(Network.CurrencySymbol))
        {
            Debug.LogError($"Specified network is not a valid network, make sure it has at least Rpc, ChainId and CurrencySymbol information.");
            return false;
        }
        return true;
    }

    public EktishafNetwork GetNetwork(string ChainId)
    {
        if (!HasAnyNetwork())
        {
            return new EktishafNetwork();
        }

        foreach (var Network in Networks)
        {
            if (!IsValidNetwork(Network)) continue;

            if (Network.ChainId.ToUpper().Equals(ChainId.ToUpper()))
            {
                return Network;
            }
        }
        Debug.Log($"No network found with the chain id: {ChainId}");
        return new EktishafNetwork();
    }

    public EktishafAccount GetAccount(string Address)
    {
        if (Accounts.Count <= 0)
        {
            Debug.Log("Failed to obtain accounts from Project Settings->Game->Ektishaf->Accounts->Accounts");
            return new EktishafAccount();
        }

        foreach (var Account in Accounts)
        {
            if (Account.Address.ToUpper().Equals(Address.ToUpper()))
            {
                return Account;
            }
        }
        Debug.Log($"No account found with the address: {Address}");
        return new EktishafAccount();
    }

    private string GetUrl(string api = "")
    {
        return $"{ApiService}/{ApiVersion}/{api}";
    }

    public string[] GetMetadataUris(int[] ids)
    {
        string[] uris = new string[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            uris[i] = $"{AssetGateway}/{MetadataHash}/{ids[i]}.json";
        }
        return uris;
    }

    public string Op(ServOp servOp = ServOp.None)
    {
        switch(servOp)
        {
            case ServOp.None:     return GetUrl();
            case ServOp.Register: return GetUrl("register");
            case ServOp.Login:    return GetUrl("login");
            case ServOp.External: return GetUrl("external");
            case ServOp.Reveal:   return GetUrl("reveal");
            case ServOp.Sign:     return GetUrl("sign");
            case ServOp.Verify:   return GetUrl("verify");
            case ServOp.Balance:  return GetUrl("balance");
            case ServOp.ABI:      return GetUrl("abi");
            case ServOp.Read:     return GetUrl("read");
            case ServOp.Write:    return GetUrl("write");
            case ServOp.Accounts: return GetUrl("accounts");
            case ServOp.Send:     return GetUrl("send");
            default:              return GetUrl();
        }
    }
}