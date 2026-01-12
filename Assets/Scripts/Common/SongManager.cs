using UnityEngine;
using System.Collections;

public class SongManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicTracks;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float maxVolume = 1f;

    private AudioSource audioSource;
    private int currentTrackIndex = -1;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 0f;
    }

    private void Start()
    {
        PlayTrack(0);
    }

    public void PlayTrack(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= musicTracks.Length)
        {
            Debug.LogWarning("Invalid track index!");
            return;
        }

        currentTrackIndex = trackIndex;
        
        StopAllCoroutines();
        
        audioSource.clip = musicTracks[trackIndex];
        audioSource.volume = 0f;
        audioSource.Play();

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, elapsed / fadeInDuration);
            yield return null;
        }
        
        audioSource.volume = maxVolume;
    }

    public void PlayNext()
    {
        int next = (currentTrackIndex + 1) % musicTracks.Length;
        PlayTrack(next);
    }

    public void PlayPrevious()
    {
        int prev = (currentTrackIndex - 1 + musicTracks.Length) % musicTracks.Length;
        PlayTrack(prev);
    }
}