using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Ektishaf;

public class BlockchainServiceUnity : BlockchainService
{
    #region Variables
    public static BlockchainServiceUnity Singleton = null;

    public TMP_InputField PasswordRegisterPanel;
    public TMP_InputField PasswordLoginPanel;
    public TMP_InputField PrivateKeyImportPanel;
    public TMP_InputField PasswordImportPanel;
    public TextMeshProUGUI BalanceText;
    public GameObject PanelWallet;
    public GameObject PanelRegister;
    public GameObject PanelLogin; 
    public GameObject PanelImport;
    public GameObject PanelNFT;
    public GameObject PanelLoading;
    public GameObject NftContent;

    private List<GameObject> nfts;
    private string walletAddress;
    private string currentTicket;
    #endregion

    #region Events Declaration
    public event EventHandler ServiceInitialized;
    public event WalletConnectedEventHandler WalletConnectedEvent;
    #endregion

    #region Properties
    public BlockchainSettings Config { get { return config; } }
    public string WalletAddress { get { return walletAddress; } }
    public string CurrentTicket { get { return currentTicket; } }
    #endregion

    #region Default Methods
    protected override void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            base.Awake();
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        WalletConnectedEvent += Event_WalletConnected;
    }

    private void Start()
    {
        nfts = new List<GameObject>();
        Init();
    }

    private void OnDisable()
    {
        WalletConnectedEvent -= Event_WalletConnected;
    }
    #endregion

    #region UI Methods
    private void HideAllPanels()
    {
        PanelWallet.SetActive(false);
        PanelRegister.SetActive(false);
        PanelLogin.SetActive(false);
        PanelImport.SetActive(false);
        PanelNFT.SetActive(false);
    }

    public void ShowLoading()
    {
        PanelLoading.SetActive(true);
        Invoke(nameof(HideLoading), 10f);
    }

    public void HideLoading()
    {
        PanelLoading.SetActive(false);
    }

    public void Reset()
    {
        walletAddress = "";
        currentTicket = "";
        ClearNfts();
        BalanceText.gameObject.SetActive(false);
    }
    #endregion

    #region Nfts UI Methods
    public void ClearNfts()
    {
        if (NftContent.transform.childCount > 0)
        {
            for (int i = 0; i < NftContent.transform.childCount; i++)
            {
                Destroy(NftContent.transform.GetChild(i).gameObject);
            }
        }
        nfts.Clear();
    }

    private void AddNft(int id, int amount, string uri)
    {
        GameObject nftItem = new GameObject(id.ToString());
        nftItem.AddComponent<CanvasRenderer>();

        LayoutElement layoutElement = nftItem.AddComponent<LayoutElement>();
        layoutElement.minWidth = 720;
        layoutElement.minHeight = 405;
        layoutElement.preferredWidth = 720;
        layoutElement.preferredHeight = 405;

        GameObject label = new GameObject("Label");
        TextMeshProUGUI textMesh = label.AddComponent<TextMeshProUGUI>();
        textMesh.text = $"Id: {id}, Amount: {amount}";

        label.transform.SetParent(nftItem.transform);
        label.transform.localScale = Vector3.one;

        nftItem.AddComponent<RawImage>();

        NftUI nftUI = nftItem.AddComponent<NftUI>();
        nftUI.uri = uri;

        nftItem.transform.SetParent(NftContent.transform);
        nftItem.transform.localScale = Vector3.one;

        nfts.Add(nftItem);
    }

    private void AddNfts(List<List<string>> _nfts)
    {
        if (_nfts.Count > 0)
        {
            foreach (var nft in _nfts)
            {
                AddNft(int.Parse(nft[0]), int.Parse(nft[1]), nft[2]);
            }
        }
    }

    private bool HasPendingDownloads()
    {
        if (nfts.Count > 0)
        {
            foreach (GameObject nft in nfts)
            {
                if (!nft.GetComponent<NftUI>().isDownloaded)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void DownloadNfts()
    {
        if(nfts.Count > 0)
        {
            foreach (GameObject nft in nfts)
            {
                NftUI nftUI = nft.GetComponent<NftUI>();
                if (!nftUI.isDownloaded)
                {
                    nftUI.GetRawImage();
                }
            }
        }
    }

    private IEnumerator WaitForPendingNftsCoroutine()
    {
        if (nfts.Count > 0)
        {
            while (HasPendingDownloads())
            {
                ShowLoading();
                yield return new WaitForSeconds(1f);
            }
            HideLoading();
        }
    }

    private void WaitForPendingNfts()
    {
        StartCoroutine(WaitForPendingNftsCoroutine());
    }

    private void GrabNfts(List<List<string>> _nfts)
    {
        AddNfts(_nfts);
        DownloadNfts();
        WaitForPendingNfts();
    }
    #endregion

    #region Events Handling
    private void Event_WalletConnected(object sender, WalletConnectedEventArgs e)
    {
        walletAddress = e.address;
        currentTicket = e.ticket;

        Balance();

        HideAllPanels();
        PanelNFT.SetActive(true);
        BalanceText.gameObject.SetActive(true);
    }
    #endregion

    #region Core Methods
    public void Init()
    {
        Host((success, result) =>
        {
            if (success)
            {
                Log(result);
                ServiceInitialized?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Log("Failed to initialize blockchain service, retrying ...");
                Init();
            }
        });
    }

    public void Register()
    {
        ShowLoading();
        Register(PasswordRegisterPanel.text, (success, address, ticket, error) =>
        {
            if (success)
            {
                Log($"Successfully registered a new wallet {address}");
                WalletConnectedEvent?.Invoke(this, new WalletConnectedEventArgs(address, ticket));
            }
            else
            {
                Log($"Failed to register a new wallet.");
            }
            HideLoading();
        });
    }

    public void Login()
    {
        ShowLoading();
        Login(config.TestTicket, PasswordLoginPanel.text, (success, address, ticket, error) =>
        {
            if (success)
            {
                Log($"Successfully logged in wallet {address}");
                WalletConnectedEvent?.Invoke(this, new WalletConnectedEventArgs(address, ticket));
            }
            else
            {
                Log($"Failed to login wallet.");
            }
            HideLoading();
        });
    }

    public void Import()
    {
        ShowLoading();
        External(PrivateKeyImportPanel.text, PasswordImportPanel.text, (success, address, ticket, error) =>
        {
            if (success)
            {
                Log($"Successfully imported and logged in wallet {address}");
                WalletConnectedEvent?.Invoke(this, new WalletConnectedEventArgs(address, ticket));
            }
            else
            {
                Log($"Failed to import wallet.");
            }
            HideLoading();
        });
    }

    public void Balance()
    {
        Balance(config.GetRpc("TestRpc"), (success, balance, balanceString, error) =>
        {
            if (success)
            {
                Log($"Balance: {balance}");
                BalanceText.text = $"BAL: {balanceString}";
            }
            else
            {
                Log($"Failed to get balance.");
            }
        }, currentTicket);
    }

    public void GetNfts()
    {
        ShowLoading();
        Read(config.GetRpc("TestRpc"), EktishafNftCollection.Address, EktishafNftCollection.getNfts_0_, (success, JsonObject, error) =>
        {
            if (success)
            {
                Log(JsonObject["data"].ToString());
                JArray JsonArray = JArray.FromObject(JsonObject["data"]);
                List<List<string>> nftList = JsonConvert.DeserializeObject<List<List<string>>>(JsonArray.ToString());
                GrabNfts(nftList);
            }
            else
            {
                Log("Couldn't get data: " + error);
            }
        }, currentTicket);
    }

    public void MintBatch(string to, int[] ids, int[] amounts, string[] uris)
    {
        if (ids.Length != amounts.Length || ids.Length != uris.Length)
        {
            Log("Incorrect mint parameters");
            return;
        }
        
        Write(config.GetRpc("TestRpc"), EktishafNftCollection.Address, EktishafNftCollection.mintBatch_4_Address_Uint256Array_Uint256Array_StringArray, (success, JsonObject, error) =>
        {
            if (success)
            {
                Log(JsonObject["data"].ToString());
            }
            else
            {
                Log("Couldn't write data: " + error);
            }
        }, currentTicket, new object[] { to, ids, amounts, uris });
    }
    #endregion
}

public class WalletConnectedEventArgs : EventArgs
{
    public string address;
    public string ticket;

    public WalletConnectedEventArgs(string address, string ticket)
    {
        this.address = address;
        this.ticket = ticket;
    }
}

public delegate void WalletConnectedEventHandler(object sender, WalletConnectedEventArgs e);