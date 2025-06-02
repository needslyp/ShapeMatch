using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] private GameObject shapePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private List<ShapeData> shapesLibrary;

    [SerializeField] private float spawnDelay = 0.2f;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    
    [SerializeField] private float freezeDelayTime = 5f;
    
    private int _shapesRemovedCount;
    
    private void Awake()
    {
        Instance = this;
        LoadSpritesAutomatically();
    }

    private void LoadSpritesAutomatically()
    {
        ShapeData[] allShapeData = Resources.LoadAll<ShapeData>("ShapeData");
        shapesLibrary = new List<ShapeData>(allShapeData);

        Debug.Log($"Loaded {shapesLibrary.Count} ShapeData assets");
    }

    private void StartSpawning(int totalCount)
    {
        StartCoroutine(SpawnShapesWithDelay(totalCount));
    }

    private List<ShapeData> GetShuffledShapeData(int totalCount)
    {
        var shuffledList = new List<ShapeData>();

        var normalShapes = shapesLibrary.Where(s => s.specialType == SpecialType.Normal).ToList();
        if (normalShapes.Count == 0) normalShapes = shapesLibrary.ToList();

        for (var i = 0; i < totalCount / 3 - 4; i++)
        {
            var randomData = normalShapes[Random.Range(0, normalShapes.Count)];
            for (var j = 0; j < 3; j++)
            {
                shuffledList.Add(randomData);
            }
        }

        AddSpecialShapes(shuffledList);

        for (var i = 0; i < shuffledList.Count; i++)
        {
            var randomIndex = Random.Range(i, shuffledList.Count);
            (shuffledList[randomIndex], shuffledList[i]) = (shuffledList[i], shuffledList[randomIndex]);
        }

        return shuffledList;
    }
    
    private void AddSpecialShapes(List<ShapeData> list)
    {
        var specialTypes = new[] { SpecialType.Heavy, SpecialType.Sticky, SpecialType.Frozen };
    
        foreach (var type in specialTypes)
        {
            var specialShapes = shapesLibrary.Where(s => s.specialType == type).ToList();
            if (specialShapes.Count == 0) continue;
        
            var specialData = specialShapes[Random.Range(0, specialShapes.Count)];
            for (var j = 0; j < 3; j++)
            {
                list.Add(specialData);
            }
        }
    }

    private IEnumerator SpawnShapesWithDelay(int totalCount)
    {
        var shuffledShapes = GetShuffledShapeData(totalCount);

        foreach (var shapeData in shuffledShapes)
        {
            var spawnPos = spawnPoint.position;
            spawnPos.x += Random.Range(-75, 75);

            var shapeObj = Instantiate(shapePrefab, spawnPos, Quaternion.identity);
            var shape = shapeObj.GetComponent<Shape>();
            shape.Init(shapeData);
            shape.SetFreezeDelay(freezeDelayTime);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void RestartLevel()
    {
        var allShapes = FindObjectsOfType<Shape>(includeInactive: true);
        var allShapesCount = allShapes.Length;
        foreach (var shape in allShapes)
        {
            if (shape != null && shape.gameObject != null)
                Destroy(shape.gameObject);
        }

        ActionBar.Instance.Clear();

        StartCoroutine(SpawnShapesWithDelay(allShapesCount));
    }

    private void WinGame()
    {
        winScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        loseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void CheckWinCondition()
    {
        StartCoroutine(CheckWinWithDelay());
    }

    private IEnumerator CheckWinWithDelay()
    {
        yield return null; 
        
        var shapes = FindObjectsOfType<Shape>()
            .Where(s => s != null && s.gameObject != null)
            .ToList();

        if (shapes.Count == 0 && ActionBar.Instance.Length() == 0)
        {
            WinGame();
        }
    }
    
    public void ShapesRemoved(int count)
    {
        _shapesRemovedCount += count;

        if (_shapesRemovedCount < 5) return;
        UnfreezeShapes();
        _shapesRemovedCount = 0;
    }

    private void UnfreezeShapes()
    {
        var frozenShapes = FindObjectsOfType<Shape>().Where(s => s.IsFrozen).ToList();
        foreach (var shape in frozenShapes)
        {
            shape.UnfreezeShape();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        var allShapes = FindObjectsOfType<Shape>();
        foreach (var shape in allShapes)
        {
            Destroy(shape.gameObject);
        }
        ActionBar.Instance.Clear();
        
        StartSpawning(51);
    }

    private void Start()
    {
        StartSpawning(51);
    }
}
