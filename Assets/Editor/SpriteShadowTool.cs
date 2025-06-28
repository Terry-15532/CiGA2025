// 文件：Assets/Editor/ReplaceSpriteMaterialTool.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class ReplaceSpriteMaterialTool : EditorWindow
{
    private const string PREF_KEY = "ReplaceSpriteMaterialTool.TargetMatPath";
    private Material targetMaterial;

    [MenuItem("Tools/Replace SpriteRenderer Material")]
    private static void OpenWindow()
    {
        var win = GetWindow<ReplaceSpriteMaterialTool>("替换 SpriteRenderer 材质");
        win.minSize = new Vector2(350, 120);
    }

    private void OnEnable()
    {
        // 窗口打开时，从 EditorPrefs 读取之前保存的材质路径
        string savedPath = EditorPrefs.GetString(PREF_KEY, "");
        if (!string.IsNullOrEmpty(savedPath))
        {
            targetMaterial = AssetDatabase.LoadAssetAtPath<Material>(savedPath);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("批量替换场景中所有 SpriteRenderer 的材质", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        targetMaterial = (Material)EditorGUILayout.ObjectField("目标材质", targetMaterial, typeof(Material), false);
        if (EditorGUI.EndChangeCheck())
        {
            // 选了新材质就立刻保存路径；清空也同步删除
            if (targetMaterial != null)
                EditorPrefs.SetString(PREF_KEY, AssetDatabase.GetAssetPath(targetMaterial));
            else
                EditorPrefs.DeleteKey(PREF_KEY);
        }

        GUI.enabled = targetMaterial != null;
        if (GUILayout.Button("执行替换"))
        {
            if (EditorUtility.DisplayDialog(
                "确认替换？",
                $"将场景中所有 SpriteRenderer 的材质替换为 “{targetMaterial.name}” 并开启阴影？",
                "是", "取消"))
            {
                ReplaceAllSpriteMaterials();
            }
        }
        GUI.enabled = true;
    }

    private void ReplaceAllSpriteMaterials()
    {
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        int count = 0;

        foreach (var sr in Object.FindObjectsOfType<SpriteRenderer>())
        {
            Undo.RecordObject(sr, "Replace SpriteRenderer Material");
            sr.sharedMaterial = targetMaterial;
            sr.receiveShadows = true;
            sr.shadowCastingMode = ShadowCastingMode.TwoSided;
            EditorUtility.SetDirty(sr);
            count++;
        }

        EditorSceneManager.MarkAllScenesDirty();
        Undo.CollapseUndoOperations(group);

        EditorUtility.DisplayDialog("替换完成", $"共替换了 {count} 个 SpriteRenderer", "好的");
    }
}
