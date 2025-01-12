using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ektishaf
{
    public class BlockchainService : MonoBehaviour
    {
        #region Variables
        [HideInInspector]
        public BlockchainSettings Config;

        [HideInInspector]
        public EktishafNetwork CurrentNetwork;

        [HideInInspector]
        public EktishafAccount CurrentAccount;
        #endregion

        #region Default Methods
        protected virtual void Awake()
        {
            Config = Resources.Load<BlockchainSettings>("BlockchainSettings");
            if (Config.HasAnyNetwork())
            {
                CurrentNetwork = Config.GetDefaultNetwork();
            }
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
            Log($"{nameof(BlockchainService)} - {nameof(GetRequest)} - Url: {url}");
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
            Log($"{nameof(BlockchainService)} - {nameof(PostRequest)} - Request: {body}");
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
            Log($"{nameof(BlockchainService)} - {nameof(GetTexture)} - Url: {url}");
            StartCoroutine(GetTextureCoroutine(url, callback));
        }
        #endregion

        #region Core Methods
        public void Host(Action<bool, string> callback)
        {
            GetRequest(Config.Op(), (success, result, error) => callback?.Invoke(success, result));
        }
        
        public void Register(string password, Action<bool, string, string, string> callback)
        {
            string body = PayloadBuilder.CreateAuthRequest(password);
            PostRequest(Config.Op(ServOp.Register), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Register)} - Response - {result}");

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
            string body = PayloadBuilder.CreateAuthRequest(password);
            PostRequest(Config.Op(ServOp.Login), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Login)} - Response - {result}");

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
            string body = PayloadBuilder.CreateExternalWalletRequest(privateKey, password);
            PostRequest(Config.Op(ServOp.External), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(External)} - Response - {result}");
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
            string body = PayloadBuilder.CreateAuthRequest(password);
            PostRequest(Config.Op(ServOp.Reveal), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Reveal)} - Response - {result}");
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
            string body = PayloadBuilder.CreateSignRequest(message);
            PostRequest(Config.Op(ServOp.Sign), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Sign)} - Response - {result}");
                if (success && PayloadBuilder.IsExecNormal(result, out JObject JsonObject))
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
            string body = PayloadBuilder.CreateVerifyRequest(address, message, signature);
            PostRequest(Config.Op(ServOp.Verify), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Verify)} - Response - {result}");
                if (success && PayloadBuilder.IsExecNormal(result, out JObject JsonObject))
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

        public void Balance(string rpc, string address, Action<bool, string, string> callback)
        {
            string body = PayloadBuilder.CreateBalanceRequest(rpc, address);
            PostRequest(Config.Op(ServOp.Balance), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Balance)} - Response - {result}");
                if (success && PayloadBuilder.IsExecNormal(result, out JObject JsonObject))
                {
                    string data = PayloadBuilder.ValidateString(JsonObject["data"].ToString());
                    callback?.Invoke(true, data.Length >= 5 ? data.Substring(0, 5) : data, null);
                }
                else
                {
                    callback?.Invoke(false, "0", error);
                }
            });
        }

        public void ABI(string abi, bool minimal, Action<bool, string[], string> callback)
        {
            string body = PayloadBuilder.CreateABIRequest(abi, minimal);
            PostRequest(Config.Op(ServOp.ABI), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(ABI)} - Response - {result}");

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
            string body = PayloadBuilder.CreateContractRequest(rpc, contract, abi, function, args);
            PostRequest(Config.Op(ServOp.Read), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Read)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && PayloadBuilder.IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }

        public void Read(string rpc, string contract, string funcSig, Action<bool, JObject, string> callback, string ticket, params object[] args)
        {
            string body = PayloadBuilder.CreateContractRequest(rpc, contract, PayloadBuilder.ExtractFunctionABI(funcSig), PayloadBuilder.ExtractFunctionName(funcSig), args);
            PostRequest(Config.Op(ServOp.Read), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Read)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && PayloadBuilder.IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }

        public void Write(string rpc, string contract, string abi, string function, Action<bool, JObject, string> callback, string ticket, params object[] args)
        {
            string body = PayloadBuilder.CreateContractRequest(rpc, contract, abi, function, args);
            PostRequest(Config.Op(ServOp.Write), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Write)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && PayloadBuilder.IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }

        public void Write(string rpc, string contract, string funcSig, Action<bool, JObject, string> callback, string ticket, params object[] args)
        {
            string body = PayloadBuilder.CreateContractRequest(rpc, contract, PayloadBuilder.ExtractFunctionABI(funcSig), PayloadBuilder.ExtractFunctionName(funcSig), args);
            PostRequest(Config.Op(ServOp.Write), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Write)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && PayloadBuilder.IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }

        public void Write(string rpc, string contract, string abi, string function, Action<bool, JObject, string> callback, string ticket, string value, params object[] args)
        {
            string body = PayloadBuilder.CreateContractRequest(rpc, contract, abi, function, value, args);
            PostRequest(Config.Op(ServOp.WriteWithValue), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Write)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && PayloadBuilder.IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }

        public void Write(string rpc, string contract, string funcSig, Action<bool, JObject, string> callback, string ticket, string value, params object[] args)
        {
            string body = PayloadBuilder.CreateContractRequest(rpc, contract, PayloadBuilder.ExtractFunctionABI(funcSig), PayloadBuilder.ExtractFunctionName(funcSig), value, args);
            PostRequest(Config.Op(ServOp.WriteWithValue), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Write)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && PayloadBuilder.IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }

        public void Accounts(int registers, string password, Action<bool, List<EktishafAccount>, string> callback)
        {
            string body = PayloadBuilder.CreateAccountsRequest(registers, password);
            PostRequest(Config.Op(ServOp.Accounts), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(Accounts)} - Response - {result}");
                JObject JsonObject = JObject.Parse(result);
                JArray array = JArray.FromObject(JsonObject["accounts"]);
                List<EktishafAccount> accounts = JsonConvert.DeserializeObject<List<EktishafAccount>>(array.ToString());
                callback?.Invoke(success, accounts, success ? null : error);
            });
        }

        public void SendEther(string rpc, string to, string amount, string ticket, Action<bool, JObject, string> callback)
        {
            string body = PayloadBuilder.CreateSendRequest(rpc, to, amount);
            PostRequest(Config.Op(ServOp.Send), body, (success, result, error) =>
            {
                Log($"{nameof(BlockchainService)} - {nameof(SendEther)} - Response - {result}");
                JObject JsonObject = null;
                bool normal = success && PayloadBuilder.IsExecNormal(result, out JsonObject);
                callback?.Invoke(normal, JsonObject, normal ? null : error);
            }, ticket);
        }
        #endregion

        public void Log(string message)
        {
            if (Config.ShowLogs) Debug.Log(message);
        }
    }
}