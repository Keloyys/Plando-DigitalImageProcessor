using ImageProcessingFilters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;

namespace PlandoDigitalImagerProcessor
{
    public partial class Form1 : Form
    {
        private Device[] devices;
        private Device currentDevice;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void onImageButtonClicked(object sender, EventArgs e)
        {
            if (currentDevice != null)
            {
                currentDevice.Stop();
                currentDevice = null;
                pictureBox2.Invalidate(); 
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select an Image";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image = new Bitmap(openFileDialog.FileName);
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private void onBackGroundImageClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select an Image";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    pictureBox1.Image = new Bitmap(openFileDialog.FileName);

                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        private unsafe void onCopyClicked(object sender, EventArgs e)
        {
            Bitmap source = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();

                if (Clipboard.ContainsImage())
                {
                    source = new Bitmap(Clipboard.GetImage());
                }
                else
                {
                    MessageBox.Show("Failed to capture snapshot from webcam.");
                    return;
                }
            }
            else if (pictureBox2.Image != null)
            {
                source = new Bitmap(pictureBox2.Image);
            }
            else
            {
                MessageBox.Show("No image is found to be copied. Kindly put an image or turn on the camera. Thank you!!");
                return;
            }

            if (source != null)
            {
                Bitmap output = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                System.Drawing.Imaging.BitmapData srcData = source.LockBits(
                    new Rectangle(0, 0, source.Width, source.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                System.Drawing.Imaging.BitmapData dstData = output.LockBits(
                    new Rectangle(0, 0, output.Width, output.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                int height = source.Height;
                int width = source.Width;
                int strideSrc = srcData.Stride;
                int strideDst = dstData.Stride;

                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    byte* srcRow = srcPtr + (y * strideSrc);
                    byte* dstRow = dstPtr + (y * strideDst);

                    for (int x = 0; x < width * 3; x++) 
                    {
                        dstRow[x] = srcRow[x]; 
                    }
                }

                source.UnlockBits(srcData);
                output.UnlockBits(dstData);

                pictureBox3.Image = output;
                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }



        private unsafe void onGreyScaledButtonClicked(object sender, EventArgs e)
        {
            Bitmap sourceImage = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    sourceImage = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                sourceImage = new Bitmap(pictureBox2.Image);
            }

            if (sourceImage != null)
            {
                Bitmap grayImage = new Bitmap(sourceImage.Width, sourceImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                System.Drawing.Imaging.BitmapData srcData = sourceImage.LockBits(
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                System.Drawing.Imaging.BitmapData dstData = grayImage.LockBits(
                    new Rectangle(0, 0, grayImage.Width, grayImage.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                int width = sourceImage.Width;
                int height = sourceImage.Height;
                int strideSrc = srcData.Stride;
                int strideDst = dstData.Stride;

                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    byte* srcRow = srcPtr + (y * strideSrc);
                    byte* dstRow = dstPtr + (y * strideDst);

                    for (int x = 0; x < width; x++)
                    {
                        byte b = srcRow[x * 3 + 0];
                        byte g = srcRow[x * 3 + 1];
                        byte r = srcRow[x * 3 + 2];

                        byte gray = (byte)((r + g + b) / 3);

                        dstRow[x * 3 + 0] = gray; // B
                        dstRow[x * 3 + 1] = gray; // G
                        dstRow[x * 3 + 2] = gray; // R
                    }
                }

                sourceImage.UnlockBits(srcData);
                grayImage.UnlockBits(dstData);

                pictureBox3.Image = grayImage;
                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                MessageBox.Show("No image is found to be grey scaled. Kindly put an image or turn on the camera. Thank you!!");
            }
        }


        private unsafe void onColorInversionClicked(object sender, EventArgs e)
        {
            Bitmap sourceImage = null;
            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    sourceImage = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                sourceImage = new Bitmap(pictureBox2.Image);
            }

            if (sourceImage != null)
            {
                Bitmap invertedImage = new Bitmap(sourceImage.Width, sourceImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                System.Drawing.Imaging.BitmapData srcData = sourceImage.LockBits(
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                System.Drawing.Imaging.BitmapData dstData = invertedImage.LockBits(
                    new Rectangle(0, 0, invertedImage.Width, invertedImage.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                int width = sourceImage.Width;
                int height = sourceImage.Height;
                int strideSrc = srcData.Stride;
                int strideDst = dstData.Stride;

                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    byte* srcRow = srcPtr + (y * strideSrc);
                    byte* dstRow = dstPtr + (y * strideDst);

                    for (int x = 0; x < width; x++)
                    {
                        // BGR order
                        byte b = srcRow[x * 3 + 0];
                        byte g = srcRow[x * 3 + 1];
                        byte r = srcRow[x * 3 + 2];

                        dstRow[x * 3 + 0] = (byte)(255 - b);
                        dstRow[x * 3 + 1] = (byte)(255 - g);
                        dstRow[x * 3 + 2] = (byte)(255 - r);
                    }
                }

                sourceImage.UnlockBits(srcData);
                invertedImage.UnlockBits(dstData);

                pictureBox3.Image = invertedImage;
                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                MessageBox.Show("No image is found to be inverted. Kindly put an image or turn on the camera. Thank you!!");
            }
        }

        private unsafe void onHistogramClicked(object sender, EventArgs e)
        {
            Bitmap sourceImage = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    sourceImage = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                sourceImage = new Bitmap(pictureBox2.Image);
            }

            if (sourceImage != null)
            {
                int[] histogram = new int[256];

                System.Drawing.Imaging.BitmapData srcData = sourceImage.LockBits(
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                int stride = srcData.Stride;
                int width = sourceImage.Width;
                int height = sourceImage.Height;

                byte* ptr = (byte*)srcData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    byte* row = ptr + (y * stride);

                    for (int x = 0; x < width; x++)
                    {
                        byte b = row[x * 3 + 0]; // Blue
                        byte g = row[x * 3 + 1]; // Green
                        byte r = row[x * 3 + 2]; // Red

                        int grayValue = (r + g + b) / 3;
                        histogram[grayValue]++;
                    }
                }

                sourceImage.UnlockBits(srcData);

                int max = histogram.Max();

                Bitmap histImage = new Bitmap(256, 200);
                using (Graphics g = Graphics.FromImage(histImage))
                {
                    g.Clear(Color.White);

                    for (int i = 0; i < 256; i++)
                    {
                        int hVal = (int)((histogram[i] / (float)max) * histImage.Height);
                        g.DrawLine(Pens.Black, new Point(i, histImage.Height), new Point(i, histImage.Height - hVal));
                    }
                }

                pictureBox3.Image = histImage;
                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                MessageBox.Show("No image is found to perform histogram. Kindly put an image or turn on the camera. Thank you!!");
            }
        }



        private unsafe void onSepiaClicked(object sender, EventArgs e)
        {
            Bitmap sourceImage = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    sourceImage = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                sourceImage = new Bitmap(pictureBox2.Image);
            }

            if (sourceImage != null)
            {
                Bitmap sepiaImage = new Bitmap(sourceImage.Width, sourceImage.Height);

                System.Drawing.Imaging.BitmapData srcData = sourceImage.LockBits(
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                System.Drawing.Imaging.BitmapData dstData = sepiaImage.LockBits(
                    new Rectangle(0, 0, sepiaImage.Width, sepiaImage.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                int stride = srcData.Stride;
                int width = sourceImage.Width;
                int height = sourceImage.Height;

                byte* srcPtr = (byte*)srcData.Scan0;
                byte* dstPtr = (byte*)dstData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    byte* srcRow = srcPtr + (y * stride);
                    byte* dstRow = dstPtr + (y * stride);

                    for (int x = 0; x < width; x++)
                    {
                        byte b = srcRow[x * 3 + 0];
                        byte g = srcRow[x * 3 + 1];
                        byte r = srcRow[x * 3 + 2];

                        int tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                        int tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                        int tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);

                        dstRow[x * 3 + 2] = (byte)(tr > 255 ? 255 : tr); // R
                        dstRow[x * 3 + 1] = (byte)(tg > 255 ? 255 : tg); // G
                        dstRow[x * 3 + 0] = (byte)(tb > 255 ? 255 : tb); // B
                    }
                }

                sourceImage.UnlockBits(srcData);
                sepiaImage.UnlockBits(dstData);

                pictureBox3.Image = sepiaImage;
                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                MessageBox.Show("No image is found to perform sepia effect. Kindly put an image or turn on the camera. Thank you!!");
            }
        }



        private void onSaveImageClicked(object sender, EventArgs e)
        {
            if (pictureBox3.Image != null)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Title = "Save Processed Image";
                    saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|Bitmap Image|*.bmp";
                    saveFileDialog.DefaultExt = "png";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                        string extension = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                        if (extension == ".jpg" || extension == ".jpeg")
                            format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        else if (extension == ".bmp")
                            format = System.Drawing.Imaging.ImageFormat.Bmp;

                        pictureBox3.Image.Save(saveFileDialog.FileName, format);

                        MessageBox.Show("Image saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                MessageBox.Show("No image is found to save. Kindly put an image to be saved. Thank you!!");
            }
        }

        private unsafe void onSubtractClicked(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("There is no background image found. Please upload a background image. Thank you!!");
                return;
            }

            Bitmap background = new Bitmap(pictureBox1.Image);
            Bitmap foreground = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    foreground = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                foreground = new Bitmap(pictureBox2.Image);
            }

            if (foreground == null)
            {
                MessageBox.Show("There is no image found. Please provide a foreground. Thank you!!");
                return;
            }

            Bitmap fgScaled = new Bitmap(foreground, background.Width, background.Height);
            Bitmap output = new Bitmap(background.Width, background.Height);

            int greenThreshold = 60;

            var bgData = background.LockBits(new Rectangle(0, 0, background.Width, background.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var fgData = fgScaled.LockBits(new Rectangle(0, 0, fgScaled.Width, fgScaled.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var outData = output.LockBits(new Rectangle(0, 0, output.Width, output.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int stride = bgData.Stride;
            int width = background.Width;
            int height = background.Height;

            byte* bgPtr = (byte*)bgData.Scan0;
            byte* fgPtr = (byte*)fgData.Scan0;
            byte* outPtr = (byte*)outData.Scan0;

            for (int y = 0; y < height; y++)
            {
                byte* bgRow = bgPtr + (y * stride);
                byte* fgRow = fgPtr + (y * stride);
                byte* outRow = outPtr + (y * stride);

                for (int x = 0; x < width; x++)
                {
                    byte fb = fgRow[x * 3 + 0];
                    byte fg = fgRow[x * 3 + 1];
                    byte fr = fgRow[x * 3 + 2];

                    byte bb = bgRow[x * 3 + 0];
                    byte bg = bgRow[x * 3 + 1];
                    byte br = bgRow[x * 3 + 2];

                    if (fg > 100 && fg > fr + greenThreshold && fg > fb + greenThreshold)
                    {
                        outRow[x * 3 + 0] = bb;
                        outRow[x * 3 + 1] = bg;
                        outRow[x * 3 + 2] = br;
                    }
                    else
                    {
                        outRow[x * 3 + 0] = fb;
                        outRow[x * 3 + 1] = fg;
                        outRow[x * 3 + 2] = fr;
                    }
                }
            }

            background.UnlockBits(bgData);
            fgScaled.UnlockBits(fgData);
            output.UnlockBits(outData);

            pictureBox3.Image = output;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void onOnOffCameraClicked(object sender, EventArgs e)
        {
            if (currentDevice != null)
            {
                currentDevice.Stop();
                currentDevice = null;

                pictureBox2.Image = null;
                pictureBox2.Invalidate();

                return;
            }

            devices = DeviceManager.GetAllDevices();

            if (devices.Length == 0)
            {
                MessageBox.Show("No camera detected!");
                return;
            }

            currentDevice = devices[0];
            currentDevice.ShowWindow(pictureBox2);

        }

        private void onSmoothingClicked(object sender, EventArgs e)
        {
            Bitmap input = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    input = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                input = new Bitmap(pictureBox2.Image);
            }

            if (input == null)
            {
                MessageBox.Show("No image or frame is found to apply smoothing. Kindly put an image or turn on the camera. Thank you!!");
                return;
            }

            Bitmap output = (Bitmap)input.Clone();

            ConvMatrix smooth = new ConvMatrix();
            smooth.SetAll(1);
            smooth.Pixel = 1;
            smooth.Factor = 9;

            BitmapFilter.Conv3x3(output, smooth);

            pictureBox3.Image = output;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void onGaussianClicked(object sender, EventArgs e)
        {
            Bitmap input = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    input = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                input = new Bitmap(pictureBox2.Image);
            }

            if (input == null)
            {
                MessageBox.Show("No image or frame is found to apply Gaussian blur. Kindly put an image or turn on the camera. Thank you!!");
                return;
            }

            // Clone input image
            Bitmap output = (Bitmap)input.Clone();
 
            ConvMatrix gaus = new ConvMatrix();
            gaus.TopLeft = gaus.TopRight = gaus.BottomLeft = gaus.BottomRight = 1;
            gaus.TopMid = gaus.MidLeft = gaus.MidRight = gaus.BottomMid = 2;
            gaus.Pixel = 4;
            gaus.Factor = 16;

            BitmapFilter.Conv3x3(output, gaus);

            pictureBox3.Image = output;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void onSharpenClicked(object sender, EventArgs e)
        {
            Bitmap input = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    input = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                input = new Bitmap(pictureBox2.Image);
            }
            
            if (input == null) 
            {
                MessageBox.Show("No image or frame is found to apply sharpening. Kindly put an image or turn on the camera. Thank you!!");
                return;
            }

            Bitmap output = (Bitmap)input.Clone();

            ConvMatrix sharp = new ConvMatrix();
            sharp.TopMid = sharp.MidLeft = sharp.MidRight = sharp.BottomMid = -2;
            sharp.Pixel = 11;
            sharp.Factor = 3;

            BitmapFilter.Conv3x3(output, sharp);


            pictureBox3.Image = output;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void onMeanRemovalClicked(object sender, EventArgs e)
        {
            Bitmap input = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    input = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                input = new Bitmap(pictureBox2.Image);
            }

            if (input == null)
            {
                MessageBox.Show("No image or frame is found to apply mean removal. Kindly put an image or turn on the camera. Thank you!!");
                return;
            }

            Bitmap output = (Bitmap)input.Clone();

            ConvMatrix mean = new ConvMatrix();
            mean.SetAll(-1);
            mean.Pixel = 9;
            mean.Factor = 1;

            BitmapFilter.Conv3x3(output, mean);

            pictureBox3.Image = output;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
        }


        private void onEmbossClicked(object sender, EventArgs e)
        {
            Bitmap input = null;

            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                if (Clipboard.ContainsImage())
                {
                    input = new Bitmap(Clipboard.GetImage());
                }
            }
            else if (pictureBox2.Image != null)
            {
                input = new Bitmap(pictureBox2.Image);
            }

            if (input == null)
            {
                MessageBox.Show("No image or frame is found to apply emboss. Kindly put an image or turn on the camera. Thank you!!");
                return;
            }

            Bitmap output = (Bitmap)input.Clone();
            ConvMatrix emboss = new ConvMatrix();

            using (var dlg = new Form2())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    switch (dlg.SelectedEmboss)
                    {
                        case Form2.EmbossType.Laplacian:
                            emboss.TopLeft = emboss.TopRight = emboss.BottomLeft = emboss.BottomRight = -1;
                            emboss.Pixel = 4;
                            emboss.Factor = 1;
                            emboss.Offset = 127;
                            break;

                        case Form2.EmbossType.Vertical:
                            emboss.TopMid = -1;
                            emboss.BottomMid = 1;
                            emboss.Pixel = 0;
                            emboss.Factor = 1;
                            emboss.Offset = 127;
                            break;

                        case Form2.EmbossType.Horizontal:
                            emboss.MidLeft = -1;
                            emboss.MidRight = -1;
                            emboss.Pixel = 2;
                            emboss.Factor = 1;
                            emboss.Offset = 127;
                            break;

                        case Form2.EmbossType.Lossy:
                            emboss.TopLeft = emboss.TopRight = emboss.BottomMid = 1;
                            emboss.TopMid = emboss.MidLeft = emboss.MidRight = emboss.BottomLeft = emboss.BottomRight = -2;
                            emboss.Pixel = 4;
                            emboss.Factor = 1;
                            emboss.Offset = 127;
                            break;

                        case Form2.EmbossType.AllDirections:
                            emboss.SetAll(-1);
                            emboss.Pixel = 8;
                            emboss.Factor = 1;
                            emboss.Offset = 127;
                            break;

                        case Form2.EmbossType.HorzVertical:
                            emboss.TopMid = emboss.MidLeft = emboss.MidRight = emboss.BottomMid = -1;
                            emboss.Pixel = 4;
                            emboss.Factor = 1;
                            emboss.Offset = 127;
                            break;

                        default:
                            MessageBox.Show("No emboss effect chosen.");
                            return;
                    }
                }
                else
                {
                    return; 
                }
            }

            BitmapFilter.Conv3x3(output, emboss);

            pictureBox3.Image = output;
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
        }

    }
}
