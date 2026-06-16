using UnityEngine;
using Indigo.UI;

[RequireComponent(typeof(AudioSource))]
public class SyncSfxVolume : MonoBehaviour
{
    private AudioSource _audioSource;
    private float _baseVolume;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        
        // Skripta na početku zapamti originalnu jačinu zvuka koju si ti podesio u Inspectoru (npr. 0.5)
        _baseVolume = _audioSource.volume; 
    }

    void Update()
    {
        // Svakog trenutka množi tu originalnu jačinu sa postotkom iz Settingsa!
        _audioSource.volume = _baseVolume * SettingsService.SFXVolume;
    }
}