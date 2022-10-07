using _2048.GameBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _2048
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // VARIABLES

        // Relative au plateau de jeu :
        // Y;X
        private static UserControl_GameBoardCase[,] GameBoard; // Le plateau de jeu contenant les numéros. 0 = aucun numéro dans la case.

        // Relative au déroulement du jeu :
        Random Rdn = new Random();
        bool DidMovement; // est-ce que un mouvement a été fait dans une des 4 directions
        internal static bool IsInMovement; // est-ce que un mouvement est en cours?
        internal static List<Coordonnées> changedCo; // liste des coordonées changé lors d'un déplacement
        private int Score;


        public MainWindow()
        {
            InitializeComponent();

            // - Initialize les valeurs dans le tableau 2D GameBoard
            // - Ajout du plateau de jeu
            InitVariables();

        }

        private void InitVariables()
        {
            GameBoard = new UserControl_GameBoardCase[4, 4];
            DidMovement = false; // est-ce que un mouvement a été fait dans une des 4 directions
            IsInMovement = false; // est-ce que un mouvement est en cours?
            changedCo = new List<Coordonnées>(); // liste des coordonées changé lors d'un déplacement
            Score = 0;

            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                {
                    GameBoard[x, y] = new UserControl_GameBoardCase(); // 0 = la case est vide
                    uniformGrid_gameBoard.Children.Add(GameBoard[x, y]);
                }
        }

        /// <summary>
        /// 
        /// _Called_when:
        ///     La taille de la fenêtre a changé
        ///     
        /// _Do:
        ///     Change la taille du border_GameBoard et du canvas_gameBoard_animation (qui sont superposé) pour qu'elles
        ///     occupent la place majoritaire sur la fenêtre en fonction de sa taille
        ///     
        /// </summary>
        /// 
        /// <param name="e">Permet de récupérer la nouvelle taille de la fenêtre</param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // init de la variable ou sera stocker la taille que doit avoir l'uniformGrid_gameBoard et le canvas_gameBoard_animation
            double boardSize;

            // Si la fenêtre est plus grande en largeur après le redimensionnement
            if (e.NewSize.Width >= e.NewSize.Height)
            {
                // La taille est prise de la longueur  de la fenêtre (qui est plus petite)
                boardSize = e.NewSize.Height - 80; // 40 de margin en haut et en bas, d'où le - 40
            }
            else // Si la fenêtre est plus grande en longueur après le redimensionnement
            {
                // La taille est prise de la largeur de la fenêtre (qui est plus petite)
                boardSize = e.NewSize.Width - 80; // 40 de margin en haut et en bas, d'où le - 40
            }

            // Application de la taille obtenue à le border_GameBoard et le canvas_gameBoard_animation
            border_GameBoard.Width = boardSize;
            border_GameBoard.Height = boardSize;

            canvas_gameBoard_animation.Height = boardSize;
            canvas_gameBoard_animation.Width = boardSize;
        }

        /// <summary>
        /// 
        /// _Called_when:
        ///     La fenêtre à finit de s'initialiser
        ///     
        /// _Do:
        ///     Lance le jeu ; ajoute 2 cases de "2" aléatoirement dans le gameBoard
        /// 
        /// </summary>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
                AddPieceInRandomPlace();

        }

        /// <summary>
        /// 
        /// _Called_when:
        ///     Au lancement du jeu dans le Window_ContentRendered(), ou après avoir fait un déplacement
        ///     des pièces dans une des 4 directions
        ///     
        /// _Do:
        ///     Ajoute la pièce (par défaut 2) dans une case aléatoire non remplis par un autre nombre
        ///     et vérifie la condition suivante

        /// 
        /// </summary>
        /// <param name="piece_number">Le numéro de la pièce à ajouter, par défaut 2</param>
        internal void AddPieceInRandomPlace(int piece_number = 2, int y_ = -1, int x_ = -1)
        {
            // On liste les cases vides
            var coordonnées_case_vide = new List<Coordonnées>();

            for (int y = 0; y < 4; y++)          
                for (int x = 0; x < 4; x++)               
                    if (GameBoard[x, y].CaseNumber == 0) // Si la case du gameBoard est vide
                        coordonnées_case_vide.Add(new Coordonnées(x, y));

            // Si le plateau a en moins une case vide
            if(coordonnées_case_vide.Any())
            {
                // On prend une case vide aléatoire 
                Coordonnées rdn_co = coordonnées_case_vide[Rdn.Next(coordonnées_case_vide.Count)];

                // Si coordonné non random
                if (y_ != -1 && x_ != -1)
                {
                    rdn_co.X = x_;
                    rdn_co.Y = y_;                    
                }

                // On ajoute à cette case vide le numéro 'piece_number'
                GameBoard[rdn_co.X, rdn_co.Y].ChangeCaseNumber(piece_number);
            }

            // Si aucun déplacement n'est possible on perd la partie 
            if (!IsMovingPossible())
            {
                // GAME OVER
                grid_gameOver.Visibility = Visibility.Visible;
                textblock_gameOver.Text = "GAME OVER!\nVotre score est de " + Score;
            }
        }

        /// <summary>
        /// 
        /// _Called_when:
        ///     Après qu'on ai ajouté une pièce au plateau AddPieceInRandomPlace() et que l'on veut
        ///     savoir si le joueur est coincé ou pas
        /// 
        ///_Do:
        ///     Si le plateau est remplis:
        ///         Si une des 4 directions est possible (une pièce
        ///         sera combiné avec une autre):
        ///             return true
        ///         Si aucune des 4 directions est possible :
        ///             return false
        ///     Si le plateau a une ou plusieurs case(s) vide:
        ///         return true
        ///                 
        /// 
        /// </summary>
        /// <returns>Est-ce que l'ont peut bouger dans une des 4 directions (= est-ce que le joueur a perdu)</returns>
        internal static bool IsMovingPossible()
        {
            // si il y a une case vide une direction est forcément possible

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (GameBoard[x, y].CaseNumber == 0)
                        return true;
                }
            }

            // check pour un plateau plein
            // si 2 même numéro sont à côté = true 
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    try
                    {
                        if (GameBoard[y - 1, x].CaseNumber == GameBoard[y, x].CaseNumber)
                            return true;
                    }
                    catch { }
                    try
                    {
                        if (GameBoard[y + 1, x].CaseNumber == GameBoard[y, x].CaseNumber)
                            return true;
                    }
                    catch { }
                    try
                    {
                        if (GameBoard[y, x + 1].CaseNumber == GameBoard[y, x].CaseNumber)
                            return true;
                    }
                    catch { }
                    try
                    {
                        if (GameBoard[y, x - 1].CaseNumber == GameBoard[y, x].CaseNumber)
                            return true;
                    }
                    catch { }
                }
            }

            // game over, sinon on serait déjà sortie de la fonction
            return false;
        }

        /// <summary>
        /// 
        /// _Called_When:
        ///     Une touche du clavier a été appuyé
        /// 
        /// _Do:
        ///     Si la touche du clavier appuyé est une des 4 flèches directionnel, on appel la méthode permettant d'effectuer le
        ///     mouvement sur le plateau de jeu.
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // une des 4 flèches directionnel est enfoncé
            if(e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
            {
                // effectue le mouvement 
                MoveInDirection(e.Key, false);
            }
        }

        /// <summary>
        /// 
        /// _Called_When:
        ///     Une des 4 flèches directionnel est appuyé (voir Window_KeyDown())
        /// 
        /// _Do:
        ///     Effectue le mouvement sur le plateau de jeu 
        /// 
        /// </summary>
        /// <param name="direction">La direction</param>
        private bool MoveInDirection(Key direction, bool directionCheck)
        {
            // si il y a pas animation on cours
            if (canvas_gameBoard_animation.Children.Count == 0 && !IsInMovement)
            {
                DidMovement = false;

                switch (direction)
                {
                    case Key.Right:

                        // On prend les lignes une par une
                        for (int y = 0; y < 4; y++)
                        {
                            // On prend les colonnes une par une en partant de la droite vers la gauche
                            // 2; 1; 0 car [3] ne peut pas aller plus à droite (indexOutOfRange)
                            for (int x = 2; x >= 0; x--)
                            {
                                // On regarde si on peut avancer cette case à droite
                                int caseNumber = GameBoard[y, x].CaseNumber;

                                if (caseNumber != 0)
                                {
                                    // On prend les coordonnées des cases à droite de la case se faisant check
                                    List<Coordonnées> coordonnées_possible = new List<Coordonnées>();
                                    for (int i = 3; i > x; i--)
                                        coordonnées_possible.Add(new Coordonnées(i, y));

                                    CheckAndMoveIfMovingPossible(caseNumber, coordonnées_possible, new Coordonnées(x, y), directionCheck);
                                }
                            }
                        }

                        break;

                    case Key.Down:

                        // On prend les colonnes une par une en partant de haut en bas
                        // 2; 1; 0 car [3] ne peut pas aller plus en bas (indexOutOfRange)
                        for (int y = 2; y >= 0; y--)
                        {
                            // On prend les colonnes une par une
                            for (int x = 0; x < 4; x++)
                            {
                                // On regarde si on peut avancer cette case en bas
                                int caseNumber = GameBoard[y, x].CaseNumber;

                                if (caseNumber != 0)
                                {
                                    // On prend les coordonnées des cases en haut de la case se faisant check
                                    List<Coordonnées> coordonnées_possible = new List<Coordonnées>();
                                    for (int i = 3; i > y; i--)
                                        coordonnées_possible.Add(new Coordonnées(x, i));

                                    CheckAndMoveIfMovingPossible(caseNumber, coordonnées_possible, new Coordonnées(x, y), directionCheck);
                                }
                            }
                        }

                        break;

                    case Key.Up:

                        // On prend les colonnes une par une en partant du bas vers le haut
                        // 1; 2; 3 car [0] ne peut pas aller plus en bas (indexOutOfRange)
                        for (int y = 1; y <= 3; y++)
                        {
                            // On prend les colonnes une par une
                            for (int x = 0; x < 4; x++)
                            {
                                // On regarde si on peut avancer cette case en haut
                                int caseNumber = GameBoard[y, x].CaseNumber;

                                if (caseNumber != 0)
                                {
                                    // On prend les coordonnées des cases en haut de la case se faisant check
                                    List<Coordonnées> coordonnées_possible = new List<Coordonnées>();
                                    for (int i = 0; i < y; i++)
                                        coordonnées_possible.Add(new Coordonnées(x, i));

                                    CheckAndMoveIfMovingPossible(caseNumber, coordonnées_possible, new Coordonnées(x, y), directionCheck);
                                }
                            }
                        }

                        break;

                    case Key.Left:

                        // On prend les lignes une par une
                        for (int y = 0; y < 4; y++)
                        {
                            // On prend les colonnes une par une en partant de la gauche vers la droite
                            // 1; 2; 3 car [0] ne peut pas aller plus à gauche (indexOutOfRange)
                            for (int x = 1; x <= 3; x++)
                            {
                                // On regarde si on peut avancer cette case à gauche
                                int caseNumber = GameBoard[y, x].CaseNumber;

                                if (caseNumber != 0)
                                {
                                    // On prend les coordonnées des cases à droite de la case se faisant check
                                    List<Coordonnées> coordonnées_possible = new List<Coordonnées>();
                                    for (int i = 0; i < x; i++)
                                        coordonnées_possible.Add(new Coordonnées(i, y));

                                    CheckAndMoveIfMovingPossible(caseNumber, coordonnées_possible, new Coordonnées(x, y), directionCheck);
                                }
                            }
                        }

                        break;
                }

                // Si on voulait juste check les directions on s'arrête ici
                if(directionCheck)
                    return DidMovement;

                // Ajoute une pièce car on a fait une direction
                if (DidMovement)
                {
                    changedCo.ForEach(x =>
                    {
                        GameBoard[x.Y, x.X].Tag = "";
                    });

                    changedCo.Clear();

                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            if (GameBoard[y, x].textBlock_case.Text == "0" || GameBoard[y, x].CaseNumber ==0)
                            {
                                GameBoard[y, x].border_case.Width = 0;
                                GameBoard[y, x].border_case.Height = 0;
                            }
                        }
                    }

                    AddPieceInRandomPlace();
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// _Called_When:
        ///     On veut déplacer une case sur une des coordonnées_possible
        ///     
        /// _Do:
        ///     Vérifie si l'on peut translater une pièce sur une case des coordonnées_possible
        /// 
        /// </summary>
        /// <param name="caseNumber"></param>
        /// <param name="coordonnées_possible"></param>
        /// <param name="cordonnée_toMove"></param>
        /// <param name="directionCheck"></param>
        private void CheckAndMoveIfMovingPossible(int caseNumber, List<Coordonnées> coordonnées_possible, Coordonnées cordonnée_toMove, bool directionCheck)
        {
            foreach(Coordonnées coordonnée in coordonnées_possible)
            {
                // on vérifie qu'il n'y a pas d'obstacle entre les 2 coordonnées (= une case différente de la case a déplacé)
                bool obastacle = false;

                for (int i = coordonnées_possible.IndexOf(coordonnée) + 1; i < coordonnées_possible.Count; i++)
                {
                    if (GameBoard[coordonnées_possible[i].Y, coordonnées_possible[i].X].CaseNumber != 0)
                    {
                        // pas possible : il y a un obstacle
                        obastacle = true;
                        break;
                    }
                }

                if (!obastacle)
                {
                    if (GameBoard[coordonnée.Y, coordonnée.X].CaseNumber == caseNumber
                        && GameBoard[coordonnée.Y, coordonnée.X].Tag != "changed")
                    {
                        // on veut pas vraiment faire bouger la pièce, on voulait
                        // juste voir si c'était possible
                        if (!directionCheck)
                        {
                            // préviens que l'on vient de changer ceci, donc qu'il n'est pas possible de combiner avec cette pièce
                            changedCo.Add(new Coordonnées(coordonnée.X, coordonnée.Y));

                            // On effectue une animation de déplacement 

                            Point posDepart = GameBoard[cordonnée_toMove.Y, cordonnée_toMove.X].TranslatePoint(new Point(0, 0), uniformGrid_gameBoard);
                            Point posArrivee = GameBoard[coordonnée.Y, coordonnée.X].TranslatePoint(new Point(0, 0), uniformGrid_gameBoard);

                            Animation.SlideAnimation(canvas_gameBoard_animation, posDepart, posArrivee, GameBoard[cordonnée_toMove.Y, cordonnée_toMove.X], GameBoard[coordonnée.Y, coordonnée.X], caseNumber * 2);

                            GameBoard[coordonnée.Y, coordonnée.X].Tag = "changed";
                            GameBoard[coordonnée.Y, coordonnée.X].CaseNumber = caseNumber * 2;
                            Score += caseNumber * 2;
                            label_score.Content = "SCORE : " + Score;
                            GameBoard[cordonnée_toMove.Y, cordonnée_toMove.X].CaseNumber = 0;
                            GameBoard[cordonnée_toMove.Y, cordonnée_toMove.X].ChangeCaseNumber(0, false);

                        }

                        DidMovement = true;
                        // On sort de la boucle, on l'a déplacé le plus à [direction] possible
                        break;

                    }
                    // Ou si la case est vide
                    else if (GameBoard[coordonnée.Y, coordonnée.X].CaseNumber == 0
                        && GameBoard[coordonnée.Y, coordonnée.X].Tag != "changed")
                    {
                        // on veut pas vraiment faire bouger la pièce, on voulait
                        // juste voir si c'était possible
                        if (!directionCheck)
                        {
                            // préviens que l'on vient de changer ceci, donc qu'il n'est pas possible de combiner avec cette pièce
                            changedCo.Add(new Coordonnées(coordonnée.X, coordonnée.Y));

                            // On effectue une animation de déplacement 
                            Point posDepart = GameBoard[cordonnée_toMove.Y, cordonnée_toMove.X].TranslatePoint(new Point(0, 0), uniformGrid_gameBoard);
                            Point posArrivee = GameBoard[coordonnée.Y, coordonnée.X].TranslatePoint(new Point(0, 0), uniformGrid_gameBoard);

                            Animation.SlideAnimation(canvas_gameBoard_animation, posDepart, posArrivee, GameBoard[cordonnée_toMove.Y, cordonnée_toMove.X], GameBoard[coordonnée.Y, coordonnée.X], caseNumber);

                            //GameBoard[coordonnée.Y, coordonnée.X].Tag = "changed";
                            GameBoard[coordonnée.Y, coordonnée.X].CaseNumber = caseNumber;
                            GameBoard[cordonnée_toMove.Y, cordonnée_toMove.X].CaseNumber = 0;
                            GameBoard[cordonnée_toMove.Y, cordonnée_toMove.X].ChangeCaseNumber(0, false);

                        }

                        DidMovement = true;
                        // On sort de la boucle, on l'a déplacé le plus à [direction] possible
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Recommence la game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grid_gameOver_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                grid_gameOver.Visibility = Visibility.Hidden;
                label_score.Content = "SCORE : 0";
                uniformGrid_gameBoard.Children.Clear();
                InitVariables();
                for (int i = 0; i < 2; i++)
                    AddPieceInRandomPlace();
            }
        }
    }
}
