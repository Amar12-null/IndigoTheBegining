using UnityEngine;

namespace Indigo.UI
{
    public static class SettingsService
    {
        const string KeyVolume = "indigo.settings.volume";
        const string KeyMusic  = "indigo.settings.music"; // NOVO
        const string KeySFX    = "indigo.settings.sfx";   // NOVO
        const string KeySens   = "indigo.settings.sensitivity";
        const string KeyFS     = "indigo.settings.fullscreen";
        const string KeyTut    = "indigo.settings.tutorial"; 

        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat(KeyVolume, 0.8f);
            set { PlayerPrefs.SetFloat(KeyVolume, value); AudioListener.volume = value; }
        }

        // --- NOVO: Muzika ---
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(KeyMusic, 0.8f);
            set => PlayerPrefs.SetFloat(KeyMusic, value);
        }

        // --- NOVO: Zvučni Efekti (SFX) ---
        public static float SFXVolume
        {
            get => PlayerPrefs.GetFloat(KeySFX, 0.8f);
            set => PlayerPrefs.SetFloat(KeySFX, value);
        }

        public static float MouseSensitivity
        {
            get => PlayerPrefs.GetFloat(KeySens, 2f);
            set => PlayerPrefs.SetFloat(KeySens, value);
        }

        public static bool Fullscreen
        {
            get => PlayerPrefs.GetInt(KeyFS, Screen.fullScreen ? 1 : 0) == 1;
            set
            {
                PlayerPrefs.SetInt(KeyFS, value ? 1 : 0);
                Screen.fullScreen = value;
            }
        }

        public static bool ShowTutorial
        {
            get => PlayerPrefs.GetInt(KeyTut, 1) == 1; 
            set
            {
                PlayerPrefs.SetInt(KeyTut, value ? 1 : 0);
                Indigo.Settings.TutorialSettings.ShowTutorial = value; 
            }
        }

        public static void ApplyOnLoad()
        {
            AudioListener.volume = MasterVolume;
            Indigo.Settings.TutorialSettings.ShowTutorial = ShowTutorial; 
        }
    }
}