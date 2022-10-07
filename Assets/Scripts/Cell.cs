using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeasingGame
{
    /// <summary>
    /// Classe repr�sentant une Cellule (un taquin) de la grille.
    /// </summary>
    public class Cell : MonoBehaviour
    {
        /// <summary>
        /// L'index de la cellule du milieu (celle qui n'est pas visible)
        /// </summary>
        /// 
        /// <remarks>
        /// Si l'on veut des grilles de plusieurs tailles, il faudra une fonction qui calcule cet index selon la taille
        /// </remarks>
        public static readonly int MIDDLE_CELL_ID = 4;


        /// <summary>
        /// Vitesse de d�placement par d�faut des cellules
        /// </summary>
        private static readonly float MOVE_SPEED = 10f;

        /// <summary>
        /// Vitesse de d�placement lors de la r�initialisation de la grille
        /// </summary>
        public static readonly float RESET_MOVE_SPEED = 17f;


        /// <summary>
        /// L'identifiant (de 0 � 8) du taquin selon sa position voulue dans la grille.
        /// </summary>
        private int id;

        /// <summary>
        /// Accesseur de id
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        /// <summary>
        /// Les positions x et y de la cellule
        /// </summary>
        private int xPos, yPos;

        /// <summary>
        /// Accesseur de xPos
        /// </summary>
        public int Xpos
        {
            get
            {
                return xPos;
            }
            set
            {
                xPos = value;
            }
        }

        /// <summary>
        /// Accesseur de yPos
        /// </summary>
        public int Ypos
        {
            get
            {
                return yPos;
            }
            set
            {
                yPos = value;
            }
        }

        /// <summary>
        /// Le controlleur de grille de jeu.
        /// </summary>
        [SerializeField]
        public GameGridController grid;

        /// <summary>
        /// La position r�elle vers laquelle se d�place la cellule quand elle est en mouvement.
        /// </summary>
        private Vector3 targetPosition;


        /// <summary>
        /// R�initialise la position de la cellule.
        /// </summary>
        private void Reset()
        {

        }

        /// <summary>
        /// R�agit au click en demandant � la grille s'il est possible de se d�placer.
        /// </summary>
        public void OnClick()
        {
            grid.MoveCell(this);
        }

        public bool IsMiddleCell()
        {
            return id == MIDDLE_CELL_ID;
        }


        /// <summary>
        /// Ordonne � la cellule de se d�placer vers un autre emplacement.
        /// </summary>
        /// 
        /// <param name="targetPos">
        /// La position r�elle souhait�e
        /// </param>
        /// 
        /// <param name="targetPosId">
        /// La position d'indentifiant dans la grille souhait�e
        /// </param>
        /// 
        /// <param name="moveSpeed">
        /// La vitesse de d�placement pour ce trajet (si non-renseign�e, met la vitesse par d�faut).
        /// </param>
        public IEnumerator UpdatePosition(Vector3 targetPos, int xTargetPos, int yTargetPos, bool changePos = true, float moveSpeed = -1)
        {
            grid.moving = true;
            targetPosition = targetPos;
            Vector3 startPosition = transform.localPosition;
            float time = 0;

            yield return null;

            //Le taquin est arriv�, on assigne la position.
            xPos = xTargetPos;
            yPos = yTargetPos;

            //Mettre la vitesse par d�faut si elle n'est pas sp�cifi�e
            if (moveSpeed == -1)
            {
                moveSpeed = MOVE_SPEED;
            }

            //D�placer la cellule tant qu'elle n'a pas atteint sa destination
            while (targetPosition != this.transform.localPosition)
            {
                time += moveSpeed * Time.deltaTime;
                this.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, time);
                yield return null;
            }


            grid.moving = false;

            grid.CheckWin();

            yield return null;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}