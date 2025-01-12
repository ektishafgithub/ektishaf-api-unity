using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using Ektishaf;

public class EktishafNftUI : MonoBehaviour
{
    public Button SellButton;
    public Button UnlistButton;
    public Button BuyButton;
    public Button IncreaseButton;
    public Button DecreaseButton;
    public Button SelectAmountButton;
    public TextMeshProUGUI IdAmountText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI PriceUnit;
    public TMP_InputField AmountInputField;
    public RawImage Image;
    public GameObject Loader;
    public GameObject QuantitySelector;

    private bool isDownloaded;
    private EktishafNft nft;

    public bool IsDownloaded {  get { return isDownloaded; } }
    public EktishafNft Nft { get { return nft; } }

    private void Start()
    {
        Loader.SetActive(true);
        BindListeners();
    }

    private void OnDestroy()
    {
        UnbindListeners();
    }

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
        BindButton(SellButton, OnSellButtonClicked);
        BindButton(UnlistButton, OnUnlistButtonClicked);
        BindButton(BuyButton, OnBuyButtonClicked);
        BindButton(IncreaseButton, OnIncreaseButtonClicked);
        BindButton(DecreaseButton, OnDecreaseButtonClicked);
        BindButton(SelectAmountButton, OnSelectAmountButtonClicked);
    }

    private void UnbindListeners()
    {
        UnbindButton(SellButton);
        UnbindButton(UnlistButton);
        UnbindButton(BuyButton);
        UnbindButton(IncreaseButton);
        UnbindButton(DecreaseButton);
        UnbindButton(SelectAmountButton);
    }

    public void Init(List<string> _nft)
    {
        nft = EktishafNft.FromResponse(_nft);
        Refresh();
    }

    public void Refresh()
    {
        IdAmountText.text = $"{nft.Id}/{(nft.ListForSale ? nft.ListAmount : nft.Amount)}";
        PriceText.gameObject.SetActive(nft.ListForSale ? true : false);
        PriceText.text = nft.ListForSale ? EktishafMathHelper.ParseWei(nft.ListPrice).TrimEnd('0') : "";
        PriceUnit.gameObject.SetActive(nft.ListForSale ? true : false);
        SellButton.gameObject.SetActive((!nft.ListForSale ? true : false));
        UnlistButton.gameObject.SetActive((nft.ListForSale && IsMine() ? true : false));
        BuyButton.gameObject.SetActive((nft.ListForSale && !IsMine() ? true : false));
    }

    public void Download()
    {
        BlockchainServiceUnity.Singleton.GetRequest(nft.Metadata, (success, result, error) =>
        {
            if (success)
            {
                JObject JsonObject = JObject.Parse(result);
                if (JsonObject["price"] != null)
                {
                    nft.ListPrice = EktishafMathHelper.ParseEther(JsonObject["price"]?.ToString());
                }
                BlockchainServiceUnity.Singleton.GetTexture(JsonObject["image"].ToString(), (success, texture, error) =>
                {
                    Image.texture = texture;
                    isDownloaded = true;
                    Loader.SetActive(false);
                });
            }
        });
    }

    public bool IsMine()
    {
        return BlockchainServiceUnity.Singleton.CurrentAccount.Address.ToLower() == nft.Owner.ToLower();
    }

    private void OnSellButtonClicked()
    {
        AmountInputField.text = nft.Amount.ToString();
        QuantitySelector.SetActive(true);
    }

    private void OnUnlistButtonClicked()
    {
        BlockchainServiceUnity.Singleton.Unlist(nft.Id);
    }

    private void OnBuyButtonClicked()
    {
        BlockchainServiceUnity.Singleton.Buy(nft.Owner, nft.Id, nft.ListPrice);
    }

    private void OnIncreaseButtonClicked()
    {
        int number = int.Parse(AmountInputField.text);
        number++;
        if(number >= nft.Amount)
        {
            number = nft.Amount;
        }
        AmountInputField.text = number.ToString();
    }
    private void OnDecreaseButtonClicked()
    {
        int number = int.Parse(AmountInputField.text);
        number--;
        if (number <= 1)
        {
            number = 1;
        }
        AmountInputField.text = number.ToString();
    }

    private void OnSelectAmountButtonClicked()
    {
        nft.ListAmount = int.Parse(AmountInputField.text);
        BlockchainServiceUnity.Singleton.Sell(nft.Id, nft.ListAmount, nft.ListPrice);
    }
}