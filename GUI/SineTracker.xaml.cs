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



namespace Orchestra
{
    public partial class SineTracker : Window
    {
        float elapsedTime = 0;
        float prevTime = 0;
        private double Y;
        private float hipY;
        private float headY;
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

        private void SkeletonMoved(float time, Skeleton skel)
        {
            foreach (Joint joint in skel.Joints)
            {
                if (joint.JointType == JointType.HandRight)
                {
                    Y = joint.Position.Y;
                    scaleInput();
                }
                if (joint.JointType == JointType.KneeRight)
                {
                    hipY = joint.Position.Y;
                }
                if (joint.JointType == JointType.Head)
                {
                    headY = joint.Position.Y;
                }
            }
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
            Console.WriteLine(Y);
            sineCanvas.Children.Add(datum);

            List<Rectangle> markedChildren = new List<Rectangle>();
            foreach (Rectangle child in sineCanvas.Children)
            {
                double newPos = calculateNewPos(child);
                Canvas.SetLeft(child, newPos);
                if (((double)child.Tag + elapsedTime) < 0)
                {
                    markedChildren.Add(child);
                }
            }
            foreach (Rectangle child in markedChildren)
            {
                sineCanvas.Children.Remove(child);
            }
        }
        private void scaleInput()
        {
            //        x - min                                  max - min
            //f(x) = ---------   ===>   f(min) = 0;  f(max) =  --------- = 1
            //       max - min                                 max - min
            Y = (1 - (Y - hipY) / (headY - hipY))* sineCanvas.ActualHeight;
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
    }
}
