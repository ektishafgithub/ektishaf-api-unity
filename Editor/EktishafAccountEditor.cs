using Ektishaf;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;

public class EktishafAccountEditor : EktishafEditorWindow
{
    static Vector2 Scroll;
    static Vector2 Global;

    [MenuItem("Ektishaf/Account/Generate New Accounts", false, 2)]
    public static void ShowWindow()
    {
        LoadConfig();

        EktishafAccountEditor window = CreateInstance<EktishafAccountEditor>();
        window.titleContent = new GUIContent("Generate New Accounts", "Generate new accounts for development purposes.");
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

        EditorGUILayout.LabelField($"Welcome to Account Utility!\n\nPlease click \'Generate New Accounts\' button below to generate new accounts.\n\n" +
            $"Note: {Config.MaxAccountsPerRequest} new accounts to generate based on Project Settings->Ektishaf Blockchain Settings->MaxAccountsPerRequest.\n" +
            $"After the accounts are created, they are added to Project Settings->Ektishaf Blockchain Settings->Accounts.\n\n", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button("Generate New Accounts"))
        {
            GenerateNewAccounts();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Close"))
        {
            Close();
        }

        EditorGUILayout.EndScrollView();
    }

    public void GenerateNewAccounts()
    {
        isLoading = true;
        if (Config.Accounts.Count > 10)
        {
            Debug.Log("Creating more than 10 accounts is not allowed.");
            return;
        }
        if (string.IsNullOrEmpty(Config.GenerateAccountsWithPassword) || string.IsNullOrWhiteSpace(Config.GenerateAccountsWithPassword))
        {
            Debug.Log("Please specify an account password in Project Settings->Ektishaf->Accounts->GenerateAccountsWithPassword to be used for new accounts.");
            return;
        }

        string body = $"{{\"registers\":{Config.MaxAccountsPerRequest}, \"password\":\"{Config.GenerateAccountsWithPassword}\" }}";
        Debug.Log($"{nameof(GenerateNewAccounts)} - {nameof(GenerateNewAccounts)} - Generating Accounts, Please Wait ...");
        PostAsync(Config.Op(ServOp.Accounts), body, (success, result, error) =>
        {
            Debug.Log($"{nameof(GenerateNewAccounts)} - {nameof(GenerateNewAccounts)} - Response - {result}");
            if (success)
            {
                JObject JsonObject = JObject.Parse(result);
                JArray array = JArray.FromObject(JsonObject["accounts"]);
                List<EktishafAccount> accounts = JsonConvert.DeserializeObject<List<EktishafAccount>>(array.ToString());
                if (accounts.Count > 0)
                {
                    Config.Accounts.AddRange(accounts);
                    for (int i = 0; i < accounts.Count; i++)
                    {
                        Debug.Log($"Generated Account {i + 1}: {accounts[i].Address}");
                    }
                    Debug.Log($"{nameof(GenerateNewAccounts)} - {nameof(GenerateNewAccounts)} - New accounts are created successfully.");
                }
            }
            else
            {
                Debug.Log($"Failed to create accounts with error, {error}");
            }
            isLoading = false;
        });
    }
}
