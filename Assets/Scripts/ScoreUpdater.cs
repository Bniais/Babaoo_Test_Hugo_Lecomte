using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeasingGame
{
    public class ScoreUpdater : MonoBehaviour
    {

        /// <summary>
        /// Le texte qui montre le score
        /// </summary>
        [SerializeField]
        private Text scoreText;

        /// <summary>
        /// Appelée avant le premier update, met à jour le meilleur score
        /// </summary>
        void Start()
        {
            float score = PlayerPrefs.GetFloat("BestScoreSlidingGame", -1);
            if (score != -1)
            {
                int minutes = Mathf.FloorToInt(score / 60F);
                int seconds = Mathf.FloorToInt(score - minutes * 60);
                string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
                scoreText.text = "Best Score : " + niceTime;
            }
            else
            {
                scoreText.text = "Best Score : Never Finished";

            }
        }

        public static void UpdateScore(float score)
        {
            if (PlayerPrefs.GetFloat("BestScoreSlidingGame", -1) < score)
                PlayerPrefs.SetFloat("BestScoreSlidingGame", score);
        }
    }
}