using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;
using UnityEngine.UI;

[CustomEditor(typeof(PanelInfoClass))]
public class PanelScriptEditor : Editor
{
    Transform contentTransform;

    SerializedProperty PanelListSerialized;
    SerializedProperty PanelTemplateSerialized;
    Transform contentObject;

    void Awake()
    {
        PanelListSerialized = serializedObject.FindProperty("PanelList");
        PanelTemplateSerialized = serializedObject.FindProperty("panelTemplate");
        contentObject = ((PanelInfoClass)target).gameObject.transform.GetChild(0).GetChild(0).transform;
        VerticalLayoutGroup vlg;
        vlg = (VerticalLayoutGroup) contentObject.gameObject.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
        {
            vlg = (VerticalLayoutGroup)contentObject.gameObject.AddComponent<VerticalLayoutGroup>();
        }
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = true;
        vlg.childForceExpandWidth = true;

        ScrollRect sr;
        sr = (ScrollRect)((PanelInfoClass)target).gameObject.GetComponent<ScrollRect>();
        if (sr == null)
        {
            sr = (ScrollRect)((GameObject)target).gameObject.AddComponent<ScrollRect>();
        }

        GameObject tempHScroll;
        try
        {
            tempHScroll = ((PanelInfoClass)target).gameObject.transform.Find("Scrollbar Horizontal").gameObject;
            DestroyImmediate(tempHScroll);
        } catch(NullReferenceException e)
        {
            // Do nothing
        }

        sr.horizontal = false;

        if (PanelListSerialized == null) { Debug.LogError("Panle List failed load"); }
        if (PanelTemplateSerialized == null) { Debug.LogError("Panel Template failed load"); }
    }

    private int AddPanelToContent(string name)
    {
        Color defaultColor = Color.gray;
        int array_size = PanelListSerialized.arraySize;
        if (name == null)
        {
            name = "DefaultPanelName";
        }

        var panelTemplateGO = (GameObject)PanelTemplateSerialized.objectReferenceValue;
        if (panelTemplateGO == null)
        {
            Debug.LogError("Panel Template missing!");
            return 1;
        }
        var newPanel = Instantiate(panelTemplateGO, new Vector3(0, 0, 0), Quaternion.identity, contentObject);
        newPanel.name = name;
        setPanelValues(newPanel, name, defaultColor);
        PanelListSerialized.InsertArrayElementAtIndex(array_size);
        PanelListSerialized.GetArrayElementAtIndex(array_size).FindPropertyRelative("panelGO").objectReferenceValue = newPanel;
        PanelListSerialized.GetArrayElementAtIndex(array_size).FindPropertyRelative("name").stringValue = name;
        PanelListSerialized.GetArrayElementAtIndex(array_size).FindPropertyRelative("value").intValue = 0;
        PanelListSerialized.GetArrayElementAtIndex(array_size).FindPropertyRelative("color").colorValue = defaultColor;

        serializedObject.ApplyModifiedProperties();
        return 0;
    }

    private void setPanelValues(GameObject panel, string name)
    {
        Text[] panelText = panel.GetComponentsInChildren<Text>();
        foreach (Text text in panelText)
        {
            if (text.name == "Resource")
            {
                text.text = name;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void setPanelValues(GameObject panel, string name, Color color)
    {
        setPanelValues(panel, name);
        panel.GetComponent<Image>().color = color;

        serializedObject.ApplyModifiedProperties();
    }

    private void RemovePanelToContent(int indexToRemove)
    {
        int array_size = PanelListSerialized.arraySize;

        GameObject gameObjectToRemove = (GameObject) PanelListSerialized.GetArrayElementAtIndex(indexToRemove).FindPropertyRelative("panelGO").objectReferenceValue;
        DestroyImmediate(gameObjectToRemove);
        PanelListSerialized.DeleteArrayElementAtIndex(indexToRemove);

        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();
        int array_size = PanelListSerialized.arraySize;

        PanelTemplateSerialized.objectReferenceValue = EditorGUILayout.ObjectField("Panel Template", PanelTemplateSerialized.objectReferenceValue, typeof(GameObject), true);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("+", GUILayout.Width(25), GUILayout.Height(25)))
        {
            if (AddPanelToContent("DefaultPanelName") != 0)
            {
                return;
            }
        }
        GUILayout.EndHorizontal();

        for (int i = array_size - 1; i >= 0; --i)
        {
            string resoureName;
            int value;
            Color color;
            GameObject panelGO = (GameObject)PanelListSerialized.GetArrayElementAtIndex(i).FindPropertyRelative("panelGO").objectReferenceValue;
            var panel_serialized = PanelListSerialized.GetArrayElementAtIndex(i);

            GUILayout.BeginHorizontal("box");
            EditorGUIUtility.fieldWidth = 20;
            EditorGUIUtility.labelWidth = 20;
            panel_serialized.FindPropertyRelative("color").colorValue = EditorGUILayout.ColorField(panel_serialized.FindPropertyRelative("color").colorValue);
            color = panel_serialized.FindPropertyRelative("color").colorValue;

            EditorGUIUtility.fieldWidth = 0;
            EditorGUIUtility.labelWidth = 40;
            panel_serialized.FindPropertyRelative("name").stringValue = EditorGUILayout.TextField("Name", panel_serialized.FindPropertyRelative("name").stringValue);
            panelGO.name = panel_serialized.FindPropertyRelative("name").stringValue;
            resoureName = panel_serialized.FindPropertyRelative("name").stringValue;

            //panel_serialized.FindPropertyRelative("value").intValue = EditorGUI.PropertyField
            //value = panel_serialized.FindPropertyRelative("value").intValue;

            EditorGUIUtility.fieldWidth = 0;
            EditorGUIUtility.labelWidth = 0;

            // Update panel value
            setPanelValues(panelGO, resoureName, color);
            //EditorGUILayout.LabelField("Level", panel_serialized.FindPropertyRelative("Level").intValue.ToString());

            if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(25)))
            {
                RemovePanelToContent(i);
            }

            GUILayout.EndHorizontal();

        }
        
        if (PanelTemplateSerialized != null)
        {
            float panel_height;

            try
            {
                panel_height = ((GameObject)PanelTemplateSerialized.objectReferenceValue).GetComponent<RectTransform>().rect.height;
                RectTransform contentRect = contentObject.GetComponent<RectTransform>();
                contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, array_size * panel_height);
            }
            catch (NullReferenceException e)
            {
                // Do nothing
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

//GameObject scriptObject;
//int panel_count;

//Transform[] transform = scriptObject.GetComponentsInChildren<Transform>();

//panel_count = transform.GetLength(0);
//foreach (Transform child in transform)
//{
//    Debug.Log("> Child name: " + child.name.ToString());
//}