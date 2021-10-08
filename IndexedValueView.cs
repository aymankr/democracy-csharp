using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseSim2021
{
    public class IndexedValueView : IAffichable
    {
        public Point coordonnees { get; set; }
        public Size taille { get; set; } = new Size(80, 80);

        public int epaisseur { get; set; } = 2;

        public Color couleur { get; set; }

        public IndexedValue ind { get; set; }

        public bool Affiche { get; set; } = true;

        public ProgressBar pBar { get; set; }

        public IndexedValueView(IndexedValue i, Point coord)
        {
            coordonnees = coord;
            ind = i;
        }

        public void Dessine(Graphics g)
        {
            Rectangle r = new Rectangle(coordonnees, taille);
            Rectangle r2 = new Rectangle(new Point(coordonnees.X, coordonnees.Y + 15), taille); // pour afficher nom vers le centre
            Pen p = new Pen(Color.Black, epaisseur);
            dessinerFigure(g, r, p);

            Brush black = Brushes.Black;
            StringFormat sf = new StringFormat();
            StringAlignment center = StringAlignment.Center;

            // noms
            placerElements(ind.Type.ToString(), new Font("Comic Sans Ms", 8, FontStyle.Bold | FontStyle.Italic), black, r, sf, center, g);
            placerElements(ind.Name, getFont(10), black, r2, sf, center, g);

            // valeurs
            sf.LineAlignment = StringAlignment.Far;
            placerElements(ind.Value.ToString(), getFont(10), Brushes.Red, r, sf, center, g);
            placerElements(ind.MinValue.ToString(), getFont(7), black, r, sf, StringAlignment.Near, g);
            placerElements(ind.MaxValue.ToString(), getFont(7), black, r, sf, StringAlignment.Far, g);

            pBar.Visible = Affiche;
        }

        private Font getFont(int taille) {
            return new Font("Comic Sans Ms", taille, FontStyle.Regular);
        }

        private void placerElements(String chaine, Font f, Brush b, Rectangle r, StringFormat sf, StringAlignment sa1, Graphics g)
        {
            sf.Alignment = sa1;
            g.DrawString(chaine, f, b, r, sf);
        }

        private void dessinerFigure(Graphics g, Rectangle r, Pen p)
        {
            switch (ind.Type.ToString())
            {
                case "Policy":
                    couleur = Color.FromArgb(178, 238, 255);
                    g.DrawRectangle(p, r);
                    g.FillRectangle(new SolidBrush(couleur), r);
                    break;
                case "Perk":
                    couleur = Color.FromArgb(188, 255, 143);
                    g.DrawEllipse(p, r);
                    g.FillEllipse(new SolidBrush(couleur), r);
                    break;
                case "Crisis":
                    couleur = Color.FromArgb(255, 199, 187);
                    g.DrawEllipse(p, r);
                    g.FillEllipse(new SolidBrush(couleur), r);
                    break;
                case "Indicator":
                    couleur = Color.FromArgb(255, 245, 115);

                    PointF[] pointsHexagone = new PointF[6];
                    int longueur = 50;
                    for (int i = 0; i < 6; i++)
                    {
                        pointsHexagone[i] = new PointF(
                            Centre.X + longueur * (float)Math.Cos(i * 60 * Math.PI / 180f),
                            Centre.Y + longueur * (float)Math.Sin(i * 60 * Math.PI / 180f));
                    }

                    g.DrawPolygon(p, pointsHexagone);
                    g.FillPolygon(new SolidBrush(couleur), pointsHexagone);
                    break;
                case "Group":
                    couleur = Color.FromArgb(231, 204, 255);
                    Point point1 = new Point(Centre.X, Centre.Y - 80);
                    Point point2 = new Point(Centre.X + 50, Centre.Y + 50);
                    Point point3 = new Point(Centre.X - 50, Centre.Y + 50);
                    Point[] trianglePoints = { point1, point2, point3 };
                    g.DrawPolygon(p, trianglePoints);
                    g.FillPolygon(new SolidBrush(couleur), trianglePoints);
                    break;
            }
        }

        public bool Contient(Point p)
        {
            Rectangle r = new Rectangle(coordonnees, taille);
            return r.Contains(p);
        }

        public Point Centre
        {
            get
            {
                return new Point(coordonnees.X + taille.Width / 2,
          coordonnees.Y + taille.Height / 2);
            }
        }
        //public void Deplace(Point p) => coordonnees = p;

    }
}
