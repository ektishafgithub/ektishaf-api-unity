using TMPro;
using UnityEngine;

public class DropDownHelper : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    public GameObject template;
    public GameObject content;
    public GameObject item;

    public static string SelectedAddress;

    private void Awake()
    {
        if (GetComponent<TMP_Dropdown>())
            dropdown = GetComponent<TMP_Dropdown>();
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener((index) => {
            if (index == 0) return;
            SelectedAddress = dropdown.options[index].text;
        });
        Refresh();
        Resize();
    }

    public void Refresh()
    {
        dropdown.options.Clear();
        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = "CHOOSE YOUR WALLET ..." });
        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = Menu.SampleAddress.ToUpper() });
    }

    public void Resize()
    {
        Vector2 sizeDelta = template.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.y = sizeDelta.y * dropdown.options.Count;
        template.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        content.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }
}