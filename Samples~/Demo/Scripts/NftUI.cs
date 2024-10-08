using UnityEngine;
using UnityEngine.UI;
using Ektishaf;
using Newtonsoft.Json.Linq;

public class NftUI : MonoBehaviour
{
    public GameObject content;

    public void AddNFT(int id, int amount, string uri)
    {
        GameObject item = new GameObject(id.ToString());
        item.AddComponent<CanvasRenderer>();
        LayoutElement layoutElement = item.AddComponent<LayoutElement>();
        layoutElement.minWidth = 720;
        layoutElement.minHeight = 405;
        layoutElement.preferredWidth = 720;
        layoutElement.preferredHeight = 405;
        RawImage image = item.AddComponent<RawImage>();

        GameObject label = new GameObject("Label");
        TMPro.TextMeshProUGUI textMesh = label.AddComponent<TMPro.TextMeshProUGUI>();
        textMesh.text = $"Id: {id}, Amount: {amount}";

        label.transform.SetParent(item.transform);
        label.transform.localScale = Vector3.one;

        item.transform.SetParent(content.transform);
        item.transform.localScale = Vector3.one;

        GetRawImage(uri, image);
    }

    private void GetRawImage(string uri, RawImage image)
    {
        RequestManager.Singleton.GetRequest(uri, (success, result, error) =>
        {
            if (success)
            {
                JObject JsonObject = JObject.Parse(result);
                RequestManager.Singleton.GetTexture(JsonObject["image"].ToString(), (success, texture, error) =>
                {
                    image.texture = texture;
                });
            }
        });
    }
}