using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

public class Mirror : MonoBehaviour
{
    [SerializeField] private Transform scanZoneEffect;
    [SerializeField] private BoxCollider2D scanZoneCollider;
    [SerializeField] private Transform duplicatingZoneEffectPrefab;
    [SerializeField] private Transform mirrorZoneEffectParent;
    [SerializeField] private Transform duplicatedObjectsParent;
    [SerializeField] private GameObject[] enabledObjects;
    [SerializeField] private float maxAngle;
    [SerializeField] private bool canMove;
    
    private Vector2 preEnabledUpVector;
    private List<GameObject> _duplicatedObjects;
    private List<GameObject> _zoneEffects;
    private bool _duplicatingEnabled;
    
    [HideInInspector]
    public bool IsEnabled
    {
        get => IsEnabled;
        set => ChangeActivationObjectsStates(value);
    }

    private void Awake()
    {
        _duplicatedObjects = new List<GameObject>();
        _zoneEffects = new List<GameObject>
        {
            scanZoneEffect.gameObject
        };
    }

    private void ChangeActivationObjectsStates(bool state)
    {
        foreach (var obj in enabledObjects)
        {
            obj.SetActive(state);
        }
    }
    public void Move(Vector2 movement)
    {
        if (canMove && !_duplicatingEnabled)
        {
            transform.position += (Vector3)movement;
        }
    }

    public void Rotate(float angle)
    {
        transform.RotateAround(transform.position, Vector3.back, angle);
        if (_duplicatingEnabled)
        {
            if (Vector2.Angle(transform.up, preEnabledUpVector) > maxAngle)
            {
                transform.RotateAround(transform.position, Vector3.back, -angle);
            }
        }
        else
        {
            preEnabledUpVector = transform.up;
        }
    }

    public void ToggleMode()
    {
        if (!_duplicatingEnabled)
        {
            EnableDuplicating();
        }
        else
        {
            DisableDuplicating();
        }
    }

    private void DisableDuplicating()
    {
        _duplicatingEnabled = false;

        ClearGameObjectList(ref _duplicatedObjects);
        ClearGameObjectList(ref _zoneEffects);
        
        scanZoneEffect = CreateZoneEffect();
    }

    private static void ClearGameObjectList(ref List<GameObject> list)
    {
        foreach (var go in list)
        {
            Destroy(go);
        }

        list.Clear();
    }
    private void EnableDuplicating()
    {
        _duplicatingEnabled = true;
        
        DuplicateColliders();
        
        scanZoneEffect.parent = transform.parent;
        CreateZoneEffect();
    }

    private Transform CreateZoneEffect()
    {
        var zoneEffect = Instantiate(duplicatingZoneEffectPrefab, transform.position, transform.rotation,
            mirrorZoneEffectParent);
        _zoneEffects.Add(zoneEffect.gameObject);
        return zoneEffect;
    }
    private void DuplicateColliders()
    {
        List<Collider2D> overlappingColliders = new();
        scanZoneCollider.Overlap(new ContactFilter2D().NoFilter(), overlappingColliders);
        foreach (var coll in overlappingColliders.Where(coll => !coll.CompareTag("CannotBeDuplicated")))
        {
            var go = coll.gameObject;
            DisableDuplicatedComponents(go);
            var duplicatedObject = Instantiate(coll.gameObject);
            _duplicatedObjects.Add(duplicatedObject);
            Utilities.SetParent(duplicatedObject.transform, duplicatedObjectsParent);
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
