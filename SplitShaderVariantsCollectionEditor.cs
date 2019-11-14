using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SplitShaderVariantsCollectionEditor : EditorWindow
{
    static SplitShaderVariantsCollectionEditor self;
    [MenuItem("Tools/Collect Shader Variants/SplitShaderVariantsCollectionEditor")]
    public static SplitShaderVariantsCollectionEditor GetWindow()
    {
        if (self == null)
            self = GetWindow<SplitShaderVariantsCollectionEditor>();
        self.Show();
        return self;
    }

    ShaderVariantCollection svc;
    private void OnGUI()
    {
        GUILayout.Label("Split ShaderVariantCollection");
        var tempSVC = EditorGUILayout.ObjectField(svc, typeof(ShaderVariantCollection), false) as ShaderVariantCollection;
        if (tempSVC != svc)
        {
            svc = tempSVC;
        }

        if(svc != null)
        {
            if (GUILayout.Button("Split SVC"))
            {
                SplitShaderVariantCollection(svc);
            }
        }
    }

    private void SplitShaderVariantCollection(ShaderVariantCollection svc)
    {
        var serializedObject = new SerializedObject(svc);
        var m_Shaders = serializedObject.FindProperty("m_Shaders");
        if (m_Shaders != null)
        {
            for (var i = 0; i < m_Shaders.arraySize; ++i)
            {
                var entryProp = m_Shaders.GetArrayElementAtIndex(i);
                var shader = (Shader)entryProp.FindPropertyRelative("first").objectReferenceValue;
                if (shader != null)
                {
                    Debug.Log(shader.name);
                    EditorUtility.DisplayProgressBar("Spliting SVC", "Shader -> " + shader.name, (i + 1) * 1.0f / m_Shaders.arraySize);

                    var newSVC = new ShaderVariantCollection();
                    // Variants for this shader
                    var variantsProp = entryProp.FindPropertyRelative("second.variants");
                    for (var j = 0; j < variantsProp.arraySize; ++j)
                    {
                        var prop = variantsProp.GetArrayElementAtIndex(j);
                        var keywords = prop.FindPropertyRelative("keywords").stringValue;
                        string[] keywordsArr = null;
                        if (!string.IsNullOrEmpty(keywords))
                        {
                            keywordsArr = keywords.Split(' ');
                        }
                        var passType = (UnityEngine.Rendering.PassType)prop.FindPropertyRelative("passType").intValue;
                        newSVC.Add(new ShaderVariantCollection.ShaderVariant(shader, passType, keywordsArr));
                    }

                    var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(svc));
                    var filename = shader.name.Replace('/', '_').Replace(' ', '_') + ".shadervariants";
                    var output = Path.Combine(path, filename);
                    AssetDatabase.CreateAsset(newSVC, output);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.LogWarning("has missing shader.");
                }
            }
            EditorUtility.ClearProgressBar();
        }
    }
}
