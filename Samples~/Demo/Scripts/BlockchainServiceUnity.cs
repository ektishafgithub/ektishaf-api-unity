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

    public WidgetSwitcher PanelWidgetSwitcher;
    public WidgetSwitcher TabWidgetSwitcher;

    public TextMeshProUGUI AddressText;
    public TextMeshProUGUI BalanceText;

    public Button RegisterButton;
    public Button RegisterSubmitButton;
    public TMP_InputField RegisterPasswordInputField;

    public Button LoginButton;
    public Button LoginSubmitButton;
    public TMP_Dropdown LoginAddressDropdown;
    public TMP_InputField LoginPasswordInputField;

    public Button ImportButton;
    public Button ImportSubmitButton;
    public TMP_InputField ImportPrivateKeyInputField;
    public TMP_InputField ImportPasswordInputField;

    public Button AccountWalletButton;
    public Button AccountSendButton;
    public Button AccountToggleButton;
    public Button AccountContinueButton;
    public TMP_Dropdown AccountToAddressDropdown;
    public TMP_InputField AccountToAddressInputField;
    public TMP_InputField AccountToAmountInputField;

    public Button AccountNftButton;
    public Button NftToggleButton;
    public Button NftContinueButton;
    public TMP_Dropdown NftToAddressDropdown;
    public TMP_InputField NftToAddressInputField;
    public TMP_InputField NftToAmountInputField;

    public Button BackButton;
    public GameObject PanelLoading;
    public GameObject NftContent;

    private List<GameObject> nfts;
    private int SelectedItem;
    #endregion

    #region Events Declaration
    public event EventHandler ServiceInitialized;
    public event WalletConnectedEventHandler WalletConnectedEvent;
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
        
        foreach (EktishafAccount account in Config.Accounts)
        {
            LoginAddressDropdown.options.Add(new TMP_Dropdown.OptionData() { text = account.Address.ToUpper() });
            AccountToAddressDropdown.options.Add(new TMP_Dropdown.OptionData() { text = account.Address.ToUpper() });
            NftToAddressDropdown.options.Add(new TMP_Dropdown.OptionData() { text = account.Address.ToUpper() });
        }

        if(RegisterButton)
        {
            RegisterButton.onClick.AddListener(() => { PanelWidgetSwitcher.SetActiveWidgetIndex(1); BackButton.gameObject.SetActive(true); });
        }
        if (RegisterSubmitButton)
        {
            RegisterSubmitButton.onClick.AddListener(() => { Register(); });
        }
        if (LoginButton)
        {
            LoginButton.onClick.AddListener(() => { PanelWidgetSwitcher.SetActiveWidgetIndex(3); BackButton.gameObject.SetActive(true); });
        }
        if (LoginSubmitButton)
        {
            LoginSubmitButton.onClick.AddListener(() => { Login(); });
        }
        if (ImportButton)
        {
            ImportButton.onClick.AddListener(() => { PanelWidgetSwitcher.SetActiveWidgetIndex(2); BackButton.gameObject.SetActive(true); });
        }
        if (ImportSubmitButton)
        {
            ImportSubmitButton.onClick.AddListener(() => { Import(); });
        }
        if (AccountWalletButton)
        {
            AccountWalletButton.onClick.AddListener(() => { TabWidgetSwitcher.SetActiveWidgetIndex(0); Balance(); });
        }
        if (AccountSendButton)
        {
            AccountSendButton.onClick.AddListener(() => { TabWidgetSwitcher.SetActiveWidgetIndex(2); });
        }
        if (AccountToggleButton)
        {
            AccountToggleButton.onClick.AddListener(() => 
            {
                if (AccountToAddressDropdown.gameObject.activeSelf)
                {
                    AccountToAddressDropdown.gameObject.SetActive(false);
                    AccountToAddressInputField.gameObject.SetActive(true);
                }
                else if (AccountToAddressInputField.gameObject.activeSelf)
                {
                    AccountToAddressDropdown.gameObject.SetActive(true);
                    AccountToAddressInputField.gameObject.SetActive(false);
                }
            });
        }
        if (AccountContinueButton)
        {
            AccountContinueButton.onClick.AddListener(() => 
            {
                ShowLoading();
                string to = AccountToAddressDropdown.gameObject.activeSelf ? AccountToAddressDropdown.options[AccountToAddressDropdown.value].text : AccountToAddressInputField.text;
                Send(CurrentNetwork.Rpc, to.ToLower(), AccountToAmountInputField.text, CurrentAccount.Ticket, (success, jsonObject, error) =>
                {
                    HideLoading();
                    TabWidgetSwitcher.SetActiveWidgetIndex(0);
                    Balance();
                });
            });
        }
        if (AccountNftButton)
        {
            AccountNftButton.onClick.AddListener(() => { TabWidgetSwitcher.SetActiveWidgetIndex(1); GetNfts(); });
        }
        if (NftToggleButton)
        {
            NftToggleButton.onClick.AddListener(() => 
            {
                if (NftToAddressDropdown.gameObject.activeSelf)
                {
                    NftToAddressDropdown.gameObject.SetActive(false);
                    NftToAddressInputField.gameObject.SetActive(true);
                }
                else if (NftToAddressInputField.gameObject.activeSelf)
                {
                    NftToAddressDropdown.gameObject.SetActive(true);
                    NftToAddressInputField.gameObject.SetActive(false);
                }
            });
        }
        if (NftContinueButton)
        {
            NftContinueButton.onClick.AddListener(() => 
            {
                ShowLoading();
                string to = NftToAddressDropdown.gameObject.activeSelf ? NftToAddressDropdown.options[NftToAddressDropdown.value].text : NftToAddressInputField.text;
                Write(CurrentNetwork.Rpc, EktishafNftCollection.Address, EktishafNftCollection.safeTransferFrom_5_Address_Address_Uint256_Uint256_Bytes, (success, JsonObject, error) =>
                {
                    if(success)
                    {
                        HideLoading();
                        TabWidgetSwitcher.SetActiveWidgetIndex(1);
                        GetNfts();
                    }
                }, CurrentAccount.Ticket, new object[] { CurrentAccount.Address.ToLower(), to.ToLower(), 0, NftToAmountInputField.text, "0x"});

            });
        }
        if(BackButton)
        {
            BackButton.gameObject.SetActive(false);
            BackButton.onClick.AddListener(() => 
            {
                if (PanelWidgetSwitcher.ActiveWidgetIndex == 4 && TabWidgetSwitcher.ActiveWidgetIndex != 0)
                {
                    TabWidgetSwitcher.SetActiveWidgetIndex(0);
                    Balance();
                }
                else
                {
                    BackButton.gameObject.SetActive(false);
                    PanelWidgetSwitcher.SetActiveWidgetIndex(0);

                    AddressText.text = "0x";
                    BalanceText.text = $"0 {CurrentNetwork.CurrencySymbol}";

                    ClearNfts();
                }
            });
        }
    }

    private void OnDisable()
    {
        WalletConnectedEvent -= Event_WalletConnected;
    }
    #endregion

    #region UI Methods
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
        CurrentAccount = new EktishafAccount();
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

        RawImage image = nftItem.AddComponent<RawImage>();

        NftUI nftUI = nftItem.AddComponent<NftUI>();
        nftUI.uri = uri;
        nftUI.id = id;
        nftUI.amount = amount;

        Button optionButton = nftItem.AddComponent<Button>();
        optionButton.onClick.AddListener(() =>
        {
            SelectedItem = id;
            TabWidgetSwitcher.SetActiveWidgetIndex(3);
        });

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
        ClearNfts();
        AddNfts(_nfts);
        DownloadNfts();
        WaitForPendingNfts();
    }
    #endregion

    #region Events Handling
    private void Event_WalletConnected(object sender, WalletConnectedEventArgs e)
    {
        EktishafAccount account = new EktishafAccount();
        account.Address = e.address;
        account.Ticket = e.ticket;
        CurrentAccount = account;

        AddressText.text = $"{e.address.Substring(0, 5)}...{e.address.Substring(e.address.Length - 5, 5)}";
        BalanceText.text = $"0 {CurrentNetwork.CurrencySymbol}";
        PanelWidgetSwitcher.SetActiveWidgetIndex(4);
        TabWidgetSwitcher.SetActiveWidgetIndex(0);

        Balance();
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
        Register(RegisterPasswordInputField.text, (success, address, ticket, error) =>
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
        EktishafAccount account = Config.GetAccount(LoginAddressDropdown.options[LoginAddressDropdown.value].text);
        ShowLoading();
        Login(account.Ticket, LoginPasswordInputField.text, (success, address, ticket, error) =>
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
        External(ImportPrivateKeyInputField.text, ImportPasswordInputField.text, (success, address, ticket, error) =>
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
        Balance(CurrentNetwork.Rpc, CurrentAccount.Address, (success, balance, balanceString, error) =>
        {
            if (success)
            {
                Log($"Balance: {balance}");
                BalanceText.text = $"{balanceString} {CurrentNetwork.CurrencySymbol}";
            }
            else
            {
                Log($"Failed to get balance.");
            }
        });
    }

    public void GetNfts()
    {
        ShowLoading();
        Read(CurrentNetwork.Rpc, EktishafNftCollection.Address, EktishafNftCollection.getNfts_0_, (success, JsonObject, error) =>
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
        }, CurrentAccount.Ticket);
    }

    public void MintBatch(string to, int[] ids, int[] amounts, string[] uris)
    {
        if (ids.Length != amounts.Length || ids.Length != uris.Length)
        {
            Log("Incorrect mint parameters");
            return;
        }
        
        Write(CurrentNetwork.Rpc, EktishafNftCollection.Address, EktishafNftCollection.mintBatch_4_Address_Uint256Array_Uint256Array_StringArray, (success, JsonObject, error) =>
        {
            if (success)
            {
                Log(JsonObject["data"].ToString());
            }
            else
            {
                Log("Couldn't write data: " + error);
            }
        }, CurrentAccount.Ticket, new object[] { to, ids, amounts, uris });
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