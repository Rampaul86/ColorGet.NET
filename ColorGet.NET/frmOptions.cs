using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ColorGet.NET
{
    public partial class frmOptions : Form
    {
        public class values
        {
            public static int topL = -1, topM = -1, topR = -1, midL = -1, midM = 8, midR = -1, botL = -1, botM = -1, botR = -1;
        }

        public frmOptions()
        {
            InitializeComponent();
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
    
            Form1 frmMain = new Form1();
            textBox1.Text = frmOptions.values.topL.ToString();
            textBox2.Text = frmOptions.values.topM.ToString();
            textBox3.Text = frmOptions.values.topR.ToString();
            textBox4.Text = frmOptions.values.midL.ToString();
            textBox5.Text = frmOptions.values.midM.ToString();
            textBox6.Text = frmOptions.values.midR.ToString();
            textBox7.Text = frmOptions.values.botL.ToString();
            textBox8.Text = frmOptions.values.botM.ToString();
            textBox9.Text = frmOptions.values.botR.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            values.topL = Int32.Parse(textBox1.Text);
            values.topM = Int32.Parse(textBox2.Text);
            values.topR = Int32.Parse(textBox3.Text);

            values.midL = Int32.Parse(textBox4.Text);
            values.midM = Int32.Parse(textBox5.Text);
            values.midR = Int32.Parse(textBox6.Text);

            values.botL = Int32.Parse(textBox7.Text);
            values.botM = Int32.Parse(textBox8.Text);
            values.botR = Int32.Parse(textBox9.Text);
            Close();
        }
    }
}