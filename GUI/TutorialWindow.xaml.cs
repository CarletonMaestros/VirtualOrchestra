using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
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
    /// <summary>
    /// Interaction logic for Tutorial.xaml
    /// </summary>
    public partial class TutorialWindow : Window
    {
        private const float RenderWidth = 640.0f;
        private const float RenderHeight = 480.0f;
        private const double JointThickness = 3;
        private const double BodyCenterThickness = 10;
        private const double ClipBoundsThickness = 10;
        private readonly Brush centerPointBrush = Brushes.Blue;
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        private KinectSensor sensor;
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;
        private int nextInstruction = 1;
        private Point rightHipValues;
        private Point rightHipUpperBound;
        private bool drawHipBox;
        private SkeletonPoint rightHand;
        private SkeletonPoint rightHip;
        private SkeletonPoint leftHand;
        private SkeletonPoint leftHip;
        private Point rightHandValues;

        public TutorialWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            InitKinect();
            InitDrawing();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e) 
        {
            WriteInstructions(nextInstruction);
            nextInstruction++;           
        }

        private void RedoStepButton_Click_1(object sender, RoutedEventArgs e)
        {
            if ((nextInstruction - 1) == 1) { Instructions.Text = "Welcome to the Virtual Orchestra tutorial! You should be able to see your kinect skeleton in the frame to the left."; }
            else if ((nextInstruction - 1) == 1)
            {
                hipBox.BorderBrush = Brushes.Red;
            }
            else { Instructions.Text = ""; }
            WriteInstructions(nextInstruction - 1);
        }

        private void WriteInstructions(int InstructionNumber)
        {
            if (InstructionNumber == 1)
            {
                Instructions.Text = Instructions.Text + Environment.NewLine + "The first gesture we will go over is the basic conducting gesture.";
            }
            else if (InstructionNumber == 2)
            {
                Instructions.Text = "You can choose to either bounce your hand up and down, or conduct with a standard 4/4 conducting gesture, as pictured below.";
                var newImage = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\conduct.jpg");
                conductingImage.Source = new BitmapImage(newImage);
            }
            else if (InstructionNumber == 3)
            {
                conductingImage.Source = null;
                Instructions.Text = "Your first conducting gesture must fall in the box pictured on the skeleton, which is located roughly at your hip. The box will turn green when this is successfully completed.";
                drawHipBox = true;
            }
            else if (InstructionNumber == 4)
            {
                drawHipBox = false;
            }
        }

        private void InitKinect()
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the skeleton stream to receive skeleton frames
                this.sensor.SkeletonStream.Enable();

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
        }

        private void InitDrawing()
        {
            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // Display the drawing using our image control
            SkeletonWindow.Source = this.imageSource;
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            // Get skeletons from Kinect
            Skeleton[] skeletons = new Skeleton[0];
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            foreach (Skeleton skel in skeletons)
            {
                foreach (Joint joint in skel.Joints)
                {
                    if (joint.JointType == JointType.HandRight) { rightHand = joint.Position; }
                    if (joint.JointType == JointType.HandLeft) { leftHand = joint.Position; }
                    if (joint.JointType == JointType.HipRight) { rightHip = joint.Position; }
                    if (joint.JointType == JointType.HipLeft) { leftHip = joint.Position; }
                }
                if (drawHipBox)
                {
                    Console.WriteLine(rightHip.Y);
                    rightHip.X -= .13f;
                    rightHip.Y += .13f;
                    rightHipValues = SkeletonPointToScreen(rightHip);
                    rightHandValues = SkeletonPointToScreen(rightHand);
                    rightHip.X += .26f;
                    rightHipUpperBound = SkeletonPointToScreen(rightHip);
                    rightHipUpperBound.Y -= (rightHipUpperBound.X - rightHipValues.X);
                    if (rightHipValues.X != 0 && rightHandValues.X != 0)
                    {                        
                        
                        //Console.WriteLine(rightHipUpperBound.Y);
                        //Console.WriteLine(rightHandValues.Y);
                        //Console.WriteLine(rightHipValues.Y);
                        //Console.WriteLine("");
                        hipBox.Margin = new Thickness(rightHipValues.X, rightHipValues.Y, 0, 0);
                        hipBox.Width = 85;
                        hipBox.Height = 85;
                        hipBox.BorderThickness = new Thickness(4);
                        if (rightHandValues.X > rightHipValues.X + 10 && rightHandValues.X < rightHipUpperBound.X - 10 && rightHandValues.Y < rightHipValues.Y - 10 && rightHandValues.Y > rightHipUpperBound.Y + 10)
                        {
                            hipBox.BorderBrush = Brushes.LawnGreen;
                        }
                    }
                    
                }
                
            }

            // Push event to the rest of the system
            foreach (Skeleton skel in skeletons)
            {
                if (skel.TrackingState == SkeletonTrackingState.Tracked)
                {
                    Dispatch.TriggerSkeletonMoved(skel);
                    break;
                }
            }

            // Draw skeletons
            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));

                foreach (Skeleton skel in skeletons)
                {
                    RenderClippedEdges(skel, dc);

                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        this.DrawBonesAndJoints(skel, dc);
                    }
                    else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        dc.DrawEllipse(
                        this.centerPointBrush,
                        null,
                        this.SkeletonPointToScreen(skel.Position),
                        BodyCenterThickness,
                        BodyCenterThickness);
                    }
                }

                // Prevent drawing outside of our render area
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
            }
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping skeleton data
        /// </summary>
        /// <param name="skeleton">skeleton to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private static void RenderClippedEdges(Skeleton skeleton, DrawingContext drawingContext)
        {
            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, RenderHeight - ClipBoundsThickness, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, RenderWidth, ClipBoundsThickness));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, RenderHeight));
            }

            if (skeleton.ClippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(RenderWidth - ClipBoundsThickness, 0, ClipBoundsThickness, RenderHeight));
            }
        }

        /// <summary>
        /// Draws a skeleton's bones and joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            // Render Torso
            this.DrawBone(skeleton, drawingContext, JointType.Head, JointType.ShoulderCenter);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.ShoulderRight);
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderCenter, JointType.Spine);
            this.DrawBone(skeleton, drawingContext, JointType.Spine, JointType.HipCenter);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipLeft);
            this.DrawBone(skeleton, drawingContext, JointType.HipCenter, JointType.HipRight);

            // Left Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderLeft, JointType.ElbowLeft);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowLeft, JointType.WristLeft);
            this.DrawBone(skeleton, drawingContext, JointType.WristLeft, JointType.HandLeft);

            // Right Arm
            this.DrawBone(skeleton, drawingContext, JointType.ShoulderRight, JointType.ElbowRight);
            this.DrawBone(skeleton, drawingContext, JointType.ElbowRight, JointType.WristRight);
            this.DrawBone(skeleton, drawingContext, JointType.WristRight, JointType.HandRight);

            // Left Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipLeft, JointType.KneeLeft);
            this.DrawBone(skeleton, drawingContext, JointType.KneeLeft, JointType.AnkleLeft);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleLeft, JointType.FootLeft);

            // Right Leg
            this.DrawBone(skeleton, drawingContext, JointType.HipRight, JointType.KneeRight);
            this.DrawBone(skeleton, drawingContext, JointType.KneeRight, JointType.AnkleRight);
            this.DrawBone(skeleton, drawingContext, JointType.AnkleRight, JointType.FootRight);

            // Render Joints
            foreach (Joint joint in skeleton.Joints)
            {
                Brush drawBrush = null;

                if (joint.TrackingState == JointTrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush;
                }
                else if (joint.TrackingState == JointTrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush;
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(joint.Position), JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Maps a SkeletonPoint to lie within our render space and converts to Point
        /// </summary>
        /// <param name="skelpoint">point to map</param>
        /// <returns>mapped point</returns>
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            // Convert point to depth space.  
            // We are not using depth directly, but we do want the points in our 640x480 output resolution.
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

        /// <summary>
        /// Draws a bone line between two joints
        /// </summary>
        /// <param name="skeleton">skeleton to draw bones from</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        /// <param name="jointType0">joint to start drawing from</param>
        /// <param name="jointType1">joint to end drawing at</param>
        private void DrawBone(Skeleton skeleton, DrawingContext drawingContext, JointType jointType0, JointType jointType1)
        {
            Joint joint0 = skeleton.Joints[jointType0];
            Joint joint1 = skeleton.Joints[jointType1];

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == JointTrackingState.NotTracked ||
                joint1.TrackingState == JointTrackingState.NotTracked)
            {
                return;
            }

            // Don't draw if both points are inferred
            if (joint0.TrackingState == JointTrackingState.Inferred &&
                joint1.TrackingState == JointTrackingState.Inferred)
            {
                return;
            }

            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen;
            if (joint0.TrackingState == JointTrackingState.Tracked && joint1.TrackingState == JointTrackingState.Tracked)
            {
                drawPen = this.trackedBonePen;
            }

            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }
    }
}
