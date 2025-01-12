using System;
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
    public Button RegisterButton;
    public Button RegisterSubmitButton;
    public Button LoginButton;
    public Button LoginSubmitButton;
    public Button ImportButton;
    public Button ImportSubmitButton;
    public Button AccountWalletButton;
    public Button AccountSendButton;
    public Button AccountToggleButton;
    public Button AccountContinueButton;
    public Button AccountNftButton;
    public Button AccountMarketButton;
    public Button BackButton;
    public TextMeshProUGUI AddressText;
    public TextMeshProUGUI BalanceText;
    public TextMeshProUGUI MessageNftPanel;
    public TextMeshProUGUI MessageMarketPanel;
    public TMP_InputField RegisterPasswordInputField;
    public TMP_InputField LoginPasswordInputField;
    public TMP_InputField ImportPrivateKeyInputField;
    public TMP_InputField ImportPasswordInputField;
    public TMP_InputField AccountToAddressInputField;
    public TMP_InputField AccountToAmountInputField;
    public TMP_Dropdown LoginAddressDropdown;
    public TMP_Dropdown AccountToAddressDropdown;
    public GameObject PanelLoading;
    public GameObject NftContent;
    public GameObject ListingsContent;
    public GameObject Prefab;

    private List<GameObject> nfts;
    private List<GameObject> listings;
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
        listings = new List<GameObject>();
        Init();
        RefreshAddressDropDowns();
        BindListeners();
        BackButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        WalletConnectedEvent -= Event_WalletConnected;
    }

    private void OnDestroy()
    {
        UnbindListeners();
    }
    #endregion

    #region UI Methods
    public void ShowLoading(bool autoHide = true)
    {
        if(IsInvoking(nameof(HideLoading)))
        {
            CancelInvoke(nameof(HideLoading));
        }
        PanelLoading.SetActive(true);

        if (autoHide)
        {
            Invoke(nameof(HideLoading), 10f);
        }
    }

    public void HideLoading()
    {
        PanelLoading.SetActive(false);
    }

    public void RefreshAddressDropDowns()
    {
        LoginAddressDropdown.options.Clear();
        AccountToAddressDropdown.options.Clear();
        foreach (EktishafAccount account in Config.Accounts)
        {
            LoginAddressDropdown.options.Add(new TMP_Dropdown.OptionData() { text = account.Address.ToUpper() });
            AccountToAddressDropdown.options.Add(new TMP_Dropdown.OptionData() { text = account.Address.ToUpper() });
        }
    }

    public void Reset()
    {
        CurrentAccount = new EktishafAccount();
        ClearNfts();
        ClearListings();
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

    public void ClearListings()
    {
        if (ListingsContent.transform.childCount > 0)
        {
            for (int i = 0; i < ListingsContent.transform.childCount; i++)
            {
                Destroy(ListingsContent.transform.GetChild(i).gameObject);
            }
        }
        listings.Clear();
    }

    private void AddNft(List<string> nft)
    {
        GameObject item = Instantiate(Prefab);
        item.GetComponent<EktishafNftUI>().Init(nft);
        item.transform.SetParent(NftContent.transform);
        item.transform.localScale = Vector3.one;
        nfts.Add(item);
    }

    private void AddListing(List<string> nft)
    {
        GameObject item = Instantiate(Prefab);
        item.GetComponent<EktishafNftUI>().Init(nft);
        item.transform.SetParent(ListingsContent.transform);
        item.transform.localScale = Vector3.one;
        listings.Add(item);
    }

    private void AddNfts(List<List<string>> _nfts)
    {
        if (_nfts.Count > 0)
        {
            foreach (var nft in _nfts)
            {
                if (EktishafNft.HasZeroTokens(nft) || EktishafNft.IsListForSale(nft)) continue;
                AddNft(nft);
            }
        }
        MessageNftPanel.gameObject.SetActive(nfts.Count <= 0 ? true : false);
    }

    private void AddListings(List<List<string>> _nfts)
    {
        if (_nfts.Count > 0)
        {
            foreach (var nft in _nfts)
            {
                if (EktishafNft.HasZeroTokens(nft) || !EktishafNft.IsListForSale(nft)) continue;
                AddListing(nft);
            }
        }
        MessageMarketPanel.gameObject.SetActive(listings.Count <= 0 ? true : false);
    }

    private void DownloadNfts()
    {
        if (nfts.Count > 0)
        {
            foreach (GameObject nft in nfts)
            {
                EktishafNftUI nftUI = nft.GetComponent<EktishafNftUI>();
                if (!nftUI.IsDownloaded)
                {
                    nftUI.Download();
                }
            }
        }
    }

    private void DownloadListings()
    {
        if (listings.Count > 0)
        {
            foreach (GameObject listing in listings)
            {
                EktishafNftUI nftUI = listing.GetComponent<EktishafNftUI>();
                if (!nftUI.IsDownloaded)
                {
                    nftUI.Download();
                }
            }
        }
    }

    private void GrabNfts(List<List<string>> _nfts)
    {
        ClearNfts();
        AddNfts(_nfts);
        DownloadNfts();
    }

    private void GrabListings(List<List<string>> _nfts)
    {
        ClearListings();
        AddListings(_nfts);
        DownloadListings();
    }
    #endregion

    #region Events Handling
    private void Event_WalletConnected(object sender, WalletConnectedEventArgs e)
    {
        EktishafAccount account = new EktishafAccount();
        account.Address = e.address;
        account.Ticket = e.ticket;
        CurrentAccount = account;
        if (!Config.HasAccount(CurrentAccount.Address))
        {
            Config.Accounts.Add(CurrentAccount);
        }
        
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
        if (!EktishafNetwork.IsValid(CurrentNetwork)) return;

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
        if (!EktishafNetwork.IsValid(CurrentNetwork)) return;

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
        if (!EktishafNetwork.IsValid(CurrentNetwork)) return;

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

    [ContextMenu("Reveal Wallet UnitTest")]
    public void Reveal()
    {
        if (!EktishafNetwork.IsValid(CurrentNetwork)) return;

        ShowLoading();
        Reveal(CurrentAccount.Ticket, "password", (success, address, publicKey, privateKey, phrase, error) =>
        {
            if (success)
            {
                Log($"Wallet revealed: Address: {address}, PrivateKey: {privateKey}");
            }
            else
            {
                Log($"Failed to reveal wallet.");
            }
            HideLoading();
        });
    }

    public void Balance()
    {
        Balance(CurrentNetwork.Rpc, CurrentAccount.Address, (success, balance, error) =>
        {
            if (success)
            {
                Log($"Balance: {balance}");
                BalanceText.text = $"{balance} {CurrentNetwork.CurrencySymbol}";
            }
            else
            {
                Log($"Failed to get balance.");
            }
        });
    }

    public void GetNfts(int index)
    {
        ShowLoading();
        Read(CurrentNetwork.Rpc, DemoContract.Address, DemoContract.getNfts_1_Uint256, (success, JsonObject, error) =>
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
            HideLoading();
        }, CurrentAccount.Ticket, new object[] { index });
    }

    public void GetListings(int index)
    {
        ShowLoading();
        Read(CurrentNetwork.Rpc, DemoContract.Address, DemoContract.getListings_1_Uint256, (success, JsonObject, error) =>
        {
            if (success)
            {
                Log(JsonObject["data"].ToString());
                JArray JsonArray = JArray.FromObject(JsonObject["data"]);
                List<List<string>> nftList = JsonConvert.DeserializeObject<List<List<string>>>(JsonArray.ToString());
                GrabListings(nftList);
            }
            else
            {
                Log("Couldn't get data: " + error);
            }
            HideLoading();
        }, CurrentAccount.Ticket, new object[] { index });
    }

    public void Mint(string to, int id, int amount, string uri)
    {
        if (!EktishafAccount.IsValid(to))
        {
            Log($"Address {to} is not a valid address.");
            return;
        }

        if (id == 0 || amount == 0 || uri.Length == 0)
        {
            Log("Can not mint with zero or empty values.");
            return;
        }

        Write(CurrentNetwork.Rpc, DemoContract.Address, DemoContract.mint_4_Address_Uint256_Uint256_String, (success, JsonObject, error) =>
        {
            if (success)
            {
                Log(JsonObject["data"].ToString());
            }
            else
            {
                Log("Couldn't write data: " + error);
            }
        }, CurrentAccount.Ticket, new object[] { to, id, amount, uri });
    }

    public void MintBatch(string to, int[] ids, int[] amounts, string[] uris)
    {
        if (!EktishafAccount.IsValid(to))
        {
            Log($"Address {to} is not a valid address.");
            return;
        }

        if (ids.Length != amounts.Length || ids.Length != uris.Length)
        {
            Log("Incorrect mint parameters");
            return;
        }

        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] == 0 || amounts[i] == 0 || uris[i].Length == 0)
            {
                Log("Can not mint with zero or empty values.");
                return;
            }
        }

        Write(CurrentNetwork.Rpc, DemoContract.Address, DemoContract.mintBatch_4_Address_Uint256Array_Uint256Array_StringArray, (success, JsonObject, error) =>
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

    public void Sell(int id, int listAmount, System.Numerics.BigInteger listPrice)
    {
        ShowLoading(false);
        Write(CurrentNetwork.Rpc, DemoContract.Address, DemoContract.sell_3_Uint256_Uint256_Uint256, (bool success, JObject jsonObject, string error) =>
            {
                if (success)
                {
                    GetNfts(0);

                }
                else
                {
                    HideLoading();
                }
            },
            CurrentAccount.Ticket, new object[] { id, listAmount, listPrice });
    }

    public void Unlist(int id)
    {
        ShowLoading(false);
        Write(CurrentNetwork.Rpc, DemoContract.Address, DemoContract.unlist_1_Uint256, (bool success, JObject jsonObject, string error) =>
            {
                if (success)
                {
                    GetListings(0);
                }
                else
                {
                    HideLoading();
                }
            },
            CurrentAccount.Ticket, new object[] { id });
    }

    public void Buy(string seller, int id, System.Numerics.BigInteger listPrice)
    {
        ShowLoading(false);
        Write(CurrentNetwork.Rpc, DemoContract.Address, DemoContract.buy_2_Address_Uint256, (bool success, JObject jsonObject, string error) =>
            {
                if (success)
                {
                    GetListings(0);

                }
                else
                {
                    HideLoading();
                }
            },
            CurrentAccount.Ticket, EktishafMathHelper.ParseWei(listPrice).TrimEnd('0'), new object[] { seller, id });
    }
    #endregion

    #region Bind/Unbind Events
    private void BindButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button)
        {
            button.onClick.AddListener(action);
        }
    }
    private void UnbindButton(Button button)
    {
        if (button)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void BindListeners()
    {
        BindButton(RegisterButton, () =>
        {
            PanelWidgetSwitcher.SetActiveWidgetIndex(1);
            BackButton.gameObject.SetActive(true);
        });

        BindButton(RegisterSubmitButton, () =>
        {
            Register();
        });

        BindButton(LoginButton, () =>
        {
            RefreshAddressDropDowns();
            PanelWidgetSwitcher.SetActiveWidgetIndex(3);
            BackButton.gameObject.SetActive(true);
        });

        BindButton(LoginSubmitButton, () =>
        {
            Login();
        });

        BindButton(ImportButton, () =>
        {
            PanelWidgetSwitcher.SetActiveWidgetIndex(2);
            BackButton.gameObject.SetActive(true);
        });

        BindButton(ImportSubmitButton, () =>
        {
            Import();
        });

        BindButton(AccountWalletButton, () =>
        {
            TabWidgetSwitcher.SetActiveWidgetIndex(0); Balance();
        });

        BindButton(AccountSendButton, () =>
        {
            TabWidgetSwitcher.SetActiveWidgetIndex(2);
        });

        BindButton(AccountToggleButton, () =>
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

        BindButton(AccountContinueButton, () =>
        {
            ShowLoading();
            string to = AccountToAddressDropdown.gameObject.activeSelf ? AccountToAddressDropdown.options[AccountToAddressDropdown.value].text : AccountToAddressInputField.text;
            SendEther(CurrentNetwork.Rpc, to.ToLower(), AccountToAmountInputField.text, CurrentAccount.Ticket, (success, jsonObject, error) =>
            {
                HideLoading();
                TabWidgetSwitcher.SetActiveWidgetIndex(0);
                Balance();
            });
        });

        BindButton(AccountNftButton, () =>
        {
            TabWidgetSwitcher.SetActiveWidgetIndex(1); GetNfts(0);
        });

        BindButton(AccountMarketButton, () =>
        {
            TabWidgetSwitcher.SetActiveWidgetIndex(3); GetListings(0);
        });

        BindButton(BackButton, () =>
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
                ClearListings();
            }
        });
    }

    private void UnbindListeners()
    {
        UnbindButton(RegisterButton);
        UnbindButton(RegisterSubmitButton);
        UnbindButton(LoginButton);
        UnbindButton(LoginSubmitButton);
        UnbindButton(ImportButton);
        UnbindButton(ImportSubmitButton);
        UnbindButton(AccountWalletButton);
        UnbindButton(AccountSendButton);
        UnbindButton(AccountToggleButton);
        UnbindButton(AccountContinueButton);
        UnbindButton(AccountNftButton);
        UnbindButton(AccountMarketButton);
        UnbindButton(BackButton);
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