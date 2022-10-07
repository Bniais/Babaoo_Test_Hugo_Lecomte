using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeasingGame
{ 
    /// <summary>
    /// Stocke et modifie les options
    /// </summary>
    public class Options : MonoBehaviour
    {
        /// <summary>
        /// Les toggles pour les deux options
        /// </summary>
        [SerializeField]
        Toggle toggleTouch, toggleModelVisible;


        /// <summary>
        /// Appel�e avant le premier update, met � jour les options
        /// </summary>
        void Start()
        {
            toggleTouch.isOn = PlayerPrefs.GetInt("ToggleTouchSlidingGame", 0) != 0;
            toggleModelVisible.isOn = PlayerPrefs.GetInt("ToggleModelSlidingGame", 1) != 0;
        }


        /// <summary>
        /// Appel�e quand on change le toggle touch, change les preferences du joueur
        /// </summary>
        public void UpdateToggleTouch()
        {
            PlayerPrefs.SetInt("ToggleTouchSlidingGame", toggleTouch.isOn ? 1 : 0);

        }

        /// <summary>
        /// Appel�e quand on change le toggle mod�le, change les preferences du joueur
        /// </summary>
        public void UpdateToggleModel()
        {
            PlayerPrefs.SetInt("ToggleModelSlidingGame", toggleModelVisible.isOn ? 1 : 0);
        }
    }
}