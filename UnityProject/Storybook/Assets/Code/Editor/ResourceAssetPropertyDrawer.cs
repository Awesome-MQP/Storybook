using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

[CustomPropertyDrawer(typeof(ResourceAsset))]
public class ResourceAssetPropertyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty fullAssetNameProperty = property.FindPropertyRelative("m_editorAsset");
        string fullAssetName = fullAssetNameProperty.stringValue;
        SerializedProperty assetTypeProperty = property.FindPropertyRelative("m_baseTypeName");
        string typeName = assetTypeProperty.stringValue;


        UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(fullAssetName);
        Type type = TypeHelper.GetType(typeName);

        UnityEngine.Object obj = EditorGUI.ObjectField(position, label, asset, type, false);
        if (obj == asset)
            return;

        SerializedProperty assetNameProperty = property.FindPropertyRelative("m_asset");
        string assetName = assetNameProperty.stringValue;

        if (obj == null)
        {
            assetNameProperty.stringValue = "";
            return;
        }

        string fullAssetPath = AssetDatabase.GetAssetPath(obj);
        string assetPath = fullAssetPath;
        int dotIndex = assetPath.LastIndexOf('.');
        if (dotIndex > -1)
            assetPath = assetPath.Substring(0, dotIndex);

        int resourcesIndex = assetPath.LastIndexOf("/Resources/", StringComparison.CurrentCulture);
        if (resourcesIndex < 0)
        {
            Debug.LogWarning("Resource assets require an asset in the resources folder.");
            return;
        }

        assetPath = assetPath.Substring(resourcesIndex + "/Resources/".Length);

        assetNameProperty.stringValue = assetPath;
        fullAssetNameProperty.stringValue = fullAssetPath;
    }
}
