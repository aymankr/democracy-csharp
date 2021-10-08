using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseSim2021
{
    public partial class GameView : Form
    {
        private readonly WorldState theWorld;
        private List<IndexedValueView> allTypes;
        private List<IndexedValueView> policies;
        private List<IndexedValueView> perksAndCrisis;
        private List<IndexedValueView> indicators;
        private List<IndexedValueView> groups;
        private List<Trait> traits;
        private IndexedValueView selection;

        /// <summary>
        /// The constructor for the main window
        /// </summary>
        public GameView(WorldState world)
        {
            InitializeComponent();
            theWorld = world;
            allTypes = new List<IndexedValueView>();
            policies = chargerTypes(new Rectangle(0, 380, 1500, 300), theWorld.Policies, null);
            perksAndCrisis = chargerTypes(new Rectangle(0, 0, 1500, 300), theWorld.Perks, theWorld.Crises);
            indicators = chargerTypes(new Rectangle(0, 245, 850, 300), theWorld.Indicators, null);
            groups = chargerTypes(new Rectangle(735, 245, 850, 300), theWorld.Groups, null);
            traits = new List<Trait>();
        }
        /// <summary>
        /// Method called by the controler whenever some text should be displayed
        /// </summary>
        /// <param name="s"></param>
        public void WriteLine(string s)
        {
            List<string> strs = s.Split('\n').ToList();
            strs.ForEach(str => outputListBox.Items.Add(str));
            if (outputListBox.Items.Count > 0)
            {
                outputListBox.SelectedIndex = outputListBox.Items.Count - 1;
            }
            outputListBox.Refresh();
        }
        /// <summary>
        /// Method called by the controler whenever a confirmation should be asked
        /// </summary>
        /// <returns>Yes iff confirmed</returns>
        public bool ConfirmDialog()
        {
            string message = "Confirmer ?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            return MessageBox.Show(message, "", buttons) == DialogResult.Yes;
        }
        #region Event handling
        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode.Equals(Keys.Enter))
            {
                e.SuppressKeyPress = true; // Or beep.
                GameController.Interpret(inputTextBox.Text);
            }
        }

        private void GameView_Paint(object sender, PaintEventArgs e)
        {

            diffLabel.Text = "Difficulté : " + theWorld.TheDifficulty;
            turnLabel.Text = "Tour " + theWorld.Turns;
            moneyLabel.Text = "Trésor : " + theWorld.Money;
            gloryLabel.Text = "Gloire : " + theWorld.Glory;
            nextButton.Visible = true;
            policies.FindAll(i => estModifiable(i) && i.Affiche).ForEach(i => i.Dessine(e.Graphics));
            perksAndCrisis.FindAll(i => i.Affiche).ForEach(i => i.Dessine(e.Graphics));
            indicators.FindAll(i => i.Affiche).ForEach(i => i.Dessine(e.Graphics));
            groups.FindAll(i => i.Affiche).ForEach(i => i.Dessine(e.Graphics));
            traits.FindAll(i => i.Affiche).ForEach(t => t.Dessine(e.Graphics));
        }
        #endregion

        private List<IndexedValueView> chargerTypes(Rectangle r, List<IndexedValue> indVals, List<IndexedValue> indVals2)
        {
            List<IndexedValue> listIndexedValues = indVals;
            int margin = 45;
            int h = 60;
            int w = 60;
            Rectangle rectangle = r;
            int x = rectangle.X + margin;
            int y = rectangle.Y + margin;
            List<IndexedValueView> views = new List<IndexedValueView>();
            if (indVals2 != null) listIndexedValues.AddRange(indVals2); // ajouter crises avec perks

            foreach (IndexedValue p in listIndexedValues)
            {
                IndexedValueView indValView = new IndexedValueView(p, new Point(x, y));
                loadProgressBar(indValView, x, y);

                views.Add(indValView);
                x += w + margin;
                if (x > rectangle.Right)
                {
                    x = rectangle.X;
                    y += h + margin;
                }
                if (x > 1020)
                {
                    x = rectangle.X + margin;
                    y = rectangle.Y + margin + 100;  // retour à la ligne
                }
            }
            allTypes.AddRange(views);
            return views;
        }

        private IndexedValueView Selection(Point p)
        {
            return allTypes.FirstOrDefault(i => i.Affiche && i.Contient(p));
        }

        private void loadProgressBar(IndexedValueView i, int x, int y)
        {
            ProgressBar prog = new ProgressBar();
            prog.Location = new Point(x+13,y+50);
            prog.Enabled = false;
            prog.Minimum = i.ind.MinValue;
            prog.Maximum = i.ind.MaxValue;
            prog.Value = i.ind.Value;
            prog.Width = 55;
            prog.Height = 10;
            prog.Visible = false;
            i.pBar = prog;
            Controls.Add(prog);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            GameController.Interpret("suivant");
            Refresh();
        }

        public void LoseDialog(IndexedValue indexedValue)
        {
            if (indexedValue == null)
            {
                MessageBox.Show("Partie perdue : dette insurmontable.");
            }
            else
            {
                MessageBox.Show("Partie perdue : " + indexedValue.CompletePresentation());
            }
            nextButton.Enabled = false;
        }

        public void WinDialog()
        {
            MessageBox.Show("Partie gagnée.");
            nextButton.Enabled = false;
        }

        private void GameView_MouseDown(object sender, MouseEventArgs e)
        {
            selection = Selection(e.Location);
            if (selection != null && e.Button == MouseButtons.Left && estUnePolitique(selection) && estModifiable(selection))
            {
                Policy_Click(sender, e);
            }
            else if (selection != null && e.Button == MouseButtons.Left && !estUnePolitique(selection))
            {
                MessageBox.Show("Description : " + selection.ind.Description);
            }
            else if (selection != null && e.Button == MouseButtons.Right && estDisponible(selection))
            {
                ValueExplorer param = new ValueExplorer(selection.ind, selection);
                param.ShowDialog();
            }
            Refresh();
        }

        private void GameView_MouseMove(object sender, MouseEventArgs e)
        {
            selection = Selection(e.Location);
            if (selection != null && selection.ind.OutputWeights.Count > 0
                && (!estUnePolitique(selection) || estModifiable(selection)))
            {
                setAffichables(false); // cacher

                foreach (IndexedValue i in selection.ind.OutputWeights.Keys)
                {
                    selection.Affiche = true;
                    IndexedValueView fin = getFromIndexedValue(i);
                    fin.Affiche = true;
                    selection.ind.OutputWeights.TryGetValue(i, out double val);
                    Trait t = new Trait { Source = selection, Destination = fin, lienSigne = val };
                    t.Affiche = true;
                    traits.Add(t);
                }
            }
            else
            {
                traits.Clear();
                setAffichables(true); // faire tout réapparaître
            }
            Refresh();
        }

        private bool estUnePolitique(IndexedValueView i)
        {
            return i.ind.Type.ToString().Equals("Policy");
        }

        private bool estModifiable(IndexedValueView i)
        {
            return (bool)i.ind.Active || i.ind.AvailableAt <= theWorld.Turns;
        }

        private bool estDisponible(IndexedValueView i)
        {
            return ((estUnePolitique(i) && estModifiable(i)) || !estUnePolitique(i));
        }

        private void Policy_Click(object sender, EventArgs e)
        {
            ParametersPolicy param = new ParametersPolicy(selection.ind.Value, selection.ind);
            if (param.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                GameController.ApplyPolicyChanges(selection.ind.Name + " " + param.value.ToString());
                selection.pBar.Value = param.value;
            }
        }
        private void setAffichables(bool afficher)
        {
            // afficher ou cacher indexedViewViews et leurs progressBar
            allTypes.ForEach(i => { i.Affiche = afficher; i.pBar.Visible = afficher && estDisponible(i); } );

            // cacher ou montrer labels
            foreach (Label lbl in Controls.OfType<Label>())
            {
                if (lbl.Font.Size >= 13 && afficher)
                {
                    lbl.Show();
                }
                else if (lbl.Font.Size >= 13 && !afficher)
                {
                    lbl.Hide();
                }
            }
        }

        private IndexedValueView getFromIndexedValue(IndexedValue ind)
        {
            return allTypes.FirstOrDefault(i => ind.Equals(i.ind));
        }
    }
}
