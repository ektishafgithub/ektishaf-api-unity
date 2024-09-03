using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ektishaf
{
    public class RequestManager : MonoBehaviour
    {
        #region Constants
        public const string HostUrl = "https://api.ektishaf.com/v1";
        public const string RegisterUrl = HostUrl + "/register";
        public const string LoginUrl = HostUrl + "/login";
        public const string ExternalUrl = HostUrl + "/external";
        public const string RevealUrl = HostUrl + "/reveal";
        public const string BalanceUrl = HostUrl + "/balance";
        public const string ABIUrl = HostUrl + "/abi";
        public const string ReadUrl = HostUrl + "/read";
        public const string WriteUrl = HostUrl + "/write";
        public const string SignUrl = HostUrl + "/sign";
        public const string VerifyUrl = HostUrl + "/verify";

        public const string SAVE_KEY = "Wallets";
        #endregion

        #region Variables
        public static RequestManager Singleton = null;
        #endregion

        #region Default Methods
        private void Awake()
        {
            if (Singleton != null && Singleton != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Singleton = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private  void Start()
        {
            Host((success, result) => { Debug.Log($"{nameof(Host)} - Success: {success}, Result: {result}"); });
        }
        #endregion

        #region Request Methods
        private IEnumerator GetRequestCoroutine(string url, Action<bool, string, string> callback = null)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                bool success = (request.result == UnityWebRequest.Result.Success);
                callback?.Invoke(success, success ? request.downloadHandler.text : null, success ? null : request.error);
            }
        }

        private IEnumerator GetTextureCoroutine(string url, Action<bool, Texture, string> callback = null)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();
                bool success = (request.result == UnityWebRequest.Result.Success);
                callback?.Invoke(success, success ? ((DownloadHandlerTexture)request.downloadHandler).texture : null, success ? null : request.error);
            }
        }

        private IEnumerator PostRequestCoroutine(string url, string body, Action<bool, string, string> callback = null, string ticket = null)
        {
            using (UnityWebRequest request = UnityWebRequest.Post(url, body, "application/json"))
            {
                if (!string.IsNullOrEmpty(ticket)) { request.SetRequestHeader("Authorization", string.Format("Bearer {0}", ticket)); }
                yield return request.SendWebRequest();
                bool success = (request.result == UnityWebRequest.Result.Success);
                callback?.Invoke(success, success ? request.downloadHandler.text : null, success ? null : request.error);
            }
        }

        public void GetRequest(string url, Action<bool, string, string> callback = null)
        {
            Debug.Log($"{nameof(RequestManager)} - {nameof(GetRequest)} - Url: {url}");
            StartCoroutine(GetRequestCoroutine(url, callback));
        }

        public void GetTexture(string url, Action<bool, Texture, string> callback = null)
        {
            Debug.Log($"{nameof(RequestManager)} - {nameof(GetTexture)} - Url: {url}");
            StartCoroutine(GetTextureCoroutine(url, callback));
        }

        public void PostRequest(string url, string body, Action<bool, string, string> callback = null, string ticket = null)
        {
            Debug.Log($"{nameof(RequestManager)} - {nameof(PostRequest)} - Request: {body}");
            StartCoroutine(PostRequestCoroutine(url, body, callback, ticket));
        }

        public void Host(Action<bool, string> callback)
        {
            GetRequest(HostUrl, (success, result, error) => callback?.Invoke(success, result));
        }

        public void Register(string password, Action<bool, string, string, string> callback)
        {
            string body = CreateAuthRequest(password);
            PostRequest(RegisterUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(Register)} - Response - {result}");

                if (success)
                {
                    JObject JsonObject = JObject.Parse(result);
                    string address = JsonObject["address"]?.ToString();
                    string ticket = JsonObject["ticket"]?.ToString();
                    callback?.Invoke(true, address, ticket, null);
                }
                else
                {
                    callback?.Invoke(false, null, null, error);
                }
            });
        }

        public void Login(string ticket, string password, Action<bool, string, string, string> callback)
        {
            string body = CreateAuthRequest(password);
            PostRequest(LoginUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(Login)} - Response - {result}");

                if (success)
                {
                    JObject JsonObject = JObject.Parse(result);
                    string address = JsonObject["address"]?.ToString();
                    string ticket = JsonObject["ticket"]?.ToString();
                    callback?.Invoke(true, address, ticket, null);
                }
                else
                {
                    callback?.Invoke(false, null, null, error);
                }
            }, ticket);
        }

        public void External(string privateKey, string password, Action<bool, string, string, string> callback, string ticket)
        {
            string body = CreateExternalWalletRequest(privateKey, password);
            PostRequest(ExternalUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(External)} - Response - {result}");
                if (success)
                {
                    JObject JsonObject = JObject.Parse(result);
                    string address = JsonObject["address"]?.ToString();
                    string ticket = JsonObject["ticket"]?.ToString();
                    callback?.Invoke(true, address, ticket, null);
                }
                else
                {
                    callback?.Invoke(false, null, null, error);
                }
            }, ticket);
        }

        public void Reveal(string ticket, string password, Action<bool, string, string, string, string, string> callback)
        {
            string body = CreateAuthRequest(password);
            PostRequest(RevealUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(Reveal)} - Response - {result}");
                if (success)
                {
                    JObject JsonObject = JObject.Parse(result);
                    string address = JsonObject["address"]?.ToString();
                    string publicKey = JsonObject["publicKey"]?.ToString();
                    string privateKey = JsonObject["privateKey"]?.ToString();
                    string phrase = JsonObject["phrase"]?.ToString();
                    callback?.Invoke(true, address, publicKey, privateKey, phrase, null);
                }
                else
                {
                    callback?.Invoke(false, null, null, null, null, error);
                }
            }, ticket);
        }

        public void Sign(string message, Action<bool, string, string, string> callback, string ticket)
        {
            string body = CreateSignRequest(message);
            PostRequest(SignUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(Sign)} - Response - {result}");
                if (success && IsExecNormal(result, out JObject JsonObject))
                {
                    string message = JsonObject["message"]?.ToString();
                    string signature = JsonObject["signature"]?.ToString();
                    callback?.Invoke(true, message, signature, null);
                }
                else
                {
                    callback?.Invoke(false, null, null, error);
                }
            }, ticket);
        }

        public void Verify(string address, string message, string signature, Action<bool, bool, string> callback)
        {
            string body = CreateVerifyRequest(address, message, signature);
            PostRequest(VerifyUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(Verify)} - Response - {result}");
                if (success && IsExecNormal(result, out JObject JsonObject))
                {
                    bool verification = bool.Parse(JsonObject["verification"]?.ToString());
                    callback?.Invoke(true, verification, null);
                }
                else
                {
                    callback?.Invoke(false, false, error);
                }
            });
        }

        public void Balance(string rpc, Action<bool, BigInteger, string> callback, string ticket)
        {
            string body = CreateBalanceRequest(rpc);
            PostRequest(BalanceUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(Balance)} - Response - {result}");
                if (success && IsExecNormal(result, out JObject JsonObject))
                {
                    if (BigInteger.TryParse(ValidateString(JsonObject["data"].ToString()), out BigInteger balance))
                    {
                        callback?.Invoke(true, balance, null);
                    }
                    else
                    {
                        callback?.Invoke(true, BigInteger.Zero, null);
                    }
                }
                else
                {
                    callback?.Invoke(false, BigInteger.Zero, error);
                }
            }, ticket);
        }

        public void ABI(string abi, bool minimal, Action<bool, string[], string> callback)
        {
            string body = CreateABIRequest(abi, minimal);
            PostRequest(ABIUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(ABI)} - Response - {result}");

                if (success)
                {
                    string[] abi = JsonConvert.DeserializeObject<string[]>(result);
                    callback?.Invoke(true, abi, null);
                }
                else
                {
                    callback?.Invoke(success, null, error);
                }
            });
        }

        public void Read(string rpc, string contract, string abi, string function, Action<bool, JObject, string> callback, string ticket, params object[] args)
        {
            string body = CreateContractRequest(rpc, contract, abi, function, args);
            PostRequest(ReadUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(Read)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }

        public void Write(string rpc, string contract, string abi, string function, Action<bool, JObject, string> callback, string token, params object[] args)
        {
            string body = CreateContractRequest(rpc, contract, abi, function, args);
            PostRequest(WriteUrl, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(RequestManager)} - {nameof(Write)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, token);
        }
        #endregion

        #region Request Helper Methods
        public static string CreateBodyJson(Dictionary<string, JToken> properties)
        {
            JObject JsonObject = new JObject();
            foreach (KeyValuePair<string, JToken> entry in properties)
            {
                JsonObject.Add(entry.Key, entry.Value);
            }
            return JsonObject.ToString();
        }

        public static string CreateAuthRequest(string password)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "password", password } });
        }

        public static string CreateExternalWalletRequest(string privateKey, string password)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "privateKey", privateKey }, { "password", password } });
        }

        public static string CreateSignRequest(string message)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "message", message } });
        }

        public static string CreateVerifyRequest(string address, string message, string signature)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "address", address }, { "message", message }, { "signature", signature } });
        }

        public static string CreateABIRequest(string abi, bool minimal)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "abi", abi }, { "minimal", minimal } });
        }

        public static string CreateBalanceRequest(string rpc)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "rpc", rpc } });
        }

        public static string CreateContractRequest(string rpc, string contract, string abi, string function, params object[] args)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "rpc", rpc }, { "contract", contract }, { "abi", abi }, { "function", function }, { "params", JToken.FromObject(args) } });
        }
        #endregion

        #region Utility Methods
        private bool IsExecNormal(string result, out JObject JsonObject)
        {
            if (JObject.Parse(result)?["execution"]?.ToString() == "normal")
            {
                JsonObject = JObject.Parse(result);
                return true;
            }
            JsonObject = null;
            return false;
        }

        public string ValidateString(string value)
        {
            return value.Trim(new[] { '\'', '"' }).Replace("\\\"", "\"");
        }
        #endregion

        #region PlayerPrefs
        public static List<List<string>> LoadWallets()
        {
            return PlayerPrefs.HasKey(SAVE_KEY) ? JsonConvert.DeserializeObject<List<List<string>>>(PlayerPrefs.GetString(SAVE_KEY)) : new List<List<string>>();
        }

        public static void SaveWallet(string address, string ticket)
        {
            List<List<string>> wallets = LoadWallets();
            if (!TryUpdate(address, ticket, ref wallets))
            {
                wallets.Add(new List<string>() { address, ticket });
            }
            PlayerPrefs.SetString(SAVE_KEY, JsonConvert.SerializeObject(wallets));
        }

        public static bool TryFind(string address, List<List<string>> wallets, out int index)
        {
            for (int i = 0; i < wallets.Count; i++)
            {
                if (address == wallets[i][0])
                {
                    index = i;
                    return true;
                }
            }
            index = -1;
            return false;
        }

        public static bool TryUpdate(string address, string ticket, ref List<List<string>> wallets)
        {
            if (TryFind(address, wallets, out int index))
            {
                wallets[index][1] = ticket;
            }
            return false;
        }

        public static bool TryGetTicket(string address, out string ticket)
        {
            List<List<string>> wallets = LoadWallets();
            if (TryFind(address, wallets, out int index))
            {
                ticket = wallets[index][1];
                return true;
            }
            ticket = null;
            return false;
        }
        #endregion
    }
}