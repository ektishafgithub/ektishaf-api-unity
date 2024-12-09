using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockchainConfig", menuName = "Ektishaf/BlockchainConfiguration", order = 1)]
public class BlockchainSettings : ScriptableObject
{
    private const string ApiService = "https://api.ektishaf.com";
    private const string ApiVersion = "v1";

    private static Object Instance = null;

    [SerializeField]
    [Tooltip("A list of rpc urls that can be used selectively.")]
    private List<RpcData> RpcList;

    [Tooltip("A test wallet address provided to see how the demo functionality works.")]
    public string TestWalletAddress;

    [Tooltip("A test ticket provided to allow authorization for demo blockchain operation.")]
    public string TestTicket;

    [Tooltip("An ipfs url to access NFT's assets on blockchain. i.e. Pinata gateway url. (This property is only for Studio who owns the contract.")]
    public string DataUrl;

    [Tooltip("The metadata hash obtained when all metadatas are uploaded for the NFTs. (This property is only for Studio who owns the contract.)")]
    public string MetadataHash;

    [Tooltip("Shows or hides logs.")]
    public bool ShowLogs;

    private Dictionary<string, string> RpcDictionary;

    public static Object GetInstance()
    {
        if(Instance != null) return Instance;

        Object Original = Resources.Load<Object>("BlockchainConfig");
        Instance = Instantiate(Original);
        return Instance;
    }

    private void Awake()
    {
        RpcDictionary = RpcListToDictionary();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    [MenuItem("Ektishaf/Settings", false, 100)]
    public static void Settings()
    {
        Object Original = Resources.Load<Object>("BlockchainConfig");
        Selection.activeObject = Original;
        EditorGUIUtility.PingObject(Original);
    }

    private Dictionary<string, string> RpcListToDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach(RpcData data in RpcList)
        {
            dictionary.Add(data.RpcKey, data.RpcUrl);
        }
        return dictionary;
    }

    public string GetRpc(string RpcKey)
    {
        if(!RpcDictionary.ContainsKey(RpcKey))
        {
            Debug.LogError($"Could not find RpcKey: {RpcKey} in the list, please make sure to specifiy a valid RpcKey.");
            return "";
        }
        return RpcDictionary[RpcKey];
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
            uris[i] = $"{DataUrl}/{MetadataHash}/{ids[i]}.json";
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
            default:              return GetUrl();
        }
    }
}

[System.Serializable]
public struct RpcData
{
    public string RpcKey;
    public string RpcUrl;
}

public enum ServOp
{
    None,
    Register,
    Login,
    External,
    Reveal,
    Sign,
    Verify,
    Balance,
    ABI,
    Read,
    Write
}