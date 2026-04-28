using UnityEngine;

public static class SoundSliders
{
    public static float masterVol = 1;
    public static float sfxVol = .75f;
    public static float musicVol = .75f;

    public static void changeVolMaster(float vol)
    {
        masterVol = vol;
        saveVolPrefs();
    }
    public static void changeVolSFX(float vol)
    {
        sfxVol = vol;
        saveVolPrefs();
    }
    public static void changeVolMusic(float vol)
    {
        musicVol = vol;
        saveVolPrefs();
    }
    public static void saveVolPrefs()
    {
        PlayerPrefs.SetFloat("sfx", sfxVol);
        PlayerPrefs.SetFloat("music", musicVol);
        PlayerPrefs.SetFloat("master",masterVol);
        PlayerPrefs.Save();
        Debug.Log("Saved Volume to PlayerPrefs");
    }
    public static void loadVolPrefs()
    {
        masterVol = PlayerPrefs.GetFloat("master");
        sfxVol = PlayerPrefs.GetFloat("sfx");
        musicVol = PlayerPrefs.GetFloat("music");
        Debug.Log("Loaded Volume from PlayerPrefs");
    }
}
