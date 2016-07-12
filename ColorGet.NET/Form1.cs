using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;


namespace ColorGet.NET
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        public class globals
        {
            public static Bitmap image = null;
            public static int x;
            public static int y;
        }
        public static class slika
        {
            public static Bitmap slikaOriginal = null;
            public static Bitmap slikaWork = null;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            globals.image = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);
            pictureBox1.Image = globals.image;
            toolStripStatusLabel1.Text = "";
            toolStripStatusLabel2.Text = "";
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Clipboard.SetText(toolStripStatusLabel2.Text);
                globals.x = e.X;
                globals.y = e.Y;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            try
            {
                if (pictureBox1.Image != null && globals.image.Width > e.X && globals.image.Height > e.Y)
                {
                    //drag the image
                    if (e.Button == MouseButtons.Left)
                    {
                        int maxWidth = -(globals.image.Width - panel1.Width) - 17;
                        int maxHight = -(globals.image.Height - panel1.Height) - 17;
                        if ((pictureBox1.Left + e.X - globals.x) <= 0 && (pictureBox1.Left + e.X - globals.x) >= maxWidth)
                            pictureBox1.Left += e.X - globals.x;
                        if ((pictureBox1.Top + e.Y - globals.y) <= 0 && (pictureBox1.Top + e.Y - globals.y) >= maxHight)
                            pictureBox1.Top += e.Y - globals.y;
                    }

                    Color boja = globals.image.GetPixel(e.X, e.Y);
                    panel2.BackColor = boja;
                    toolStripStatusLabel1.Text = "RGB: " + boja.R.ToString() + "," + boja.G.ToString() + "," + boja.G.ToString();
                    toolStripStatusLabel2.Text = "#" + boja.R.ToString("x").ToUpper() + boja.G.ToString("x").ToUpper() + boja.B.ToString("x").ToUpper();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region perceptron
        ///<summary>
        /// Inicijalizacija matrice sa vrijednostima pixela
        /// implementacija percpetrona u vidu konvolucijskog filtera 
        ///</summary>
        public class ConvMatrix
        {
            // postavke operatora
            public int TopLeft = 0, TopMid = 0, TopRight = 0;
            public int MidLeft = 0, Pixel = 1, MidRight = 0;
            public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
            public int Factor = 1;
            public int Offset = 0;
            public void SetAll(int nVal)
            {
                TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
            }
        }

        public static Bitmap Conv3x3(Bitmap b, ConvMatrix m)
        {

            Bitmap bSrc = (Bitmap)b.Clone();

            // BGR
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            int stride2 = stride * 2;
            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;

                int nOffset = stride + 6 - b.Width * 3;
                int nWidth = b.Width - 2;
                int nHeight = b.Height - 2;

                int nPixel;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {

                        nPixel = (int)((((pSrc[2] * m.TopLeft) + (pSrc[5] * m.TopMid) + (pSrc[8] * m.TopRight) +
                            (pSrc[2 + stride] * m.MidLeft) + (pSrc[5 + stride] * m.Pixel) + (pSrc[8 + stride] * m.MidRight) +
                            (pSrc[2 + stride2] * m.BottomLeft) + (pSrc[5 + stride2] * m.BottomMid) + (pSrc[8 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[5 + stride] = (byte)nPixel;

                        nPixel = (int)((((pSrc[1] * m.TopLeft) + (pSrc[4] * m.TopMid) + (pSrc[7] * m.TopRight) +
                            (pSrc[1 + stride] * m.MidLeft) + (pSrc[4 + stride] * m.Pixel) + (pSrc[7 + stride] * m.MidRight) +
                            (pSrc[1 + stride2] * m.BottomLeft) + (pSrc[4 + stride2] * m.BottomMid) + (pSrc[7 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[4 + stride] = (byte)nPixel;

                        nPixel = (int)((((pSrc[0] * m.TopLeft) + (pSrc[3] * m.TopMid) + (pSrc[6] * m.TopRight) +
                            (pSrc[0 + stride] * m.MidLeft) + (pSrc[3 + stride] * m.Pixel) + (pSrc[6 + stride] * m.MidRight) +
                            (pSrc[0 + stride2] * m.BottomLeft) + (pSrc[3 + stride2] * m.BottomMid) + (pSrc[6 + stride2] * m.BottomRight)) / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;

                        p[3 + stride] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }

                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);

            return b;
        }

        public Bitmap MainFilter(Bitmap b, int nWeight)
        {
            ConvMatrix m = new ConvMatrix();
            frmOptions opcije = new frmOptions();
   
            m.TopLeft = frmOptions.values.topL; 
            m.TopMid = frmOptions.values.topM; 
            m.TopRight = frmOptions.values.topR;

            m.MidLeft = frmOptions.values.midL; 
            m.Pixel = frmOptions.values.midM; 
            m.MidRight = frmOptions.values.midR;

            m.BottomLeft = frmOptions.values.botL;
            m.BottomMid = frmOptions.values.botM;
            m.BottomRight = frmOptions.values.botR;


            m.Factor = 1;
            m.Offset = 0;

            return Conv3x3(b, m);
        }

        #endregion

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save screen shoot as...";
            sfd.Filter = "JPEG Image|*.jpg|Bitmap|*.bmp|PNG Image|*.png";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                FileStream fs = (FileStream)sfd.OpenFile();
                switch (sfd.FilterIndex)
                {
                    case 1:
                        globals.image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                    case 2:
                        globals.image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
                    break;
                    case 3:
                        globals.image.Save(fs, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                }
                fs.Close();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ColorGet.NET v1.1 ©2008\nMade by: Ivan Švogor\nisvogor@gmail.com", "About...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void loadToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Load image from file...";
            ofd.Filter = "JPEG image|*.jpg|Bitmap|*.bmp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                globals.image = new Bitmap(ofd.FileName);
                pictureBox1.Image = globals.image;
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            globals.image = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);
            pictureBox1.Image = globals.image;
        }

        private void takeScreenShootToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Graphics g = this.CreateGraphics();
            Size s = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Size;

            globals.image = new Bitmap(s.Width, s.Height, g);
            Graphics screen = Graphics.FromImage(globals.image);

            this.Hide();
            System.Threading.Thread.Sleep(100); // so the screen shoot is complete
            screen.CopyFromScreen(0, 0, 0, 0, s);
              
            this.Show();

            screen.DrawImage(globals.image, 0, 0);
            pictureBox1.Image = globals.image;
            g.Dispose();
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                saveToolStripMenuItem.Enabled = false;
            }
            else saveToolStripMenuItem.Enabled = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Left = 0;
            pictureBox1.Top = 0;
        }

        private void applyOperatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                slika.slikaOriginal = (Bitmap)pictureBox1.Image;
                slika.slikaWork = MainFilter(slika.slikaOriginal, 1);
                pictureBox1.Image = slika.slikaWork;
            }
            catch (Exception)
            {
                MessageBox.Show("The program has encountered error, please try again!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void setOperatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOptions formOptions = new frmOptions();
            formOptions.ShowDialog();
        }

    }
}