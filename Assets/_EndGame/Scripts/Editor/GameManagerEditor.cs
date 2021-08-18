using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public enum ManagerState
{
    Weapons,
    Abilities
}

public class GameManagerEditor : OdinMenuEditorWindow
{  
    private int enumIndex;
    private DrawSelected<WeaponScriptableObject> drawWeapons = new DrawSelected<WeaponScriptableObject>();
    private DrawSelected<AbilityScriptableObject> drawAbilities = new DrawSelected<AbilityScriptableObject>();
    private bool treeRebuild;
    
    // paths to ScriptableObjects
    private string weaponsPath = "Assets/_EndGame/Resources/GameData/Weapons";
    private string abilitiesPath = "Assets/_EndGame/Resources/GameData/Abilities";
    
    [LabelText("Editor Mode")] [LabelWidth(100f)] [EnumToggleButtons] [ShowInInspector][OnValueChanged("ManagerStateChange")]
    private ManagerState managerState;
    
    [MenuItem("✧ End Game ✧/Game Editor")]
    private static void OpenWindow()
    {
        GetWindow<GameManagerEditor>().Show();
    }

    protected override void Initialize()
    {
        drawWeapons.SetPath(weaponsPath);
        drawAbilities.SetPath(abilitiesPath);
    }

    private void ManagerStateChange()
    {
        treeRebuild = true;
    }


    protected override void OnGUI()
    {
        if (treeRebuild && Event.current.type == EventType.Layout)
        {
            ForceMenuTreeRebuild();
            treeRebuild = false;
        }
        SirenixEditorGUI.Title("✧ End Game ✧", "Game Manager", TextAlignment.Center, true,true);
        EditorGUILayout.Space();
        
        switch (managerState)
        {
            case ManagerState.Weapons:
            case ManagerState.Abilities:
                DrawEditor(enumIndex);
                break;
            default:
                break;
        }
        EditorGUILayout.Space();

        base.OnGUI();
    }


    protected override void DrawEditors()
    {
        switch (managerState)
        {
            case ManagerState.Weapons:
                drawWeapons.SetSelected(this.MenuTree.Selection.SelectedValue);
                break;
            case ManagerState.Abilities:
                drawAbilities.SetSelected(this.MenuTree.Selection.SelectedValue);
                break;
            default:
                break;
        }
        
        DrawEditor((int)managerState);
    }

    protected override IEnumerable<object> GetTargets()
    {
       List<object> targets = new List<object>();
       targets.Add(drawWeapons);
       targets.Add(drawAbilities);
       targets.Add(base.GetTarget());

       enumIndex = targets.Count - 1;

       return targets;
    }

    protected override void DrawMenu()
    {
        switch (managerState)
        {
            case ManagerState.Weapons:
            case ManagerState.Abilities:
                base.DrawMenu();
                break;
            default:
                break;
        }
        
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree();

        switch (managerState)
        {
            case ManagerState.Weapons:
                tree.AddAllAssetsAtPath("Weapon Scriptable Objects", weaponsPath, typeof(WeaponScriptableObject));
                break;
            case ManagerState.Abilities:
                tree.AddAllAssetsAtPath("Ability Scriptable Objects", abilitiesPath, typeof(AbilityScriptableObject));
                break;
            default:
                break;
        }

        tree.SortMenuItemsByName();
        return tree;
    }
}

public class DrawSelected<T> where T : ScriptableObject
{
    [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    [Space(30)]
    public T selected;
    
    private string path;
    
    [PropertyOrder(-1)]
    [ColorFoldoutGroup("Add or Delete")]
    public string nameForNew;

    [PropertyOrder(-1)]
    [ColorFoldoutGroup("Add or Delete")]
    [Button]
    [GUIColor(0f,0.8f,0f)]
    public void CreateNew()
    {
        if (nameForNew.IsNullOrWhitespace())
        {
            // todo check for dupe names
            return;
        }

        T newItem = ScriptableObject.CreateInstance<T>();
        newItem.name = "New " + typeof(T).ToString();

        if (path == "")
        {
            path = "Assets/";
        }
        
        AssetDatabase.CreateAsset(newItem,path + "\\" + nameForNew + ".asset");
        AssetDatabase.SaveAssets();
        
        // reset window
        nameForNew = "";
    }

    [PropertyOrder(-1)]
    [ColorFoldoutGroup("Add or Delete", 0.1f,0.1f,0.1f, 0.3f)]
    [Button]
    [GUIColor(0.8f,0f,0f)]
    public void DeleteSelected()
    {
        if (selected != null)
        {
            string path = AssetDatabase.GetAssetPath(selected);

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }
    }

    public void SetSelected(object item)
    {
        var attempt = item as T;
        if (attempt != null) this.selected = attempt;
    }

    public void SetPath(string path)
    {
        this.path = path;
    }
}
