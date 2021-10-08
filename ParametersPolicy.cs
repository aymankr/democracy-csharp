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
    public partial class ParametersPolicy : Form
    {
        public int value { get { return (int)newValue.Value; } }

        public ParametersPolicy(int v, IndexedValue i)
        {
            InitializeComponent();
            newValue.Minimum = i.MinValue;
            newValue.Maximum = i.MaxValue;
            min.Text = i.MinValue.ToString();
            max.Text = i.MaxValue.ToString();
            val.Text = i.Value.ToString();
            newValue.Value = v;
        }

        private void newValue_Scroll(object sender, EventArgs e)
        {
            val.Text = newValue.Value.ToString();
            Refresh();
        }
    }
}
