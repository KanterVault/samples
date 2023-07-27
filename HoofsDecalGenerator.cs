//#define EXECUTE_ALWAYS

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if EXECUTE_ALWAYS
[ExecuteAlways]
[ExecuteInEditMode]
#endif
public class HoofsDecalGenerator : MonoBehaviour
{
    [SerializeField] private GameObject hoofDecalPrefab;
    [SerializeField] private Transform parentDecal;
    [SerializeField] private int decalCount = 16;
    [SerializeField] private float spawnOffset = 0.5f;
    [SerializeField] private float normalOffset = 0.01f;
    [SerializeField] private float collisionHeight = 1.1f;
    public bool BackWalk = false;

    private Transform _decalsRoot = null;
    private void OnEnable()
    {
        if (hoofDecalPrefab == null || _cachedDecals != null || _decalsRoot != null) return;
        _decalsRoot = new GameObject("HoofsDecalRoot").transform;
        if (parentDecal != null)
        {
            _decalsRoot.SetParent(parentDecal, true);
            _decalsRoot.position = parentDecal.position;
        }
        _cachedDecals = new GameObject[decalCount];
        for (var i = 0; i < decalCount; i++)
        {
            GameObject instanced = null;
            if (Application.isPlaying) instanced = Instantiate(hoofDecalPrefab);
#if UNITY_EDITOR
            else instanced = (GameObject)PrefabUtility.InstantiatePrefab(hoofDecalPrefab);
#endif
            instanced.transform.SetParent(_decalsRoot, false);
            instanced.SetActive(false);
            _cachedDecals[i] = instanced;
        }
    }

    private void OnDisable()
    {
        _nextIndex = 0;
        _lastPosition = Vector3.zero;

        if (_cachedDecals != null)
        {
            for (var i = 0; i < _cachedDecals.Length; i++)
            {
                if (_cachedDecals[i] != null)
                {
                    if (Application.isPlaying) Destroy(_cachedDecals[i]);
                    else DestroyImmediate(_cachedDecals[i]);
                }
                    
            }
            _cachedDecals = null;
        }
        if (_decalsRoot != null)
        {
            if (Application.isPlaying) Destroy(_decalsRoot.gameObject);
            else DestroyImmediate(_decalsRoot.gameObject);
            _decalsRoot = null;
        }
    }

    private GameObject[] _cachedDecals = null;
    private int _nextIndex = 0;
    private Vector3 _lastPosition = Vector3.zero;
    public void ForwardUpdate()
    {
        if (_cachedDecals == null || _cachedDecals.Length <= _nextIndex) return;
        if (decalCount != _cachedDecals.Length)
        {
            OnDisable();
            OnEnable();
            return;
        }
        if (Vector3.Distance(transform.position, _lastPosition) > spawnOffset)
        {
            var ray = new Ray(transform.position + Vector3.up, Vector3.down * collisionHeight);
            if (Physics.Raycast(ray, out var hit, collisionHeight, LayerMask.GetMask("Terrain")))
            {
                Debug.DrawRay(ray.origin, ray.direction * collisionHeight, Color.red, 1.0f);
                var decal = _cachedDecals[_nextIndex];
                if (decal == null)
                {
                    OnDisable();
                    OnEnable();
                    return;
                }

                if (!decal.activeInHierarchy) decal.SetActive(true);

                decal.transform.position = hit.point + hit.normal * normalOffset;
                var yRot = BackWalk ?
                    Vector3.ProjectOnPlane((_lastPosition - transform.position).normalized, Vector3.up) :
                    Vector3.ProjectOnPlane((transform.position - _lastPosition).normalized, Vector3.up);
                decal.transform.rotation =
                    Quaternion.LookRotation(hit.normal, -yRot) *
                    Quaternion.AngleAxis(180.0f, Vector3.right);

                _lastPosition = transform.position;
                _nextIndex++;
                if (_nextIndex >= _cachedDecals.Length) _nextIndex = 0;
            }
        }
    }
}
