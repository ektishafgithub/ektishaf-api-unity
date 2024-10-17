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
    public class BlockchainService : MonoBehaviour
    {
        #region Constants
        public const string RootService     = "https://api.ektishaf.com/v1/";
        public const string RegisterService = RootService + "register";
        public const string LoginService    = RootService + "login";
        public const string ExternalService = RootService + "external";
        public const string RevealService   = RootService + "reveal";
        public const string BalanceService  = RootService + "balance";
        public const string ABIService      = RootService + "abi";
        public const string ReadService     = RootService + "read";
        public const string WriteService    = RootService + "write";
        public const string SignService     = RootService + "sign";
        public const string VerifyService   = RootService + "verify";
        #endregion

        #region Variables
        public static BlockchainService Singleton = null;
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

        public void GetRequest(string url, Action<bool, string, string> callback = null)
        {
            Debug.Log($"{nameof(BlockchainService)} - {nameof(GetRequest)} - Url: {url}");
            StartCoroutine(GetRequestCoroutine(url, callback));
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

        public void PostRequest(string url, string body, Action<bool, string, string> callback = null, string ticket = null)
        {
            Debug.Log($"{nameof(BlockchainService)} - {nameof(PostRequest)} - Request: {body}");
            StartCoroutine(PostRequestCoroutine(url, body, callback, ticket));
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

        public void GetTexture(string url, Action<bool, Texture, string> callback = null)
        {
            Debug.Log($"{nameof(BlockchainService)} - {nameof(GetTexture)} - Url: {url}");
            StartCoroutine(GetTextureCoroutine(url, callback));
        }
        #endregion

        #region Core Methods
        public void Host(Action<bool, string> callback)
        {
            GetRequest(RootService, (success, result, error) => callback?.Invoke(success, result));
        }
        
        public void Register(string password, Action<bool, string, string, string> callback)
        {
            string body = CreateAuthRequest(password);
            PostRequest(RegisterService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(Register)} - Response - {result}");

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
            PostRequest(LoginService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(Login)} - Response - {result}");

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

        public void External(string privateKey, string password, Action<bool, string, string, string> callback)
        {
            string body = CreateExternalWalletRequest(privateKey, password);
            PostRequest(ExternalService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(External)} - Response - {result}");
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

        public void Reveal(string ticket, string password, Action<bool, string, string, string, string, string> callback)
        {
            string body = CreateAuthRequest(password);
            PostRequest(RevealService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(Reveal)} - Response - {result}");
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
            PostRequest(SignService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(Sign)} - Response - {result}");
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
            PostRequest(VerifyService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(Verify)} - Response - {result}");
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

        public void Balance(string rpc, Action<bool, BigInteger, string, string> callback, string ticket)
        {
            string body = CreateBalanceRequest(rpc);
            PostRequest(BalanceService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(Balance)} - Response - {result}");
                if (success && IsExecNormal(result, out JObject JsonObject))
                {
                    string data = ValidateString(JsonObject["data"].ToString());
                    if (BigInteger.TryParse(data, out BigInteger balance))
                    {
                        callback?.Invoke(true, balance, data.Substring(0, 5), null);
                    }
                    else
                    {
                        callback?.Invoke(true, BigInteger.Zero, data.Length >= 5 ? data.Substring(0, 5) : data, null);
                    }
                }
                else
                {
                    callback?.Invoke(false, BigInteger.Zero, "0", error);
                }
            }, ticket);
        }

        public void ABI(string abi, bool minimal, Action<bool, string[], string> callback)
        {
            string body = CreateABIRequest(abi, minimal);
            PostRequest(ABIService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(ABI)} - Response - {result}");

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
            PostRequest(ReadService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(Read)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }

        public void Write(string rpc, string contract, string abi, string function, Action<bool, JObject, string> callback, string token, params object[] args)
        {
            string body = CreateContractRequest(rpc, contract, abi, function, args);
            PostRequest(WriteService, body, (success, result, error) =>
            {
                Debug.Log($"{nameof(BlockchainService)} - {nameof(Write)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, token);
        }
        #endregion

        #region Request Helper Methods
        public string CreateBodyJson(Dictionary<string, JToken> properties)
        {
            JObject JsonObject = new JObject();
            foreach (KeyValuePair<string, JToken> entry in properties)
            {
                JsonObject.Add(entry.Key, entry.Value);
            }
            return JsonObject.ToString();
        }

        public string CreateAuthRequest(string password)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "password", password } });
        }

        public string CreateExternalWalletRequest(string privateKey, string password)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "privateKey", privateKey }, { "password", password } });
        }

        public string CreateSignRequest(string message)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "message", message } });
        }

        public string CreateVerifyRequest(string address, string message, string signature)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "address", address }, { "message", message }, { "signature", signature } });
        }

        public string CreateABIRequest(string abi, bool minimal)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "abi", abi }, { "minimal", minimal } });
        }

        public string CreateBalanceRequest(string rpc)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "rpc", rpc } });
        }

        public string CreateContractRequest(string rpc, string contract, string abi, string function, params object[] args)
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
    }
}