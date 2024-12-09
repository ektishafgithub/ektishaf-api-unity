using TMPro;
using UnityEngine;

public class DropDownHelper : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    public GameObject template;
    public GameObject content;
    public GameObject item;

    public string GetSelectedAddress()
    {
        if (dropdown.options.Count <= 0) return "";
        return dropdown.options[dropdown.value].text;
    }

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        Refresh();
        Resize();
    }

    public void Refresh()
    {
        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = BlockchainServiceUnity.Singleton.Config.TestWalletAddress.ToUpper() });
    }

    public void Resize()
    {
        Vector2 sizeDelta = template.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = sizeDelta.y * dropdown.options.Count;
        template.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}