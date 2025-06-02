using UnityEngine;

public class StickyShape : MonoBehaviour
{
    [SerializeField] private float stickyForce = 10f;
    [SerializeField] private float maxDistance = 2f;
    
    private Rigidbody2D _rb;
    private Collider2D _collider;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }
    
    private void FixedUpdate()
    {
        var nearbyColliders = Physics2D.OverlapCircleAll(transform.position, maxDistance);
        
        foreach (var otherCollider in nearbyColliders)
        {
            if (otherCollider == _collider) continue;
            
            var otherShape = otherCollider.GetComponent<Shape>();
            if (!otherShape || otherShape.IsFrozen) continue;
            
            Vector2 direction = otherCollider.transform.position - transform.position;
            otherCollider.attachedRigidbody.AddForce(-direction.normalized * stickyForce);
        }
    }
}