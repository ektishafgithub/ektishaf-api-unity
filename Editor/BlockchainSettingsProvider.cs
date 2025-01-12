using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BlockchainSettingsProvider : SettingsProvider
{
    private SerializedObject blockchainSettings;

    public BlockchainSettingsProvider(string path, SettingsScope scope = SettingsScope.Project)
        :base(path, scope) { }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        blockchainSettings = BlockchainSettings.GetSerializedSettings();
    }

    public override void OnGUI(string searchContext)
    {
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("Networks"));
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("DefaultNetworkIndex"));
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("Accounts"));
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("DefaultAccountIndex"));
        EditorGUILayout.Space(10, true);

        EditorGUIUtility.labelWidth = 256;
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("MaxAccountsPerRequest"));
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("GenerateAccountsWithPassword"));
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("AssetGateway"));
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("MetadataHash"));
        EditorGUILayout.PropertyField(blockchainSettings.FindProperty("ShowLogs"));
        blockchainSettings.ApplyModifiedPropertiesWithoutUndo();
    }

    [SettingsProvider]
    public static SettingsProvider CreateBlockchainSettingsProvider()
    {
        return new BlockchainSettingsProvider("Project/Ektishaf Blockchain Settings", SettingsScope.Project);
    }
}