using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class LevelSwitchTrigger : MonoBehaviour
{
    [Header("Level transition")]
    [SerializeField] private LevelTransition levelTransition;
    [SerializeField] private Transform levelTransitionParent;
    [Header("level to load")]
    [SerializeField] private bool restartCurrentScene;
    [SerializeField] private int levelToLoadBuildIndex;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Instantiate(levelTransition, levelTransitionParent).levelTransitionEvent += LoadScene;
        _audioSource.Play();
    }

    private void LoadScene()
    {
        SceneManager.LoadSceneAsync(restartCurrentScene? SceneManager.GetActiveScene().buildIndex : levelToLoadBuildIndex, LoadSceneMode.Single);
    }
}
