using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
struct SavedWallet
{
    public string Address;
    public string Ticket;
}
public class DropDownHelper : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    public GameObject template;
    public GameObject content;
    public GameObject item;

    private List<List<string>> wallets;

    public static string SelectedAddress;

    private void Awake()
    {
        wallets = new List<List<string>>();
        wallets.Add(new List<string>() { Menu.Address, Menu.Ticket });

        if (GetComponent<TMP_Dropdown>())
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener(OnValueChanged);
        Refresh();
        Resize();
    }

    public void Refresh()
    {
        foreach (var wallet in wallets)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = wallet[0] });
        }
    }

    public void Resize()
    {
        Vector2 sizeDelta = template.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = sizeDelta.y * 2;// dropdown.options.Count;
        template.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }

    public void OnValueChanged(int index)
    {
        SelectedAddress = dropdown.options[index].text;
    }
}
