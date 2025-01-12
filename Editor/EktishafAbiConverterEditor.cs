using Ektishaf;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

public class EktishafAbiConverterEditor : EktishafEditorWindow
{
    string FileName;
    string Address;
    string Abi;
    string Hbi;
    bool SaveAbi;
    bool SaveHumanReadableAbi;
    Vector2 Scroll;
    Vector2 Global;

    [MenuItem("Ektishaf/ABI/ABI Converter", false, 1)]
    public static void ShowWindow()
    {
        EktishafAbiConverterEditor window = CreateInstance<EktishafAbiConverterEditor>();
        window.titleContent = new GUIContent("ABI Converter", "Saves contract interface to a file.");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 1920, 1080);
        window.FileName = $"ContractName{Random.Range(1000, 9999)}";
        window.Address = "0x0000000000000000000000000000000000000000";
        window.SaveAbi = false;
        window.SaveHumanReadableAbi = false;
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

        EditorGUIUtility.labelWidth = 256;
        Address = EditorGUILayout.TextField("Contract Address: ", Address);
        GUILayout.Space(10);

        FileName = EditorGUILayout.TextField("Save Filename: ", FileName);
        GUILayout.Space(10);
        
        SaveAbi = EditorGUILayout.Toggle("Save ABI", SaveAbi);
        GUILayout.Space(10);

        SaveHumanReadableAbi = EditorGUILayout.Toggle("Save Human Readable ABI", SaveHumanReadableAbi);
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            if (!Directory.Exists(GeneratedDirectory))
            {
                Directory.CreateDirectory(GeneratedDirectory);
            }

            PostAsync(BlockchainSettings.GetOrCreateSettings().Op(ServOp.ABI), $"{{\"abi\":{Abi}, \"minimal\":true}}", (success, result, error) =>
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

    public void GenerateABI(string hbi, string abi, string address, string fileName)
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
                if (elements[0] != "function") continue;
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
                if (SaveAbi)
                {
                    writer.WriteLine($"\t\tpublic const string ABI = \"{abi.Replace("\"", "\\\"")}\";");
                }
                if (SaveHumanReadableAbi)
                {
                    writer.WriteLine($"\t\tpublic const string HBI = @\"{hbi.Replace("\"", "\"\"")}\";");
                }
                writer.WriteLine("}");
            }
            Compile();
        }
    }
}
