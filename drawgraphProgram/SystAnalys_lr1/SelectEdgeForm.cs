using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SystAnalys_lr1
{
    public partial class SelectEdgeForm : Form
    {
        public Line line;
        public SelectEdgeForm(Line v)
        {
            InitializeComponent();
            line = v;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                line.weight = Convert.ToInt32(textBox1.Text);
            }
            catch
            {
                MessageBox.Show("Неверное значение веса ребра");
                return;
            }
            this.Close();
        }

        private void SelectEdgeForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
            }
        }
    }
}
