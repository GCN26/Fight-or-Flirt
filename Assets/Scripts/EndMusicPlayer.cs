using UnityEngine;
using UnityEngine.Audio;

public class EndMusicPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = SoundSliders.musicVol * SoundSliders.masterVol * .55f;
    }
}
