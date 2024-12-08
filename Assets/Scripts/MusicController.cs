using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioClip[] musics;
    private AudioSource _audioSource;
    private void Awake()
    {
        //remove if other music is already playing (like if level has changed)
        if (FindObjectsByType<MusicController>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
        }
        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!_audioSource.isPlaying)
        {
            PlayRandomMusic();
        }
    }

    private void PlayRandomMusic()
    {
        _audioSource.clip = musics[Random.Range(0, musics.Length)];
        _audioSource.Play();
    }
}
