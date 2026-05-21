using UnityEngine;

namespace Indigo.UI
{
    public static class SettingsService
    {
        const string KeyVolume = "indigo.settings.volume";
        const string KeySens   = "indigo.settings.sensitivity";
        const string KeyFS     = "indigo.settings.fullscreen";

        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat(KeyVolume, 0.8f);
            set { PlayerPrefs.SetFloat(KeyVolume, value); AudioListener.volume = value; }
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

        public static void ApplyOnLoad()
        {
            AudioListener.volume = MasterVolume;
        }
    }
}
