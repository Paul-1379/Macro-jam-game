using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Mirror : MonoBehaviour
{
    [SerializeField] private Material duplicatedMaterial;
    [Header("Mirror")]
    [SerializeField] private Transform scanZoneEffect;
    [SerializeField] private BoxCollider2D scanZoneCollider;
    [SerializeField] private Transform duplicatingZoneEffectPrefab;
    [SerializeField] private Transform duplicatedObjectsParent;
    [SerializeField] private GameObject[] enabledObjects;
    [SerializeField] private float maxAngle;
    [SerializeField] private bool canMove;
    [Header("Sounds")]
    [SerializeField] private AudioSource enablingSource;
    [SerializeField] private AudioClip[] enablingClips;
    [SerializeField] private AudioSource duplicatingSource;
    [SerializeField] private AudioClip[] duplicatingClips;
    [Header("Lights colors")]
    [SerializeField] private Color scanColor;
    [SerializeField] private Color duplicatingColor;
    
    private Vector2 _preEnabledUpVector;
    private List<GameObject> _duplicatedObjects;
    private List<GameObject> _zoneEffects;
    private bool _duplicatingEnabled;
    
    [HideInInspector]
    public bool IsEnabled
    {
        get => _isEnabled;
        set { ChangeActivationObjectsStates(value); _isEnabled = value; }
    }

    private bool _isEnabled;
    private void Awake()
    {
        _duplicatedObjects = new List<GameObject>();
        _zoneEffects = new List<GameObject>
        {
            scanZoneEffect.gameObject
        };
        IsEnabled = false;
    }

    private void OnEnable()
    {
        GameManager.Instance.mirrors.Add(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.mirrors.Remove(this);
    }

    private void ChangeActivationObjectsStates(bool state)
    {
        enablingSource.clip = enablingClips[Random.Range(0, enablingClips.Length)];
        enablingSource.Play();
        
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
        transform.RotateAround(transform.position, Vector3.back, angle / 2);
        duplicatedObjectsParent.RotateAround(duplicatedObjectsParent.position, Vector3.back, angle / 2);
        if (_duplicatingEnabled)
        {
            LimitAngle(-Mathf.Sign(angle));
        }
        else
        {
            _preEnabledUpVector = duplicatedObjectsParent.up;
        }
    }

    private void LimitAngle(float rotateDir)
    {
        float currentAngle = Vector2.Angle(duplicatedObjectsParent.up, _preEnabledUpVector);
        if (currentAngle > maxAngle)
        {
            float angleCorrection = (currentAngle - maxAngle) / 2 * rotateDir;
            transform.RotateAround(transform.position, Vector3.back, angleCorrection);
            duplicatedObjectsParent.RotateAround(duplicatedObjectsParent.position, Vector3.back, angleCorrection);
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
        
        duplicatingSource.clip = duplicatingClips[Random.Range(0, duplicatingClips.Length)];
        duplicatingSource.Play();
        
        DuplicateColliders();
        
        scanZoneEffect.parent = transform.parent;
        CreateZoneEffect(true);
    }

    private Transform CreateZoneEffect(bool duplicating = false)
    {
        var zoneEffect = Instantiate(duplicatingZoneEffectPrefab, transform.position, transform.rotation,
            duplicating? duplicatedObjectsParent : transform);
        zoneEffect.GetComponentInChildren<Light2D>().color = duplicating? duplicatingColor : scanColor;
        _zoneEffects.Add(zoneEffect.gameObject);
        return zoneEffect;
    }
    private void DuplicateColliders()
    {
        List<Collider2D> overlappingColliders = new();
        scanZoneCollider.Overlap(new ContactFilter2D().NoFilter(), overlappingColliders);
        foreach (var coll in overlappingColliders.Where(coll => !coll.CompareTag("CannotBeDuplicated") && coll.gameObject != gameObject))
        {
            var go = coll.gameObject;
            DisableDuplicatedComponents(go);
            var duplicatedObject = Instantiate(coll.gameObject, coll.transform.parent);
            
            InitSpriteDuplicated(duplicatedObject);

            _duplicatedObjects.Add(duplicatedObject);
            duplicatedObject.transform.SetParent(duplicatedObjectsParent, true);
        }
    }

    private void InitSpriteDuplicated(GameObject duplicatedObject)
    {
        if (!duplicatedObject.TryGetComponent<SpriteRenderer>(out var spriteRendererDuplicated)) return;
        spriteRendererDuplicated.material = duplicatedMaterial;
        var newColor = spriteRendererDuplicated.color;
        newColor.a = .5f;
        spriteRendererDuplicated.color = newColor;
    }

    private static void DisableDuplicatedComponents(GameObject go)
    {
        if (go.TryGetComponent<Rigidbody2D>(out var rb))
        {
            Destroy(rb);
        }
    }
}
