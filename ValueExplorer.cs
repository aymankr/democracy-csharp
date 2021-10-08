using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace BaseSim2021
{
    public partial class ValueExplorer : Form
    {
        private IndexedValue i { get; set; }

        private IndexedValueView iView { get; set; }

        public ValueExplorer(IndexedValue ind, IndexedValueView indView)
        {
            InitializeComponent();
            i = ind;
            iView = indView;
            nom.Text += ind.Name;
            description.Text += ind.Description + ".";
            type.Text += ind.Type.ToString();
            trackBar1.Maximum = ind.MaxValue;
            trackBar1.Minimum = ind.MinValue;
            trackBar1.Value = ind.Value;
            value.Text += ind.Value.ToString();
            minvalue.Text += ind.MinValue.ToString();
            maxvalue.Text += ind.MaxValue.ToString();
            gloryimpact.Text += ind.GloryImpacted.ToString();
            gloryamount.Text += ind.GloryAmount.ToString();
            moneyimpact.Text += ind.MoneyImpacted.ToString();
            moneyamount.Text += ind.MoneyAmount.ToString();
            afficherStatsLiens(ind);
            if (!i.Type.ToString().Equals("Policy"))
            {
                trackBar1.Enabled = false;
            }
        }

        private void afficherStatsLiens(IndexedValue i)
        {
            foreach (IndexedValue p in i.OutputWeights.Keys)
            {
                i.OutputWeights.TryGetValue(p, out double val);
                int index = chart1.Series[0].Points.AddXY(p.Name, val);
                DataPoint points = chart1.Series[0].Points[index];
                if (points.YValues[0] < 0)
                {
                    points.Color = Color.Red;
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            value.Text = "Value : " + trackBar1.Value.ToString();
            int valueScroll = trackBar1.Value;
            i.PreviewPolicyChange(ref valueScroll, out int mCost, out int gCost);
            moneyamount.Text = "Money amount : " + mCost.ToString();
            gloryamount.Text = "Glory amount : " + gCost.ToString();
        }

        private void appliquerModif_Click(object sender, EventArgs e)
        {
            String[] chaineValeur = value.Text.Split(' ');
            Int32.TryParse(chaineValeur[2], out int newVal);
            GameController.ApplyPolicyChanges(i.Name + " " + newVal);
            iView.pBar.Value = newVal;
        }
    }
}
