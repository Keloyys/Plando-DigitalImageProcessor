using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private void onCopyClicked(object sender, EventArgs e)
        {
            if (currentDevice != null)
            {
                currentDevice.Sendmessage();

                if (Clipboard.ContainsImage())
                {
                    Image snapshot = Clipboard.GetImage();
                    pictureBox3.Image = new Bitmap(snapshot);
                    pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    MessageBox.Show("Failed to capture snapshot from webcam.");
                }
            }
            else if (pictureBox2.Image != null)
            {
                pictureBox3.Image = new Bitmap(pictureBox2.Image);
                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                MessageBox.Show("No image is found to be copied. Kindly put an image or turn on the camera. Thank you!!");
            }
        }


        private void onGreyScaledButtonClicked(object sender, EventArgs e)
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
                Bitmap grayImage = new Bitmap(sourceImage.Width, sourceImage.Height);

                for (int x = 0; x < sourceImage.Width; x++)
                {
                    for (int y = 0; y < sourceImage.Height; y++)
                    {
                        Color pixelColor = sourceImage.GetPixel(x, y);
                        int grayValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                        grayImage.SetPixel(x, y, grayColor);
                    }
                }

                pictureBox3.Image = grayImage;
                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                MessageBox.Show("No image is found to be grey scaled. Kindly put an image or turn on the camera. Thank you!!");
            }
        }

        private void onColorInversionClicked(object sender, EventArgs e)
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
                Bitmap invertedImage = new Bitmap(sourceImage.Width, sourceImage.Height);

                for (int x = 0; x < sourceImage.Width; x++)
                {
                    for (int y = 0; y < sourceImage.Height; y++)
                    {
                        Color pixelColor = sourceImage.GetPixel(x, y);
                        Color invertedColor = Color.FromArgb(255 - pixelColor.R, 255 - pixelColor.G, 255 - pixelColor.B);
                        invertedImage.SetPixel(x, y, invertedColor);
                    }
                }

                pictureBox3.Image = invertedImage;
                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                MessageBox.Show("No image is found to be inverted. Kindly put an image or turn on the camera. Thank you!!");
            }
        }

        private void onHistogramClicked(object sender, EventArgs e)
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
                for (int x = 0; x < sourceImage.Width; x++)
                {
                    for (int y = 0; y < sourceImage.Height; y++)
                    {
                        Color pixelColor = sourceImage.GetPixel(x, y);
                        int grayValue = (pixelColor.R + pixelColor.G + pixelColor.B) / 3;
                        histogram[grayValue]++;
                    }
                }

                int max = histogram.Max();

                Bitmap histImage = new Bitmap(256, 200);
                using (Graphics g = Graphics.FromImage(histImage))
                {
                    g.Clear(Color.White);

                    for (int i = 0; i < 256; i++)
                    {
                        int height = (int)((histogram[i] / (float)max) * histImage.Height);
                        g.DrawLine(Pens.Black, new Point(i, histImage.Height), new Point(i, histImage.Height - height));
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


        private void onSepiaClicked(object sender, EventArgs e)
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

                for (int x = 0; x < sourceImage.Width; x++)
                {
                    for (int y = 0; y < sourceImage.Height; y++)
                    {
                        Color pixelColor = sourceImage.GetPixel(x, y);

                        int tr = (int)(0.393 * pixelColor.R + 0.769 * pixelColor.G + 0.189 * pixelColor.B);
                        int tg = (int)(0.349 * pixelColor.R + 0.686 * pixelColor.G + 0.168 * pixelColor.B);
                        int tb = (int)(0.272 * pixelColor.R + 0.534 * pixelColor.G + 0.131 * pixelColor.B);

                        tr = Math.Min(255, tr);
                        tg = Math.Min(255, tg);
                        tb = Math.Min(255, tb);

                        sepiaImage.SetPixel(x, y, Color.FromArgb(tr, tg, tb));
                    }
                }

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

        private void onSubtractClicked(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("There is no background image found. Please upload a background image. Thank you!!");
                return;
            }

            Bitmap background = new Bitmap(pictureBox1.Image);
            Bitmap foreground = null;

            if (currentDevice!= null)
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

            for (int x = 0; x < background.Width; x++)
            {
                for (int y = 0; y < background.Height; y++)
                {
                    Color bgC = background.GetPixel(x, y);
                    Color fgC = fgScaled.GetPixel(x, y);

                    // Detect green pixels
                    if (fgC.G > 100 && fgC.G > fgC.R + greenThreshold && fgC.G > fgC.B + greenThreshold)
                    {
                        output.SetPixel(x, y, bgC); 
                    }
                    else
                    {
                        output.SetPixel(x, y, fgC); 
                    }
                }
            }

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

    }
}
