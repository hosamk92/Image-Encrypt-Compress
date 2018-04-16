using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageQuantization
{
    public partial class MainForm : Form
    {
        public bool isEncrypted = false;
        public byte tap = 1;
        public String Initial_seed = "";
        public static string filepathtemp = "";
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            if (ImageMatrix == null)
            {
                MessageBox.Show("Please open an Image");
                return;
            }

            isEncrypted = (!isEncrypted);

            bool isAlpha = AlphaNumaric.Checked;
            String intial_seed = txtGaussSigma.Text.ToString();
            if (isAlpha == false)
            {
                for (int i = 0; i < intial_seed.Length; i++)
                {
                    if (intial_seed[i] != '0' && intial_seed[i] != '1')
                    {
                        MessageBox.Show("Inital seed is alphanumaric .To enable alphanumaric please check the alphanumaric checkbox ");
                        return;
                    }
                }
            }
            int tap_postion = int.Parse(Tap_Postion.Text.ToString());
            tap = (byte)tap_postion;
            Initial_seed = intial_seed;
            long timeBefore = System.Environment.TickCount;
            double average = 0;
            ImageMatrix = ImageOperations.enc_dec(ImageMatrix, tap_postion, intial_seed, isAlpha, ref average);
            long timeAfter = System.Environment.TickCount;
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);

            textBox1.Text = (timeAfter - timeBefore).ToString();
            textBox2.Text = intial_seed.Length.ToString();

        }

        private void nudMaskSize_ValueChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            long timeBefore = System.Environment.TickCount;
            ImageMatrix = ImageOperations.decompression("", ref isEncrypted, ref tap, ref Initial_seed);
            //    MessageBox.Show(isEncrypted + " " + tap + " " + Initial_seed);
            long timeAfter = System.Environment.TickCount;
            textBox1.Text = (timeAfter - timeBefore).ToString();
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);


        }

        private void button2_Click(object sender, EventArgs e) // Compress Button 
        {
            if (ImageMatrix == null)
            {
                MessageBox.Show("Please open an Image");
                return;
            }
            long timeBefore = System.Environment.TickCount;
            double ratio = ImageOperations.compression(ImageMatrix, isEncrypted, tap, Initial_seed);
            long timeAfter = System.Environment.TickCount;
            textBox8.Text = (timeAfter - timeBefore).ToString();
            textBox3.Text = ratio.ToString();
            label12.Show();
            label16.Show();

        }

        private void button3_Click(object sender, EventArgs e)  //Decompress Button
        {
            long timeBefore = System.Environment.TickCount;
            ImageMatrix = ImageOperations.decompression("", ref isEncrypted, ref tap, ref Initial_seed);
            //  MessageBox.Show(isEncrypted + " " + tap + " " + Initial_seed);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox5);
            long timeAfter = System.Environment.TickCount;
            textBox8.Text = (timeAfter - timeBefore).ToString();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtGaussSigma_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Tap_Postion_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtWidth_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void AlphaNumaric_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel4.Show();
            panel8.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox4);
            }
            textBox5.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            textBox4.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e) //Enc/Dec tab
        {
            panel8.Show();
            panel4.Show();
            panel12.Hide();
            panel14.Hide();
            panel13.Hide();
            panel15.Hide();
            panel16.Hide();
        }

        private void button7_Click(object sender, EventArgs e) //Compression tab 
        {
            panel8.Hide();
            panel4.Hide();
            panel13.Hide();
            panel14.Hide();
            panel15.Hide();
            panel16.Hide();
            panel12.Show();

        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e) //Hack tab
        {
            panel8.Hide();
            panel4.Hide();
            panel12.Hide();
            panel15.Hide();
            panel16.Hide();
            panel13.Show();
            panel14.Show();
        }

        private void button5_Click(object sender, EventArgs e)//Filter Tab
        {
            panel8.Hide();
            panel4.Hide();
            panel12.Hide();
            panel13.Hide();
            panel14.Hide();
            panel15.Show();
            panel16.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                filepathtemp = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox6);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            pictureBox7.Hide();
            pictureBox8.Hide();
            trackBar1.Hide();
            ImageOperations.MedianFilter(ImageMatrix, 3, pictureBox6);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            pictureBox7.Show();
            pictureBox8.Show();
            trackBar1.Show();
        }

        int temp = 0;
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            int value = trackBar1.Value;
            textBox6.Text = value.ToString();

            var copy = ImageOperations.OpenImage(filepathtemp);
            if (value > 0)
            {
                
                ImageOperations.bright_tafte7(copy, value, pictureBox6);
                temp = value;
            }
            else if (value < 0)
            {
                
                ImageOperations.bright_ta8me2(copy, -value, pictureBox6);
                temp = value;
            }
             

        }

        private void button13_Click(object sender, EventArgs e)
        {
            ImageOperations.reverse(ImageMatrix, pictureBox6);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ImageOperations.greyscale(ImageMatrix, pictureBox6);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                long timeBefore = System.Environment.TickCount;
                ImageOperations.mirror(ImageMatrix, pictureBox6);
                long timeAfter = System.Environment.TickCount;
                textBox1.Text = (timeAfter - timeBefore).ToString();
            }
            catch
            {
                MessageBox.Show("orror occured");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                long timeBefore = System.Environment.TickCount;

                ImageOperations.hack(ImageMatrix, pictureBox10, textBox7, textBox10, bits);
                this.Show();
                long timeAfter = System.Environment.TickCount;
                textBox1.Text = ((timeAfter - timeBefore) / 1000) + " seconds".ToString();

            }
            catch
            {
                MessageBox.Show("error occured");
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox9.SizeMode = PictureBoxSizeMode.Normal;
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //Open the browsed image and display it
                    string OpenedFilePath = openFileDialog1.FileName;
                    ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                    ImageOperations.factor = 1;
                    ImageOperations.DisplayImage(ImageMatrix, pictureBox9);
                }
                txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
                txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
            }
            catch
            {

            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button17_Click(object sender, EventArgs e) //DECRYPT AT COMPRESS
        {
            if (ImageMatrix == null)
            {
                MessageBox.Show("Please open an image");
                return;
            }
            if (!isEncrypted)
            {
                MessageBox.Show("the image is already decrypted");
                return;
            }
            isEncrypted = (!isEncrypted);
            double average=0;
            ImageMatrix=ImageOperations.enc_dec(ImageMatrix,tap,Initial_seed,false,ref average);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox5);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            pictureBox9.SizeMode = PictureBoxSizeMode.StretchImage;
            ImageOperations.DisplayImage(ImageMatrix, pictureBox9);

        }

        private void panel12_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {
            ImageOperations.save(ImageMatrix);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            ImageOperations.save(ImageMatrix);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            ImageOperations.save(ImageMatrix);
        }
    }
}