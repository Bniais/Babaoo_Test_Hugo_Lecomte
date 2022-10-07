using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace TeasingGame
{
    /// <summary> 
    /// Permet de stocker la configuration actuelle du jeu, de le contr�ler et de valider les mouvements possibles.
    /// </summary>
    public class GameGridController : MonoBehaviour
    {
        /// <summary>
        /// Constante de taille de la grille
        /// </summary>
        static readonly int SIZE = 3;

        /// <summary>
        /// Constante de temps entre les animations individuelles des cellules lors de la r�initialisation de grille
        /// </summary>
        static readonly float SECONDS_BETWEEN_RESET_ANIM = 0.06f;

        /// <summary>
        /// Constante de temps que le joueur a pour termin� la partie
        /// </summary>
        static readonly float TIME_TO_COMPLETE = 3 * 60f;

        /// <summary>
        /// Temps restant pour la partie
        /// </summary>
        private float time;

        /// <summary>
        /// Texte affichant le temps restant
        /// </summary>
        [SerializeField]
        private Text timeText;

        /// <summary>
        /// Stocke l'�tat actif/inactif du jeu (true veut dire que le jeu est en cours)
        /// </summary>
        public bool play;

        /// <summary>
        /// Stocke l'�tat en mouvement / immobile de la grille.
        /// </summary>
        public bool moving = false;


        /// <summary>
        /// Le parent de la grille de mod�le
        /// </summary>
        [SerializeField]
        private Transform model;

        /// <summary>
        /// Les cellules de la grille de jeu
        /// </summary>
        private Cell[,] cells = new Cell[SIZE, SIZE];

        /// <summary>
        /// Les cellules de la grille de mod�le
        /// </summary>
        private Cell[,] cellsModel = new Cell[SIZE, SIZE];

        /// <summary>
        /// Position initiales des cellules de la grille de jeu
        /// </summary>
        public Vector3[,] initialPositions = new Vector3[SIZE, SIZE];

        /// <summary>
        /// Position initiales des cellules de la grille de jeu
        /// </summary>
        public Vector3[,] initialModelPositions = new Vector3[SIZE, SIZE];


        /// <summary>
        /// Les textes de victoire et d�faite
        /// </summary>
        [SerializeField]
        private Text winText, loseText;

        /// <summary>
        /// Le mod�le fini
        /// </summary>
        [SerializeField]
        private GameObject finishedModel;

        /// <summary>
        /// Determine si le mode d'Inputs est Drag and drop ou Simple Touch.
        /// </summary>
        bool dragAndDrop;

        /// <summary>
        /// Appel�e avant le premier Update, initialise le jeu
        /// </summary>
        void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Fait en sorte que le jeu soit en mode actif
        /// </summary>
        void Play()
        {
            play = true;
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Initialise le jeu
        /// </summary>
        private void Initialize()
        {
            //Attribuer les cellules
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    cells[x, y] = transform.GetChild(SIZE * y + x).gameObject.GetComponent<Cell>();
                    cells[x, y].Xpos = x;
                    cells[x, y].Ypos = y;
                    cells[x, y].Id = y * SIZE + x;

                    //Attribuer les bonnes textures des cellules
#if UNITY_ANDROID
                    cells[x, y].GetComponent<RawImage>().texture = Resources.Load("Android/" + (SIZE * y + x+1).ToString()) as Texture2D;
#elif UNITY_IOS
                    cells[x, y].GetComponent<RawImage>().texture = Resources.Load("Apple/" + (SIZE * y + x+1).ToString()) as Texture2D;
#endif

                    initialPositions[x, y] = cells[x, y].transform.localPosition;
                }
            }

            //Attribuer les cellules du mod�le
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    cellsModel[x, y] = model.GetChild(SIZE * y + x).gameObject.GetComponent<Cell>();
                    cellsModel[x, y].Xpos = x;
                    cellsModel[x, y].Ypos = y;
                    cellsModel[x, y].Id = y * SIZE + x;

                    //Attribuer les bonnes textures des cellules du mod�le non-fixe
#if UNITY_ANDROID
                    cellsModel[x, y].GetComponent<RawImage>().texture = Resources.Load("Android/" + (SIZE * y + x+1).ToString()) as Texture2D;
#elif UNITY_IOS
                    cellsModel[x, y].GetComponent<RawImage>().texture = Resources.Load("Apple/" + (SIZE * y + x+1).ToString()) as Texture2D;
#endif
                    initialModelPositions[x, y] = cellsModel[x, y].transform.localPosition;
                }
            }

            //Attribuer le bon mod�le fixe
            if (PlayerPrefs.GetInt("ToggleModelSlidingGame", 1) != 0)
            {
#if UNITY_ANDROID
                finishedModel.GetComponent<RawImage>().texture = Resources.Load("Android/Result") as Texture2D;
#elif UNITY_IOS
            finishedModel.GetComponent<RawImage>().texture = Resources.Load("Apple/Result") as Texture2D;
#endif
            }
            else
            {
                finishedModel.SetActive(false);
            }


            //Input modes :
            dragAndDrop = PlayerPrefs.GetInt("ToggleTouchSlidingGame", 0) == 0;
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    if(x != 1 || y != 1)//La cellule du milieu n'a pas de bouton
                    {
                        transform.GetChild(SIZE * y + x).gameObject.GetComponent<Button>().interactable = !dragAndDrop;
                        transform.GetChild(SIZE * y + x).gameObject.GetComponent<DragAndDrop>().enabled = dragAndDrop;
                    } 
                        
                }
            }

            time = TIME_TO_COMPLETE;
            Play();
            OnResetClick();
        }

        /// <summary>
        /// Appel� chaque frame pour actualiser le temps
        /// </summary>
        void Update()
        {
            if (play)
            {
                UpdateTime();
                if (CheckTime())
                {
                    Lose();
                }
            }
           
        }

        /// <summary> 
        /// Met � jour le temps restant
        /// </summary> 
        void UpdateTime()
        {
            time -= Time.deltaTime;
            UpdateTimeText();
        }

        /// <summary>
        /// Met � jour l'affichage du temps restant.
        /// </summary>
        void UpdateTimeText()
        {
            int minutes = Mathf.FloorToInt(time / 60F);
            int seconds = Mathf.FloorToInt(time - minutes * 60);
            string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
            timeText.text = niceTime;
        }

        /// <summary> 
        /// V�rifie que le temps n'est pas �coul�.
        /// </summary> 
        /// <returns>
        /// Retourne vrai si le temps est �coul�, sinon faux.
        /// </returns>
        bool CheckTime()
        {
            if (time < 0)
            {
                time = 0;
                return true;
            }
            return false;
        }

        /// <summary> 
        /// V�rifie que le temps n'est pas �coul�.
        /// </summary> >
        void Lose()
        {
            play = false;
            loseText.gameObject.SetActive(true);
        }

        /// <summary> 
        /// D�place une cellule 
        /// </summary>
        /// 
        /// <param name="cell">
        /// La cellule que l'on souhaite d�placer.
        /// </param>
        /// 
        /// <returns>
        /// Retourne vrai si le taquin peut �tre d�plac�, sinon faux.
        /// </returns>
        public bool MoveCell(Cell cell, Cell optCell = null)
        {
            //V�rifier les diff�rentes directions
            Cell targetCell;

            if(optCell != null)
            {
                targetCell = cell;
            }
            else{
                targetCell = CheckIfWeCanMove(cell);
            }


            cells[targetCell.Xpos, targetCell.Ypos] = cell;
            cells[cell.Xpos, cell.Ypos] = targetCell;

            StartCoroutine(cell.UpdatePosition(initialPositions[targetCell.Xpos, targetCell.Ypos], targetCell.Xpos, targetCell.Ypos));
            StartCoroutine(targetCell.UpdatePosition(initialPositions[cell.Xpos, cell.Ypos], cell.Xpos, cell.Ypos));

            StartCoroutine(cellsModel[targetCell.Xpos, targetCell.Ypos].UpdatePosition(initialModelPositions[cell.Xpos, cell.Ypos], cell.Xpos, cell.Ypos));
            StartCoroutine(cellsModel[cell.Xpos, cell.Ypos].UpdatePosition(initialModelPositions[targetCell.Xpos, targetCell.Ypos], targetCell.Xpos, targetCell.Ypos));

            Cell tmp = cellsModel[targetCell.Xpos, targetCell.Ypos];
            cellsModel[targetCell.Xpos, targetCell.Ypos] = cellsModel[cell.Xpos, cell.Ypos];
            cellsModel[cell.Xpos, cell.Ypos] = tmp;

            return true;
        }

        public Cell GetEmptyCell()
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    if (cells[x,y].Id == Cell.MIDDLE_CELL_ID)
                    {
                        return cells[x,y];
                    }
                }
            }

            throw new Exception("No emptycell found");
            return null;
        }

        /// <summary>
        /// V�rifie si la cellule pass�e en param�tre peut se d�placer
        /// </summary>
        /// 
        /// <param name="cell">
        /// La cellule � d�placer
        /// </param>
        /// 
        /// <returns>
        /// L� o� peut se d�placer la cellule si elle le peut, sinon cette m�me cellule
        /// </returns>
        private Cell CheckIfWeCanMove(Cell cell)
        {
            int Xpos = cell.Xpos;
            int Ypos = cell.Ypos;

            //V�rifier chaque direction
            Cell dirTry = CheckMoveLeft(Xpos, Ypos);
            if (dirTry != null)
            {
                return dirTry;
            }

            dirTry = CheckMoveRight(Xpos, Ypos);
            if (dirTry != null)
            {
                return dirTry;
            }

            dirTry = CheckMoveDown(Xpos, Ypos);
            if (dirTry != null)
            {
                return dirTry;
            }

            dirTry = CheckMoveUp(Xpos, Ypos);
            if (dirTry != null)
            {
                return dirTry;
            }

            return cell;
        }

        private Cell CheckMoveLeft(int Xpos, int Ypos)
        {
            if (Xpos != 0 && cells[Xpos - 1, Ypos].IsMiddleCell())
                return cells[Xpos - 1, Ypos];

            return null;
        }

        private Cell CheckMoveRight(int Xpos, int Ypos)
        {
            if (Xpos != SIZE - 1 && cells[Xpos + 1, Ypos].IsMiddleCell())
                return cells[Xpos + 1, Ypos];

            return null;
        }

        private Cell CheckMoveDown(int Xpos, int Ypos)
        {
            if (Ypos != SIZE - 1 && cells[Xpos, Ypos + 1].IsMiddleCell())
                return cells[Xpos, Ypos + 1];

            return null;
        }

        private Cell CheckMoveUp(int Xpos, int Ypos)
        {
            if (Ypos != 0 && cells[Xpos, Ypos - 1].IsMiddleCell())
                return cells[Xpos, Ypos - 1];

            return null;
        }


        /// <summary>
        /// M�thode de r�action au bouton Reset, qui lance le processus de r�initialisation de grille
        /// </summary>
        public void OnResetClick()
        {

            StopAllCoroutines();
            moving = false;

            StartCoroutine(Reset());
           
            
        }

        /// <summary> 
        /// R�initialise la grille � son �tat initial.
        /// </summary> 
        public IEnumerator Reset()
        {
            time = TIME_TO_COMPLETE;
            UpdateTimeText();
            play = false;

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    StartCoroutine(cells[x, y].UpdatePosition(initialPositions[Cell.MIDDLE_CELL_ID%3, Cell.MIDDLE_CELL_ID/4], Cell.MIDDLE_CELL_ID%3, Cell.MIDDLE_CELL_ID/3, false, Cell.RESET_MOVE_SPEED));
                    StartCoroutine(cellsModel[x, y].UpdatePosition(initialModelPositions[Cell.MIDDLE_CELL_ID % 3, Cell.MIDDLE_CELL_ID / 3], Cell.MIDDLE_CELL_ID % 3, Cell.MIDDLE_CELL_ID / 3, false, Cell.RESET_MOVE_SPEED));

                    yield return new WaitForSeconds(SECONDS_BETWEEN_RESET_ANIM);
                }
            }

            while (moving) yield return null;

            do
            {
                Shuffle();
            } while (!Solvable());

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    StartCoroutine(cells[x, y].UpdatePosition(initialPositions[cells[x, y].Xpos, cells[x, y].Ypos], cells[x, y].Xpos, cells[x, y].Ypos, true, Cell.RESET_MOVE_SPEED));
                    StartCoroutine(cellsModel[x, y].UpdatePosition(initialModelPositions[cellsModel[x, y].Xpos, cellsModel[x, y].Ypos], cellsModel[x, y].Xpos, cellsModel[x, y].Ypos, true, Cell.RESET_MOVE_SPEED));

                    yield return new WaitForSeconds(SECONDS_BETWEEN_RESET_ANIM);
                }
            }

            while (moving) yield return null;

            Play();
        }

        /// <summary>
        /// Teste si la grille est soluble, en comptant les inversions.
        /// </summary>
        /// 
        /// <returns>
        /// Si le nombre d'inversion est pair, la grille est soluble et cette m�thode retourne vrai, sinon faux.
        /// </returns>
        public bool Solvable()
        {
            int cellMiddleId = -1;
            int count = 0;
            for (int i = 0; i < SIZE * SIZE - 1; i++)
            {
                for (int j = i + 1; j < SIZE * SIZE; j++)
                {
                    if (cells[i % 3, i / 3].Id == 4)
                    {
                        cellMiddleId = i;
                    }
                    int iId = cells[i % 3, i / 3].Id == 4 ? 0 : (cells[i % 3, i / 3].Id == 0 ? 4 : cells[i % 3, i / 3].Id);
                    int jId = cells[j % 3, j / 3].Id == 4 ? 0 : (cells[j % 3, j / 3].Id == 0 ? 4 : cells[j % 3, j / 3].Id);

                    if (iId  > jId)
                        count++;
                }
            }

            return count % 2 == 0;
        }

        /// <summary> 
        /// M�lange la grille.
        /// </summary>
        /// 
        /// <remarks>
        /// On aurait pu m�langer la grille "� la main", en faisant des mouvements succ�sifs des cellules, j'ai cependant de d�cider de faire un m�lange
        /// al�atoire et v�rifier qu'elle est soluble pour permettre une animation de r�initialisation plus esth�tique.
        /// </remarks>
        private void Shuffle()
        {
            Cell[,] cpy = new Cell[SIZE, SIZE];
            Array.Copy(cells, cpy, SIZE*SIZE);

            Cell[,] cpyModel = new Cell[SIZE, SIZE];
            Array.Copy(cellsModel, cpyModel, SIZE * SIZE);

            int[] id = (Enumerable.Range(0, SIZE*SIZE)).OrderBy(a => UnityEngine.Random.value).ToArray();
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {

                    cpy[x, y].Xpos = id[SIZE * y + x] % 3;
                    cpy[x, y].Ypos = id[SIZE * y + x] / 3;

                    cpyModel[x, y].Xpos = id[SIZE * y + x] % 3;
                    cpyModel[x, y].Ypos = id[SIZE * y + x] / 3;
                }
            }

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    cells[cpy[x,y].Xpos, cpy[x, y].Ypos] = cpy[x, y];
                    cellsModel[cpyModel[x, y].Xpos, cpyModel[x, y].Ypos] = cpyModel[x, y];

                    /*cellsModel[x, y]s = cpy[x, y].Xpos;
                    cellsModel[x, y].Ypos = cpy[x, y].Ypos;*/
                }
            }
        }

        /// <summary> 
        /// V�rifie la condition de victoire du jeu.
        /// </summary> 
        public void CheckWin()
        {
            if (play)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    for (int y = 0; y < SIZE; y++)
                    {
                        if (cells[x, y].Id != SIZE * y + x)
                            return;
                    }
                }


                ScoreUpdater.UpdateScore(time);

                play = false;
                winText.gameObject.SetActive(true);
            }
        }

    }
}