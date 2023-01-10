using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public enum MenuSceneBuildIndex
    {
        MAIN_MENU = 0,
        LEVEL_SELECTOR = 1,
        SETTINGS = 2
    }

    [SerializeField] private MenuSceneBuildIndex sceneToLoad = MenuSceneBuildIndex.MAIN_MENU;

    public void LoadScene()
    {
        SceneManager.LoadScene((int)sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
