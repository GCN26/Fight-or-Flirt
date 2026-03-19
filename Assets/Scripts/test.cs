using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class test : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip[] footstepSounds;

    Coroutine footstepSFXCo;

    private void Start()
    {

        footstepSFXCo = StartCoroutine(FootStepSoundPlay());
    }
    IEnumerator FootStepSoundPlay()
    {
        AudioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
        while (AudioSource.isPlaying)
        {
            yield return null;
        }
        footstepSFXCo = StartCoroutine(FootStepSoundPlay());
    }
}