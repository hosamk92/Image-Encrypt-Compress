namespace WindowsFormsApplication2
{
     partial class Form1
     {
          /// <summary>
          /// Required designer variable.
          /// </summary>
          private System.ComponentModel.IContainer components = null;

          /// <summary>
          /// Clean up any resources being used.
          /// </summary>
          /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
          protected override void Dispose(bool disposing)
          {
               if (disposing && (components != null))
               {
                    components.Dispose();
               }
               base.Dispose(disposing);
          }

          #region Windows Form Designer generated code

          /// <summary>
          /// Required method for Designer support - do not modify
          /// the contents of this method with the code editor.
          /// </summary>
          private void InitializeComponent()
          {
               this.original = new System.Windows.Forms.PictureBox();
               this.cropped = new System.Windows.Forms.PictureBox();
               this.open = new System.Windows.Forms.Button();
               this.crop_btn = new System.Windows.Forms.Button();
               ((System.ComponentModel.ISupportInitialize)(this.original)).BeginInit();
               ((System.ComponentModel.ISupportInitialize)(this.cropped)).BeginInit();
               this.SuspendLayout();
               // 
               // original
               // 
               this.original.Location = new System.Drawing.Point(45, 27);
               this.original.Name = "original";
               this.original.Size = new System.Drawing.Size(389, 302);
               this.original.TabIndex = 0;
               this.original.TabStop = false;
               this.original.Click += new System.EventHandler(this.original_Click);
               this.original.Paint += new System.Windows.Forms.PaintEventHandler(this.original_Paint);
               this.original.MouseDown += new System.Windows.Forms.MouseEventHandler(this.original_MouseDown);
               this.original.MouseMove += new System.Windows.Forms.MouseEventHandler(this.original_MouseMove);
               this.original.MouseUp += new System.Windows.Forms.MouseEventHandler(this.original_MouseUp);
               // 
               // cropped
               // 
               this.cropped.Location = new System.Drawing.Point(480, 27);
               this.cropped.Name = "cropped";
               this.cropped.Size = new System.Drawing.Size(400, 302);
               this.cropped.TabIndex = 1;
               this.cropped.TabStop = false;
               // 
               // open
               // 
               this.open.Location = new System.Drawing.Point(268, 336);
               this.open.Name = "open";
               this.open.Size = new System.Drawing.Size(75, 23);
               this.open.TabIndex = 2;
               this.open.Text = "open_btn";
               this.open.UseVisualStyleBackColor = true;
               this.open.Click += new System.EventHandler(this.open_Click);
               // 
               // crop_btn
               // 
               this.crop_btn.Location = new System.Drawing.Point(696, 336);
               this.crop_btn.Name = "crop_btn";
               this.crop_btn.Size = new System.Drawing.Size(75, 23);
               this.crop_btn.TabIndex = 3;
               this.crop_btn.Text = "crop";
               this.crop_btn.UseVisualStyleBackColor = true;
               this.crop_btn.Click += new System.EventHandler(this.crop_btn_Click);
               // 
               // Form1
               // 
               this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
               this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
               this.ClientSize = new System.Drawing.Size(943, 378);
               this.Controls.Add(this.crop_btn);
               this.Controls.Add(this.open);
               this.Controls.Add(this.cropped);
               this.Controls.Add(this.original);
               this.Name = "Form1";
               this.Text = "Form1";
               this.Activated += new System.EventHandler(this.trackBar1_Scroll);
               this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
               this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
               ((System.ComponentModel.ISupportInitialize)(this.original)).EndInit();
               ((System.ComponentModel.ISupportInitialize)(this.cropped)).EndInit();
               this.ResumeLayout(false);

          }

          #endregion

          private System.Windows.Forms.PictureBox original;
          private System.Windows.Forms.PictureBox cropped;
          private System.Windows.Forms.Button open;
          private System.Windows.Forms.Button crop_btn;
     }
}

