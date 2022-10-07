using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeasingGame
{
    /// <summary>
    /// Classe représentant une Cellule (un taquin) de la grille.
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
        /// Vitesse de déplacement par défaut des cellules
        /// </summary>
        private static readonly float MOVE_SPEED = 10f;

        /// <summary>
        /// Vitesse de déplacement lors de la réinitialisation de la grille
        /// </summary>
        public static readonly float RESET_MOVE_SPEED = 17f;


        /// <summary>
        /// L'identifiant (de 0 à 8) du taquin selon sa position voulue dans la grille.
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
        /// La position réelle vers laquelle se déplace la cellule quand elle est en mouvement.
        /// </summary>
        private Vector3 targetPosition;


        /// <summary>
        /// Réinitialise la position de la cellule.
        /// </summary>
        private void Reset()
        {

        }

        /// <summary>
        /// Réagit au click en demandant à la grille s'il est possible de se déplacer.
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
        /// Ordonne à la cellule de se déplacer vers un autre emplacement.
        /// </summary>
        /// 
        /// <param name="targetPos">
        /// La position réelle souhaitée
        /// </param>
        /// 
        /// <param name="targetPosId">
        /// La position d'indentifiant dans la grille souhaitée
        /// </param>
        /// 
        /// <param name="moveSpeed">
        /// La vitesse de déplacement pour ce trajet (si non-renseignée, met la vitesse par défaut).
        /// </param>
        public IEnumerator UpdatePosition(Vector3 targetPos, int xTargetPos, int yTargetPos, bool changePos = true, float moveSpeed = -1)
        {
            grid.moving = true;
            targetPosition = targetPos;
            Vector3 startPosition = transform.localPosition;
            float time = 0;

            yield return null;

            //Le taquin est arrivé, on assigne la position.
            xPos = xTargetPos;
            yPos = yTargetPos;

            //Mettre la vitesse par défaut si elle n'est pas spécifiée
            if (moveSpeed == -1)
            {
                moveSpeed = MOVE_SPEED;
            }

            //Déplacer la cellule tant qu'elle n'a pas atteint sa destination
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