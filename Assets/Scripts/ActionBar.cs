using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour
{
    public static ActionBar Instance;
    [SerializeField] private Image[] shapeSlots;
    [SerializeField] private Image[] animalSlots;
    [SerializeField] private int maxSlots = 7;

    private List<ShapeData> _collected = new List<ShapeData>();

    private void Awake()
    {
        Instance = this;
    }
    
    public void AddShape(Shape shape)
    {
        if (_collected.Count >= maxSlots)
        {
            LevelManager.Instance.GameOver();
            return;
        }
        
        _collected.Add(shape.Data);
        UpdateUI();
        CheckMatches();
    }

    private void CheckMatches()
    {
        var matches = _collected
            .GroupBy(s => new { s.shapeSprite, s.animalSprite })
            .FirstOrDefault(g => g.Count() >= 3);

        if (matches != null)
        {
            RemoveMatches(matches.Key.shapeSprite, matches.Key.animalSprite);
        }
    }

    private void RemoveMatches(Sprite shapeSprite, Sprite animalSprite)
    {
        var removedCount = _collected.RemoveAll(s => 
            s.shapeSprite == shapeSprite && 
            s.animalSprite == animalSprite
        );
    
        LevelManager.Instance.ShapesRemoved(removedCount);
        UpdateUI();
        
        LevelManager.Instance.Invoke(nameof(LevelManager.CheckWinCondition), 0.1f);
    
    }

    private void UpdateUI()
    {
        for (var i = 0; i < shapeSlots.Length; i++)
        {
            if (i < _collected.Count)
            {
                shapeSlots[i].sprite = _collected[i].shapeSprite;
                animalSlots[i].sprite = _collected[i].animalSprite;
                
                shapeSlots[i].gameObject.SetActive(true);
                animalSlots[i].gameObject.SetActive(true);
            }
            else
            {
                shapeSlots[i].gameObject.SetActive(false);
                animalSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public int Length()
    {
        return _collected.Count;
    }

    public void Clear()
    {
        _collected.Clear();
        UpdateUI();
    }
}