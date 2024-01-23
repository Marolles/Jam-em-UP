using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{

    public static ScoreBoard instance;

    [Header("Settings")]
    public int scoreOnKill;
    public int scoreOnTickle;

    [Header("References")]
    public TextMeshProUGUI scoreText;

    private static int currentScore;
    private void Awake()
    {
        if (instance != null) { Destroy(instance); }
        instance = this;
        SetScore(0);
    }

    public void SetScore(int _amount)
    {
        currentScore = _amount;
        UpdateScoreText();
    }
    public void IncreaseScore(int _amount)
    {
        currentScore += _amount;
        UpdateScoreText();
    }

    public void UpdateScoreText()
    {
        scoreText.text = currentScore.ToString();
    }
}
