using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSim2021
{
    public class Trait : IAffichable
    {
        public IndexedValueView Source { get; set; }
        public IndexedValueView Destination { get; set; }
        public bool Affiche { get; set; } = true;

        public float epaisseur { get; set; }

        public Color couleur { get; set; } = Color.Blue;

        public double lienSigne { get; set; }

        public String lienImportance { get; set; }

        public void Dessine(Graphics g)
        {
            Pen p = getPen();
            Point milieu = new Point((Source.Centre.X + Destination.Centre.X) / 2, (Source.Centre.Y + Destination.Centre.Y) / 2);
            g.DrawLine(p, Source.Centre, Destination.Centre);
            g.DrawString(lienImportance, new Font("Arial", 11, FontStyle.Regular), Brushes.Black, milieu);
        }
        
        private Pen getPen()
        {
            System.Drawing.Drawing2D.DashStyle style = System.Drawing.Drawing2D.DashStyle.Dot;

            String signe = "+";
            if (lienSigne < 0) { signe = "-"; couleur = Color.Red; }

            if (lienSigne.ToString().Contains("0,00"))
            {
                lienImportance = "+".Replace("+", signe);
                epaisseur = 1;
            }
            else if (lienSigne.ToString().Contains("0,0"))
            {
                lienImportance = "+ +".Replace("+", signe);
                epaisseur = 2;
                style = System.Drawing.Drawing2D.DashStyle.DashDot;
            }
            else
            {
                lienImportance = "+ + +".Replace("+", signe);
                epaisseur = 2.5f;
                style = System.Drawing.Drawing2D.DashStyle.Dash;
            }

            Pen p = new Pen(couleur, epaisseur);
            p.DashStyle = style;
            return p;
        }
    }
}
