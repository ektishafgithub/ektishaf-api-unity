using System.Collections.Generic;
using UnityEngine;
using Ektishaf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using System;

public class Menu : MonoBehaviour
{
    public const string SampleAddress = "0xf0deb67ec9064794211e14938c639728bda2481a";
    public const string SampleTicket = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcm9qZWN0IjoiZ29yIiwicGFzc3dvcmQiOiJFa3Rpc2hhZiBBaHdheiIsImVuY3J5cHRpb24iOiJ7XCJhZGRyZXNzXCI6XCJmMGRlYjY3ZWM5MDY0Nzk0MjExZTE0OTM4YzYzOTcyOGJkYTI0ODFhXCIsXCJpZFwiOlwiZmZkYWVjOTUtOThjMi00YTdiLWI1YzItYWQ0YzZkYjNkZWY5XCIsXCJ2ZXJzaW9uXCI6MyxcIkNyeXB0b1wiOntcImNpcGhlclwiOlwiYWVzLTEyOC1jdHJcIixcImNpcGhlcnBhcmFtc1wiOntcIml2XCI6XCIyMWIyODY2ZjdlMjJkMTc2MjRhMTM4YWY2NDhmM2EzN1wifSxcImNpcGhlcnRleHRcIjpcImQ1NzFjOWZjNGRkN2Q5NGE2MGRkMjIwYzY0ZWRjZDlmZGQyNGYxMmU3ODVkZDEzM2Q0ZWI4OTc1OWM0OTFlNmRcIixcImtkZlwiOlwic2NyeXB0XCIsXCJrZGZwYXJhbXNcIjp7XCJzYWx0XCI6XCI3NmVjNzcxMDc2NTc0OTkzOWY3ZDliOTM0MjM3YTFlY2RhYjNiMzYzNDI3ZDk5OGY3OGFmMTdiNTMxYzVmODIwXCIsXCJuXCI6MTMxMDcyLFwiZGtsZW5cIjozMixcInBcIjoxLFwiclwiOjh9LFwibWFjXCI6XCI2ZGYyNTMwODQ4ZWY1MjI3MjNmYzU5MWJkZjVmYzU1ZmI3MTg0ODFiZjA2MzRkYWQ1Y2M4MWE5ZTlmMDZiN2FmXCJ9LFwieC1ldGhlcnNcIjp7XCJjbGllbnRcIjpcImV0aGVycy82LjEzLjJcIixcImdldGhGaWxlbmFtZVwiOlwiVVRDLS0yMDI0LTA4LTIzVDExLTQzLTAwLjBaLS1mMGRlYjY3ZWM5MDY0Nzk0MjExZTE0OTM4YzYzOTcyOGJkYTI0ODFhXCIsXCJwYXRoXCI6XCJtLzQ0Jy82MCcvMCcvMC8wXCIsXCJsb2NhbGVcIjpcImVuXCIsXCJtbmVtb25pY0NvdW50ZXJcIjpcImVkOGI4OTllNWVhMzU0ZDc5MGRmMDAxOGRlMzc3M2NmXCIsXCJtbmVtb25pY0NpcGhlcnRleHRcIjpcImU1NDZjNTBlYzVhZTcxNzlmNDMyNDFkYzFiZjJhM2I4XCIsXCJ2ZXJzaW9uXCI6XCIwLjFcIn19IiwiaWF0IjoxNzI0ODE5NzM5LCJleHAiOjE3MjQ5MDYxMzl9.2QPVZ6t_AHWKDdPTzj25Fj41dMzF764CagpFWKFGPuA";
    public static string Address = SampleAddress;
    public static string Ticket = SampleTicket;
    public string Rpc = "https://eth-sepolia.g.alchemy.com/v2/YBy3ka0SJ5YW7aGgnz7oj-U_QUJ_pND4";
    private string PinataGatewayUrl = "https://azure-elaborate-quelea-290.mypinata.cloud/ipfs";
    private string MetadataHash = "QmRE6YQTpQxu725mXr5usqVGfcwGZFJrxDGjnZgCr7Ruso";

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

    public event WalletConnectedEventHandler WalletConnectedEvent;

    private void OnEnable()
    {
        WalletConnectedEvent += Menu_WalletConnectedEvent;
    }

    private void OnDisable()
    {
        WalletConnectedEvent -= Menu_WalletConnectedEvent;
    }

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
        Address = SampleAddress;
        Ticket = SampleTicket;
        FindObjectOfType<NftUI>().Clear();
        BalanceText.gameObject.SetActive(false);
    }

    private void Menu_WalletConnectedEvent(object sender, WalletConnectedEventArgs e)
    {
        Address = e.address;
        Ticket = e.ticket;

        Balance();

        HideAllPanels();
        PanelNFT.SetActive(true);
        BalanceText.gameObject.SetActive(true);
    }

    public void Register()
    {
        ShowLoading();
        BlockchainService.Singleton.Register(PasswordRegisterPanel.text, (success, address, ticket, error) =>
        {
            if (success)
            {
                Debug.Log($"Successfully registered a new wallet {address}");
                WalletConnectedEvent?.Invoke(this, new WalletConnectedEventArgs(address, ticket));
            }
            else
            {
                Debug.Log($"Failed to register a new wallet.");
            }
            HideLoading();
        });
    }

    public void Login()
    {
        ShowLoading();
        BlockchainService.Singleton.Login(Ticket, PasswordLoginPanel.text, (success, address, ticket, error) =>
        {
            if (success)
            {
                Debug.Log($"Successfully Logged in wallet {address}");
                WalletConnectedEvent?.Invoke(this, new WalletConnectedEventArgs(address, ticket));
            }
            else
            {
                Debug.Log($"Failed to Login wallet.");
            }
            HideLoading();
        });
    }

    public void Import()
    {
        ShowLoading();
        BlockchainService.Singleton.External(PrivateKeyImportPanel.text, PasswordImportPanel.text, (success, address, ticket, error) =>
        {
            if (success)
            {
                Debug.Log($"Successfully Logged in wallet {address}");
                WalletConnectedEvent?.Invoke(this, new WalletConnectedEventArgs(address, ticket));
            }
            else
            {
                Debug.Log($"Failed to Login wallet.");
            }
            HideLoading();
        });
    }

    public void Balance()
    {
        BlockchainService.Singleton.Balance(Rpc, (success, balance, balanceString, error) =>
        {
            if (success)
            {
                Debug.Log($"Balance: {balance}");
                BalanceText.text = $"BAL: {balanceString}";
            }
            else
            {
                Debug.Log($"Failed to get balance.");
            }
        }, Ticket);

    }

    public void GetNfts()
    {
        ShowLoading();
        BlockchainService.Singleton.Read(Rpc, EktishafNftCollection.Address, @$"[""{EktishafNftCollection.getNfts_0_}""]", "getNfts", (success, JsonObject, error) =>
        {
            if (success)
            {
                Debug.Log(JsonObject["data"].ToString());
                JArray JsonArray = JArray.FromObject(JsonObject["data"]);
                List<List<string>> nfts = JsonConvert.DeserializeObject<List<List<string>>>(JsonArray.ToString());

                Debug.Log($"Number of NFTs: {nfts.Count}");
                NftUI nftUI = FindObjectOfType<NftUI>();
                foreach (var nft in nfts)
                {
                    nftUI.AddNFT(int.Parse(nft[0]), int.Parse(nft[1]), nft[2]);
                }
            }
            else
            {
                Debug.Log("Couldn't get data: " + error);
            }
        }, Ticket);
    }

    public void MintBatch(string to, int[] ids, int[] amounts, string[] uris)
    {
        if (ids.Length != amounts.Length || ids.Length != uris.Length)
        {
            Debug.Log("Incorrect mint parameters");
            return;
        }
        BlockchainService.Singleton.Write(Rpc, EktishafNftCollection.Address, @$"[""{EktishafNftCollection.mintBatch_4_Address_Uint256Array_Uint256Array_StringArray}""]", "mintBatch", (success, JsonObject, error) =>
        {
            if (success)
            {
                Debug.Log(JsonObject["data"].ToString());
            }
            else
            {
                Debug.Log("Couldn't write data: " + error);
            }
        }, Ticket, new object[] { to, ids, amounts, uris });
    }

    public string[] GetMetadataUris(int[] ids)
    {
        string[] uris = new string[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            uris[i] = $"{PinataGatewayUrl}/{MetadataHash}/{ids[i]}.json";
        }
        return uris;
    }
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