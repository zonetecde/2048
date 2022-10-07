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

namespace _2048.GameBoard
{
    /// <summary>
    /// Logique d'interaction pour UserControl_GameBoardCase.xaml
    /// </summary>
    public partial class UserControl_GameBoardCase : UserControl
    {
        // Le nombre de la pièce
        // 0 = vide
        internal int CaseNumber = 0;

        /// <summary>
        /// Représente une case dans le plateau de jeu
        /// </summary>
        public UserControl_GameBoardCase()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Change le numéro de la case
        /// Si avant le changement la case est vide, on fait une animation pop up.
        /// Si avant le changement la case est déjà doté d'un numéro, on change directement sans animation car
        ///     l'animation ne se fait pas là. L'animation se fait en faisait un mouvent dans une direction.
        /// </summary>
        /// <param name="piece_number"></param>
        internal async void ChangeCaseNumber(int piece_number, bool anim = true)
        {
            if (piece_number != 0)
            {
                int temp_ = Convert.ToInt32((CaseNumber.ToString()));
                // Attribut la nouvelle valeur à la case
                CaseNumber = piece_number;

                // Change le numéro de la case
                textBlock_case.Text = piece_number.ToString();
                // Change la couleur de fond qui va avec la case
                border_case.Background = Utilities.ColorOf(piece_number);
                // Si piece_number == 2 ou == 4, le foreground est de la couleur #776E65
                // pour des raisons de visibilité
                textBlock_case.Foreground = Utilities.ForegroundColorOf(piece_number);

                // Si la case est vide et que l'on veut qu'elle soit un nombre
                if (temp_ == 0 &&  anim)
                {
                    // Animation pop up de la case
                    await Task.Delay(200);
                    Animation.PopUp(border_case, 0, this.ActualWidth);
                }

                // Si on veut que la case soit vide
                if (!anim) // Si on ne veut pas d'animation
                {
                    border_case.Width = Double.NaN;
                    border_case.Height = Double.NaN;
                    border_case.HorizontalAlignment = HorizontalAlignment.Stretch;
                    border_case.VerticalAlignment = VerticalAlignment.Stretch;
                }
            }
            else
            {
                border_case.Width = 0;
                border_case.Height = 0;
            }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(CaseNumber.ToString());
        }
    }
}
