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
    [SerializeField] private AudioClip ufoHighPitch;

    [SerializeField] private AudioClip fastInvader1;
    [SerializeField] private AudioClip fastInvader2;
    [SerializeField] private AudioClip fastInvader3;
    [SerializeField] private AudioClip fastInvader4;

    [SerializeField] private AudioClip playerShooting;
    [SerializeField] private AudioClip playerDestroyed;
    [SerializeField] private AudioClip invaderDestroyed;

    private int invaderDeathsThisFrame = 0;
    // Maximum overlapping explosions allowed at once
    private int maxInvaderSoundsPerFrame = 2; 
    private int lastProcessedFrame = -1;

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
        laserSource.volume = .4f;
        laserSource.Play();
    }

    private IEnumerator LasserFadeIn()
    {
        laserSource.volume = 0f;

        while (laserSource.volume < .4f)
        {
            laserSource.volume += Time.unscaledDeltaTime * 4f;
            yield return null;
        }
    }

    public void PlayUFOHighPitch()
    {
        ufoSource.clip = ufoHighPitch;
        ufoSource.loop = true;
        ufoSource.Play();
    }

    public void PlayUFOLowPitch()
    {
        ufoSource.Stop();

        ufoSource.loop = false;
        ufoSource.clip = ufoLowPitch;
        ufoSource.Play();
    }

    public void StopUFOPitch()
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
        // Check if moved to a new frame. If so, reset tracker.
        if (Time.frameCount != lastProcessedFrame)
        {
            invaderDeathsThisFrame = 0;
            lastProcessedFrame = Time.frameCount;
        }

        // If already hit our max explosion sound cap for this frame, ignore any extra requests
        if (invaderDeathsThisFrame >= maxInvaderSoundsPerFrame)
        {
            return;
        }

        // Slightly randomize pitch so overlapping sounds don't phase-cancel each other out
        sfxSource.pitch = Random.Range(0.9f, 1.1f);

        sfxSource.PlayOneShot(invaderDestroyed);
        invaderDeathsThisFrame++;
    }

}