using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject database that maps a string ID to an AudioClip.
/// </summary>
[CreateAssetMenu(fileName = "SFXDatabase", menuName = "Basketball/SFX Database")]
public class SFXDatabase : ScriptableObject
{
    [Serializable]
    public struct SFXEntry
    {
        public string id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
    }

    [Header("SFX Entries")]
    [SerializeField] private List<SFXEntry> _entries = new List<SFXEntry>();

    // Dictionary for fast O(1) lookup
    private Dictionary<string, SFXEntry> _lookupTable;

    /// <summary>
    /// Initializes the dictionary on first access at runtime.
    /// </summary>
    private void BuildLookup()
    {
        if (_lookupTable != null) return;
        
        _lookupTable = new Dictionary<string, SFXEntry>();
        foreach (var entry in _entries)
        {
            if (string.IsNullOrEmpty(entry.id)) continue;
            
            if (!_lookupTable.ContainsKey(entry.id))
            {
                _lookupTable.Add(entry.id, entry);
            }
            else
            {
                Debug.LogWarning($"[SFXDatabase] Duplicate ID found: {entry.id}");
            }
        }
    }

    /// <summary>
    /// Retrieves the AudioClip for the given ID. Returns null if not found.
    /// </summary>
    public AudioClip GetClip(string id, out float volume)
    {
        BuildLookup();
        volume = 1f;

        if (_lookupTable != null && _lookupTable.TryGetValue(id, out SFXEntry entry))
        {
            volume = entry.volume > 0f ? entry.volume : 1f; // Default missing volume to 1
            return entry.clip;
        }

        Debug.LogWarning($"[SFXDatabase] No SFX found with ID: {id}");
        return null;
    }
}
