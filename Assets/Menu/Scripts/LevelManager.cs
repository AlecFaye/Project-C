using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    [SerializeField] 
    [Tooltip("List of build indices of playable levels")]
    private List<int> playableLevels = new();

    [SerializeField] private LevelButton levelButtonPrefab;
    [SerializeField] private int buttonWidth = 325;
    [SerializeField] private int buttonHeight = 100;

    private int startingLevelIndex = 2;
    private int endingLevelIndex = 2;
    private int currentLevel = 0;

    private List<LevelButton> levelButtons = new();
    private List<int> unlockedLevels = new();

    private GridLayoutGroup levelButtonGridLayout;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeButtonGridLayout();

        startingLevelIndex = playableLevels[0];
        endingLevelIndex = playableLevels[^1];

        unlockedLevels.Add(currentLevel);
        AddStartingLevels();
    }

    private void InitializeButtonGridLayout()
    {
        levelButtonGridLayout = canvas.GetComponentInChildren<GridLayoutGroup>();

        if (levelButtonGridLayout == null)
            Debug.LogError("Could not find level selector button grid layout.");

        levelButtonGridLayout.cellSize = new Vector2(buttonWidth, buttonHeight);
    }

    private void AddStartingLevels()
    {
        for (int index = startingLevelIndex; index < endingLevelIndex + 1; index++)
        {
            LevelButton levelButton = Instantiate(levelButtonPrefab);
            levelButton.transform.SetParent(levelButtonGridLayout.transform);
            levelButton.Init((index - startingLevelIndex + 1).ToString(), index, this);

            if (!unlockedLevels.Contains(index - startingLevelIndex))
                levelButton.DisableButton();

            levelButtons.Add(levelButton);
        }
    }
    
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void UnlockNextLevel()
    {
        currentLevel++;

        if (currentLevel > levelButtons.Count)
            return;

        levelButtons[currentLevel].EnableButton();
    }

    public void LoadPreviousLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;

        if (currentLevel < startingLevelIndex) // Change to load the finished game screen
        {
            Debug.LogError("No previous level");
            return;
        }

        LoadLevel(currentLevel);
    }

    public void LoadNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex + 1;

        if (currentLevel > endingLevelIndex) // Change to load the finished game screen
        {
            LoadLevel("MainMenu");
            return;
        }

        LoadLevel(currentLevel);
    }
}
