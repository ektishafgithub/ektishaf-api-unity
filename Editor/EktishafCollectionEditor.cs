using Ektishaf;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EktishafCollectionEditor : EktishafEditorWindow
{
    static Vector2 Scroll;
    static Vector2 Global;
    static string Address;
    static string Fee;

    [MenuItem("Ektishaf/Collection/Register Collection", false, 3)]
    public static void ShowWindow()
    {
        LoadConfig();

        EktishafCollectionEditor window = CreateInstance<EktishafCollectionEditor>();
        window.titleContent = new GUIContent("Register Collection", "Registers a new collection.");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 1920, 1080);
        window.ShowUtility();
    }

    void OnGUI()
    {
        if (isLoading)
        {
            DrawLoading();
            return;
        }

        Global = EditorGUILayout.BeginScrollView(Global);
        GUILayout.Space(10);

        EditorGUILayout.LabelField($"Welcome to Collection Utility!\n\nPlease check registration fee below before purchasing a collection.", EditorStyles.wordWrappedLabel);
        GUI.contentColor = Color.yellow;
        EditorGUILayout.LabelField($"Registration Fee: {Fee}", EditorStyles.wordWrappedLabel);
        GUI.contentColor = Color.white;
        if (GUILayout.Button("Check Registration Fee"))
        {
            GetRegistrationFee();
        }

        GUILayout.Space(50);

        EditorGUILayout.LabelField($"Check to see if you already purchased a collection address.", EditorStyles.wordWrappedLabel);
        GUILayout.BeginHorizontal();
        Address = EditorGUILayout.TextField("Contract Address: ", Address);
        if (GUILayout.Button("Get Collection"))
        {
            GetCollectionAddress();
        }
        if (!string.IsNullOrEmpty(Address))
        {
            if (GUILayout.Button("Etherscan"))
            {
                Application.OpenURL($"{Config.GetDefaultNetwork().BlockExplorer}/address/{Address}");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(50);

        EditorGUILayout.LabelField($"Make sure your wallet has sufficient funds to cover the registration fee.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField("Purchase a new collection", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Purchase Collection"))
        {
            PurchaseCollection();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Close"))
        {
            Close();
        }

        EditorGUILayout.EndScrollView();
    }

    private void GetRegistrationFee()
    {
        isLoading = true;
        string body = PayloadBuilder.CreateContractRequest(Config.GetDefaultNetwork().Rpc, CollectionFactory.Address, PayloadBuilder.ExtractFunctionABI(CollectionFactory.getRegistrationFee_0_), PayloadBuilder.ExtractFunctionName(CollectionFactory.getRegistrationFee_0_));
        Debug.Log($"{nameof(GetRegistrationFee)} - {nameof(PurchaseCollection)} - Get Registration Fee ..., Body: {body}");
        PostAsync(Config.Op(ServOp.Read), body, (success, result, error) =>
        {
            Debug.Log($"{nameof(GetRegistrationFee)} - {nameof(GetRegistrationFee)} - Response - {result}");
            if (success && PayloadBuilder.IsExecNormal(result, out JObject JsonObject))
            {
                Debug.Log($"Registration Fee with result, {JsonObject["data"]}");
                Fee = EktishafMathHelper.ParseWei(System.Numerics.BigInteger.Parse(JsonObject["data"].ToString())).TrimEnd('0') + " ETH";
            }
            else
            {
                Debug.Log($"Failed to get registration fee, {error}");
            }
            isLoading = false;
        }, Config.GetDefaultAccount().Ticket);
    }

    private void PurchaseCollection()
    {
        isLoading = true;
        string body = PayloadBuilder.CreateContractRequest(Config.GetDefaultNetwork().Rpc, CollectionFactory.Address, PayloadBuilder.ExtractFunctionABI(CollectionFactory.createCollection_0_), PayloadBuilder.ExtractFunctionName(CollectionFactory.createCollection_0_), "0.2");
        Debug.Log($"{nameof(PurchaseCollection)} - {nameof(PurchaseCollection)} - Purchasing Collection ..., Body: {body}");
        PostAsync(Config.Op(ServOp.WriteWithValue), body, (success, result, error) =>
        {
            Debug.Log($"{nameof(PurchaseCollection)} - {nameof(PurchaseCollection)} - Response - {result}");
            if (success && PayloadBuilder.IsExecNormal(result, out JObject JsonObject))
            {
                Debug.Log($"Collection Purchased Successfully with result, {result}");
                GetCollectionAddress();
            }
            else
            {
                Debug.Log($"Failed to purchase collection with error, {error}");
                isLoading = false;
            }
        }, Config.GetDefaultAccount().Ticket);
    }

    private void GetCollectionAddress()
    {
        isLoading = true;
        string body = PayloadBuilder.CreateContractRequest(Config.GetDefaultNetwork().Rpc, CollectionFactory.Address, PayloadBuilder.ExtractFunctionABI(CollectionFactory.getCollection_0_), PayloadBuilder.ExtractFunctionName(CollectionFactory.getCollection_0_));
        Debug.Log($"{nameof(GetCollectionAddress)} - {nameof(GetCollectionAddress)} - Get Collection Address ..., Body: {body}");
        PostAsync(Config.Op(ServOp.Read), body, (success, result, error) =>
        {
            if (success && PayloadBuilder.IsExecNormal(result, out JObject JsonObject))
            {
                Debug.Log($"Collection address with result, {result}");
                Address = JsonObject["data"].ToString();
            }
            else
            {
                Debug.Log($"Failed to obtain collection address with error, {error}");
            }
            isLoading = false;
        }, Config.GetDefaultAccount().Ticket);
    }
}
