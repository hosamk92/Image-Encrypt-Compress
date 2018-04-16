using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }
    
  
    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        public static double factor = 1;
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }
        
        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }

        public static void MedianFilter(RGBPixel[,] ImageMatrix, int matrixSize, PictureBox PicBox, int bias = 0)
        {
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }



            BitmapData sourceData = ImageBMP.LockBits(new Rectangle(0, 0, ImageBMP.Width, ImageBMP.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);


            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];


            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];


            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                                       pixelBuffer.Length);


            ImageBMP.UnlockBits(sourceData);


            

            int filterOffset = (matrixSize - 1) / 2;
            int calcOffset = 0;


            int byteOffset = 0;

            List<int> neighbourPixels = new List<int>();
            byte[] middlePixel;


            for (int offsetY = filterOffset; offsetY <
                ImageBMP.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    ImageBMP.Width - filterOffset; offsetX++)
                {
                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;


                    neighbourPixels.Clear();


                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {


                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                (filterY * sourceData.Stride);


                            neighbourPixels.Add(BitConverter.ToInt32(
                                             pixelBuffer, calcOffset));
                        }
                    }


                    neighbourPixels.Sort();

                    middlePixel = BitConverter.GetBytes(
                                       neighbourPixels[filterOffset]);


                    resultBuffer[byteOffset] = middlePixel[0];
                    resultBuffer[byteOffset + 1] = middlePixel[1];
                    resultBuffer[byteOffset + 2] = middlePixel[2];
                    resultBuffer[byteOffset + 3] = middlePixel[3];
                }
            }


            Bitmap resultBitmap = new Bitmap(ImageBMP.Width,
                                             ImageBMP.Height);


            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);


            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);


            resultBitmap.UnlockBits(resultData);

            PicBox.Image = resultBitmap;

        }

        public static void bright_ta8me2(RGBPixel[,] ImageMatrix, int value, PictureBox picbox)
        {
            byte val = Convert.ToByte(value);
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if (ImageMatrix[i, j].red - val > 0)
                            ImageMatrix[i, j].red -= val;
                        if (ImageMatrix[i, j].green - val > 0)

                            ImageMatrix[i, j].green -= val;

                        if (ImageMatrix[i, j].blue - val > 0)
                            ImageMatrix[i, j].blue -= val;

                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            picbox.Image = ImageBMP;

        }
        public static void bright_tafte7(RGBPixel[,] ImageMatrix, int value, PictureBox picbox)
        {

            byte val = Convert.ToByte(value);
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        if (ImageMatrix[i, j].red + val < 255)
                            ImageMatrix[i, j].red += val;
                        if (ImageMatrix[i, j].green + val < 255)

                            ImageMatrix[i, j].green += val;
                        if (ImageMatrix[i, j].blue + val < 255)

                            ImageMatrix[i, j].blue += val;

                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            picbox.Image = ImageBMP;
        }

        public static void mirror(RGBPixel[,] ImageMatrix, PictureBox pic)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);
            RGBPixel tmp;
            for (int i = 0; i < Height; i++)
            {

                for (int j = 0; j < Width / 2; j++)
                {
                    //swap
                    tmp = ImageMatrix[i, j];
                    ImageMatrix[i, j] = ImageMatrix[i, Width - j - 1];
                    ImageMatrix[i, Width - j - 1] = tmp;

                }

            }

            DisplayImage(ImageMatrix, pic);
        }
        /*

             O(H*W*(N*(2^N)) ) 
             N=initial seed length
             H=height
             W=width
            
            */
        public static void hack(RGBPixel[,] ImageMatrix, PictureBox pic, TextBox tap, TextBox seed, TextBox bits)
        {

            int num_bits = int.Parse(bits.Text);
            int n = 1;
            int max = 0;
            string initial_seed = "";
            string CorrectSeed = "";
            string CorrectTap = "";
            bool isAlpha = false;
            double average = 0;

            //we save the image in a copy
            RGBPixel[,] ImageCopy = ImageMatrix;
            RGBPixel[,] CorrectImage = ImageMatrix;
            for (int tap_position = 0; tap_position < num_bits - 1; tap_position++)
            {
                for (int i = 1; i < Math.Pow(2, num_bits); i++)
                {
                    ImageMatrix = ImageCopy;
                    //we convert the i into binary and put it ininitial seed
                    initial_seed = Convert.ToString(i, 2).PadLeft(num_bits, '0');
                    // decrypting
                    ImageMatrix = ImageOperations.enc_dec(ImageMatrix, tap_position, initial_seed, isAlpha, ref average);

                    n = (int)average;
                    n = Math.Abs(128 - n);

                    if (n > max)
                    {
                        max = n;
                        CorrectImage = ImageMatrix;
                        CorrectSeed = initial_seed;
                        CorrectTap = tap_position.ToString();
                    }
                }
            }

            tap.Text = CorrectTap;
            seed.Text = CorrectSeed;
            ImageOperations.DisplayImage(CorrectImage, pic);
        }

        /// <summary>
        /// Inverts the colors of an RGBPixel Image
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <complexity>
        /// Heith * Width
        /// </complexity>
        /// <returns>Filtered</returns>
        public static void reverse(RGBPixel[,] ImageMatrix , PictureBox picbox)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);
            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            RGBPixel[,] Filtered = new RGBPixel[Height, Width];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    Filtered[i, j].red = ((byte)(~(int)ImageMatrix[i, j].red));
                    Filtered[i, j].green = ((byte)(~(int)ImageMatrix[i, j].green));
                    Filtered[i, j].blue = ((byte)(~(int)ImageMatrix[i, j].blue));
                }
            }

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        Filtered[i, j].red = ((byte)(~(int)ImageMatrix[i, j].red));
                        Filtered[i, j].green = ((byte)(~(int)ImageMatrix[i, j].green));
                        Filtered[i, j].blue = ((byte)(~(int)ImageMatrix[i, j].blue));

                        p[2] = Filtered[i, j].red;
                        p[1] = Filtered[i, j].green;
                        p[0] = Filtered[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }



            picbox.Image = ImageBMP;
         
        }


        /// <summary>
        /// Takes an RGBPixel Image and converts it to a bitmap greyscale image
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <complexity>
        /// Height * Width
        /// </complexity> 
        /// <returns>greyscale</returns>
        public static void greyscale(RGBPixel[,] ImageMatrix , PictureBox picbox)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            var greyscale = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    int red = ImageMatrix[j, i].red;
                    int green = ImageMatrix[j, i].green;
                    int blue = ImageMatrix[j, i].blue;
                    greyscale.SetPixel(i, j, Color.FromArgb(0, red, green, blue));
                    Color oc = greyscale.GetPixel(i, j);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    greyscale.SetPixel(i, j, nc);
                }
            }

            

            picbox.Image = greyscale;


           
        }

        /// <summary>
        /// shift string left
        /// </summary>
        /// <param name="initial_seed">initial seed</param>
        /// <param name="Tap_Postion">Tap Postion</param>
        /// <param name="Number_Of_Bits">Number Of Bits to be generated</param>
        /// <returns>generated password after shifting left</returns>
        /// <complexity>
        /// initial seed size = M;
        /// complexity O(Number of bits to be generated * M = 8 * M) = O(8 * M) = O(M)
        /// </complexity>
        public static long LFSR(ref String initial_seed, int Tap_Postion, int Number_Of_Bits)
        {
            int i, k;
            long password = 0, Val = (1 << (Number_Of_Bits - 1));
            char ans;
            char[] a = initial_seed.ToCharArray();

            for (k = 0; k < Number_Of_Bits; k++)
            {
                ans = '0';
                if (a[0] != a[a.Length - Tap_Postion - 1])
                    ans = '1';

                //shifting the char array to the left
                //===================================
                for (i = 0; i < a.Length - 1; i++)
                {
                    a[i] = a[i + 1];
                }
                //put in the last element the xor value
                //=====================================
                a[a.Length - 1] = ans;
                if (ans == '1')
                    password += (Val);
                Val /= 2;
            }
            //make the initial seed equal to the shifted string
            //==================================================
            initial_seed = new string(a);
            return password;
        }
        /// <summary>
        /// shift the seed left
        /// doesn't work with big initial seeds
        /// </summary>
        /// <param name="size">initial seed size</param>
        /// <param name="seed">seed value in decimal</param>
        /// <param name="Tap_Postion">Tap Postion</param>
        /// <param name="Number_Of_Bits">Number Of Bits to be generated</param>
        /// <returns>generated password after shifting left</returns>
        /// <complexity>
        /// O(Number of bits to be generated = 8) = O(8) = O(1)
        /// </complexity>
        public static long LFSR_Long(int size,ref long  seed, int Tap_Postion, int Number_Of_Bits)
        {
            int k;
            long password = 0, limit = (1 << size) - 1, Val = (1 << (Number_Of_Bits - 1));
            long Tap_Postion_Val, Last_Bit_Val;
            for (k = 0; k < Number_Of_Bits; k++)
            {
                Tap_Postion_Val = (1 << Tap_Postion);
                Last_Bit_Val = (1 << (size - 1));
                Tap_Postion_Val = (Tap_Postion_Val & seed);
                Last_Bit_Val = (Last_Bit_Val & seed);
                seed <<= 1;
                if ((Tap_Postion_Val > 0 && Last_Bit_Val == 0) || (Last_Bit_Val > 0 && Tap_Postion_Val == 0))
                {
                    seed = (seed | 1);
                    password += Val;
                }
                Val >>= 1;
                seed = (seed & limit);
            }
            return password;
        }


        /// <summary>
        /// Encrypt an image or Decrypt an Encrypted image
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="Tap_Postion">Tap Postion</param>
        /// <param name="initial_seed">initial seed</param>
        /// <returns>Encrypted / Decrypted color image</returns>
        /// <complexity>
        /// initial seed size = M , H = height of image , W = width of image
        /// in case of M <= 63
        /// O(H * W)
        /// in case of M > 63
        /// O(H * W * M)
        /// </complexity>
        public static RGBPixel[,] enc_dec(RGBPixel[,] ImageMatrix, int Tap_Postion, String initial_seed,bool isAlpha,ref double average)
        {

            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixel[,] Filtered = new RGBPixel[Height, Width];

            //if aplhaNumaric is checked change initial seed to binary base 64
            //================================================================
            if (isAlpha)
                alpha(ref initial_seed);
            long password, seed = 0;
            int size = initial_seed.Length;
            bool bigSize = (initial_seed.Length <= 63);
            
            //Initlize seed based on initial seed size
            //========================================
            if (bigSize) seed = Convert.ToInt64(initial_seed, 2);
            

            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    //generate new password and xor it with the color
                    //===============================================

                    //generate function check the size of the initial seed and with it choose the best way to shift left
                    //===================================================================================================
                    if (bigSize)
                        password = LFSR_Long(size, ref seed, Tap_Postion, 8);
                    else
                        password = LFSR(ref initial_seed, Tap_Postion, 8);

                    Filtered[i, j].red = (byte)(ImageMatrix[i, j].red ^ password);

                    if (bigSize)
                        password = LFSR_Long(size, ref seed, Tap_Postion, 8);
                    else
                        password = LFSR(ref initial_seed, Tap_Postion, 8);
                   
                    Filtered[i, j].green = (byte)(ImageMatrix[i, j].green ^ password);

                    if (bigSize)
                        password = LFSR_Long(size, ref seed, Tap_Postion, 8);
                    else
                        password = LFSR(ref initial_seed, Tap_Postion, 8);
                    
                    Filtered[i, j].blue = (byte)(ImageMatrix[i, j].blue ^ password);

                    average += (Filtered[i, j].red + Filtered[i, j].green + Filtered[i, j].blue);

                    
                }
            average = (double)average / (Height * Width * 3);

            return Filtered;
        }
        /// <summary>
        /// save image in file
        /// </summary>
        /// <param name="Filtered"></param>
        public static void save(RGBPixel[,] Filtered)
        {
            int Height = GetHeight(Filtered);
            int Width = GetWidth(Filtered);
            //initalize a new bitmap varible with same width and height as the image and format 24bit which we use 8bit for every color
            //=========================================================================================================================
            var bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    //Save new image (enc/dec) in bitmap file
                    //========================================
                    int red = Filtered[i, j].red;
                    int green = Filtered[i, j].green;
                    int blue = Filtered[i, j].blue;
                    bitmap.SetPixel(j, i, Color.FromArgb(0, red, green, blue));
                }

            bitmap.Save("ENC.png", ImageFormat.Png);
        }
        /// <summary>
        /// change initial seed from base64 to binery each character 6 bits
        /// <complexty>
        /// N=initial seed size
        /// O(N*6) = O(N)
        /// </complexty>
        /// </summary>
        /// <param name="initial_seed"></param>
        public static void alpha (ref string initial_seed)
        {
            int size = initial_seed.Length;
            int index = 6;
            int counter = 0;
            int[] arr = new int[(size * 6) + 1];
            string enc = "";
            int n = 0;

            foreach (char c in initial_seed)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    n = Convert.ToInt32(c - 65);
                }
                else if (c >= 'a' && c <= 'z')
                {
                    n = Convert.ToInt32(c - 71);
                }
                else if (c >= '0' && c <= '9')
                {
                    n = Convert.ToInt32(c + 4);
                }
                else if (c == '+')
                {
                    n = 62;
                }
                else if (c == '/')
                {
                    n = 63;
                }
                counter = index;
                while (n > 0)
                {
                    if (n % 2 == 0)
                    {
                        arr[counter] = 0;
                    }
                    else
                    {
                        arr[counter] = 1;
                    }
                    counter--;
                    n /= 2;
                }
                index += 6;
            }
            for (int i = 1; i <= (size * 6); i++)
            {
                enc += arr[i];
            }
            initial_seed = enc;
        }

        /// <summary>
        /// get the hestogram of the image
        /// N^2=Height*width
        /// O(N^2)
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <returns></returns>
        public static int[,] Hestogram(RGBPixel[,] ImageMatrix)
        {

            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            int[,] Hesto = new int[257, 4];

            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Hesto[(int)ImageMatrix[i, j].red, 0]++;
                    Hesto[(int)ImageMatrix[i, j].green, 1]++;
                    Hesto[(int)ImageMatrix[i, j].blue, 2]++;
                }
            return Hesto;
        }

        /// <summary>
        /// read from file hestogram and height and width from file and constract the huffman tree
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="rr"></param>
        /// <param name="bb"></param>
        /// <param name="gg"></param>
        /// <param name="H"></param>
        /// <param name="W"></param>
        /// <param name="l"></param>
        /// <returns>
        /// return by refrence Dictionary contain code as key and color freq as value
        /// and height and width
        /// </returns>
        public static void Readhuffman(ref byte[] arr,Dictionary<string, int> rr, Dictionary<string, int> bb, Dictionary<string, int> gg, ref int H, ref int W, ref int l,ref bool isEnc,ref byte tap,ref string Initial_seed)
        {
            arr = File.ReadAllBytes("compression.txt");

            int freq = 0,i=0;
            byte[] f = new byte[4];
            int[,] Hesto = new int[256, 3];
          
            Initial_seed = "";
            tap = 0;
            isEnc = false;
            /// check if image is encrypted if so read initial seed and tap postion
            if (arr[0] == 1)
            {
                isEnc = true;
                i++;
                tap = arr[1];
                i += 2;
                for (int j = 0; j < arr[2]; j++, i++)
                {
                    if (arr[i] == 1) Initial_seed += '1';
                    else Initial_seed += '0';
                }
            }
            else
                i++;
            
            // read height
            for (int j = 0; j < 4; j++)
            {
                f[j] = arr[i];
                i++;
            }
            int Height = BitConverter.ToInt32(f, 0);
            H = Height;

            // read width
            for (int j = 0; j < 4; j++)
            {
                f[j] = arr[i];
                i++;
            }
            int Width = BitConverter.ToInt32(f, 0);
            W = Width;

            // read hestogram
            for (int kk = 0; kk < 3; kk++)
            {
                for (int k = 0; k < 256; k++)
                {
                    // integer is stored in 4 bytes so we loop 4 times 
                    for (int j = 0; j < 4; j++)
                    {
                        f[j] = arr[i];
                        i++;
                    }
                    freq = BitConverter.ToInt32(f, 0);
                    Hesto[k, kk] = freq;
                }
            }

            l = i;

            // constract huffman tree from the hestogram
            int r = 0, g = 0, b = 0;

            for (i = 0; i < 256; i++)
            {
                if (Hesto[i, 0] != 0) r++;
                if (Hesto[i, 1] != 0) g++;
                if (Hesto[i, 2] != 0) b++;
            }
            int[,] red = new int[r, 2];
            int[,] green = new int[g, 2];
            int[,] blue = new int[b, 2];

            r = g = b = 0;
            for (i = 0; i < 256; i++)
            {
                if (Hesto[i, 0] != 0)
                    red[r++, 1] = i;
                if (Hesto[i, 1] != 0)
                    green[g++, 1] = i;
                if (Hesto[i, 2] != 0)
                    blue[b++, 1] = i;

                if (Hesto[i, 0] != 0)
                    red[r - 1, 0] = Hesto[i, 0];
                if (Hesto[i, 1] != 0)
                    green[g - 1, 0] = Hesto[i, 1];
                if (Hesto[i, 2] != 0)
                    blue[b - 1, 0] = Hesto[i, 2];
            }

            huffman[] RGB = new huffman[4];
            RGB[0] = new huffman();
            RGB[1] = new huffman();
            RGB[2] = new huffman();
            RGB[0].sortnodes(red);
            RGB[1].sortnodes(green);
            RGB[2].sortnodes(blue);
            RGB[0].createtree();
            RGB[1].createtree();
            RGB[2].createtree();
            Dictionary<string, int> dd = new Dictionary<string, int>();
            Dictionary<int, int> ddd = new Dictionary<int, int>();
            Dictionary<int, int> ddd1 = new Dictionary<int, int>();
            Dictionary<int, int> ddd2 = new Dictionary<int, int>();
            RGB[0].printcode(RGB[0].root, RGB[0].code, RGB[0].d, rr, ddd);
            RGB[1].printcode(RGB[1].root, RGB[1].code, RGB[1].d, gg, ddd1);
            RGB[2].printcode(RGB[2].root, RGB[2].code, RGB[2].d, bb, ddd2);
        }

        /// <summary>
        /// construct the huffman tree and write it in file
        /// </summary>
        /// <Complexty>
        /// N=min(256,number of colors of non-zero freq)
        /// O(N Log N)
        /// </Complexty>
        /// <param name="ImageMatrix">image</param>
        /// <returns></returns>
        public static huffman[] huffman(RGBPixel[,] ImageMatrix, bool isEnc, byte tap, string Initial_seed)
        {

            int[,] Hesto = Hestogram(ImageMatrix);
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            int r = 0, g = 0, b = 0;
           
            for (int i = 0; i < 256; i++)
            {
                if (Hesto[i, 0] != 0) r++;
                if (Hesto[i, 1] != 0) g++;
                if (Hesto[i, 2] != 0) b++;
            }
            int[,] red = new int[r, 2];
            int[,] green = new int[g, 2];
            int[,] blue = new int[b, 2];
            r = g = b = 0;
            for (int i = 0; i < 256; i++)
            {
                if (Hesto[i, 0] != 0)
                    red[r++, 1] = i;
                if (Hesto[i, 1] != 0)
                    green[g++, 1] = i;
                if (Hesto[i, 2] != 0)
                    blue[b++, 1] = i;
                

                if (Hesto[i, 0] != 0)
                    red[r-1, 0] = Hesto[i, 0];
                if (Hesto[i, 1] != 0)
                    green[g-1, 0] = Hesto[i, 1];
                if (Hesto[i, 2] != 0)
                    blue[b-1, 0] = Hesto[i, 2];
            }

            huffman[] RGB = new huffman[4];
            RGB[0] = new huffman();
            RGB[1] = new huffman();
            RGB[2] = new huffman();
            RGB[0].sortnodes(red);
            RGB[1].sortnodes(green);
            RGB[2].sortnodes(blue);
            RGB[0].createtree();
            RGB[1].createtree();
            RGB[2].createtree();
            Dictionary<string, int> dd=new Dictionary<string, int>();
            Dictionary<int, int> ddd = new Dictionary<int, int>();
            Dictionary<int, int> ddd1 = new Dictionary<int, int>();
            Dictionary<int, int> ddd2 = new Dictionary<int, int>();
            RGB[0].printcode(RGB[0].root, RGB[0].code, RGB[0].d, dd, ddd);

            /// write hestiogram to use it and reconstract the tree in the decompression
            using (BinaryWriter file = new BinaryWriter(File.Open("compression.txt", FileMode.Create)))
            {
                file.Write(isEnc);
                if (isEnc)
                {
                    file.Write(tap);
                    file.Write((byte)Initial_seed.Length);
                    for (int i = 0; i < Initial_seed.Length; i++)
                    {
                        if (Initial_seed[i] == '0') file.Write((byte)0);
                        else file.Write((byte)1);
                    }
                }
                file.Write(BitConverter.GetBytes(Height));
                file.Write(BitConverter.GetBytes(Width));
                for (int j = 0; j < 256; j++)
                {
                    file.Write(BitConverter.GetBytes(Hesto[j, 0]));
                }
                dd = new Dictionary<string, int>();
                RGB[1].printcode(RGB[1].root, RGB[1].code, RGB[1].d, dd, ddd1);
                for (int j = 0; j < 256; j++)
                {
                    file.Write(BitConverter.GetBytes(Hesto[j, 1]));
                }
                dd = new Dictionary<string, int>();
                RGB[2].printcode(RGB[2].root, RGB[2].code, RGB[2].d, dd, ddd2);
                for (int j = 0; j < 256; j++)
                {
                    file.Write(BitConverter.GetBytes(Hesto[j,2]));
                }
            }


            /// write huffman file
            using (StreamWriter file1 = new StreamWriter("sample.txt"))
            {
                string titles = "Color - Frequency - Huffman Representation - Total Bits";
                file1.WriteLine("--R--");
                file1.WriteLine(titles);
                double total = 0;
                double total1 = 0;
                double freq;
                foreach (var entry in RGB[0].d)
                {
                    freq = ddd[entry.Key];
                    total += freq * entry.Value.Length;
                    file1.WriteLine("{0} - {1} - {2} - {3}", entry.Key, freq, entry.Value, freq * entry.Value.Length);
                    
                }
                file1.WriteLine("*Total bits = {0} ", total);
                total1 += total;
                total = 0;
                file1.WriteLine();
                file1.WriteLine("--G--");
                foreach (var entry in RGB[1].d)
                {
                    freq = ddd1[entry.Key];
                    total += freq * entry.Value.Length;
                    file1.WriteLine("{0} - {1} - {2} - {3}", entry.Key, freq, entry.Value, freq * entry.Value.Length);
                }
                file1.WriteLine("*Total bits = {0} ", total);
                total1 += total;
                total = 0;
                file1.WriteLine();
                file1.WriteLine("--B--");
                foreach (var entry in RGB[2].d)
                {
                    freq = ddd2[entry.Key];
                    total += freq * entry.Value.Length;
                    file1.WriteLine("{0} - {1} - {2} - {3}", entry.Key, freq, entry.Value, freq * entry.Value.Length);
                }
                file1.WriteLine("*Total bits = {0} ", total);
                total1 += total;
                file1.WriteLine();
                file1.WriteLine("**Compression Output");
                file1.WriteLine(total1/8);
            }
            return RGB;
        }
        /// <summary>
        /// compress the image 
        /// </summary>
        /// <Complexty>
        /// N^2=height * width
        /// O(N^2)
        /// </Complexty>
        /// <param name="ImageMatrix">the image</param>
        /// <returns></returns>
        public static double compression(RGBPixel[,] ImageMatrix, bool isEnc, byte tap, string Initial_seed)
        {
            List<byte> byteList = new List<byte>();

            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);
            huffman[] huff = huffman(ImageMatrix, isEnc, tap, Initial_seed);
            string s1, s, s2;
            char[] arr = new char[8];
            int co = 0, sum = 0;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    s = huff[0].d[ImageMatrix[i, j].red];
                    for (int k = 0; k < s.Length; k++)
                    {
                        sum++;
                        arr[co++] = s[k];

                        /// if size of the string we read became greater than 8 we change it to 1 byte
                        if (co == 8)
                        {
                            byte byteString = 0;
                            int val = (1 << 7);
                            for (int l = 0; l < 8; l++)
                            {
                                if (arr[l] == '1')
                                    byteString += (byte)val;
                                val /= 2;
                            }
                            byteList.Add(byteString);
                            co = 0;
                        }
                    }
                    s1 = huff[1].d[ImageMatrix[i, j].green];
                    for (int k = 0; k < s1.Length; k++)
                    {
                        sum++;
                        arr[co++] = s1[k];
                        if (co == 8)
                        {
                            byte byteString = 0;
                            int val = (1 << 7);
                            for (int l = 0; l < 8; l++)
                            {
                                if (arr[l] == '1')
                                    byteString += (byte)val;
                                val /= 2;
                            }
                            byteList.Add(byteString);
                            co = 0;
                        }
                    }
                    s2 = huff[2].d[ImageMatrix[i, j].blue];
                    for (int k = 0; k < s2.Length; k++)
                    {
                        sum++;
                        arr[co++] = s2[k];
                        if (co == 8)
                        {
                            byte byteString = 0;
                            int val = (1 << 7);
                            for (int l = 0; l < 8; l++)
                            {
                                if (arr[l] == '1')
                                    byteString += (byte)val;
                                val /= 2;
                            }
                            byteList.Add(byteString);
                            co = 0;
                        }
                    }

                }
            }
            /// if there still bits change it to byte
            if (co != 0)
            {
                byte byteString = 0;
                int val = (1 << 7);
                for (int l = 0; l < co; l++)
                {
                    if (arr[l] == '1')
                        byteString += (byte)val;
                    val /= 2;
                }
                byteList.Add(byteString);
                co = 0;
            }
            double ratio = ((double)sum / ((double)Height * Width * 24)) * 100;

            /// write the byte array in the file
            BinaryWriter file = new BinaryWriter(File.Open("compression.txt", FileMode.Append));
            for (int i = 0; i < byteList.Count; i++)
                file.Write(byteList[i]);
            file.Close();
            return ratio;
        }

        /// <summary>
        /// decompress the image
        /// </summary>
        /// <Complexty>
        /// N^2=height * width
        /// O(N^2)
        /// </Complexty>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static RGBPixel[,] decompression(string filepath,ref bool isEnc,ref byte tap,ref string Initial_seed)
        { 
            Dictionary<string, int> red = new Dictionary<string, int>();
            Dictionary<string, int> green = new Dictionary<string, int>();
            Dictionary<string, int> blue = new Dictionary<string, int>();

            int Width = 0, Height = 0, i = 0;

            byte[] arr = new byte[0];

            Readhuffman(ref arr, red, blue, green, ref Height, ref Width, ref i, ref isEnc, ref tap, ref Initial_seed);

            RGBPixel[,] Filtered = new RGBPixel[Height, Width];

            string st = "";
           
            int k = i, l = 0;

            for (i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    while (true)
                    {
                        if (red.ContainsKey(st))
                        {
                            Filtered[i, j].red = (byte)red[st];
                            st = "";
                            break;
                        }

                        if (((arr[k] >> (8 - l - 1)) & 1) != 0)
                            st += '1';
                        else
                            st += '0';
                        
                        if (l == 7)
                        {
                            k++;
                            l = 0;
                        }
                        else
                            l++;
                    }
                    while (true)
                    {
                        if (k >= arr.Length)
                            break;
                        if (green.ContainsKey(st))
                        {
                            Filtered[i, j].green = (byte)green[st];
                            st = "";
                            break;
                        }
                        if (((arr[k] >> (8 - l - 1)) & 1) != 0)
                            st += '1';
                        else
                            st += '0';
                        
                        if (l == 7)
                        {
                            k++;
                            l = 0;  
                        }
                        else
                            l++;
                    }
                    while (true)
                    {                        
                        if (k >= arr.Length)
                            break;
                        if (blue.ContainsKey(st))
                        {
                            Filtered[i, j].blue = (byte)blue[st];
                            st = "";
                            break;
                        }

                        if (((arr[k] >> (8 - l - 1)) & 1) != 0)
                            st += '1';
                        else
                            st += '0';
                        
                        if (l == 7)
                        {
                            k++; 
                            l = 0;
                        }
                        else
                            l++;
                    }
                }
            return Filtered;
        }

       /// <summary>
       /// Apply Gaussian smoothing filter to enhance the edge detection 
       /// </summary>
       /// <param name="ImageMatrix">Colored image matrix</param>
       /// <param name="filterSize">Gaussian mask size</param>
       /// <param name="sigma">Gaussian sigma</param>
       /// <returns>smoothed color image</returns>
        public static RGBPixel[,] GaussianFilter1D(RGBPixel[,] ImageMatrix, int filterSize, double sigma)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);

            RGBPixelD[,] VerFiltered = new RGBPixelD[Height, Width];
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];

           
            // Create Filter in Spatial Domain:
            //=================================
            //make the filter ODD size
            if (filterSize % 2 == 0) filterSize++;

            double[] Filter = new double[filterSize];

            //Compute Filter in Spatial Domain :
            //==================================
            double Sum1 = 0;
            int HalfSize = filterSize / 2;
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                //Filter[y+HalfSize] = (1.0 / (Math.Sqrt(2 * 22.0/7.0) * Segma)) * Math.Exp(-(double)(y*y) / (double)(2 * Segma * Segma)) ;
                Filter[y + HalfSize] = Math.Exp(-(double)(y * y) / (double)(2 * sigma * sigma));
                Sum1 += Filter[y + HalfSize];
            }
            for (int y = -HalfSize; y <= HalfSize; y++)
            {
                Filter[y + HalfSize] /= Sum1;
            }

            //Filter Original Image Vertically:
            //=================================
            int ii, jj;
            RGBPixelD Sum;
            RGBPixel Item1;
            RGBPixelD Item2;

            for (int j = 0; j < Width; j++)
                for (int i = 0; i < Height; i++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int y = -HalfSize; y <= HalfSize; y++)
                    {
                        ii = i + y;
                        if (ii >= 0 && ii < Height)
                        {
                            Item1 = ImageMatrix[ii, j];
                            Sum.red += Filter[y + HalfSize] * Item1.red;
                            Sum.green += Filter[y + HalfSize] * Item1.green;
                            Sum.blue += Filter[y + HalfSize] * Item1.blue;
                        }
                    }
                    VerFiltered[i, j] = Sum;
                }

            //Filter Resulting Image Horizontally:
            //===================================
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    Sum.red = 0;
                    Sum.green = 0;
                    Sum.blue = 0;
                    for (int x = -HalfSize; x <= HalfSize; x++)
                    {
                        jj = j + x;
                        if (jj >= 0 && jj < Width)
                        {
                            Item2 = VerFiltered[i, jj];
                            Sum.red += Filter[x + HalfSize] * Item2.red;
                            Sum.green += Filter[x + HalfSize] * Item2.green;
                            Sum.blue += Filter[x + HalfSize] * Item2.blue;
                        }
                    }
                    Filtered[i, j].red = (byte)Sum.red;
                    Filtered[i, j].green = (byte)Sum.green;
                    Filtered[i, j].blue = (byte)Sum.blue;
                }

            return Filtered;
        }

    }
}
