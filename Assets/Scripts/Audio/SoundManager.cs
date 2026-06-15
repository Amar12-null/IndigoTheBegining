using System.Collections.Generic;
using UnityEngine;

namespace Indigo.Audio
{
    public enum SfxId
    {
        Pickup, Drop, Place, BrewStart, BrewLoop, BrewDone, DeliverySuccess,
        TipCoin, LevelUp, Purchase, DayStart, DayEnd, CustomerAngry, DoorOpen, ButtonClick,
    }

    [System.Serializable]
    public class SfxEntry
    {
        public SfxId id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.1f, 3f)] public float pitchMin = 0.95f;
        [Range(0.1f, 3f)] public float pitchMax = 1.05f;
    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Mix")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.5f;

        [Header("SFX library")]
        public SfxEntry[] sfx;

        // --- NOVO: PLEJLISTA ZA POZADINSKU MUZIKU ---
        [Header("Music Playlist")]
        [Tooltip("Ubaci svojih 6 audio fajlova ovdje!")]
        public AudioClip[] ambientMusicPlaylist; 
        public bool loopMusic = true;

        [Header("Pool")]
        public int sfxSourcePoolSize = 8;

        private Dictionary<SfxId, SfxEntry> _byId;
        private List<AudioSource> _sfxPool;
        private int _nextSource;
        private AudioSource _musicSource;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            BuildIndex();
            BuildPool();
            BuildMusicSource();
            
            // --- NOVO: BIRA NASUMIČNU PJESMU IZ TVOJE PLEJLISTE ---
            if (ambientMusicPlaylist != null && ambientMusicPlaylist.Length > 0)
            {
                AudioClip randomSong = ambientMusicPlaylist[Random.Range(0, ambientMusicPlaylist.Length)];
                PlayMusic(randomSong);
            }
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void BuildIndex()
        {
            _byId = new Dictionary<SfxId, SfxEntry>();
            if (sfx == null) return;
            for (int i = 0; i < sfx.Length; i++)
            {
                if (sfx[i] == null) continue;
                _byId[sfx[i].id] = sfx[i];
            }
        }

        void BuildPool()
        {
            _sfxPool = new List<AudioSource>(sfxSourcePoolSize);
            for (int i = 0; i < sfxSourcePoolSize; i++)
            {
                var go = new GameObject("SfxSource_" + i);
                go.transform.SetParent(transform, false);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                _sfxPool.Add(src);
            }
        }

        void BuildMusicSource()
        {
            var go = new GameObject("MusicSource");
            go.transform.SetParent(transform, false);
            _musicSource = go.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = loopMusic; // Muzika će se ponavljati zahvaljujući ovoj liniji
            _musicSource.spatialBlend = 0f;
        }

        public void Play(SfxId id)
        {
            if (_byId == null || !_byId.TryGetValue(id, out var entry)) return;
            if (entry.clip == null) return;

            var src = _sfxPool[_nextSource];
            _nextSource = (_nextSource + 1) % _sfxPool.Count;

            src.clip = entry.clip;
            src.volume = entry.volume * sfxVolume * masterVolume;
            src.pitch = Random.Range(entry.pitchMin, entry.pitchMax);
            src.Play();
        }

        public void PlayAt(SfxId id, Vector3 worldPos)
        {
            if (_byId == null || !_byId.TryGetValue(id, out var entry)) return;
            if (entry.clip == null) return;
            AudioSource.PlayClipAtPoint(entry.clip, worldPos, entry.volume * sfxVolume * masterVolume);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (_musicSource == null || clip == null) return;
            _musicSource.clip = clip;
            _musicSource.volume = musicVolume * masterVolume;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            if (_musicSource != null) _musicSource.Stop();
        }

        public void SetMusicVolume(float v) { musicVolume = Mathf.Clamp01(v); if (_musicSource != null) _musicSource.volume = musicVolume * masterVolume; }
        public void SetSfxVolume(float v)   { sfxVolume = Mathf.Clamp01(v); }
        public void SetMasterVolume(float v){ masterVolume = Mathf.Clamp01(v); if (_musicSource != null) _musicSource.volume = musicVolume * masterVolume; }
    }
}