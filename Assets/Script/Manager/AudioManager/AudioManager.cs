using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip laserLoop;
    [SerializeField] private AudioClip laserEnding;

    public void PlayLaserLoop()
    {
        audioSource.clip = laserLoop;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Play();

        StartCoroutine(FadeIn());
    }

    public void PlayLaserEnding()
    {
        audioSource.Stop();

        audioSource.loop = false;
        audioSource.clip = laserEnding;
        audioSource.volume = 1f;
        audioSource.Play();
    }

    private IEnumerator FadeIn()
    {
        audioSource.volume = 0f;

        while (audioSource.volume < 1f)
        {
            audioSource.volume += Time.unscaledDeltaTime * 4f;
            yield return null;
        }
    }
}