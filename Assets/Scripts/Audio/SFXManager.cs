using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton manager for audio playback using an object pool of AudioSources.
/// Supports 2D and 3D audio playback via an ID lookup in the assigned SFXDatabase.
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [Header("Configuration")]
    [SerializeField] private SFXDatabase _database;
    [SerializeField] private int _initialPoolSize = 10;
    
    [Header("Audio Settings")]
    [SerializeField] private UnityEngine.Audio.AudioMixerGroup _sfxMixerGroup; // Optional
    
    private List<AudioSource> _pool = new List<AudioSource>();
    private Transform _poolContainer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_database == null)
        {
            Debug.LogError("[SFXManager] SFX Database is missing! Please assign it in the Inspector.");
        }

        // Initialize pool
        _poolContainer = new GameObject("AudioSourcePool").transform;
        _poolContainer.SetParent(transform);

        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    /// <summary>
    /// Play a sound effect by its ID.
    /// Provide 'position' for 3D sound. If 'position' is null, it plays as 2D sound.
    /// </summary>
    public void PlaySFX(string id, Vector3? position = null)
    {
        if (_database == null || string.IsNullOrEmpty(id)) return;

        AudioClip clip = _database.GetClip(id, out float volume);
        if (clip == null) return;

        AudioSource source = GetFreeSource();

        // Configure 2D or 3D
        if (position.HasValue)
        {
            source.transform.position = position.Value;
            source.spatialBlend = 1f; // 100% 3D
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 2f;
            source.maxDistance = 30f;
        }
        else
        {
            source.transform.position = transform.position;
            source.spatialBlend = 0f; // 100% 2D
        }

        source.volume = volume;
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// Finds a free AudioSource in the pool. Instantiates a new one if none are available.
    /// </summary>
    private AudioSource GetFreeSource()
    {
        foreach (var source in _pool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // Pool expansion (if all are busy)
        Debug.Log("[SFXManager] Pool expanded. Consider increasing initial size if this happens frequently.");
        return CreateNewAudioSource();
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject go = new GameObject($"SFXSource_{_pool.Count}");
        go.transform.SetParent(_poolContainer);
        
        AudioSource source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        
        if (_sfxMixerGroup != null)
        {
            source.outputAudioMixerGroup = _sfxMixerGroup;
        }

        _pool.Add(source);
        return source;
    }
}
