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

namespace Ektishaf
{
    public class EktishafUtility : EditorWindow
    {
        static string OutputDir = $"{Application.dataPath}/Ektishaf/Generated";
        string fileName = "{ContractInterface}";
        static string address = "0x";
        static string abi;
        static string hbi;
        static Vector2 scroll;
        static Vector2 global;

        [MenuItem("Ektishaf/ABI Converter", false, 100)]
        public static void Init()
        {
            EktishafUtility window = CreateInstance<EktishafUtility>();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 1920, 1080);
            window.ShowUtility();
        }

        void OnGUI()
        {
            global = EditorGUILayout.BeginScrollView(global);
            GUILayout.Space(10);

            EditorGUILayout.LabelField("Contract ABI", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            abi = EditorGUILayout.TextArea(abi);
            EditorGUILayout.EndScrollView();
            GUILayout.Space(10);

            address = EditorGUILayout.TextField("Contract Address: ", address);
            GUILayout.Space(10);

            fileName = EditorGUILayout.TextField("Save Filename: ", fileName);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                if (!Directory.Exists(OutputDir))
                {
                    Directory.CreateDirectory(OutputDir);
                }

                PostJsonDataAsync(RequestManager.ABIUrl, RequestManager.Singleton.CreateABIRequest(abi, true), (success, result, error) =>
                {
                    if (success)
                    {
                        string[] hbiArray = JsonConvert.DeserializeObject<string[]>(result);
                        hbi = JsonConvert.SerializeObject(hbiArray, Formatting.Indented);
                        object abiObject = JsonConvert.DeserializeObject<object>(abi);
                        string abiString = JsonConvert.SerializeObject(abiObject, Formatting.None);
                        GenerateABI(hbi, abiString, address, fileName);
                    }
                    else
                    {
                        Debug.Log("Failed to generate ABI, please enter a valid ABI of any contract.");
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
            Dictionary<(string, int, string), string> functions = new Dictionary<(string, int, string), string>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            using (StringReader reader = new StringReader(hbi))
            {
                string raw;
                while ((raw = reader.ReadLine()) is object)
                {
                    if (string.IsNullOrEmpty(raw) || string.IsNullOrWhiteSpace(raw) || (raw == "[") || (raw == "]")) continue;
                    string line = raw.Trim(new char[] { ' ', '"', ',' });
                    string constructor = line.Substring(0, 11);
                    if (constructor == nameof(constructor)) continue;

                    // Arguments Signature Stamp
                    int argStartIndex = line.IndexOf("(");
                    int argEndIndex = line.IndexOf(")");
                    string rawArgs = line.Substring(argStartIndex, (argEndIndex + 1) - argStartIndex);
                    string lineWithoutReturn = line.Remove(argEndIndex + 1, line.Length - (argEndIndex + 1));

                    // Number of Arguments
                    string args = rawArgs.Trim(new char[] { '(', ')' });
                    int numOfArgs = 0;
                    string a = "";
                    if (!string.IsNullOrEmpty(args) || !string.IsNullOrWhiteSpace(args))
                    {
                        string[] splits = args.Split(",");
                        numOfArgs = splits.Length;

                        for (int i = 0; i < numOfArgs; i++)
                        {
                            string[] argSplits = splits[i].Trim(new char[] { ' ' }).Split(" ");
                            a += $"{System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(argSplits[0])}".Replace("[]", "Array");
                            if (i < numOfArgs - 1) a += "_";
                        }
                    }
                    
                    string lineWithoutArgs = line.Replace(rawArgs, "");
                    lineWithoutArgs = lineWithoutArgs.Trim(new char[] { '"', ',' });
                    string[] elements = lineWithoutArgs.Split(" ");
                    string functionName = elements[1];
                    
                    functions.Add((functionName, numOfArgs, rawArgs), line);
                    string dump = $"{functionName}_{numOfArgs}_{a}";
                    dictionary.Add(dump, line);

                }
            }
            using (StreamWriter writer = new StreamWriter($"{OutputDir}/{fileName}.cs"))
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
            // return functions;
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