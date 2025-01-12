using System;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEditor;
#if UNITY_2019_3_OR_NEWER && UNITY_EDITOR
using UnityEditor.Compilation;
#elif UNITY_2017_1_OR_NEWER && UNITY_EDITOR
using System.Reflection;
#endif

public class EktishafEditorWindow : EditorWindow
{
    protected string GeneratedDirectory = $"{Application.dataPath}/Ektishaf/Generated";
    protected static BlockchainSettings Config;
    protected bool isLoading;
    protected string LoadingText = "Please Wait ...";

    private void OnEnable()
    {
        LoadConfig();
    }

    protected virtual void DrawLoading()
    {
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.fontSize = 18;
        Vector2 size = style.CalcSize(new GUIContent(LoadingText));
        float x = (position.width - size.x) / 2;
        float y = (position.height - size.y) / 2;
        GUI.Label(new Rect(x, y, size.x, size.y), LoadingText, style);
    }

    protected static void LoadConfig()
    {
        Config = BlockchainSettings.GetOrCreateSettings();
        if (!Config)
        {
            Debug.LogError("Failed to load configuration file.");
        }
    }

    protected async void PostAsync(string url, string body, Action<bool, string, string> callback, string ticket = "")
    {
        using (var client = new HttpClient())
        {
            if (!string.IsNullOrEmpty(ticket) && !string.IsNullOrWhiteSpace(ticket))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", string.Format("{0}", ticket));
            }
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                callback?.Invoke(true, result, null);
            }
            else
            {
                string result = await response.Content?.ReadAsStringAsync();
                callback?.Invoke(false, null, result);
            }
        }
    }

    protected void Compile()
    {
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
