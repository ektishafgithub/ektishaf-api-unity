using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class NftUI : MonoBehaviour
{
    public bool isDownloaded;
    public string uri;

    public void GetRawImage()
    {
        BlockchainServiceUnity.Singleton.GetRequest(uri, (success, result, error) =>
        {
            if (success)
            {
                JObject JsonObject = JObject.Parse(result);
                BlockchainServiceUnity.Singleton.GetTexture(JsonObject["image"].ToString(), (success, texture, error) =>
                {
                    GetComponent<RawImage>().texture = texture;
                    isDownloaded = true;
                });
            }
        });
    }
}