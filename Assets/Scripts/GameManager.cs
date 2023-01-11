using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        int scenesOpened = SceneManager.sceneCount;
        if(scenesOpened > 1)
        {
            for(int i = 1; i < scenesOpened; i++)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            }
        }
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
    }

    public void StartGame()
    {
        PlayersManager.instance.SwitchToPlayMode();
        SceneManager.UnloadSceneAsync(1);
        SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
    }
}
