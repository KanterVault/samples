using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using Assets.DuckType.Jiggle;
using Color = UnityEngine.Color;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class ViewCustomize : MonoBehaviour
{
    [SerializeField] private bool debugMainPlayer;
    [SerializeField] private bool debugStartInvoke;
    [SerializeField] private bool debugUpdateInvoke;

    [Space(40)]
    public CustomizeDataPackage LocalData = new CustomizeDataPackage();
    public ItemsChecker ItemsCheckerData = new ItemsChecker();
    [SerializeField] private ItemsReferences itemsReferences = new ItemsReferences();

    [Serializable]
    public class ItemsChecker
    {
        public bool itemCarrot;
        public bool itemCorn;
        public bool itemApple;
        public bool itemBracelet;
        public bool itemWatch;
        public bool itemGlass1;
        public bool itemGlass2;
        public bool itemGlass3;
        public bool itemHornBall;
        public bool itemHeadBow;
        public bool itemCap;
        public bool itemCone;
        public bool itemCowboyHat;
        public bool itemFeathers;
        public bool itemHorns1;
        public bool itemHorns2;
        public bool itemLamp;
        public bool itemButterly1;
        public bool itemButterly2;
        public bool itemButterly3;
        public bool itemButterly4;
        public bool itemHandkerchief;
        public bool itemTie;
        public bool itemScaf;
        public bool itemChoker;
        public bool itemBackBow;
        public bool itemPacks;
        public bool itemHarnessFront;
        public bool itemHarnessBack;
        public bool itemSwitter;
        public bool itemAviator;
        public bool itemSocks1;
        public bool itemSocks2;
        public bool itemBackpack1;
        public bool itemBackpack2;
    }
    [Serializable]
    public class ItemsReferences
    {
        [Header("ItemsView Static")]
        public GameObject itemCarrot;
        public GameObject itemCorn;
        public GameObject itemApple;
        public GameObject itemBracelet;
        public GameObject itemWatch;
        public GameObject itemGlass1;
        public GameObject itemGlass2;
        public GameObject itemGlass3;
        public GameObject itemHornBall;
        public GameObject itemHeadBow;
        public GameObject itemCap;
        public GameObject itemCone;
        public GameObject itemCowboyHat;
        public GameObject itemFeathers;
        public GameObject itemHorns1;
        public GameObject itemHorns2;
        public GameObject itemLamp;

        public Transform butterlyRoot;
        public GameObject itemButterly1;
        public GameObject itemButterly2;
        public GameObject itemButterly3;
        public GameObject itemButterly4;
        public GameObject itemHandkerchief;
        public GameObject itemTie;
        public GameObject itemScaf;

        public Transform chokerRoot;
        public GameObject itemChoker;
        public GameObject itemBackBow;
        public GameObject itemPacks;

        public Transform harnessFrontRoot;
        public GameObject itemHarnessFront;
        public Transform harnessBackRoot;
        public GameObject itemHarnessBack;
        
        [Header("ItemsView Dynamic")]
        public GameObject itemSwitterPrefab;
        public GameObject itemAviatorPrefab;
        public GameObject itemSocks1Prefab;
        public GameObject itemSocks2Prefab;
        public GameObject itemBackpack1Prefab;
        public GameObject itemBackpack2Prefab;
    }

    [Space(40)]
    public SkinnedMeshRenderer bodySkinnedMesh;
    [SerializeField] private Transform playerRootBone;
    [SerializeField] private Texture2D[] pupilPatterns;
    [SerializeField] private Texture2D[] patterns;
    [SerializeField] private Texture2D[] itemMask;
    [SerializeField] private Texture2D maleDecalTexture;
    [SerializeField] private Texture2D femaleDecalTexture;

    private GameObject _itemSwitterInstance;
    private GameObject _itemAviatorInstance;
    private GameObject _itemSocks1Instance;
    private GameObject _itemSocks2Instance;
    private GameObject _itemBackpack1Instance;
    private GameObject _itemBackpack2Instance;

    [Header("Hairs root references:")]
    [SerializeField] private Transform rootPhysic;
    [SerializeField] private Transform rootTail;
    [SerializeField] private Transform rootHead;
    [SerializeField] private Vector3[] tailsRotationEulers_Sit1;
    [SerializeField] private Vector3[] tailsRotationEulers_Sit2;

    [Header("Ears root references:")]
    public Transform earsTransformRight;
    public Transform earsTransformLeft;

    [Header("Emotions:")]
    [SerializeField] private GameObject teeth;
    [SerializeField] private GameObject tongue;
    [SerializeField] private GameObject tongueShowen;

    [Header("Horn and wings:")]
    [SerializeField] private GameObject horn;
    [SerializeField] private GameObject[] wingsBat;
    [SerializeField] private GameObject[] wingsPeg;
    [SerializeField] private GameObject eyes;
    [SerializeField] private GameObject hornLight;

    [Serializable]
    public enum RootName
    {
        None,
        Spine,
        ForwardRightUp,
        ForwardLeftUp,
        ForwardRightDown,
        ForwardLeftDown,
        BackRight,
        BackLeft,
        Neck
    }
    [Serializable]
    public class Root
    {
        public RootName name;
        public float offset;
        public float maxValue;
        public Transform direction;
        public Transform spRoot;
        public Transform spTarget;
        [HideInInspector] public Transform control;
    }
    [Header("Body root directions:")] [SerializeField] private Root[] forwards;

    [Header("Hairs prefabs references:")]
    [SerializeField] private GameObject[] tailsPrefabs;
    [SerializeField] private GameObject[] manesPrefabs;
    [SerializeField] private GameObject[] backmPrefabs;

    private static readonly int[] CachedHairsColors =
    {
        Shader.PropertyToID("Color_d9f800817a03467ea7f3244ea7cbcadd"),
        Shader.PropertyToID("Color_d9f800817a03467ea7f3244ea7cbcadd_1"),
        Shader.PropertyToID("Color_d9f800817a03467ea7f3244ea7cbcadd_2"),
        Shader.PropertyToID("Color_d9f800817a03467ea7f3244ea7cbcadd_3"),
        Shader.PropertyToID("Color_d9f800817a03467ea7f3244ea7cbcadd_4"),
        Shader.PropertyToID("Color_d9f800817a03467ea7f3244ea7cbcadd_5")
    };
    private static readonly int[] CachedWingsColors =
    {
        Shader.PropertyToID("Color_36615bffa5b44d9399560b1a7bccfd17"),
        Shader.PropertyToID("Color_732ae565c10143debec821cde6536fbd"),
        Shader.PropertyToID("Color_bc5461b062544a1cb82023c109c3866b")
    };
    private static readonly int[] CachedEyesParams =
{
        Shader.PropertyToID("Color_34766a089263412bb2c8a3232403ea8a"),
        Shader.PropertyToID("Color_c43d0d2d61734b05819e81a7a0fab66a"),
        Shader.PropertyToID("Vector1_a061f590f4934f0987d2f1b118e1cc9d")
    };
    private static readonly int Pattern0 = Shader.PropertyToID("Color_40acf69348b84a14a825d7b0ce541d2d");
    private static readonly int Pattern1 = Shader.PropertyToID("Color_321c8ddcbef444eca6772bfb4659be8f");
    private static readonly int ItemMask = Shader.PropertyToID("_ItemsMask");
    private static readonly int DecalTexture = Shader.PropertyToID("Texture2D_6e24fc4f3d8a4d98935ac556d08ea3bb");
    private static readonly int MainPattern = Shader.PropertyToID("Texture2D_31db5761348a4047a800d9d40a80a45e");
    private static readonly int PupilTexture = Shader.PropertyToID("Texture2D_a493df894c2d48e6a878513c4c2f62c4");
    private const float SpineMaxLength = 0.03f;
    private const float NeckMaxLenght = 0.03f;
    private Vector2 ClampHornSize = new Vector2(0.7f, 1.2f);
    private Vector2 ClampWingsSize = new Vector2(0.8f, 1.2f);

    private class CurrentParameters
    {
        public int Id = -1;
        public GameObject[] InstanceObjects;
        public Material Material;
    }
    private CurrentParameters _currentManeParam = new CurrentParameters();
    private CurrentParameters _currentTailParam = new CurrentParameters();
    private CurrentParameters _currentBackParam = new CurrentParameters();
    private Material _wingsMaterial;
    private bool _dreamResetEvent;
    [HideInInspector] public Material eyesMaterial;

    private List<Transform> _playerBones = new List<Transform>();

    private Transform _tailRoot;
    private Vector3 _tailSavedRotation;
    private float[] _tailSpringStrength;
    private float[] _tailDampening;
    private float[] _tailNoiseScale;
    private float[] _tailNoiseSpeed;
    private float[] _tailNoiseStrength;
    private bool[] _tailAddNoize;
    private Jiggle[] _tailJiggles;
    private int _lastTailIndex;
    private int _lastSitState = 0;
    private IEnumerator _updateTailRotate;
    private WaitForSeconds _updateTailRotateTimer = new WaitForSeconds(0.05f);

    private Light _hornlight;
    private Material _hornMeshMat;
    private Material _hornPSMat;
    private Material _hornTrailMat;

    private void SetupHorn()
    {
        _hornPSMat = hornLight.GetComponentInChildren<ParticleSystemRenderer>().material;
        hornLight.GetComponentInChildren<ParticleSystemRenderer>().material = _hornPSMat;
        _hornMeshMat = hornLight.GetComponentInChildren<MeshRenderer>().material;
        hornLight.GetComponentInChildren<MeshRenderer>().material = _hornMeshMat;
        _hornlight = hornLight.GetComponentInChildren<Light>();
        _hornTrailMat = new Material(hornLight.GetComponentsInChildren<ParticleSystemRenderer>()[1].trailMaterial);
        hornLight.GetComponentsInChildren<ParticleSystemRenderer>()[1].trailMaterial = _hornTrailMat;
        _hornTrailMat.color = _hornlight.color;
    }
    private void SetupEmotions()
    {
        tongue.SetActive(true);
        tongueShowen.SetActive(false);
    }
    private void SetupBodyRoots()
    {
        foreach (var forwardBone in forwards)
        {
            var root = new GameObject($"ControlRoot_{ forwardBone.spTarget.gameObject.name }");
            root.transform.SetParent(forwardBone.spRoot, false);
            forwardBone.spTarget.SetParent(root.transform, false);
            forwardBone.control = root.transform;
        }
    }
    private void RemoveBodyRoots()
    {
        foreach (var forwardBone in forwards)
        {
            if (forwardBone.control != null) forwardBone.control.position =
                forwardBone.spRoot.position +
                forwardBone.direction.forward *
                (forwardBone.maxValue * 1.0f);

            forwardBone.spTarget.SetParent(forwardBone.spRoot, false);
            if (forwardBone.control != null) Destroy(forwardBone.control.gameObject);
            forwardBone.spTarget.SetSiblingIndex(0);
        }
    }
    private void SetupPlayerBones()
    {
        _playerBones = new List<Transform>() { bodySkinnedMesh.rootBone }.Concat(bodySkinnedMesh.bones).ToList();
    }
    private void SetupBodyAndEyesMaterials()
    {
        var mat = bodySkinnedMesh.material;
        bodySkinnedMesh.material = mat;
        eyesMaterial = eyes.GetComponent<MeshRenderer>().material;
        eyes.GetComponent<MeshRenderer>().material = eyesMaterial;
    }
    private void SetupHornAndWingsMaterial()
    {
        var wingBatLeftRenderer = wingsBat[0].GetComponentInChildren<SkinnedMeshRenderer>();
        var wingBatRightRenderer = wingsBat[1].GetComponentInChildren<SkinnedMeshRenderer>();

        var wingPegLeftRenderer = wingsPeg[0].GetComponentInChildren<SkinnedMeshRenderer>();
        var wingPegRightRenderer = wingsPeg[1].GetComponentInChildren<SkinnedMeshRenderer>();

        _wingsMaterial = wingBatLeftRenderer.material;
        wingBatLeftRenderer.material = _wingsMaterial;
        wingBatRightRenderer.material = _wingsMaterial;
        wingPegLeftRenderer.material = _wingsMaterial;
        wingPegRightRenderer.material = _wingsMaterial;

        var hornRenderer = horn.GetComponentInChildren<MeshRenderer>();
        hornRenderer.material = bodySkinnedMesh.sharedMaterial;
    }
    private void SetupTailRotations(int tailId)
    {
        if (_lastSitState != 0 || tailId < 0 || tailId >= tailsPrefabs.Length) return;
        _lastTailIndex = tailId;
        _tailJiggles = rootTail.GetComponentsInChildren<Jiggle>();
        _tailRoot = rootTail.GetChild(0).GetChild(0);
        _tailSavedRotation = _tailRoot.localEulerAngles;
        _tailSpringStrength = new float[_tailJiggles.Length];
        _tailDampening = new float[_tailJiggles.Length];
        _tailNoiseScale = new float[_tailJiggles.Length];
        _tailNoiseSpeed = new float[_tailJiggles.Length];
        _tailNoiseStrength = new float[_tailJiggles.Length];
        _tailAddNoize = new bool[_tailJiggles.Length];
        for (var i = 0; i < _tailJiggles.Length; i++)
        {
            _tailSpringStrength[i] = _tailJiggles[i].SpringStrength;
            _tailDampening[i] = _tailJiggles[i].Dampening;
            _tailNoiseScale[i] = _tailJiggles[i].NoiseScale;
            _tailNoiseSpeed[i] = _tailJiggles[i].NoiseSpeed;
            _tailNoiseStrength[i] = _tailJiggles[i].NoiseStrength;
            _tailAddNoize[i] = _tailJiggles[i].AddNoise;
        }
    }
    private void TrySpawnHairs(int id, ref GameObject[] hairPrefabs, ref CurrentParameters currentParams, ref Transform rootMesh)
    {
        var selectedId = Mathf.Clamp(id, -1, hairPrefabs.Length - 1);
        if (currentParams.Id == selectedId) return;
        currentParams.Id = selectedId;

        if (currentParams.InstanceObjects != null) foreach (var t in currentParams.InstanceObjects) Destroy(t.gameObject);
        if (id < 0 || id >= hairPrefabs.Length) return;

        var instance = Instantiate(hairPrefabs[selectedId]);

        currentParams.InstanceObjects = new[]
        {
            instance.transform.GetChild(0).gameObject,
            instance.transform.GetChild(1).gameObject
        };

        Destroy(instance);

        rootPhysic.SetPositionAndRotation(
            bodySkinnedMesh.gameObject.transform.position,
            bodySkinnedMesh.gameObject.transform.rotation);
        currentParams.InstanceObjects[0].transform.SetParent(rootMesh);
        currentParams.InstanceObjects[1].transform.SetParent(rootPhysic);

        var mat = currentParams.InstanceObjects[0].GetComponentInChildren<Renderer>().material;
        currentParams.InstanceObjects[0].GetComponentInChildren<Renderer>().material = mat;
        currentParams.Material = currentParams.InstanceObjects[0].transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial;

        foreach (var tile in currentParams.InstanceObjects)
        {
            tile.transform.localPosition = Vector3.zero;
            tile.transform.localRotation = Quaternion.identity;
        }
    }
    private void UpdateRace(ref CustomizeDataPackage data)
    {
        wingsPeg[0].transform.parent.parent.localScale = Vector3.one * Mathf.Clamp(data.WingsSize, ClampWingsSize.x, ClampWingsSize.y);
        wingsPeg[1].transform.parent.parent.localScale = Vector3.one * Mathf.Clamp(data.WingsSize, ClampWingsSize.x, ClampWingsSize.y);
        horn.transform.localScale = Vector3.one * Mathf.Clamp(data.HornSize, ClampHornSize.x, ClampHornSize.y);

        for (var i = 0; i < 3; i++) _wingsMaterial.SetColor(CachedWingsColors[i], data.WingsColors[i].Color);

        switch (data.Race)
        {
            case 0:
                horn.gameObject.SetActive(false);
                wingsPeg[0].gameObject.SetActive(false);
                wingsPeg[1].gameObject.SetActive(false);
                wingsBat[0].gameObject.SetActive(false);
                wingsBat[1].gameObject.SetActive(false);
                bodySkinnedMesh.SetBlendShapeWeight(29, 0.0f);
                teeth.SetActive(false);
                eyesMaterial.SetTexture(PupilTexture, pupilPatterns[0]);
                break;
            case 1:
                horn.gameObject.SetActive(true);
                wingsPeg[0].gameObject.SetActive(false);
                wingsPeg[1].gameObject.SetActive(false);
                wingsBat[0].gameObject.SetActive(false);
                wingsBat[1].gameObject.SetActive(false);
                bodySkinnedMesh.SetBlendShapeWeight(29, 0.0f);
                teeth.SetActive(false);
                eyesMaterial.SetTexture(PupilTexture, pupilPatterns[0]);
                break;
            case 2:
                horn.gameObject.SetActive(false);
                wingsPeg[0].gameObject.SetActive(true);
                wingsPeg[1].gameObject.SetActive(true);
                wingsBat[0].gameObject.SetActive(false);
                wingsBat[1].gameObject.SetActive(false);
                bodySkinnedMesh.SetBlendShapeWeight(29, 0.0f);
                teeth.SetActive(false);
                eyesMaterial.SetTexture(PupilTexture, pupilPatterns[0]);
                break;
            case 3:
                horn.gameObject.SetActive(false);
                wingsPeg[0].gameObject.SetActive(false);
                wingsPeg[1].gameObject.SetActive(false);
                wingsBat[0].gameObject.SetActive(true);
                wingsBat[1].gameObject.SetActive(true);
                bodySkinnedMesh.SetBlendShapeWeight(29, 100.0f);
                teeth.SetActive(true);
                eyesMaterial.SetTexture(PupilTexture, pupilPatterns[1]);
                break;
        }
    }
    private void UpdateBodyRoots(ref CustomizeDataPackage data)
    {
        earsTransformRight.localScale = Vector3.one * data.EarsSizeSize;
        earsTransformRight.localRotation =
            Quaternion.AngleAxis(data.EarsRotateRightX, Vector3.right) *
            Quaternion.AngleAxis(-data.EarsRotateRightY, Vector3.forward);

        earsTransformLeft.localScale = Vector3.one * data.EarsSizeSize;
        earsTransformLeft.localRotation =
            Quaternion.AngleAxis(data.EarsRotateLeftX, Vector3.right) *
            Quaternion.AngleAxis(data.EarsRotateLeftY, Vector3.forward);

        float legsLenght = data.LegsLenght;
        foreach (var forwardBone in forwards)
        {

            void BoneTransform()
            {
                forwardBone.control.position =
                    forwardBone.spRoot.position +
                    forwardBone.direction.forward *
                    (forwardBone.maxValue * legsLenght);
            }

            switch (forwardBone.name)
            {
                case RootName.Neck:
                    forwardBone.control.position =
                        forwardBone.spRoot.position +
                        forwardBone.direction.forward *
                        (NeckMaxLenght * data.NeckLenght);
                    break;
                case RootName.Spine:
                    forwardBone.control.position =
                        forwardBone.spRoot.position +
                        forwardBone.direction.forward *
                        (SpineMaxLength * data.BackLenght);
                    break;
                case RootName.BackLeft: BoneTransform(); break;
                case RootName.BackRight: BoneTransform(); break;
                case RootName.ForwardLeftDown: BoneTransform(); break;
                case RootName.ForwardLeftUp: BoneTransform(); break;
                case RootName.ForwardRightDown: BoneTransform(); break;
                case RootName.ForwardRightUp: BoneTransform(); break;
                default:
                    forwardBone.control.position =
                        forwardBone.spRoot.position +
                        forwardBone.direction.forward *
                        (forwardBone.maxValue * forwardBone.offset);
                    break;
            }
        }
    }
    private void UpdateBodyAndEyesMaterials(ref CustomizeDataPackage data)
    {
        bodySkinnedMesh.sharedMaterial.SetColor(Pattern0, data.BodyColor0.Color);
        bodySkinnedMesh.sharedMaterial.SetColor(Pattern1, data.BodyColor1.Color);
        bodySkinnedMesh.sharedMaterial.SetTexture(MainPattern, patterns[Mathf.Clamp(data.Pattern, 0, patterns.Length - 1)]);
        bodySkinnedMesh.sharedMaterial.SetTexture(DecalTexture, data.Gender == 0 ? femaleDecalTexture : maleDecalTexture);

        if (ItemsCheckerData.itemAviator) bodySkinnedMesh.sharedMaterial.SetTexture(ItemMask, itemMask[0]);
        else if (ItemsCheckerData.itemSwitter) bodySkinnedMesh.sharedMaterial.SetTexture(ItemMask, itemMask[1]);
        else if (ItemsCheckerData.itemSocks1) bodySkinnedMesh.sharedMaterial.SetTexture(ItemMask, itemMask[2]);
        else bodySkinnedMesh.sharedMaterial.SetTexture(ItemMask, null);

        eyesMaterial.SetColor(CachedEyesParams[0], data.EyesColors[0].Color);
        eyesMaterial.SetColor(CachedEyesParams[1], data.EyesColors[1].Color);
        eyesMaterial.SetFloat(CachedEyesParams[2], data.PupilSize);
    }
    private void UpdateHairsMaterials(ref CustomizeDataPackage data)
    {
        for (var i = 0; i < CachedHairsColors.Length; i++)
        {
            _currentManeParam?.Material?.SetColor(CachedHairsColors[i], data.ManeColors[i].Color);
            _currentTailParam?.Material?.SetColor(CachedHairsColors[i], data.TailsColors[i].Color);
            _currentBackParam?.Material?.SetColor(CachedHairsColors[i], data.BackColors[i].Color);
        }
    }
    private void UpdateBodyBlendShapes(ref CustomizeDataPackage data)
    {
        if (_dreamResetEvent)
        {
            for (var i = 0; i < data.BlendShapes.Length; i++) bodySkinnedMesh.SetBlendShapeWeight(i, data.BlendShapes[i]);
        }
        else
        {
            for (var i = 0; i < 15; i++) bodySkinnedMesh.SetBlendShapeWeight(i, data.BlendShapes[i]);
            for (var i = 25; i < data.BlendShapes.Length; i++) bodySkinnedMesh.SetBlendShapeWeight(i, data.BlendShapes[i]);
        }
        if (ItemsCheckerData.itemCarrot ||
            ItemsCheckerData.itemApple ||
            ItemsCheckerData.itemCorn)
        {
            bodySkinnedMesh.SetBlendShapeWeight(3, 100.0f);
        }
    }
    public void UpdateMagicColor(ref CustomizeDataPackage data)
    {
        Color.RGBToHSV(data.MagicColor.Color, out float hue, out _, out _);
        var color = Color.HSVToRGB(hue, 1.0f, 1.0f);
        _hornlight.color = color;
        _hornMeshMat.color = color;
        _hornTrailMat.color = _hornlight.color;
        _hornMeshMat.SetColor("_EmissionColor", color * 25.0f);
        _hornPSMat.color = color;
        _hornPSMat.SetColor("_EmissionColor", color * 25.0f);
    }
    private void UpdateItemBones(SkinnedMeshRenderer skinned, List<Transform> itemBones)
    {
        itemBones.ForEach(f =>
        {
            var root = _playerBones.FirstOrDefault(r => r.name.Equals(f.name));
            if (root != null)
            {
                f.SetParent(root);
                f.localPosition = Vector3.zero;
                f.localRotation = Quaternion.identity;
                switch (f.gameObject.name)
                {
                    case "SP0":
                        f.localScale = Vector3.one * 1100.0f + Vector3.one * LocalData.BlendShapes[26] * -1.0f;
                        break;
                    case "SP1":
                        f.localScale = Vector3.one * 1050.0f + Vector3.one * LocalData.BlendShapes[26] * -1.0f;
                        break;
                    case "NECK":
                        f.localScale = Vector3.one * 1050.0f + Vector3.one * LocalData.BlendShapes[26] * -1.0f;
                        break;
                    default:
                        f.localScale = Vector3.one * 1050.0f + Vector3.one * LocalData.BlendShapes[25] * -1.0f;
                        break;

                }
                if (skinned.gameObject.name.ToLower().Contains("backpack1"))
                {
                    f.localScale =
                        Vector3.one * 1250.0f + Vector3.one * LocalData.BlendShapes[26] * -1.0f +
                        Vector3.up * (LocalData.BackLenght + 0.1f) * 1.2f;
                }
            }
        });
    }
    public void UpdateItemsView()
    {
        itemsReferences.itemApple.SetActive(ItemsCheckerData.itemApple);
        itemsReferences.itemCarrot.SetActive(ItemsCheckerData.itemCarrot);
        itemsReferences.itemCorn.SetActive(ItemsCheckerData.itemCorn);

        itemsReferences.itemBracelet.SetActive(ItemsCheckerData.itemBracelet); //legs
        itemsReferences.itemBracelet.transform.localScale = Vector3.one * (1.0f - LocalData.BlendShapes[25] * 0.0011f + 0.05f);

        itemsReferences.itemWatch.SetActive(ItemsCheckerData.itemWatch);       //legs
        itemsReferences.itemWatch.transform.localScale = Vector3.one * (1.0f - LocalData.BlendShapes[25] * 0.0011f + 0.05f);

        itemsReferences.itemGlass1.SetActive(ItemsCheckerData.itemGlass1);
        itemsReferences.itemGlass2.SetActive(ItemsCheckerData.itemGlass2);
        itemsReferences.itemGlass3.SetActive(ItemsCheckerData.itemGlass3);
        itemsReferences.itemHornBall.SetActive(ItemsCheckerData.itemHornBall);
        itemsReferences.itemHeadBow.SetActive(ItemsCheckerData.itemHeadBow);
        itemsReferences.itemCap.SetActive(ItemsCheckerData.itemCap);
        itemsReferences.itemCone.SetActive(ItemsCheckerData.itemCone);
        itemsReferences.itemCowboyHat.SetActive(ItemsCheckerData.itemCowboyHat);
        itemsReferences.itemFeathers.SetActive(ItemsCheckerData.itemFeathers);
        itemsReferences.itemHorns1.SetActive(ItemsCheckerData.itemHorns1);
        itemsReferences.itemHorns2.SetActive(ItemsCheckerData.itemHorns2);
        itemsReferences.itemLamp.SetActive(ItemsCheckerData.itemLamp);

        itemsReferences.itemButterly1.SetActive(ItemsCheckerData.itemButterly1); //body
        itemsReferences.itemButterly2.SetActive(ItemsCheckerData.itemButterly2); //body
        itemsReferences.itemButterly3.SetActive(ItemsCheckerData.itemButterly3); //body
        itemsReferences.itemButterly4.SetActive(ItemsCheckerData.itemButterly4); //body
        itemsReferences.butterlyRoot.localScale = Vector3.one * (1.0f - LocalData.BlendShapes[26] * 0.001f + 0.05f);

        itemsReferences.itemHandkerchief.SetActive(ItemsCheckerData.itemHandkerchief); //body
        itemsReferences.itemTie.SetActive(ItemsCheckerData.itemTie); //body
        itemsReferences.itemScaf.SetActive(ItemsCheckerData.itemScaf); //body

        itemsReferences.itemChoker.SetActive(ItemsCheckerData.itemChoker); //body
        itemsReferences.chokerRoot.localScale = Vector3.one * (1.0f - LocalData.BlendShapes[26] * 0.001f + 0.05f);

        itemsReferences.itemBackBow.SetActive(ItemsCheckerData.itemBackBow);
        itemsReferences.itemPacks.SetActive(ItemsCheckerData.itemPacks);

        var coefxy = (1.0f - LocalData.BlendShapes[26] * 0.0005f + 0.109f);
        var coefz = (1.0f - LocalData.BlendShapes[26] * 0.0007f + 0.156f);
        itemsReferences.itemHarnessFront.SetActive(ItemsCheckerData.itemHarnessFront); //body
        itemsReferences.harnessFrontRoot.localScale = new Vector3(coefxy, coefxy, coefz); 
        itemsReferences.itemHarnessBack.SetActive(ItemsCheckerData.itemHarnessBack); //body
        itemsReferences.harnessBackRoot.localScale = new Vector3(coefxy, coefxy, coefz);

        if (ItemsCheckerData.itemAviator) InstanceDynamicItem(ref _itemAviatorInstance, ref itemsReferences.itemAviatorPrefab);
        else RemoveDynamicItem(ref _itemAviatorInstance);
        if (ItemsCheckerData.itemBackpack1) InstanceDynamicItem(ref _itemBackpack1Instance, ref itemsReferences.itemBackpack1Prefab);
        else RemoveDynamicItem(ref _itemBackpack1Instance);
        if (ItemsCheckerData.itemBackpack2) InstanceDynamicItem(ref _itemBackpack2Instance, ref itemsReferences.itemBackpack2Prefab);
        else RemoveDynamicItem(ref _itemBackpack2Instance);
        if (ItemsCheckerData.itemSocks1) InstanceDynamicItem(ref _itemSocks1Instance, ref itemsReferences.itemSocks1Prefab);
        else RemoveDynamicItem(ref _itemSocks1Instance);
        if (ItemsCheckerData.itemSocks2) InstanceDynamicItem(ref _itemSocks2Instance, ref itemsReferences.itemSocks2Prefab);
        else RemoveDynamicItem(ref _itemSocks2Instance);
        if (ItemsCheckerData.itemSwitter) InstanceDynamicItem(ref _itemSwitterInstance, ref itemsReferences.itemSwitterPrefab);
        else RemoveDynamicItem(ref _itemSwitterInstance);

        UpdateBodyAndEyesMaterials(ref LocalData);
        UpdateBodyBlendShapes(ref LocalData);
    }
    public void UpdateTailRotation(int sitState)
    {
        if (_lastTailIndex < 0 || _lastTailIndex >= tailsPrefabs.Length || _tailJiggles == null) return;
        if (!gameObject.activeInHierarchy) return;
        if (_updateTailRotate != null)
        {
            StopCoroutine(_updateTailRotate);
            _updateTailRotate = null;
        }
        _updateTailRotate = UpdateTailRotate(sitState);
        StartCoroutine(_updateTailRotate);
    }

    private void InstanceDynamicItem(ref GameObject itemInstanceRef, ref GameObject prefabRef)
    {
        if (itemInstanceRef == null)
        {
            itemInstanceRef = Instantiate(prefabRef, transform);
            var itemSkin = itemInstanceRef.GetComponent<SkinnedMeshRenderer>();
            UpdateItemBones(itemSkin, new List<Transform>() { itemSkin.rootBone }.Concat(itemSkin.bones).ToList());
        }
    }
    private void RemoveDynamicItem(ref GameObject itemInstanceRef)
    {
        if (itemInstanceRef != null)
        {
            var skin = itemInstanceRef.GetComponent<SkinnedMeshRenderer>();
            skin.bones.ToList().ForEach(bone => Destroy(bone.gameObject));
            Destroy(skin.rootBone.gameObject);
            Destroy(itemInstanceRef);
        }
    }
    private IEnumerator UpdateEyesDream()
    {
        while (true)
        {
            var timer = 0.0f;
            while (true)
            {
                _dreamResetEvent = false;
                timer += Time.deltaTime;
                if (timer < 0.2f)
                {
                    for (var i = 19; i < 25; i++)
                    {
                        var current = Mathf.Lerp(bodySkinnedMesh.GetBlendShapeWeight(i), 0.0f, 20.0f * Time.deltaTime);
                        bodySkinnedMesh.SetBlendShapeWeight(i, current);
                    }
                    for (var i = 15; i < 19; i++)
                    {
                        var current = Mathf.Lerp(bodySkinnedMesh.GetBlendShapeWeight(i), 100.0f, 20.0f * Time.deltaTime);
                        bodySkinnedMesh.SetBlendShapeWeight(i, current);
                    }
                }
                else if (timer < 0.4f)
                {
                    for (var i = 15; i < 25; i++)
                    {
                        var current = Mathf.Lerp(bodySkinnedMesh.GetBlendShapeWeight(i), LocalData.BlendShapes[i], 20.0f * Time.deltaTime);
                        bodySkinnedMesh.SetBlendShapeWeight(i, current);
                    }
                }
                else break;
                yield return null;
            }
            _dreamResetEvent = true;
            yield return new WaitForSeconds(Random.Range(0.0f, 6.0f));
        }
    }
    private IEnumerator UpdateTailRotate(int sitState)
    {
        _lastSitState = sitState;
        var savedTime = Time.time + 0.5f;
        Quaternion currentRootTileRotation;
        while (true)
        {
            if (_lastSitState == 0)
            {
                currentRootTileRotation = Quaternion.Euler(_tailSavedRotation);
                for (var i = 0; i < _tailJiggles.Length; i++)
                {
                    _tailJiggles[i].SpringStrength = _tailSpringStrength[i];
                    _tailJiggles[i].Dampening = _tailDampening[i];
                    _tailJiggles[i].AddNoise = _tailAddNoize[i];
                    _tailJiggles[i].NoiseScale = _tailNoiseScale[i];
                    _tailJiggles[i].NoiseSpeed = _tailNoiseSpeed[i];
                    _tailJiggles[i].NoiseStrength = _tailNoiseStrength[i];
                }
            }
            else
            {
                currentRootTileRotation = Quaternion.Euler(_lastSitState == 1 ?
                    tailsRotationEulers_Sit1[_lastTailIndex] :
                    tailsRotationEulers_Sit2[_lastTailIndex]);

                for (var i = 0; i < _tailJiggles.Length; i++)
                {
                    _tailJiggles[i].SpringStrength = 0.25f;
                    _tailJiggles[i].Dampening = 1.0f;
                    _tailJiggles[i].AddNoise = false;
                }
            }
          
            if (savedTime < Time.time)
            {
                _updateTailRotate = null;
                _tailRoot.localRotation = currentRootTileRotation;
               yield break;
            }
            if (_tailRoot != null) _tailRoot.localRotation = Quaternion.Lerp(_tailRoot.localRotation, currentRootTileRotation, 0.25f);
            yield return _updateTailRotateTimer;
        }
    }

    [ContextMenu("GetCustomizeJson")] public void GetCustomizeJson() => Debug.Log(JsonUtility.ToJson(LocalData));

    private void InitView()
    {
        SetupHorn();
        SetupEmotions();
        SetupBodyRoots();
        SetupPlayerBones();
        SetupBodyAndEyesMaterials();
        SetupHornAndWingsMaterial();
    }

    public void UpdateView(ref CustomizeDataPackage data)
    {
        LocalData = data;

        TrySpawnHairs(data.TailID, ref tailsPrefabs, ref _currentTailParam, ref rootTail);
        TrySpawnHairs(data.ManeID, ref manesPrefabs, ref _currentManeParam, ref rootHead);
        TrySpawnHairs(data.BackID, ref backmPrefabs, ref _currentBackParam, ref rootHead);
        SetupTailRotations(data.TailID);
        UpdateTailRotation(_lastSitState);

        UpdateRace(ref data);
        UpdateBodyRoots(ref data);
        UpdateBodyAndEyesMaterials(ref data);
        UpdateHairsMaterials(ref data);
        UpdateBodyBlendShapes(ref data);
        UpdateMagicColor(ref data);

        UpdateItemsView();
    }

    private void OnEnable()
    {
        InitView();
        StartCoroutine(UpdateEyesDream());
#if UNITY_EDITOR
        if (debugStartInvoke && SRef.Authorization.ServerBypass)
        {
            if (debugMainPlayer) SRef.Authorization.MainPlayerViewData = LocalData;
            UpdateView(ref LocalData);
        }
#endif
    }
    private void Update()
    {
#if UNITY_EDITOR
        if (debugUpdateInvoke && SRef.Authorization.ServerBypass)
        {
            if (debugMainPlayer) SRef.Authorization.MainPlayerViewData = LocalData;
            UpdateView(ref LocalData);
        }
#endif
    }
    private void OnDisable()
    {
        RemoveBodyRoots();
        StopCoroutine(UpdateEyesDream());
        if (_updateTailRotate != null)
        {
            StopCoroutine(_updateTailRotate);
            _updateTailRotate = null;
        }
    }
}