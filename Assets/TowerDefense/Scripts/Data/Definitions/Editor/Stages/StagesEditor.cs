using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TowerDefense.Data;
using TowerDefense.Data.Definitions;
using TowerDefense.Data.Definitions.CastlePrototype.Data.Definitions;
using TowerDefense.Scripts.Data.Definitions.Editor;
using TowerDefensePrototype.Scripts.Data.Definitions.Editor;
using UnityEditor.Search;

public class StagesEditorWindow : EditorWindow
{
    private List<StageDefinitionsTable> stagesDefinitions = new();
    private VisualElement editorContainer;

    [MenuItem("TD/Stage Definition Editor")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<StagesEditorWindow>();
        wnd.titleContent = new GUIContent("Stage Definition Editor");
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;

        // Load heroes from a folder
        var loadStagesDefinitionsButton = new Button(() =>
            {
                LoadStagesDefinitions();
                ShowStages();
            })
            { text = "Load stage definitions" };
        root.Add(loadStagesDefinitionsButton);

        editorContainer = new VisualElement();
        root.Add(editorContainer);
    }

    private void LoadStagesDefinitions()
    {
        stagesDefinitions.Clear();

        // Change path to match your project structure
        string[] guids =
            AssetDatabase.FindAssets("t:StageDefinitionsTable", new[] { "Assets/TowerDefense/Data/" });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var stagesTable = AssetDatabase.LoadAssetAtPath<StageDefinitionsTable>(path);
            if (stagesTable != null)
                stagesDefinitions.Add(stagesTable);
        }

        Debug.Log($"Loaded {stagesDefinitions.Count} stages definitions.");
    }

    private void ShowStages()
    {
        editorContainer.Clear();

        foreach (var stage in stagesDefinitions)
        {
            var stageRow = new VisualElement();
            stageRow.style.flexDirection = FlexDirection.Row;
            stageRow.style.marginBottom = 4;
            stageRow.style.alignItems = Align.Center;

            Label nameLabel = new Label(stage.name);
            nameLabel.style.minWidth = 150;

            var editButton = new Button(() => ShowStageEditor(stage)) { text = "Edit" };
            var uploadButton = new Button(() => UploadToFirebase(stage)) { text = "Upload" };
            var downloadButton = new Button(() => DownloadFromFirebase(stage)) { text = "Download" };



            stageRow.Add(nameLabel);
            stageRow.Add(editButton);
            stageRow.Add(uploadButton);
            stageRow.Add(downloadButton);



            editorContainer.Add(stageRow);
        }
    }

    private void UploadToFirebase(StageDefinitionsTable stageDefinition) => 
        FirebaseRemoteUploader.UploadToFirebase(TypeToDataKeyBinding.StageDefinitionsTable, stageDefinition.Serialize());

    private void DownloadFromFirebase(StageDefinitionsTable stageDefinition)
    {
        FirebaseRemoteUploader.DownloadFromFirebase(TypeToDataKeyBinding.StageDefinitionsTable)
            .ContinueWith(json =>
            {
                stageDefinition.Load(json);
                EditorUtility.SetDirty(stageDefinition);
                AssetDatabase.SaveAssets();
            });
    }

    private void ShowStageEditor(StageDefinitionsTable stageDefinition)
    {
        var editorRoot = rootVisualElement;
        editorRoot.Clear();

        Button backButton = new Button(() =>
            {
                editorRoot.Clear();
                CreateGUI();
                ShowStages();
            })
            { text = "< Back to Stages" };
        editorRoot.Add(backButton);
        
        Button addNewStageButton = new Button(() =>
            {
                editorRoot.Clear();
                AddNewStage(stageDefinition);
                CreateGUI();
                ShowStages();
            })
            { text = "Add" };
        editorRoot.Add(addNewStageButton);
        
        
        var assetField = new ObjectField("Editing Asset")
            { objectType = typeof(StageDefinitionsTable), value = stageDefinition };
        editorRoot.Add(assetField);

        var scrollView = VisualElementFactory.CreateScrollView(true, null);
        
        editorRoot.Add(scrollView);
        
        for (int i = 0; i < stageDefinition.Data.Count; i++)
        {
            var stage = stageDefinition.Data[i];
            var rootStageElement = new GroupElement().Create();
            rootStageElement.VisualElement.style.maxWidth = 300;
            
            rootStageElement.VisualElement.Add(new Label( "Stage:" + (i+1)));
            
            rootStageElement.AddChild(new StageDifficultyElement(StageDifficultyCalculator.Calculate(stage)).Create());
            rootStageElement.VisualElement.Add( new Button(() => SaveStageDefinition(stageDefinition, rootStageElement)) { text = "Save" });
            
            
            rootStageElement.AddChild(new StageElement(stage).Create());
            rootStageElement.AddChild(new RewardElement(stage.Reward).Create());
            rootStageElement.AddChild(new WaveElement(stage.Waves).Create());
            
            scrollView.Add(rootStageElement.VisualElement);
        }
    }
    
    private void SaveStageDefinition(StageDefinitionsTable stageDefinition, CustomElement rootStageElement)
    {
        Undo.RecordObject(stageDefinition, "Edit Staged");
        rootStageElement.Save();
        EditorUtility.SetDirty(stageDefinition);
        AssetDatabase.SaveAssets();
        ShowStageEditor(stageDefinition);
    }
    private void AddNewStage(StageDefinitionsTable stageDefinitions)
    {
        stageDefinitions.Data.Add(new StageDefinition());
        Undo.RecordObject(stageDefinitions, "Added Stage");
        EditorUtility.SetDirty(stageDefinitions);
        AssetDatabase.SaveAssets();
    }
}
