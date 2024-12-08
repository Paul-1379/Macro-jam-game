using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitchTrigger : MonoBehaviour
{
    [Header("Level transition")]
    [SerializeField] private LevelTransition levelTransition;
    [SerializeField] private Transform levelTransitionParent;

    [Header("level to load")]
    [SerializeField] private bool restartCurrentScene;
    [SerializeField] private int levelToLoadBuildIndex;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Instantiate(levelTransition, levelTransitionParent).levelTransitionEvent += LoadScene;
    }

    private void LoadScene()
    {
        SceneManager.LoadSceneAsync(restartCurrentScene? SceneManager.GetActiveScene().buildIndex : levelToLoadBuildIndex, LoadSceneMode.Single);
    }
}
