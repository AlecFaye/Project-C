using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;

    private int levelBuildIndex = 0;
    private LevelManager levelManager;

    public void Init(string name, int levelBuildIndex, LevelManager levelManager)
    {
        this.levelBuildIndex = levelBuildIndex;
        this.levelManager = levelManager;

        textMeshPro.SetText($"Level {name}");
        button.onClick.AddListener(OnLevelClick);
    }

    private void OnLevelClick()
    {
        levelManager.LoadLevel(levelBuildIndex);
    }

    public void DisableButton()
    {
        button.interactable = false;
    }

    public void EnableButton()
    {
        button.interactable = true;
    }
}
