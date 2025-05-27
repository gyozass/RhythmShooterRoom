using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    [Header("Score Settings")]
    public int score = 0;
    public int combo = 0;
    public int maxCombo = 0;

    [Header("Combo Settings")]
    public int comboMultiplierStart = 1;
    public int comboMultiplierIncreaseEvery = 5; 

    private int currentMultiplier = 1;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI comboText;

    void Start()
    {
        ResetScore();
        UpdateUI();
    }

    public void AddScore(HitType hitType)
    {
        if (hitType == HitType.Miss)
        {
            ResetCombo();
            UpdateUI();
            return;
        }

        int points = 0;
        switch (hitType)
        {
            case HitType.Perfect:
                points = 100;
                break;
            case HitType.Good:
                points = 70;
                break;
            case HitType.Okay:
                points = 50;
                break;
            default:
                points = 0;
                break;
        }

        combo++;
        if (combo > maxCombo)
            maxCombo = combo;

        UpdateMultiplier();

        score += points * currentMultiplier;

        //Debug.Log($"Hit: {hitType} | Points: {points} | Combo: {combo} | Multiplier: {currentMultiplier}x | Total Score: {score}");
    }

    void UpdateMultiplier()
    {
        currentMultiplier = comboMultiplierStart + (combo / comboMultiplierIncreaseEvery);
    }

    void ResetCombo()
    {
        combo = 0;
        currentMultiplier = comboMultiplierStart;
        //Debug.Log("Combo broken!");
    }

    public void ResetScore()
    {
        score = 0;
        combo = 0;
        maxCombo = 0;
        currentMultiplier = comboMultiplierStart;
    }

    public void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();

        if (comboText != null)
            comboText.text = combo > 0 ? $"{combo}x" : "";
    }

    public int GetScore()
    {
        return score;
    }
}