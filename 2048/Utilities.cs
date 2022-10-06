using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

#pragma warning disable CS8603

namespace _2048
{
    /// <summary>
    /// Fonctions utile
    /// </summary>
    internal class Utilities
    {
        // Permet de convertir un code HEX en couleur Brush
        private static BrushConverter ColorConverter = new BrushConverter();

        /// <summary>
        /// Renvois la couleur du texte de la case en fonction du numéro
        /// </summary>
        /// <param name="piece_number">Le numéro de la case</param>
        /// <returns></returns>
        internal static Brush ForegroundColorOf(int piece_number)
        {
            if(piece_number <= 4)
                return (Brush)ColorConverter.ConvertFromString("#776E65");
            else                
                return (Brush)ColorConverter.ConvertFromString("#F9F5F2");
        }

        /// <summary>
        /// Renvois la couleur de la case en fonction du numéro
        /// </summary>
        /// <param name="piece_number">Le numéro de la case</param>
        /// <returns>La couleur de la pièce</returns>
        internal static Brush ColorOf(int piece_number)
        {
            switch (piece_number)
            {
                case 2:
                    return (Brush)ColorConverter.ConvertFromString("#EEE4DA");
                case 4:
                    return (Brush)ColorConverter.ConvertFromString("#F2D6DA");
                case 8:
                    return (Brush)ColorConverter.ConvertFromString("#F2B179");
                case 16:
                    return (Brush)ColorConverter.ConvertFromString("#F68160");
                case 32:
                    return (Brush)ColorConverter.ConvertFromString("#F5673F");
                case 64:
                    return (Brush)ColorConverter.ConvertFromString("#F5673F");
                case 128:
                    return (Brush)ColorConverter.ConvertFromString("#ECCD70");
                case 256:
                    return (Brush)ColorConverter.ConvertFromString("#EDCC61");
                case 512:
                    return (Brush)ColorConverter.ConvertFromString("#EDC951");
                case 1024:
                    return (Brush)ColorConverter.ConvertFromString("#EDC742");
                case 2048:
                    return (Brush)ColorConverter.ConvertFromString("#ECC333");
                default: // > 2048
                    return (Brush)ColorConverter.ConvertFromString("#3D3A33");

            }
        }
    }
}
