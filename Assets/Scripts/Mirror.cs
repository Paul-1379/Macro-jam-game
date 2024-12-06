using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Mirror : MonoBehaviour
{
    [SerializeField] private Transform scanZoneEffect;
    [SerializeField] private BoxCollider2D scanZoneCollider;
    [SerializeField] private Transform duplicatingZoneEffectPrefab;
    [SerializeField] private Transform mirrorZoneEffectparent;
    
    private List<GameObject> _toDuplicate;
    public bool DuplicatingEnabled {get; private set;}

    private void Awake()
    {
        _toDuplicate = new List<GameObject>();
    }

    public void Move(Vector2 movement)
    {
        if (!DuplicatingEnabled)
        {
            transform.position += (Vector3)movement;
        }
    }

    public void Rotate(float angle)
    {
        transform.RotateAround(transform.position, Vector3.back, angle);
    }

    public void ToggleMode()
    {
        if (!DuplicatingEnabled)
        {
            EnableDuplicating();
        }
    }

    private void EnableDuplicating()
    {
        DuplicatingEnabled = true;
        
        DuplicateColliders();
        
        scanZoneEffect.parent = transform.parent;
        Instantiate(duplicatingZoneEffectPrefab, transform.position, transform.rotation, mirrorZoneEffectparent);
    }

    private void DuplicateColliders()
    {
        _toDuplicate.Clear();
        List<Collider2D> overlappingColliders = new();
        scanZoneCollider.Overlap(new ContactFilter2D().NoFilter(), overlappingColliders);
        foreach (var coll in overlappingColliders.Where(coll => !coll.CompareTag("CannotBeDuplicated")))
        {
            var go = coll.gameObject;
            DisableDuplicatedComponents(go);
            _toDuplicate.Add(coll.gameObject);
            var duplicatedObject = Instantiate(coll.gameObject);
            Utilities.SetParent(duplicatedObject.transform, transform);
        }
    }
    private static void DisableDuplicatedComponents(GameObject go)
    {
        if (go.TryGetComponent<Rigidbody2D>(out var rb))
        {
            Destroy(rb);
        }
    }
}
