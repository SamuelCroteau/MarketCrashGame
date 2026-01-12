using UnityEngine;

public class AudioPlayerPool : MonoBehaviour
{
    [SerializeField] public AudioClip[] pool;
    [SerializeField] public float pitchRange = 0.1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 0.3f;
    }

    public void PlaySound()
    {
        AudioClip randomClip = pool[Random.Range(0, pool.Length)];
        audioSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        audioSource.PlayOneShot(randomClip);
    }
}
