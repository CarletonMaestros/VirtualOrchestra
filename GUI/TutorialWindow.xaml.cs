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
using System.Diagnostics;

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
        private Point boxTopLeftValues;
        private Point boxBottomRightValues;
        private bool drawBox;
        private SkeletonPoint rightHand;
        private SkeletonPoint rightHip;
        private SkeletonPoint leftHand;
        private SkeletonPoint leftHip;
        private Point handValues;
        private bool checkIfRightHandStill;
        private bool checkIfLeftHandStill;
        private int counter;
        private SkeletonPoint prevOne;
        private SkeletonPoint prevTwo;
        private bool aboveHip;
        private float curY;
        private Point hipValues;
        Stopwatch stopwatch;

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
            else if ((nextInstruction - 1) == 3)
            {
                box.BorderThickness = new Thickness(0);
                checkIfRightHandStill = false;
                box.BorderBrush = Brushes.Red;
            }
            else { Instructions.Text = ""; }
            WriteInstructions(nextInstruction - 1);
        }

        private void WriteInstructions(int InstructionNumber)
        {
            if (InstructionNumber == 1)
            {
                Instructions.Text += Environment.NewLine + "The first gesture we will go over is the basic conducting gesture.";
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
                Instructions.Text = "To begin, put your right hand up at a comfortable starting height (must be above your hip), and leave it in the same place for about one second. When this is done, a box, located roughly at your hip will appear on the skelton. Your first beat must fall in this box. The box will turn green when this is successfully completed.";
                checkIfRightHandStill = true;
                drawBox = true;
            }
            else if (InstructionNumber == 4)
            {
                conductingImage.Source = null;
                checkIfRightHandStill = false;
                Instructions.Text = "The next key gesture to learn is the volume gesture.";
                checkIfLeftHandStill = true;
                Instructions.Text += Environment.NewLine + "Similar to the conducting gesture, you must leave your left hand in the same place for about a second. When you do, a green rectangle will appear. Move your hand up and down in this box to increase and decrease the volume. If you want to stop changing the volume, simply move your hand outside of the box. The box will turn red.";
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
                if (checkIfRightHandStill)
                {
                    if (rightHand.X != 0 && rightHip.X != 0)
                    {
                        if (rightHand.Y > rightHip.Y) { aboveHip = true; }
                        if (counter == 15)
                        {
                            if (drawBox)
                            {
                                handValues = SkeletonPointToScreen(rightHand);
                                rightHip.X -= .11f;
                                rightHip.Y += .12f;
                                boxTopLeftValues = SkeletonPointToScreen(rightHip);
                                rightHip.X += .22f;
                                boxBottomRightValues = SkeletonPointToScreen(rightHip);
                                boxBottomRightValues.Y -= (boxBottomRightValues.X - boxTopLeftValues.X);
                                box.Margin = new Thickness(boxTopLeftValues.X, boxTopLeftValues.Y, 0, 0);
                                box.Width = 80;
                                box.Height = 80;
                                box.BorderThickness = new Thickness(4);
                                if (handValues.X > boxTopLeftValues.X && handValues.X < boxBottomRightValues.X && handValues.Y < boxTopLeftValues.Y && handValues.Y > boxBottomRightValues.Y)
                                {
                                    box.BorderBrush = Brushes.LawnGreen;
                                    Instructions.Text = "Good job!" + Environment.NewLine + "You are now free to conduct as you please. Spend some time practicing! Your beats no longer need to land in the box. Below you can see again the picture of the 4/4 conducting gesture.";
                                    var newImage = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\conduct.jpg");
                                    conductingImage.Source = new BitmapImage(newImage);
                                }
                            }
                            //counter = 0;
                        }
                        if (counter < 15 && Math.Abs(prevOne.X - rightHand.X) < .08 && Math.Abs(prevOne.Y - rightHand.Y) < .08 && Math.Abs(prevTwo.X - rightHand.X) < .08 && Math.Abs(prevTwo.Y - rightHand.Y) < .08 && aboveHip == true)
                        {
                            counter++;
                        }
                        else if (counter < 15)
                        {
                            counter = 0;
                        }
                        prevTwo = prevOne;
                        prevOne = rightHand;
                    }
                }
                if (checkIfLeftHandStill)
                {
                    if (leftHand.X != 0 && leftHip.X != 0)
                    {
                        if (leftHand.Y > leftHip.Y) { aboveHip = true; }
                        else { aboveHip = false; }
                        if (counter == 15)
                        {
                            if (drawBox)
                            {
                                handValues = SkeletonPointToScreen(leftHand);
                                leftHand.X -= .1f;
                                curY = leftHand.Y;
                                leftHand.Y = .1f;
                                boxTopLeftValues = SkeletonPointToScreen(leftHand);
                                leftHand.X += .2f;
                                leftHand.Y = curY;
                                boxBottomRightValues = SkeletonPointToScreen(leftHand);
                                hipValues = SkeletonPointToScreen(leftHip);
                                box.Width = 75;
                                if (box.Height == 0) { box.Height =  hipValues.Y + 20; }
                                if (box.Margin.Left == 0 && box.Margin.Right == 0 && box.Margin.Top == 0) { box.Margin = new Thickness(boxTopLeftValues.X, 20, 0, 0); }
                                box.BorderThickness = new Thickness(4);
                                if (handValues.X > box.Margin.Left && handValues.X < box.Margin.Left + 75 && handValues.Y > 20 && handValues.Y < hipValues.Y + 20) { box.BorderBrush = Brushes.Green; }
                                else 
                                { 
                                    box.BorderBrush = Brushes.Red;
                                    counter = 0;
                                }                             
                            }
                        }
                        if (counter < 15 && Math.Abs(prevOne.X - leftHand.X) < .08 && Math.Abs(prevOne.Y - leftHand.Y) < .08 && Math.Abs(prevTwo.X - leftHand.X) < .08 && Math.Abs(prevTwo.Y - leftHand.Y) < .08 && aboveHip == true)
                        {
                            counter++;
                        }
                        else if (counter < 15)
                        {
                            counter = 0;
                        }
                        prevTwo = prevOne;
                        prevOne = leftHand;
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
