using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Video;
using Random = UnityEngine.Random;

public class Shape : MonoBehaviour
{
    private SpriteRenderer _shapeRenderer;
    private SpriteRenderer _animalRenderer;
    private Rigidbody2D _rb;
    private Collider2D _collider;

    public ShapeData Data { get; private set; }
    public bool IsFrozen { get; private set; }

    private float _freezeDelay;
    

    private void Awake()
    {
        _shapeRenderer = GetComponent<SpriteRenderer>();
        
        var animalObj = transform.Find("Animal");
        if (animalObj == null) return;
        _animalRenderer = animalObj.GetComponent<SpriteRenderer>();
        
        _rb = GetComponent<Rigidbody2D>();
        gameObject.tag = "Shape";
    }

    private void Start()
    {
        if (_rb == null) return;
        
        ApplySpecialProperties();
        
        _rb.AddForce(new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f)), ForceMode2D.Impulse);
        if (Data.specialType == SpecialType.Frozen)
        {
            StartCoroutine(FreezeAfterDelay());
        }
    }

    private void ApplySpecialProperties()
    {
        switch (Data.specialType)
        {
            case SpecialType.Heavy:
                _rb.mass = 10f;
                break;
                
            case SpecialType.Sticky:
                _shapeRenderer.color = new Color(0.5f, 0.9f, 0f, 1f);
                gameObject.AddComponent<StickyShape>();
                break;
                
            case SpecialType.Frozen:
                _shapeRenderer.color = new Color(0f, 0.5f, 1f, 1f);
                break;

            case SpecialType.Normal:
                break;
        }
    }

    private IEnumerator FreezeAfterDelay()
    {
        yield return new WaitForSeconds(_freezeDelay);
        FreezeShape();
    }
    
    public void SetFreezeDelay(float delay)
    {
        _freezeDelay = delay;
    }

    
    private void FreezeShape()
    {
        IsFrozen = true;
        _rb.bodyType = RigidbodyType2D.Static;
    }

    public void UnfreezeShape()
    {
        IsFrozen = false;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _shapeRenderer.color = Color.white;
    }

    private void CreateCollider(ShapeData data)
    {
        if (!_shapeRenderer || !_shapeRenderer.sprite)
        {
            AddFallbackCollider();
            return;
        }

        Sprite sprite = _shapeRenderer.sprite;

        if (sprite.GetPhysicsShapeCount() == 0)
        {
            AddFallbackCollider();
            return;
        }

        CreatePreciseCollider(sprite);
    }

    private void CreatePreciseCollider(Sprite sprite)
    {
        var coll = gameObject.AddComponent<PolygonCollider2D>();
        coll.autoTiling = true;
        
        var path = new List<Vector2>();
        for (var i = 0; i < sprite.GetPhysicsShapeCount(); i++)
        {
            path.Clear();
            sprite.GetPhysicsShape(i, path);
            coll.SetPath(i, path);
        }
    }

    private void AddFallbackCollider()
    {
        var boxCollider = gameObject.AddComponent<BoxCollider2D>();
        if (!_shapeRenderer)
        {
            boxCollider.size = _shapeRenderer.bounds.size;
        }
    }
    
    public void Init(ShapeData data)
    {
        Data = data;
        _shapeRenderer.sprite = data.shapeSprite;
        _animalRenderer.sprite = data.animalSprite;
        
        CreateCollider(data);
    }

    private void OnMouseDown()
    {
        if (IsFrozen) return;
    
        gameObject.tag = "Destroying";
    
        ActionBar.Instance.AddShape(this);
        Destroy(gameObject);
    
        Invoke(nameof(DelayedWinCheck), 0.1f);
    }

    private void DelayedWinCheck()
    {
        LevelManager.Instance.CheckWinCondition();
    }
}