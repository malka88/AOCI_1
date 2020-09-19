using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace AOCI_1
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> sourceImage;
        int frameCounter = 0;
        private ImageProcessing resultImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(); // открытие диалога выбора файла
            if (result == DialogResult.OK) // открытие выбранного файла
            {
                string fileName = openFileDialog.FileName;
                sourceImage = new Image<Bgr, byte>(fileName).Resize(640, 480, Inter.Linear);
                resultImage = new ImageProcessing(sourceImage);

                imageBox1.Image = sourceImage;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {           
            imageBox2.Image = resultImage.Processing();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            resultImage.ImageProcessed += ImageProcessed;
            resultImage.StartVideoFromCam();
        }

        private void ImageProcessed(object sender, ImageProcessing.ImageEventArgs e)
        {
            imageBox2.Image = e.Image;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                resultImage.VideoProcessing(fileName);

                //capture = new VideoCapture(fileName);
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var frame = resultImage.capture.QueryFrame();

            Image<Bgr, byte> videoImage = frame.ToImage<Bgr, byte>();

            imageBox2.Image = videoImage;

            frameCounter++;

            if (frameCounter >= resultImage.capture.GetCaptureProperty(CapProp.FrameCount))
                timer1.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            resultImage.Color_1 = trackBar1.Value;
            imageBox2.Image = resultImage.Processing();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            resultImage.Color_2 = trackBar2.Value;
            imageBox2.Image = resultImage.Processing();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            resultImage.Color_3 = trackBar3.Value;
            imageBox2.Image = resultImage.Processing();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            resultImage.cannyThreshold = trackBar4.Value;
            imageBox2.Image = resultImage.Processing();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            resultImage.cannyThresholdLinking = trackBar5.Value;
            imageBox2.Image = resultImage.Processing();
        }
    }
}
