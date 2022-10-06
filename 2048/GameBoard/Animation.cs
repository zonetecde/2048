using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace _2048.GameBoard
{
    internal class Animation
    {
        /// <summary>
        /// Animation de grossissement style "pop up"
        /// </summary>
        /// <param name="border">La bordure ou faire l'animation</param>
        /// <param name="minSize">Taille au début de l'animation</param>
        /// <param name="maxSize">Taille final de l'animation</param>
        internal static void PopUp(Border border, double minSize = 0, double maxSize = 100)
        {
            // Applique la taille que doit avoir la case au début de l'animation
            border.Width = minSize;
            border.Height = minSize;

            // Timer : speed de l'animation
            Timer t_anim = new Timer(5);
            MainWindow.IsInMovement = true;

            t_anim.Elapsed += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Si la size actuelle est toujours inférieur à ce qui est attendu
                    if (border.Width < maxSize)
                    {
                        // Augmente légèrement la taille 
                        border.Width += 8;
                        border.Height += 8;
                    }
                    else // Si c'est la bonne taille
                    {
                        // Si border.Width > maxSize
                        border.Width = Double.NaN;
                        border.Height = Double.NaN;
                        border.HorizontalAlignment = HorizontalAlignment.Stretch;
                        border.VerticalAlignment = VerticalAlignment.Stretch;

                        // On stop l'animation
                        MainWindow.IsInMovement = false;
                        t_anim.Stop();
                    }
                });
            };

            t_anim.Start();
        }

        /// <summary>
        /// Animation de style "slide" entre 2 coordonnée
        /// </summary>
        /// <param name="canvas_gameBoard_animation">Le canvas où il y aura l'animation</param>
        /// <param name="posDepart">La pos de départ de l'animation</param>
        /// <param name="posArrivee">La pos de fin de l'animation</param>
        /// <param name="userControl_GameBoardCase">Référence pour l'UI à animer</param>
        internal static void SlideAnimation(Canvas canvas_gameBoard_animation, Point posDepart, Point posArrivee, UserControl_GameBoardCase userControl_GameBoardCase, UserControl_GameBoardCase userControl_GameBoardCase_final, int finalNumber)
        {
            double SLIDE_ANIMATION_SPEED = 40;

            // Créer le contrôle de l'animation
            UserControl_GameBoardCase uc_anim = new UserControl_GameBoardCase();
            uc_anim.Width = userControl_GameBoardCase.ActualWidth;
            uc_anim.Height = userControl_GameBoardCase.ActualHeight;
            uc_anim.ChangeCaseNumber(userControl_GameBoardCase.CaseNumber, false);

            // Le positionne en pos de départ
            canvas_gameBoard_animation.Children.Add(uc_anim);
            Canvas.SetLeft(uc_anim, posDepart.X);
            Canvas.SetTop(uc_anim, posDepart.Y);

            // le UC doit se déplacer sur l'axe des abscisses ou des ordonnées ?
            bool directionAbscisse = false;
            // le UC va dans quelle direction (de la droite vers la gauche, ou de la gauche vers la droite?)
            bool remonte = false;

            if (posDepart.X != posArrivee.X)
            {
                directionAbscisse = true;

                if (posDepart.X > posArrivee.X)                
                    remonte = true;
                
            }
            // bas en haut = remonte
            else if(posDepart.Y > posArrivee.Y)
                remonte = true;

            // distance à parcourir :
            double dParcourir = 0;
            if(directionAbscisse)
            {
                dParcourir = Math.Abs(posDepart.X - posArrivee.X);
            }
            else
            {
                dParcourir = Math.Abs(posDepart.Y - posArrivee.Y);
            }

            // calcul le temps d'animation en fonction de la distance à parcourir
            SLIDE_ANIMATION_SPEED = dParcourir / (double)14;

            // Timer : speed de l'animation
            Timer t_anim = new Timer(10);
            MainWindow.IsInMovement = true;
            t_anim.Elapsed += (sender, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if(directionAbscisse)
                    {
                        if(remonte)
                            Canvas.SetLeft(uc_anim, Canvas.GetLeft(uc_anim) - SLIDE_ANIMATION_SPEED);
                        else
                            Canvas.SetLeft(uc_anim, Canvas.GetLeft(uc_anim) + SLIDE_ANIMATION_SPEED);
                    }
                    // ordonnée 
                    else if(remonte)
                        Canvas.SetTop(uc_anim, Canvas.GetTop(uc_anim) - SLIDE_ANIMATION_SPEED);
                    else
                        Canvas.SetTop(uc_anim, Canvas.GetTop(uc_anim) + SLIDE_ANIMATION_SPEED);

                    // Regarde si l'animation est fini
                    // abscisse 
                    if (directionAbscisse)
                    {
                        if ((remonte && Canvas.GetLeft(uc_anim) < posArrivee.X) || (!remonte && Canvas.GetLeft(uc_anim) > posArrivee.X))
                        {
                            userControl_GameBoardCase_final.ChangeCaseNumber(finalNumber, false);
                            canvas_gameBoard_animation.Children.Remove(uc_anim);
                            t_anim.Stop();
                            MainWindow.IsInMovement = false;
                        }
                    }
                    // ordonnée
                    else if ((remonte && Canvas.GetTop(uc_anim) < posArrivee.Y) || (!remonte && Canvas.GetTop(uc_anim) > posArrivee.Y))
                    {
                        userControl_GameBoardCase_final.ChangeCaseNumber(finalNumber, false);
                        canvas_gameBoard_animation.Children.Remove(uc_anim);
                        t_anim.Stop();
                        MainWindow.IsInMovement = false;
                    }
                });
            };

            t_anim.Start();
        }
    }
}