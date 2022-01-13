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
            radioButton1.Text =(line.v1+1).ToString()+ " -> "+(line.v2+1).ToString();
            radioButton1.Checked = true;
            radioButton2.Text= (line.v2+1).ToString() + " -> " + (line.v1+1).ToString();
            radioButton3.Text = "Без направления";
            textBox1.Text = "1";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                line.weight = Convert.ToInt32(textBox1.Text);
                if (radioButton1.Checked)
                    line.direction = radioButton1.Text;
                else if (radioButton2.Checked)
                    line.direction = radioButton2.Text;
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
