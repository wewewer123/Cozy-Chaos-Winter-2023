using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager i;
    // public SfxClip[] sfxClips;

    private AudioSource _audioSource;

    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private float musicTypeChangeTime = 1.0f;

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
        _audioSource = GetComponent<AudioSource>();
        LoadVolumeSettings();

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
}
