using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;

    public AudioSource efxSource;
    public AudioSource musicSource;

    private bool isFading = false;

    private readonly float fadingTime = 0.03f;

    private readonly float lowPitchRange = 0.93f;
    private readonly float highPitchRange = 1.08f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
        Instance.musicSource.volume = 0;
    }

    public void PlaySingle(AudioClip audioClip)
    {
        efxSource.clip = audioClip;
        efxSource.Play();
    }

    public void ChangeMusic(AudioClip audioClip)
    {
        if (isFading)
        {
            StopCoroutine(nameof(FadeRoutine));
            musicSource.Stop();
            //musicSource.volume = 1;
            isFading = false;
        }
        musicSource.clip = audioClip;
        musicSource.Play();
    }

    public void FadeMusic(float timeToFade)
    {
        StartCoroutine(FadeRoutine(timeToFade));
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }

    IEnumerator FadeRoutine(float timeToFade)
    {
        isFading = true;
        float currentTime = 0;

        while(currentTime < timeToFade && musicSource.volume > 0)
        {
            currentTime += Time.deltaTime;

            musicSource.volume -= fadingTime / timeToFade * musicSource.volume;

            yield return null;
        }

        musicSource.Stop();
        //musicSource.volume = 1;

        isFading = false;
        yield return null;
    }
}
