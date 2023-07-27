using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

public class PlayerEffects : MonoBehaviour
{
    [Header("Audios")]
    public AudioSource airAudioSource;
    public AudioSource hoofAudioSource;
    public AudioSource gallopAudioSource;
    public AudioSource foodEatingSource;
    public AudioSource leavesAudioSource;
    public AudioSource stepUp;
    public AudioSource stepDown;
    public AudioSource riverAudioSource;
    public AudioSource gravityHitAudioSource;
    public AudioSource legsHitAudioSource;
    public AudioSource magicStart;
    public AudioSource magicUpdate;
    public AudioSource magicEnd;

    [Header("Referenses:")]
    public NoMainPlayerController noMainPlayerController;
    public ParticleSystem hornExplorePS;
    public GameObject hornLight;
    public ParticleSystem[] ps;
    public ParticleSystem[] psExplore;
    public ParticleSystem leavesParticles;
    public ParticleSystem waterParticles;
    public ParticleSystem waterExploreParticles;
    //public TrailRenderer trail;
    public HoofsDecalGenerator hoofsDecalGenerator;
    public float[] pitch = new float[3];
    public float[] volume = new float[3];

    [Header("Actions:")]
    public float animDistance;
    public float speed;
    public float downRayDistance;
    public int currentMovementState;
    public Vector3 forwardDirection;
    public float movementVelocity;

    private Light _hornlight;
    private Material _hornMeshMat;
    private Material _hornPSMat;
    private Material _hornTrailMat;
    private void SetupHorn()
    {
        hornLight.SetActive(false);
        _hornMeshMat = hornLight.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        _hornlight = hornLight.GetComponentInChildren<Light>();
        _hornTrailMat = hornLight.GetComponentsInChildren<ParticleSystemRenderer>()[1].trailMaterial;
        _hornTrailMat.color = _hornlight.color;
        hornLight.GetComponentsInChildren<ParticleSystemRenderer>()[1].trailMaterial = _hornTrailMat;
    }

    private WaitForSeconds _magicWait = new WaitForSeconds(0.1f);
    private bool _cachedMagicState = false;
    private IEnumerator MagicEffect(bool enabled)
    {
        Color.RGBToHSV(_hornMeshMat.color, out float hue, out _, out _);
        if (enabled)
        {
            if (!magicStart.isPlaying) magicStart.Play();
            yield return _magicWait;
            hornLight.SetActive(true);
            if (!hornExplorePS.isPlaying) hornExplorePS.Play();
            var timer = Time.time + 0.6f;
            var v = 0.0f;
            while (timer > Time.time)
            {
                v = Mathf.Lerp(v, 1.0f, 5.0f * Time.deltaTime);
                _hornlight.intensity = Mathf.Lerp(_hornlight.intensity, 1.5f, 8.0f * Time.deltaTime);
                _hornMeshMat.SetColor("_EmissionColor", Color.HSVToRGB(hue, 1.0f, v) * 25.0f);
                yield return null;                
            }
            _hornlight.intensity = 1.5f;
            _hornMeshMat.SetColor("_EmissionColor", Color.HSVToRGB(hue, 1.0f, 1.0f) * 25.0f);
            if (!magicUpdate.isPlaying) magicUpdate.Play();
        }
        else
        {
            if (!magicEnd.isPlaying) magicEnd.Play();
            var timer = Time.time + 0.6f;
            var v = 1.0f;
            while (timer > Time.time)
            {
                v = Mathf.Lerp(v, 0.0f, 5.0f * Time.deltaTime);
                _hornlight.intensity = Mathf.Lerp(_hornlight.intensity, 0.0f, 8.0f * Time.deltaTime);
                _hornMeshMat.SetColor("_EmissionColor", Color.HSVToRGB(hue, 1.0f, v) * 25.0f);
                yield return null;
            }
            _hornlight.intensity = 0.0f;
            _hornMeshMat.SetColor("_EmissionColor", Color.HSVToRGB(hue, 1.0f, 0.0f) * 25.0f);
            hornLight.SetActive(false);
            if (magicUpdate.isPlaying) magicUpdate.Stop();
        }
        _magicEffect = null;
    }
    [HideInInspector] public bool MagicState;
    private IEnumerator _magicEffect = null;
    public void MagicEffectEnable(bool enabled)
    {
        MagicState = enabled;
        if (_cachedMagicState == enabled) return;
        _cachedMagicState = enabled;
        if (_magicEffect != null)
        {
            StopCoroutine(_magicEffect);
            _magicEffect = null;
        }
        _magicEffect = MagicEffect(enabled);
        StartCoroutine(_magicEffect);
    }

    public void LegsHitEffect()
    {
        if (legsHitAudioSource.gameObject.activeInHierarchy) legsHitAudioSource.Play();
        for (var i = 0; i < psExplore.Length; i++) if (!psExplore[i].isPlaying) psExplore[i].Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_collisionEnabled) return;
        if (other.CompareTag("Leaves"))
        {
            leavesAudioSource.Play();
            leavesAudioSource.volume = Mathf.Clamp(animDistance * 0.1f, 0.0f, 1.0f);
            leavesAudioSource.pitch = Random.Range(0.9f, 1.6f);
            if (!leavesParticles.isPlaying)
            {
                leavesParticles.Play();
            }
            else
            {
                leavesParticles.Stop();
                leavesParticles.Play();
            }
        }
        if (other.CompareTag("Water"))
        {
            if (!waterExploreParticles.isPlaying) waterExploreParticles.Play();
            riverAudioSource.volume = 1.0f;
        }
    }

    private float triggerTime = 0.0f;
    private void OnTriggerStay(Collider other)
    {
        triggerTime = Time.time + 0.25f;
        if (!_collisionEnabled) return;
        if (other.CompareTag("Water"))
        {
            if (animDistance > 0.75f)
            {
                if (!waterParticles.isPlaying) waterParticles.Play();

                if (animDistance > 3.0f)
                {
                    _cachedWaterPS.maxParticles = 80;
                    _cachedWaterPS.startSpeed = 2.0f;
                    _riverEffect = 2.0f;
                }
                else if (animDistance > 1.75f)
                {
                    _cachedWaterPS.maxParticles = 25;
                    _cachedWaterPS.startSpeed = 2.0f;
                    _riverEffect = 1.5f;
                }
                else
                {
                    _cachedWaterPS.maxParticles = 4;
                    _cachedWaterPS.startSpeed = 0.5f;
                    _riverEffect = 1.0f;
                }
            }
            else
            {
                if (waterParticles.isPlaying) waterParticles.Stop();
                _riverEffect = 0.0f;
            }
        }
    }

    private bool _stepTrigger = true;
    private float _riverEffect;
    public void UpdateEffects()
    {
        if (triggerTime < Time.time)
        {
            if (waterParticles.isPlaying) waterParticles.Stop();
            _riverEffect = 0.0f;
        }

        if (!_collisionEnabled) return;

        if (currentMovementState < 2)
        {
            gallopAudioSource.volume = 0.0f;
            hoofAudioSource.pitch = pitch[currentMovementState];
            hoofAudioSource.volume =
                downRayDistance < 0.1f ?

                movementVelocity *
                volume[currentMovementState] *
                speed

                : 0.0f;
        }
        else
        {
            hoofAudioSource.volume = 0.0f;
            gallopAudioSource.pitch = pitch[currentMovementState];
            gallopAudioSource.volume =
                downRayDistance < 0.1f ?

                movementVelocity *
                volume[currentMovementState] *
                speed

                : 0.0f;
        }

        for (var i = 0; i < ps.Length; i++)
        {
            ParticleSystem.MainModule mainMod = ps[i].main;
            mainMod.startRotation = Mathf.Atan2(-forwardDirection.y, forwardDirection.x);
            if (animDistance > 2.3f && downRayDistance < 0.2f)
            {
                if (!ps[i].isPlaying) ps[i].Play();
            }
            else
            {
                if (ps[i].isPlaying) ps[i].Stop();
            }
        }

        hoofsDecalGenerator.ForwardUpdate();

        airAudioSource.volume = Mathf.Lerp(airAudioSource.volume, Mathf.Pow(animDistance * 0.06f, 2.0f), 8 * Time.deltaTime);
        riverAudioSource.volume = Mathf.Lerp(riverAudioSource.volume, _riverEffect * 0.5f, 8 * Time.deltaTime);
        riverAudioSource.pitch = Mathf.Lerp(riverAudioSource.pitch, Mathf.Clamp(_riverEffect, 0.75f, 2.0f), 8 * Time.deltaTime);

        if (downRayDistance >= 0.04f)
        {
            if (_stepTrigger)
            {
                _stepTrigger = false;
                stepUp.Play();
            }
        }
        else
        {
            if (!_stepTrigger)
            {
                _stepTrigger = true;
                stepDown.Play();
                for (var i = 0; i < psExplore.Length; i++) if (!psExplore[i].isPlaying) psExplore[i].Play();
            }
        }
    }

    private void OffEffects()
    {
        //trail.time = 40;
        if (waterParticles.isPlaying) waterParticles.Stop();
        if (waterExploreParticles.isPlaying) waterExploreParticles.Stop();
        if (leavesParticles.isPlaying) leavesParticles.Stop();
        for (var i = 0; i < ps.Length; i++) if (ps[i].isPlaying) ps[i].Stop();
        //trail.emitting = false;
        hoofAudioSource.volume = 0.0f;
        gallopAudioSource.volume = 0.0f;
        if (leavesAudioSource.isPlaying) leavesAudioSource.Stop();
        airAudioSource.volume = 0.0f;
    }

    private bool _collisionEnabled = false;
    private void EnableCollisions() => _collisionEnabled = true;

    private ParticleSystem.MainModule _cachedWaterPS;
    private void Start() => _cachedWaterPS = waterParticles.main;

    private void OnEnable()
    {
        SetupHorn();
        Invoke("EnableCollisions", 2.0f);
        OffEffects();
    }
    private void OnDisable()
    {
        OffEffects();
        _collisionEnabled = false;
    }
}