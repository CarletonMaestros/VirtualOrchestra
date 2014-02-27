using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;

using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;



namespace Orchestra
{
    public partial class SineTracker : Window
    {
        float elapsedTime = 0;
        float prevTime = 0;
        private double tempo;
        private double fitTempo;


        int maxi = 0;

        double canvasWidthInSeconds = 1;
        Canvas sineCanvas;
        //TextBlock t1;
        int dataRectSize = 8;

        Complex[] samples = new Complex[32];
        double[] sineApprox = new double[32];

        public double PixelsPerSecond
        {
            get { if (!(sineCanvas == null)) { return sineCanvas.ActualWidth / canvasWidthInSeconds; } else { return 0; } }
        }

        public SineTracker()
        {
            InitializeComponent();
        }

        ~SineTracker()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }

        public void WindowLoaded(object sender, RoutedEventArgs e)
        {
            object chart = FindName("chartCanvas");
            sineCanvas = (Canvas)chart;
            Dispatch.SkeletonMoved += this.SkeletonMoved;

            //AddChart();
            //Hide();    
        }

        
        private void SkeletonMoved(float time, Skeleton skel)
        {
            double Y = skel.Joints[JointType.HandRight].Position.Y;
            double hipY = skel.Joints[JointType.KneeRight].Position.Y;
            double headY = skel.Joints[JointType.Head].Position.Y;
            scaleInput(); //scales Y: 0 to 1

            for (int i = 0; i < samples.Length-1; ++i) samples[i] = samples[i + 1]; //shift samples down anbd place new sample at the end
            samples[samples.Length - 1] = Y;

            elapsedTime = time - prevTime;
            prevTime = time;
            UpdateChart(Y);
        }

        private void UpdateChart(double latestY)
        {
            double pixelOffset = elapsedTime * PixelsPerSecond;
            Rectangle datum = new Rectangle { Width = dataRectSize, Fill = Brushes.Red, Height = dataRectSize, Stroke = Brushes.Black, StrokeThickness = 2 };
            datum.Tag = canvasWidthInSeconds + elapsedTime;
            Canvas.SetTop(datum, latestY*sineCanvas.ActualHeight);
            sineCanvas.Children.Add(datum);

            Point maxY = new Point();
            Point minY = new Point();
            maxY.Y = 0;
            minY.Y = double.MaxValue;

            List<UIElement> markedChildren = new List<UIElement>();
            foreach (UIElement p in sineCanvas.Children)
            {
                Rectangle child = p as Rectangle;
                if (child == null)
                {
                    markedChildren.Add(p);
                    continue;
                }
                double newPos = calculateNewXPos(child);
                Canvas.SetLeft(child, newPos);
                if (((double)child.Tag + elapsedTime) < 0)
                {
                    markedChildren.Add(child);
                }
                else
                {
                    if (Canvas.GetTop(child)/sineCanvas.ActualHeight > maxY.Y)
                    {
                        maxY.Y = Canvas.GetTop(child) / sineCanvas.ActualHeight;
                        maxY.X = (double)child.Tag;
                    }
                    if (Canvas.GetTop(child) / sineCanvas.ActualHeight < minY.Y)
                    {
                        minY.Y = Canvas.GetTop(child) / sineCanvas.ActualHeight;
                        minY.X = (double)child.Tag;
                    }
                }
            }
            foreach (var child in markedChildren)
            {
                sineCanvas.Children.Remove(child);
            }

            Complex[] fft = new Complex[samples.Length];
            samples.CopyTo(fft, 0);
            MathNet.Numerics.IntegralTransforms.Transform.FourierForward(fft);
            for (int i = 0; i < fft.Length; ++i) fft[i] = fft[i].Magnitude;

            double max = 0;
            for (int i = 1; i < fft.Length/2; ++i)
                if (fft[i].Real > max) { max = fft[i].Real; maxi = i; }
            t1.Text = findFrequency().ToString("0.##") + " Hz";
            t2.Text = "i+1 = " + fft[maxi + 1].Real.ToString("0.##");
            t3.Text = "i-1 = " + fft[maxi - 1].Real.ToString("0.##");    
            t4.Text = "fit = " + err().ToString("0.##");

            tempo = (double)maxi / 30 * fft.Length;

            AddChart();
        }
        private void scaleInput()
        {
            //        x - min                                  max - min
            //f(x) = ---------   ===>   f(min) = 0;  f(max) =  --------- = 1
            //       max - min                                 max - min
            Y = (Y - hipY) / (headY - hipY);// *sineCanvas.ActualHeight;
            return;
        }
        private double calculateNewXPos(Rectangle child)
        {
            child.Tag = (double)child.Tag - elapsedTime;
            double pixelPosition = (double)child.Tag * PixelsPerSecond;
            return pixelPosition;
        }
        private double err()
        {
            double total = 0 ;
            for (int i = 0; i < sineApprox.Length; ++i)
            {

                if (samples[i].Real != double.NaN)
                {
                    total += Math.Pow((sineApprox[i] - samples[i].Real) , 2); //add the square of the difference
                }
                else
                {
                    Console.Write("ble");
                }
            }
            return total;
        }
        public double amp { get { return (maxY.Y - minY.Y) / 2; } }
        public double mean()
        {
            double total= 0 ;
            int numPoints = 0;
            for (int i = 0; i < samples.Length; ++i)
            {

                if (samples[i].Real != double.NaN)
                {
                    total += (samples[i].Real);
                    numPoints++;
                }
            }
            return total/numPoints;
        }
        private double findFrequency()
        {
            double bestFreq = 0;
            double bestFit = double.MaxValue;
            for (int j = -25; j < 25; j++){
                for (int i = 0; i < samples.Length; ++i)
                {
                    double curTempo = tempo-j/50d;
                    double x = 1.0*(i-maxi)/32;
                    if (samples[i].Real != double.NaN)
                    {
                        double y = amp * Math.Cos(x * curTempo * 2 * Math.PI) - mean();
                        sineApprox[i] = y;
                    }
                }
                double curErr = err();
                if (curErr < bestFit)
                {
                    bestFit = curErr;
                    bestFreq = tempo-j/50d;
                }
            }
            fitTempo = bestFreq;
            return bestFreq;
        }

        
        int resolution = 96;
        private void AddChart()
        {
            // Draw sine curve:
            Polyline pl = new Polyline();
            pl.Stroke = Brushes.Black;
            for (int i = 0; i < resolution; i++)
            {
                double x = 1.0*(i-(maxi*(resolution/32)/resolution));
                double y = amp*Math.Cos(x*fitTempo*2*Math.PI)-mean();
                Point newPoint = CurvePoint(new Point(x, y));
                pl.Points.Add(newPoint);
                if (i % 3 == 0)
                {
                    sineApprox[i / 3] = newPoint.Y;
                }
            }
            sineCanvas.Children.Add(pl);
        }
        private Point CurvePoint(Point pt)
        {
            double xmin = 0, xmax = 1, ymin = 0, ymax = 1;

            Point result = new Point();
            result.X = (pt.X - xmin) * sineCanvas.ActualWidth / (xmax - xmin);
            result.Y = sineCanvas.ActualHeight - (pt.Y - ymin) * sineCanvas.ActualHeight / (ymax - ymin);
            return result;
        }
    }
}
