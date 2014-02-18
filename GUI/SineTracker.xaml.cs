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
        private double Y;
        private float hipY;
        private float headY;
        private double tempo;
        double canvasWidthInSeconds = 1;
        Canvas sineCanvas;
        int dataRectSize = 8;
        //private double xmin = 0;

        //private double xmax = 6.5;
        //private double ymin = -1.1;
        //private double ymax = 1.1;
        //private Polyline pl;

        public double PixelsPerSecond
        {
            get { if (!(sineCanvas == null)) { return sineCanvas.ActualWidth / canvasWidthInSeconds; } else { return 0; } }
        }

        public double HeadHipScaler
        {
            get { return (headY - hipY); } 
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
        }

        Complex[] samples = new Complex[32];
        private void SkeletonMoved(float time, Skeleton skel)
        {
            Y = skel.Joints[JointType.HandRight].Position.Y;
            hipY = skel.Joints[JointType.KneeRight].Position.Y;
            headY = skel.Joints[JointType.Head].Position.Y;
            scaleInput();

            for (int i = 0; i < samples.Length-1; ++i) samples[i] = samples[i + 1];
            samples[samples.Length - 1] = Y;

            elapsedTime = time - prevTime;
            prevTime = time;
            UpdateChart();
        }

        private void UpdateChart()
        {
            double pixelOffset = elapsedTime * PixelsPerSecond;
            Rectangle datum = new Rectangle { Width = dataRectSize, Fill = Brushes.Red, Height = dataRectSize, Stroke = Brushes.Black, StrokeThickness = 2 };
            datum.Tag = canvasWidthInSeconds + elapsedTime;
            Canvas.SetTop(datum, Y);
            sineCanvas.Children.Add(datum);

            List<UIElement> markedChildren = new List<UIElement>();
            foreach (UIElement p in sineCanvas.Children)
            {
                Rectangle child = p as Rectangle;
                if (child == null)
                {
                    markedChildren.Add(p);
                    continue;
                }
                double newPos = calculateNewPos(child);
                Canvas.SetLeft(child, newPos);
                if (((double)child.Tag + elapsedTime) < 0)
                {
                    markedChildren.Add(child);
                }
            }
            foreach (var child in markedChildren)
            {
                sineCanvas.Children.Remove(child);
            }

            Complex[] fft = new Complex[samples.Length];
            samples.CopyTo(fft, 0);
            MathNet.Numerics.IntegralTransforms.Transform.FourierForward(fft);
            //foreach (var i in fft) Console.WriteLine(fft);
            for (int i = 0; i < fft.Length; ++i) fft[i] = fft[i].Magnitude;

            double max = 0, maxi = 0;
            for (int i = 1; i < fft.Length/2; ++i)
                if (fft[i].Real > max) { max = fft[i].Real; maxi = i; }
            //Console.WriteLine("{0} {1}", maxi/30*fft.Length, max);
            tempo = maxi / 30 * fft.Length;

            AddChart();
        }
        private void scaleInput()
        {
            //        x - min                                  max - min
            //f(x) = ---------   ===>   f(min) = 0;  f(max) =  --------- = 1
            //       max - min                                 max - min
            Y = (1 - (Y - hipY) / (headY - hipY))* sineCanvas.ActualHeight;
            //mathY = (Y - hipY) / (headY - hipY);
            return;
        }
        private double calculateNewPos(Rectangle child)
        {
            child.Tag = (double)child.Tag - elapsedTime;
            double pixelPosition = (double)child.Tag * PixelsPerSecond;
            return pixelPosition;
        }
        //private void AddChart()
        //{
        //    // Draw sine curve:
        //    pl = new Polyline();
        //    pl.Stroke = Brushes.Black;
        //    for (int i = 0; i < 70; i++)
        //    {
        //        double x = i / 5.0;
        //        double y = Math.Sin(x);
        //        pl.Points.Add(CurvePoint(
        //        new Point(x, y)));
        //    }
        //    chartCanvas.Children.Add(pl);
        //    // Draw cosine curve:
        //    pl = new Polyline();
        //    pl.Stroke = Brushes.Black;
        //    pl.StrokeDashArray = new DoubleCollection(
        //    new double[] { 4, 3 });
        //    for (int i = 0; i < 70; i++)
        //    {
        //        double x = i / 5.0;
        //        double y = Math.Cos(x);
        //        pl.Points.Add(CurvePoint(
        //        new Point(x, y)));
        //    }
        //    chartCanvas.Children.Add(pl);
        //}
        //private Point CurvePoint(Point pt)
        //{
        //    Point result = new Point();
        //    result.X = (pt.X - xmin) * chartCanvas.Width / (xmax - xmin);
        //    result.Y = chartCanvas.Height - (pt.Y - ymin) * chartCanvas.Height
        //    / (ymax - ymin);
        //    return result;
        //}

        int resolution = 70;
        private void AddChart()
        {
            // Draw sine curve:
            Polyline pl = new Polyline();
            pl.Stroke = Brushes.Black;
            for (int i = 0; i < resolution; i++)
            {
                double x = 1.0*i/resolution;
                double y = Math.Sin(x*tempo*2*Math.PI);
                pl.Points.Add(CurvePoint(new Point(x, y)));
            }
            sineCanvas.Children.Add(pl);
        }
        private Point CurvePoint(Point pt)
        {
            double xmin = 0, xmax = 1, ymin = -1, ymax = 1;

            Point result = new Point();
            result.X = (pt.X - xmin) * sineCanvas.ActualWidth / (xmax - xmin);
            result.Y = sineCanvas.ActualHeight - (pt.Y - ymin) * sineCanvas.ActualHeight / (ymax - ymin);
            return result;
        }
    }
}
