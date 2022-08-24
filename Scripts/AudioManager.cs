using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioClip audioPeaceful;
    [SerializeField] private AudioClip audioBattle;
    private AudioSource audioSource;
    public GameObject musicVolumeSliderGameObject;
    public GameObject soundVolumeSliderGameObject;
    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;

    private Music currentlyPlaying = Music.Peaceful;

    private float musicVolume = 0.3f;

    /// <summary>
    /// The sound volume with which sounds should get played. Can get changed by the player, is
    /// being used by other scripts that play sounds.
    /// </summary>
    public static float soundVolume = 0.3f;

    public enum Music {Peaceful, Battle }

    // Start is called before the first frame update
    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioPeaceful;
        audioSource.Play();

        musicVolumeSlider = musicVolumeSliderGameObject.GetComponent<Slider>();
        musicVolumeSlider.onValueChanged.AddListener(delegate { MusicVolumeSliderChanged(); });

        soundVolumeSlider = soundVolumeSliderGameObject.GetComponent<Slider>();
        soundVolumeSlider.onValueChanged.AddListener(delegate { SoundVolumeSliderChanged(); });

        musicVolumeSlider.value = musicVolume;
        soundVolumeSlider.value = soundVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(Music m)
    {
        if (m == currentlyPlaying)
        {
            return;
        }

        AudioClip toPlay = audioPeaceful;

        switch (m)
        {
            case Music.Peaceful:
                toPlay = audioPeaceful;
                break;
            case Music.Battle:
                toPlay = audioBattle;
                break;

        }

        currentlyPlaying = m;
        StartCoroutine(SwitchMusic(1f, toPlay));

    }

    /// <summary>
    /// Changes the music ingame.
    /// </summary>
    /// <param name="fadeDuration">How many seconds the old music track fades out and how many seconds 
    /// the new music track fades in.</param>
    /// <param name="newClip">The new music track that shall get played.</param>
    /// <returns>IEnumerator cause of coroutine threat.</returns>
    IEnumerator SwitchMusic(float fadeDuration, AudioClip newClip)
    {
        float currentTime = 0;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(musicVolume, 0, currentTime / fadeDuration);
            yield return null;
        }

        audioSource.clip = newClip;
        audioSource.Play();
        currentTime = 0;


        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, musicVolume, currentTime / fadeDuration);
            yield return null;
        }


        yield break;
    }

    public void MusicVolumeSliderChanged()
    {
        musicVolume = musicVolumeSlider.value;
        audioSource.volume = musicVolumeSlider.value;
    }
    public void SoundVolumeSliderChanged()
    {
        soundVolume = soundVolumeSlider.value;
    }
}
