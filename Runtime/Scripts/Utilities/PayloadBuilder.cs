using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Ektishaf
{
    public class PayloadBuilder : MonoBehaviour
    {
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

        public static string CreateBalanceRequest(string rpc, string address)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "rpc", rpc }, { "address", address } });
        }

        public static string CreateContractRequest(string rpc, string contract, string abi, string function, params object[] args)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "rpc", rpc }, { "contract", contract }, { "abi", abi }, { "function", function }, { "params", JToken.FromObject(args) } });
        }

        public static string CreateContractRequest(string rpc, string contract, string abi, string function, string value, params object[] args)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "rpc", rpc }, { "contract", contract }, { "abi", abi }, { "function", function }, { "value", value }, { "params", JToken.FromObject(args) } });
        }

        public static string CreateAccountsRequest(int registers, string password)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "registers", registers }, { "password", password } });
        }

        public static string CreateSendRequest(string rpc, string to, string amount)
        {
            return CreateBodyJson(new Dictionary<string, JToken>() { { "rpc", rpc }, { "to", to }, { "amount", amount } });
        }

        public static string ExtractFunctionABI(string funcSig)
        {
            return @$"[""{funcSig.Trim(new[] { '[', ']', '"' })}""]";
        }

        public static string ExtractFunctionName(string funcSig)
        {
            string func = funcSig.Trim(new[] { '[', ']', '"' }).Split(" ")[1];
            return func.Substring(0, func.IndexOf("("));
        }

        public static bool IsExecNormal(string result, out JObject JsonObject)
        {
            if (JObject.Parse(result)?["execution"]?.ToString() == "normal")
            {
                JsonObject = JObject.Parse(result);
                return true;
            }
            JsonObject = null;
            return false;
        }

        public static string ValidateString(string value)
        {
            return value.Trim(new[] { '\'', '"' }).Replace("\\\"", "\"");
        }
    }
}
