using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalScoreText;

   private void Start()
   {
       SetupEndScreen();
   }
    public void SetupEndScreen()
    {
        Debug.Log("working");
        //int finalScore = FindObjectOfType<PlayerScore>().GetScore();

        int finalScore = PlayerPrefs.GetInt("Score");
        finalScoreText.text = $"YOUR Score: {finalScore}";
    }

}
