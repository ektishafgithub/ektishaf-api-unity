using System;
using System.IO;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.Collections.Generic;
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
using UnityEditor.Compilation;
#elif UNITY_2017_1_OR_NEWER && UNITY_EDITOR
using System.Reflection;
#endif

public class EktishafUtility : EditorWindow
{
    static string GeneratedDirectory = $"{Application.dataPath}/Ektishaf/Generated";
    static string FileName = "{ContractInterface}";
    static string Address = "0x";
    static string Abi;
    static string Hbi;
    static Vector2 Scroll;
    static Vector2 Global;

    [MenuItem("Ektishaf/ABI Converter", false, 100)]
    public static void Init()
    {
        EktishafUtility window = CreateInstance<EktishafUtility>();
        window.titleContent = new GUIContent("ABI Converter", "Saves contract interface to a file.");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 1920, 1080);
        window.ShowUtility();
    }

    void OnGUI()
    {
        Global = EditorGUILayout.BeginScrollView(Global);
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Contract ABI", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);

        Scroll = EditorGUILayout.BeginScrollView(Scroll);
        Abi = EditorGUILayout.TextArea(Abi);
        EditorGUILayout.EndScrollView();
        GUILayout.Space(10);

        Address = EditorGUILayout.TextField("Contract Address: ", Address);
        GUILayout.Space(10);

        FileName = EditorGUILayout.TextField("Save Filename: ", FileName);
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            if (!Directory.Exists(GeneratedDirectory))
            {
                Directory.CreateDirectory(GeneratedDirectory);
            }

            PostJsonDataAsync(((BlockchainSettings)BlockchainSettings.GetInstance()).Op(ServOp.ABI), $"{{\"abi\":{Abi}, \"minimal\":true}}", (success, result, error) =>
            {
                if (success)
                {
                    Hbi = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<string[]>(result), Formatting.Indented);
                    Abi = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<object>(Abi), Formatting.None);
                    GenerateABI(Hbi, Abi, Address, FileName);
                }
                else
                {
                    Debug.Log("Failed to generate ABI, please enter a valid ABI of a contract.");
                }
            });

        }
        if (GUILayout.Button("Cancel")) Close();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }

    private async void PostJsonDataAsync(string url, string body, Action<bool, string, string> callback)
    {
        using (var client = new HttpClient())
        {
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                callback?.Invoke(true, result, null);
            }
            else
            {
                callback?.Invoke(false, null, response.StatusCode.ToString());
            }
        }
    }

    public static void GenerateABI(string hbi, string abi, string address, string fileName)
    {
        string[] ValueArray = JsonConvert.DeserializeObject<string[]>(hbi);
        if (ValueArray != null || ValueArray.Length > 0)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 1; i < ValueArray.Length; i++)
            {
                string raw = ValueArray[i];

                int argStartIndex = raw.IndexOf("(");
                int argEndIndex = raw.IndexOf(")");
                string rawArgs = raw.Substring(argStartIndex, (argEndIndex + 1) - argStartIndex);
                string lineWithoutReturn = raw.Remove(argEndIndex + 1, raw.Length - (argEndIndex + 1));

                string args = rawArgs.Trim(new char[] { '(', ')' });
                int numOfArgs = 0;
                string a = "";
                if (!string.IsNullOrEmpty(args) || !string.IsNullOrWhiteSpace(args))
                {
                    string[] splits = args.Split(",");
                    numOfArgs = splits.Length;

                    for (int j = 0; j < numOfArgs; j++)
                    {
                        string[] argSplits = splits[j].Trim(new char[] { ' ' }).Split(" ");
                        a += $"{System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(argSplits[0])}".Replace("[]", "Array");
                        if (j < numOfArgs - 1) a += "_";
                    }
                }

                string lineWithoutArgs = raw.Replace(rawArgs, "");
                lineWithoutArgs = lineWithoutArgs.Trim(new char[] { '"', ',' });
                string[] elements = lineWithoutArgs.Split(" ");
                string functionName = elements[1];

                string dump = $"{functionName}_{numOfArgs}_{a}";
                dictionary.Add(dump, raw);
            }
            using (StreamWriter writer = new StreamWriter($"{GeneratedDirectory}/{fileName}.cs"))
            {
                writer.WriteLine($"public static class {fileName}");
                writer.WriteLine("{");
                foreach (string entry in dictionary.Keys)
                {
                    writer.WriteLine($"\t\tpublic const string {entry} = \"{dictionary[entry]}\";");
                }
                writer.WriteLine($"\n\t\tpublic const string Address = \"{address}\";");
                writer.WriteLine($"\t\tpublic const string ABI = \"{abi.Replace("\"", "\\\"")}\";");
                writer.WriteLine($"\t\tpublic const string HBI = @\"{hbi.Replace("\"", "\"\"")}\";");

                writer.WriteLine("}");
            }
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
            CompilationPipeline.RequestScriptCompilation();
#elif UNITY_2017_1_OR_NEWER && UNITY_EDITOR
                var editorAssembly = Assembly.GetAssembly(typeof(Editor));
                var editorCompilationInterfaceType = editorAssembly.GetType("UnityEditor.Scripting.ScriptCompilation.EditorCompilationInterface");
                var dirtyAllScriptsMethod = editorCompilationInterfaceType.GetMethod("DirtyAllScripts", BindingFlags.Static | BindingFlags.Public);
                dirtyAllScriptsMethod.Invoke(editorCompilationInterfaceType, null);
#endif

#if UNITY_EDITOR
            //AssetDatabase.Refresh();
#endif
        }
    }
}

