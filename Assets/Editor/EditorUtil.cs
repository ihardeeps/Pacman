using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtil : Editor
{
    [MenuItem("Tools/Create/Game Configuration Asset")]
    public static void InitConfig()
    {
        string path = "Assets/Resources/" + GameConfiguration.AssetPath + ".asset";
        GameConfiguration config = ScriptableObject.CreateInstance<GameConfiguration>();
        config.Init();
        AssetDatabase.CreateAsset(config, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.objects = new Object[] { config };
    }
    [MenuItem("Tools/Create/Game Balance Asset")]
    public static void InitGameBalance()
    {
        string path = "Assets/Resources/" + GameBalance.AssetPath + ".asset";
        GameBalance gameBalance = ScriptableObject.CreateInstance<GameBalance>();
        AssetDatabase.CreateAsset(gameBalance, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Select new adventure
        Selection.objects = new Object[] { gameBalance };
    }
}
