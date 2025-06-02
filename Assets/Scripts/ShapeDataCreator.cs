#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ShapeDataCreator : EditorWindow
{
    [MenuItem("Tools/Create ShapeData Assets")]
    public static void ShowWindow()
    {
        GetWindow<ShapeDataCreator>("Shape Data Creator");
    }

    private Object _shapeFolder;
    private Object _animalFolder;
    private const string _assetPath = "Assets/Resources/ShapeData";

    private void OnGUI()
    {
        GUILayout.Label("Shape Data Generator", EditorStyles.boldLabel);
        
        _shapeFolder = EditorGUILayout.ObjectField("Shapes Folder", _shapeFolder, typeof(Object), false);
        _animalFolder = EditorGUILayout.ObjectField("Animals Folder", _animalFolder, typeof(Object), false);
        
        if (GUILayout.Button("Generate ShapeData Assets"))
        {
            GenerateAllShapeData();
        }
    }

    private void GenerateAllShapeData()
    {
        if (!Directory.Exists(_assetPath))
        {
            Directory.CreateDirectory(_assetPath);
        }

        var shapeSprites = LoadSpritesFromFolder(AssetDatabase.GetAssetPath(_shapeFolder));
        var animalSprites = LoadSpritesFromFolder(AssetDatabase.GetAssetPath(_animalFolder));

        int counter = 0;
        SpecialType[] allTypes = (SpecialType[])Enum.GetValues(typeof(SpecialType));
    
        foreach (var shapeSprite in shapeSprites)
        {
            foreach (var animalSprite in animalSprites)
            {
                ShapeData data = ScriptableObject.CreateInstance<ShapeData>();
                data.shapeSprite = shapeSprite;
                data.animalSprite = animalSprite;
            
                data.specialType = Random.Range(0, 10) < 7 ? SpecialType.Normal : 
                    allTypes[Random.Range(1, allTypes.Length)];
            
                var assetName = $"SD_{shapeSprite.name}_{animalSprite.name}_{data.specialType}.asset";
                var fullPath = Path.Combine(_assetPath, assetName);
                AssetDatabase.CreateAsset(data, fullPath);
            
                counter++;
                EditorUtility.DisplayProgressBar("Generating...", $"Created {counter} assets", 
                    counter / (float)(shapeSprites.Length * animalSprites.Length));
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    Sprite[] LoadSpritesFromFolder(string folderPath)
    {
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
        Sprite[] sprites = new Sprite[guids.Length];
        
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        
        return sprites;
    }
}
#endif