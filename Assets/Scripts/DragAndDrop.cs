using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TeasingGame
{
    /// <summary>
    /// Permet aux objets d'être drag and drop
    /// </summary>
    public class DragAndDrop : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        /// <summary>
        /// Permet de rendre le drag and drop un peu moins compliqué et raccourcir la distance vers la case vide necessaire pour réaliser l'échange de cellule
        /// </summary>
        static readonly float COEF_DISTANCE_TARGET = 0.8f;

        /// <summary>
        /// La cellule attachée
        /// </summary>
        [SerializeField]
        Cell cell;

        [SerializeField]
        private float dampingSpeed = 0.005f;

        private RectTransform draggingObjectRectTransform;
        private Vector3 velocity = Vector3.zero;

        private void Awake()
        {
            draggingObjectRectTransform = transform as RectTransform;
        }

        /// <summary>
        /// COmportement lors du drag and drop
        /// </summary>
        /// <param name="eventData">
        /// Les données de l'évènement
        /// </param>
        public void OnDrag(PointerEventData eventData)
        {
            if (!cell.grid.play)
            {
                return;
            }

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingObjectRectTransform, eventData.position, eventData.pressEventCamera, out var globalMousePosition))
            {
                draggingObjectRectTransform.position = Vector3.SmoothDamp(draggingObjectRectTransform.position, globalMousePosition, ref velocity, dampingSpeed);
            }
        }

        /// <summary>
        /// COmportement lors de la fin de drag and drop, c'est à dire bouger la cellule si elle est proche de la case vide
        /// </summary>
        /// <param name="eventData">
        /// Les données de l'évènement
        /// </param>
        public void OnEndDrag(PointerEventData eventData)
        {
            Cell emptyCell = cell.grid.GetEmptyCell();

            //Si la distance vers la case vide est plus petite que la distance vers la position actuelle, on déplace.
            float distanceTarget = Vector2.Distance(cell.transform.localPosition, emptyCell.gameObject.transform.localPosition);
            float distanceSource = Vector2.Distance(cell.transform.localPosition, cell.grid.initialPositions[cell.Xpos, cell.Ypos]);

            if(COEF_DISTANCE_TARGET*distanceTarget < distanceSource)
            {
                cell.grid.MoveCell(cell);
            }  
            else
            {
                cell.grid.MoveCell(cell, cell);
            }
        }
    }
}