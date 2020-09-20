using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace AOCI_1
{
    public class ImageProcessing
    {
        public Image<Bgr, byte> sourceImage;
        public VideoCapture capture;
        public event EventHandler<ImageEventArgs> ImageProcessed;
        int frameCounter = 0;

        public class ImageEventArgs : EventArgs
        {
            public IInputArray Image { get; set; }
        }

        public ImageProcessing(Image<Bgr, byte> image)
        {
            sourceImage = image;
        }

        public double cannyThreshold { get; set; } = 20.0;
        public double cannyThresholdLinking { get; set; } = 30.0;
        public int Color_1 { get; set; } = 50;
        public int Color_2 { get; set; } = 150;
        public int Color_3 { get; set; } = 200;

        public Image<Bgr, byte> Processing()
        {
            Image<Gray, byte> grayImage = sourceImage.Convert<Gray, byte>();

            var tempImage = grayImage.PyrDown();
            var destImage = tempImage.PyrUp();
            
            Image<Gray, byte> cannyEdges = destImage.Canny(cannyThreshold, cannyThresholdLinking);

            //cannyEdges._Dilate(1);

            var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
            var resultImage = sourceImage.Sub(cannyEdgesBgr);

            for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                for (int x = 0; x < resultImage.Width; x++)
                    for (int y = 0; y < resultImage.Height; y++) // обход по пискелям
                    {
                        // получение цвета пикселя
                        byte color = resultImage.Data[y, x, channel];
                        if (color <= Color_1)
                            color = 0;
                        //else if (color <= 100)
                        //    color = 25;
                        else if (color <= Color_2)
                            color = 100;
                        else if (color <= Color_3)
                            color = 200;
                        else
                            color = 255;
                        resultImage.Data[y, x, channel] = color; // изменение цвета пикселя
                    }
            return resultImage;
        }

        public void StartVideoFromCam()
        {
            capture = new VideoCapture();
            capture.ImageGrabbed += ProcessFrame;
            capture.Start();
        }

        public Image<Bgr, byte> timerVideo()
        {
            var frame = capture.QueryFrame();
            
            sourceImage = frame.ToImage<Bgr, byte>(); //обрабатываемое изображение из функции Processing приравниваем к фрейму
            var videoImage = Processing(); //на финальное изображение накладываем фильтр, вызывая функцию

            frameCounter++;
            return videoImage;
        }

        public void VideoProcessing(string fileName)
        {
            capture = new VideoCapture(fileName); //берем кадры из видео
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            try
            {
                var frame = new Mat();
                capture.Retrieve(frame);
                sourceImage = frame.ToImage<Bgr, byte>();
                var result = Processing();
                ImageProcessed?.Invoke(this, new ImageEventArgs { Image = result });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                capture.ImageGrabbed -= ProcessFrame;
                capture.Stop();
            }
        }
    }
}
