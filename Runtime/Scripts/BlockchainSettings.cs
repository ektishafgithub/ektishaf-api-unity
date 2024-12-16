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
    [Tooltip("How many accounts to create when a request is made from Ektishaf->Generate New Accounts.")]
    public int MaxAccountsPerRequest;

    [SerializeField]
    [Tooltip("When a request is made to create new accounts, this password will be used for all of them. Make sure to modify (if needed) and remember this before Generate New Accounts.")]
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

            // Sample Network
            EktishafNetwork Sepolia = new EktishafNetwork();
            Sepolia.NetworkName = "Sepolia test network";
            Sepolia.Rpc = "https://eth-sepolia.g.alchemy.com/v2/YBy3ka0SJ5YW7aGgnz7oj-U_QUJ_pND4";
            Sepolia.ChainId = "11155111";
            Sepolia.CurrencySymbol = "SepoliaETH";
            Sepolia.BlockExplorer = "https://sepolia.etherscan.io";
            settings.Networks.Add(Sepolia);

            // Sample Account
            EktishafAccount SampleAccount;
            SampleAccount.Address = "0xf0deb67ec9064794211e14938c639728bda2481a";
            SampleAccount.Ticket = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcm9qZWN0IjoiZ29yIiwicGFzc3dvcmQiOiJFa3Rpc2hhZiBBaHdheiIsImVuY3J5cHRpb24iOiJ7XCJhZGRyZXNzXCI6XCJmMGRlYjY3ZWM5MDY0Nzk0MjExZTE0OTM4YzYzOTcyOGJkYTI0ODFhXCIsXCJpZFwiOlwiZmZkYWVjOTUtOThjMi00YTdiLWI1YzItYWQ0YzZkYjNkZWY5XCIsXCJ2ZXJzaW9uXCI6MyxcIkNyeXB0b1wiOntcImNpcGhlclwiOlwiYWVzLTEyOC1jdHJcIixcImNpcGhlcnBhcmFtc1wiOntcIml2XCI6XCIyMWIyODY2ZjdlMjJkMTc2MjRhMTM4YWY2NDhmM2EzN1wifSxcImNpcGhlcnRleHRcIjpcImQ1NzFjOWZjNGRkN2Q5NGE2MGRkMjIwYzY0ZWRjZDlmZGQyNGYxMmU3ODVkZDEzM2Q0ZWI4OTc1OWM0OTFlNmRcIixcImtkZlwiOlwic2NyeXB0XCIsXCJrZGZwYXJhbXNcIjp7XCJzYWx0XCI6XCI3NmVjNzcxMDc2NTc0OTkzOWY3ZDliOTM0MjM3YTFlY2RhYjNiMzYzNDI3ZDk5OGY3OGFmMTdiNTMxYzVmODIwXCIsXCJuXCI6MTMxMDcyLFwiZGtsZW5cIjozMixcInBcIjoxLFwiclwiOjh9LFwibWFjXCI6XCI2ZGYyNTMwODQ4ZWY1MjI3MjNmYzU5MWJkZjVmYzU1ZmI3MTg0ODFiZjA2MzRkYWQ1Y2M4MWE5ZTlmMDZiN2FmXCJ9LFwieC1ldGhlcnNcIjp7XCJjbGllbnRcIjpcImV0aGVycy82LjEzLjJcIixcImdldGhGaWxlbmFtZVwiOlwiVVRDLS0yMDI0LTA4LTIzVDExLTQzLTAwLjBaLS1mMGRlYjY3ZWM5MDY0Nzk0MjExZTE0OTM4YzYzOTcyOGJkYTI0ODFhXCIsXCJwYXRoXCI6XCJtLzQ0Jy82MCcvMCcvMC8wXCIsXCJsb2NhbGVcIjpcImVuXCIsXCJtbmVtb25pY0NvdW50ZXJcIjpcImVkOGI4OTllNWVhMzU0ZDc5MGRmMDAxOGRlMzc3M2NmXCIsXCJtbmVtb25pY0NpcGhlcnRleHRcIjpcImU1NDZjNTBlYzVhZTcxNzlmNDMyNDFkYzFiZjJhM2I4XCIsXCJ2ZXJzaW9uXCI6XCIwLjFcIn19IiwiaWF0IjoxNzI0ODE5NzM5LCJleHAiOjE3MjQ5MDYxMzl9.2QPVZ6t_AHWKDdPTzj25Fj41dMzF764CagpFWKFGPuA";
            settings.Accounts.Add(SampleAccount);

            settings.MaxAccountsPerRequest = 5;

            settings.AssetGateway = "https://azure-elaborate-quelea-290.mypinata.cloud/ipfs";
            settings.MetadataHash = "QmRE6YQTpQxu725mXr5usqVGfcwGZFJrxDGjnZgCr7Ruso";
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

    public EktishafNetwork GetNetwork(string ChainId)
    {
        if (Networks.Count <= 0)
        {
            Debug.Log("Failed to obtain networks from Project Settings->Ektishaf->Blockchain->Networks");
            return new EktishafNetwork();
        }

        foreach (var Network in Networks)
        {
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