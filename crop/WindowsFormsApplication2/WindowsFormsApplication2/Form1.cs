using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
namespace WindowsFormsApplication2
{
     public partial class Form1 : Form
     {
          public Form1()
          {
               InitializeComponent();
          }
          private Bitmap orig;
          private int x1, x2, y1, y2;
          private Rectangle rect;
          private void open_Click(object sender, EventArgs e)
          {
               OpenFileDialog file = new OpenFileDialog();
               if (file.ShowDialog() == DialogResult.OK)
               {
                    orig = new Bitmap(file.FileName);
                    original.Image = orig;
                    original.SizeMode = PictureBoxSizeMode.Normal;

                    
               }
          }

          private void original_Paint(object sender, PaintEventArgs e)
          {
               Pen p = new Pen(Color.Blue);
               p.DashStyle = DashStyle.DashDot;
               e.Graphics.DrawRectangle(p, rect);
               original.Refresh();
          }

          private void original_MouseMove(object sender, MouseEventArgs e)
          {
              
          }

          private void original_MouseDown(object sender, MouseEventArgs e)
          {
               x1 = e.X;
               y1 = e.Y;
          }

          private void original_MouseUp(object sender, MouseEventArgs e)
          {
               x2 = e.X;
               y2 = e.Y;
               rect=new Rectangle(Math.Min(x1, x2),Math.Min(y1, y2),Math.Abs(x1 - x2),Math.Abs(y1 - y2));

          }

          private void original_Click(object sender, EventArgs e)
          {
              
          }

          private void trackBar1_Scroll(object sender, EventArgs e)
          {

          }

          private void Form1_KeyDown(object sender, KeyEventArgs e)
          {

          }

          private void Form1_KeyUp(object sender, KeyEventArgs e)
          {

          }

          private void crop_btn_Click(object sender, EventArgs e)
          {
               cropped.Refresh();
              Graphics graphics= cropped.CreateGraphics();
              graphics.DrawImage(orig, new Rectangle(new Point(0,0),cropped.Size), rect, GraphicsUnit.Pixel);
              cropped.SizeMode=PictureBoxSizeMode.Normal;
              
              
          }

       

     }
}
