using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{

    public static AudioManager i;
    // public SfxClip[] sfxClips;


    [SerializeField] private AudioSource track1;
    [SerializeField] private AudioSource track2;

    [SerializeField] private M_AudioClip[] audioClips;

    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private float musicFadeTime = 5.0f;

    private List<M_AudioClip> menuClips;
    private List<M_AudioClip> levelClips;

    private List<AudioSource> tracks;
    private int _currentTrackIndex;
    private Coroutine _currentTriggerCoroutine;
    private M_AudioClip _lastPlayedClip;
    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        tracks = new List<AudioSource>() {
            track1,
            track2
        };
        _currentTrackIndex = 0;
        LoadVolumeSettings();
        SplitClips();
        PlayDefaultMusic();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_currentTriggerCoroutine != null)
        {
            StopCoroutine(_currentTriggerCoroutine);
        }
        PlayDefaultMusic();
    }
    private void LoadVolumeSettings()
    {
        var masterVolume = PlayerPrefs.GetFloat(VolumeManager.MasterVolumeKey, 1.0f);
        var musicVolume = PlayerPrefs.GetFloat(VolumeManager.MusicVolumeKey, 0.6f);
        var sfxVolume = PlayerPrefs.GetFloat(VolumeManager.SfxVolumeKey, 0.8f);

        masterMixer.SetFloat(VolumeManager.MasterVolumeKey, ConvertToDecibel(masterVolume));
        masterMixer.SetFloat(VolumeManager.MusicVolumeKey, ConvertToDecibel(musicVolume));
        masterMixer.SetFloat(VolumeManager.SfxVolumeKey, ConvertToDecibel(sfxVolume));
    }

    public static float ConvertToDecibel(float volume)
    {
        var clamped = Mathf.Clamp(volume, 0.0001f, float.MaxValue);
        return Mathf.Log10(clamped) * 20.0f;
    }

    private void PlayDefaultMusic()
    {
        const int mainMenuIndex = 0;
        var index = SceneManager.GetActiveScene().buildIndex;
        if (index == mainMenuIndex)
        {
            foreach (var track in tracks)
            {
                track.Stop();
            }
            tracks[_currentTrackIndex].clip = GetClipByName("MainMenu");
            tracks[_currentTrackIndex].Play();
        }
        else if (index == SceneManager.sceneCount - 1)
        {
            var clip = GetClipByName("Outro");
            FadeBetweenTrack(clip);
        }
        else
        {
            var clip = GetRandomLevelClip();
            _currentTriggerCoroutine = StartCoroutine(TriggerFadeOnMusicEnd(clip.length));
            FadeBetweenTrack(clip);
        }
    }

    private void SplitClips()
    {
        menuClips = new List<M_AudioClip>();
        levelClips = new List<M_AudioClip>();
        foreach (var clip in audioClips)
        {
            switch (clip.type)
            {
                case ClipType.Menu:
                    menuClips.Add(clip);
                    break;
                case ClipType.Level:
                    levelClips.Add(clip);
                    break;
                default:
                    break;
            }
        }
    }

    private AudioClip GetClipByName(string name)
    {
        foreach (var mClip in audioClips)
        {
            if (mClip.name == name)
            {
                return mClip.clip;
            }
        }
        return null;
    }

    private AudioClip GetRandomLevelClip()
    {

        var index = Random.Range(0, levelClips.Count);
        var temp = levelClips[index];
        levelClips.RemoveAt(index);
        if (_lastPlayedClip != null)
        {
            levelClips.Add(_lastPlayedClip);
        }
        _lastPlayedClip = temp;
        return temp.clip;
    }

    private void FadeBetweenTrack(AudioClip clip)
    {
        var currentTrack = tracks[_currentTrackIndex];
        _currentTrackIndex = (_currentTrackIndex + 1) % tracks.Count;
        var nextTrack = tracks[_currentTrackIndex];
        nextTrack.clip = clip;
        FadeMusic(currentTrack, nextTrack);
    }

    private void FadeMusic(AudioSource fromTrack, AudioSource toTrack)
    {
        toTrack.Play();
        toTrack.volume = 0;
        fromTrack.DOFade(0, musicFadeTime).onComplete = fromTrack.Stop;
        toTrack.DOFade(1, musicFadeTime);
    }

    private IEnumerator TriggerFadeOnMusicEnd(float delay)
    {
        Debug.Log("Starting Next Fade in: " + delay);
        yield return new WaitForSeconds(delay - musicFadeTime);
        PlayDefaultMusic();
    }
}
