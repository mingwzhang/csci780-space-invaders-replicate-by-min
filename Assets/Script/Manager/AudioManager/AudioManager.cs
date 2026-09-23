using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource laserSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ufoSource;


    [SerializeField] private AudioClip laserLoop;
    [SerializeField] private AudioClip laserEnding;
    [SerializeField] private AudioClip ufoLowPitch;

    [SerializeField] private AudioClip fastInvader1;
    [SerializeField] private AudioClip fastInvader2;
    [SerializeField] private AudioClip fastInvader3;
    [SerializeField] private AudioClip fastInvader4;

    [SerializeField] private AudioClip playerShooting;
    [SerializeField] private AudioClip playerDestroyed;
    [SerializeField] private AudioClip invaderDestroyed;


    public void PlayLaserLoop()
    {
        laserSource.clip = laserLoop;
        laserSource.loop = true;
        laserSource.volume = 0f;
        laserSource.Play();

        StartCoroutine(LasserFadeIn());
    }

    public void PlayLaserEnding()
    {
        laserSource.Stop();

        laserSource.loop = false;
        laserSource.clip = laserEnding;
        laserSource.volume = 1f;
        laserSource.Play();
    }

    private IEnumerator LasserFadeIn()
    {
        laserSource.volume = 0f;

        while (laserSource.volume < 1f)
        {
            laserSource.volume += Time.unscaledDeltaTime * 4f;
            yield return null;
        }
    }

    public void PlayUFOLowPitch()
    {
        ufoSource.clip = ufoLowPitch;
        ufoSource.loop = true;
        ufoSource.Play();
    }
    public void StopUFOLowPitch()
    {
        ufoSource.Stop();
    }


    public void PlayFastInvader1()
    {
        sfxSource.PlayOneShot(fastInvader1);
    }

    public void PlayFastInvader2()
    {
        sfxSource.PlayOneShot(fastInvader2);
    }

    public void PlayFastInvader3()
    {
        sfxSource.PlayOneShot(fastInvader3);
    }
    public void PlayFastInvader4()
    {
        sfxSource.PlayOneShot(fastInvader4);
    }

    public void PlayPlayerShooting()
    {
        sfxSource.PlayOneShot(playerShooting);
    }

    public void PlayPlayerDestroyed()
    {
        sfxSource.PlayOneShot(playerDestroyed);
    }

    public void PlayInvaderDestroyed()
    {
        sfxSource.PlayOneShot(invaderDestroyed);
    }

}