using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OneDay.Core.Modules.Data;
using TowerDefense.Data;
using TowerDefense.Data.Definitions;
using TowerDefense.Scripts.Data.Definitions.Editor;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor;
using UnityEditor.Search;

public abstract class DefinitionWindow<T, Y> : EditorWindow where T : IDataObject, new() where Y: DefinitionTable<T>
{
    private List<DefinitionTable<T>> definitionTables = new();
    private VisualElement editorContainer;
    protected abstract string DefinitionName();
    protected abstract Type DefinitionType();

    private Action saveAction;
    public void CreateGUI()
    {
        var root = rootVisualElement;

        // Load heroes from a folder
        var loadDefinitionsButton = new Button(() =>
            {
                LoadDefinitionTables();
                ShowDefinitionTables();
            })
            { text = "Load definitions" };
        root.Add(loadDefinitionsButton);

        editorContainer = new VisualElement();
        root.Add(editorContainer);
    }

    private void LoadDefinitionTables()
    {
        definitionTables.Clear();
        // Change path to match your project structure
        string[] guids =
            AssetDatabase.FindAssets("t:" + DefinitionName(), new[] { "Assets/TowerDefense/Data/" });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var table = AssetDatabase.LoadAssetAtPath<Y>(path);
            if (table != null)
                definitionTables.Add(table);
        }

        Debug.Log($"Loaded {definitionTables.Count} definitions.");
    }

    private void ShowDefinitionTables()
    {
        editorContainer.Clear();

        foreach (var definition in definitionTables)
        {
            var stageRow = new VisualElement();
            stageRow.style.flexDirection = FlexDirection.Row;
            stageRow.style.marginBottom = 4;
            stageRow.style.alignItems = Align.Center;

            Label nameLabel = new Label(definition.name);
            nameLabel.style.minWidth = 150;

            var editButton = new Button(() => LoadEditor(definition)) { text = "Edit" };
            var uploadButton = new Button(() => UploadToFirebase(definition)) { text = "Upload" };
            var downloadButton = new Button(() => DownloadFromFirebase(definition)) { text = "Download" };

            stageRow.Add(nameLabel);
            stageRow.Add(editButton);
            stageRow.Add(uploadButton);
            stageRow.Add(downloadButton);


            editorContainer.Add(stageRow);
        }
    }

    private void UploadToFirebase(DefinitionTable<T> stageDefinition) =>
        FirebaseRemoteUploader.UploadToFirebase(TypeToDataKeyBinding.StageDefinitionsTable,
            stageDefinition.Serialize());

    private void DownloadFromFirebase(DefinitionTable<T> stageDefinition)
    {
        FirebaseRemoteUploader.DownloadFromFirebase(TypeToDataKeyBinding.StageDefinitionsTable)
            .ContinueWith(json =>
            {
                stageDefinition.Load(json);
                EditorUtility.SetDirty(stageDefinition);
                AssetDatabase.SaveAssets();
            });
    }

    private void LoadEditor(DefinitionTable<T> definitionTable)
    {
        rootVisualElement.Clear();
        var buttonContainer = VisualElementFactory.CreatePanel(true);
   
        saveAction = null;
        Button backButton = new Button(() =>
            {
                rootVisualElement.Clear();
                CreateGUI();
                ShowDefinitionTables();
            })
            { text = "< Back" };
        buttonContainer.Add(backButton);

        Button addNew = new Button(() =>
            {
                rootVisualElement.Clear();
                AddNewItemToTable(definitionTable);
                CreateGUI();
                ShowDefinitionTables();
            })
            { text = "Add" };
        buttonContainer.Add(addNew);
        Button saveButton = new Button(() =>
            {
                SaveDefinition(definitionTable);
            })
            { text = "Save" };
        buttonContainer.Add(saveButton);

        rootVisualElement.Add(buttonContainer);

        var assetField = new ObjectField("Editing Asset") { objectType = typeof(ScriptableObject), value = definitionTable };
        rootVisualElement.Add(assetField);
        var scrollView = VisualElementFactory.CreateScrollView(true, null);

        for (int i = 0; i < definitionTable.Data.Count; i++)
        {
            var customElement = CreateTableElement(definitionTable.Data[i]);
            saveAction += () => customElement.Save();
            scrollView.Add(customElement.VisualElement);
        }

        rootVisualElement.Add(scrollView);
    }

    private void SaveDefinition(DefinitionTable<T> definition)
    {
        Undo.RecordObject(definition, "Edit");
        saveAction?.Invoke();
        EditorUtility.SetDirty(definition);
        AssetDatabase.SaveAssets();
    }

    private void AddNewItemToTable(DefinitionTable<T> definitionTable)
    {
        definitionTable.Data.Add(new T());
        Undo.RecordObject(definitionTable, "Add");
        EditorUtility.SetDirty(definitionTable);
        AssetDatabase.SaveAssets();
    }

    protected abstract CustomElement CreateTableElement(T data);
}
