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
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;
        private int nextInstruction = 1;
        private bool showStartBox = false;
        private SkeletonPoint rightHand;
        private SkeletonPoint rightHip;
        private SkeletonPoint leftHand;
        private SkeletonPoint leftHip; 
        private SkeletonPoint rightShoulder;
        private SkeletonPoint neck;
        private SkeletonPoint core;
        private SkeletonPoint spine;
        private Point rightHandValues;
        private Point leftHandValues;
        private Point rightHipValues;
        private Point leftHipValues;
        private Point rightShoulderValues;
        private Point neckValues;
        private Point spineValues;
        private Point coreValues;
        private bool checkRightHandStill;
        private bool checkLeftHandStill;
        private int counterRight;
        private int counterLeft;
        private SkeletonPoint prevOneRight;
        private SkeletonPoint prevTwoRight;
        private SkeletonPoint prevOneLeft;
        private SkeletonPoint prevTwoLeft;
        private bool aboveHip;
        Stopwatch stopwatch = new Stopwatch();
        private bool boxDisappear = false;
        private int belowHipCounter;
        private bool showCheckMark = true;
        private bool printText;
        private bool checkBothHandsStill;
        private bool leftHandStill = false;
        private bool rightHandStill = false;
        private bool drawHipLine;
        private Rect box1 = new Rect();
        private Rect box2 = new Rect();
        private Rect box3 = new Rect();
        private bool setLocation;
        private bool goBack;
        private bool tempoBox;
        private bool musicStopped;
        private bool playSong;

        public TutorialWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            InitDrawing();
            Gestures.Load();
            Dispatch.SkeletonMoved += SkeletonMoved;
            Dispatch.TriggerLock(false);
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e) 
        {
            WriteInstructions(nextInstruction);
            nextInstruction++;           
        }

        private void GoBackStepButton_Click(object sender, RoutedEventArgs e)
        {
            if (nextInstruction - 1 == 0) { return; }
            if (nextInstruction - 1 <= 3)
            {
                drawHipLine = false;
            }
            if (checkMarkImage.Source != null || goBack) { nextInstruction++; }
            if (nextInstruction - 1 == 2 || nextInstruction - 1 == 8) { nextInstruction--; }
            if (nextInstruction - 1 == 10 || nextInstruction - 1 == 9) { checkBothHandsStill = false; }
            if (nextInstruction - 1 == 1 || nextInstruction - 1 == 2) { Instructions.Text = "Welcome to the Virtual Orchestra tutorial! You should be able to see your kinect skeleton in the frame to the left."; }
            //else if (nextInstruction - 1 == 9) { Instructions.Text = "The final gesture to learn is the one that stops the music."; }
            else { Instructions.Text = ""; }
            checkRightHandStill = false;
            checkLeftHandStill = false;
            musicStopped = false;
            leftHandStill = false;
            rightHandStill = false;
            conductingImage4.Source = null;
            conductingImage1.Source = null;
            checkMarkImage.Source = null;
            showStartBox = false;
            showCheckMark = false;
            setLocation = true;
            belowHipCounter = 0;
            counterRight = 0;
            counterLeft = 0;
            goBack = false;
            boxDisappear = false;
            nextInstruction--;
            WriteInstructions(nextInstruction - 1);
        }

        private void WriteInstructions(int InstructionNumber)
        {
            if (InstructionNumber == 1)
            {
                Instructions.Text += Environment.NewLine + "If you ever want to go back a step, simply press the 'Go Back A Step' button below.";
                Instructions.Text += Environment.NewLine + "The first gesture we will go over is the basic conducting gesture.";
            }
            else if (InstructionNumber == 2)
            {
                Instructions.Text = "You can choose to either bounce your hand up and down:";
                var newImage = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\conduct2.jpg");
                conductingImage1.Source = new BitmapImage(newImage);
                Instructions.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Or conduct with a standard 4/4 conducting gesture";
                var newImage2 = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\conduct.jpg");
                conductingImage4.Source = new BitmapImage(newImage2);
            }
            else if (InstructionNumber == 3)
            {
                conductingImage4.Source = null;
                conductingImage1.Source = null;
                Instructions.Text = "You'll notice a line going across your screen at the level of your hips.";
                Instructions.Text += Environment.NewLine + "For some gestures, your hand must be above this line. Try moving your left and right hands above and below this line, and notice what happens.";
                drawHipLine = true;
            }
            else if (InstructionNumber == 4 && rightHandValues.Y < rightHipValues.Y)
            {
                Instructions.Text = "Put your hand below your hip before moving on to the next step.";
                nextInstruction--;
            }
            else if (InstructionNumber == 4)
            {
                conductingImage4.Source = null;
                conductingImage1.Source = null;
                Instructions.Text = "To begin the tempo gesture, put your right hand in the gray box, and leave it in the same place for about one second.";
                showStartBox = true;
                checkRightHandStill = true;
                showCheckMark = true;
            }
            else if (InstructionNumber == 5)
            {
                tempoBox = false;
                showStartBox = false;
                showCheckMark = false;
                checkRightHandStill = false;
                checkMarkImage.Source = null;
                Instructions.Text = "TAKEAWAYS: " + Environment.NewLine + "In order to start a song..." + Environment.NewLine + "  1. Your hand must be above your hip." + Environment.NewLine + "  2. Your hand must stay in the same place for roughly a second" + Environment.NewLine + "  3. Your first beat must be roughly at your hip" + Environment.NewLine + Environment.NewLine + "Remember: tempo is a right-hand gesture";
            }
            else if (InstructionNumber == 6)
            {
                conductingImage1.Source = null;
                conductingImage4.Source = null;
                Instructions.Text = "The next key gesture to learn is the volume gesture.";
            }
            else if (InstructionNumber == 7)
            {
                printText = true;
                checkLeftHandStill = true;
                setLocation = true;
                Instructions.Text += Environment.NewLine + "Similar to the conducting gesture, you must leave your left hand in the same place for about a second.";
                Instructions.Text += Environment.NewLine + "Make sure your hand is above your hip.";
            }
            else if (InstructionNumber == 8)
            {
                checkLeftHandStill = false;
                Instructions.Text = "TAKEAWAYS: " + Environment.NewLine + "In order to change the volume of a song..." + Environment.NewLine + "  1. Your left hand must be above your hip." + Environment.NewLine + "  2. Your hand must stay in the same place for roughly a second" + Environment.NewLine + "  3. Your hand must stay in the same X-location as you move it up and down." + Environment.NewLine + "  4. If you want to leave the volume at a certain level, move your hand to the left or right." + Environment.NewLine + Environment.NewLine + "Remember: changing the volume is a left-hand gesture";
            }
            else if (InstructionNumber == 9)
            {
                Instructions.Text = "The final gesture to learn is the one that stops the music.";
            }
            else if (InstructionNumber == 10)
            {
                counterLeft = 0;
                counterRight = 0;
                Instructions.Text += Environment.NewLine + "As with the previous gestures, your hands must be in the same place for about a second." + Environment.NewLine + "Place both of your hands somewhere in the gray box.";
                checkBothHandsStill = true;
                checkLeftHandStill = true;
                checkRightHandStill = true;
            }
            if (InstructionNumber == 11)
            {
                musicStopped = false;
                checkBothHandsStill = false;
                counterRight = 0;
                counterLeft = 0;
                Instructions.Text = "Now practice on a song.";
                ContinueButton.Content = "Quit Tutorial";
                checkRightHandStill = true;
                playSong = true;
                showStartBox = true;
                boxDisappear = false;
            }
            if (InstructionNumber == 12) 
            {
                App.tutorial = null;
                App.ShowStartScreen();
            }
        }

        private bool Contains(Point hand, Rect boxName)
        {
            if (hand.X > boxName.Left && hand.X < boxName.Right && hand.Y > boxName.Top && hand.Y < boxName.Bottom) { return true; }
            else { return false; }
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
            App.tutorial = null;
        }

        /// <summary>
        /// Event handler for Kinect sensor's SkeletonFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void SkeletonMoved(float time, Skeleton skel)
        {
            using (DrawingContext dc = this.drawingGroup.Open())
            {
                // Draw a transparent background to set the render size
                dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, RenderWidth, RenderHeight));
                RenderClippedEdges(skel, dc);

                //rightHand = skel.Joints[JointType.HandRight];
                foreach (Joint joint in skel.Joints)
                {
                    rightHand = skel.Joints[JointType.HandRight].Position;
                    leftHand = skel.Joints[JointType.HandLeft].Position;
                    rightHip = skel.Joints[JointType.HipRight].Position;
                    leftHip = skel.Joints[JointType.HipLeft].Position;
                    rightShoulder = skel.Joints[JointType.ShoulderRight].Position;
                    neck = skel.Joints[JointType.ShoulderCenter].Position;
                    spine = skel.Joints[JointType.Spine].Position;
                    core = skel.Joints[JointType.HipCenter].Position;
                }
                if (rightHand.X != 0 && rightHip.X != 0)
                {
                    rightHipValues = SkeletonPointToScreen(rightHip);
                    leftHipValues = SkeletonPointToScreen(leftHip);
                    rightHandValues = SkeletonPointToScreen(rightHand);
                    leftHandValues = SkeletonPointToScreen(leftHand);
                    rightShoulderValues = SkeletonPointToScreen(rightShoulder);
                    neckValues = SkeletonPointToScreen(neck);
                    spineValues = SkeletonPointToScreen(spine);
                    coreValues = SkeletonPointToScreen(core);
                }
                if (drawHipLine)
                {
                    if (rightHandValues.Y < rightHipValues.Y) { dc.DrawLine(new Pen(Brushes.Green, 4), new Point((leftHipValues.X + rightHipValues.X) / 2, rightHipValues.Y), new Point(1000, rightHipValues.Y)); }
                    else { dc.DrawLine(new Pen(Brushes.Red, 4), new Point((leftHipValues.X + rightHipValues.X) / 2, rightHipValues.Y), new Point(1000, rightHipValues.Y)); }
                    if (leftHandValues.Y < rightHipValues.Y) { dc.DrawLine(new Pen(Brushes.Green, 4), new Point(0, rightHipValues.Y), new Point((leftHipValues.X + rightHipValues.X) / 2, rightHipValues.Y)); }
                    else { dc.DrawLine(new Pen(Brushes.Red, 4), new Point(0, rightHipValues.Y), new Point((leftHipValues.X + rightHipValues.X) / 2, rightHipValues.Y)); }
                }
                if (showStartBox)
                {
                    box2.Location = new Point(neckValues.X, 10);
                    box2.Size = new Size(RenderWidth - neckValues.X - 10, neckValues.Y + Math.Abs(neckValues.Y - rightShoulderValues.Y) * 1.5 );
                    dc.DrawRectangle(null, new Pen(Brushes.Gray, 4), box2);                  
                }
                if (tempoBox)
                {
                    box2.Size = new Size(RenderWidth - 20, RenderHeight - 20);
                    box2.Location = new Point(10, 10);
                    if (Gestures.tempo.beat == true || stopwatch.ElapsedMilliseconds <= 200)
                    {
                        dc.DrawRectangle(null, new Pen(Brushes.Green, 4), box2);
                        if (stopwatch.ElapsedMilliseconds > 200 || stopwatch.ElapsedMilliseconds == 0) { stopwatch.Restart(); }
                    }
                    else { dc.DrawRectangle(null, new Pen(Brushes.Red, 4), box2); }
                }
                if (checkRightHandStill)
                {
                    if (rightHand.X != 0 && rightHip.X != 0)
                    {
                        if (rightHand.Y > rightHip.Y) { aboveHip = true; }
                        else { aboveHip = false; belowHipCounter++; }
                        if (counterRight == 15)
                        {
                            if (checkBothHandsStill) { rightHandStill = true; }
                            else
                            {
                                box1.Location = new Point((spineValues.X + leftHipValues.X) / 2, (spineValues.Y + coreValues.Y) / 2);
                                box1.Size = new Size(Math.Abs(spineValues.Y - rightHipValues.Y) * 1.5, Math.Abs(spineValues.Y - rightHipValues.Y) * 1.5);
                                if (!boxDisappear) { dc.DrawRectangle(null, new Pen(Brushes.Red, 4), box1); }
                                if (showCheckMark && !playSong)
                                {
                                    if (Contains(rightHandValues, box2))
                                    {
                                        Instructions.Text = "Good! See the red box? Your beats must fall in this box. Make a down and up motion roughly like this:";
                                        var newImage3 = new Uri(@"C:\Users\admin\Desktop\VirtualOrchestra\GUI\capture.jpg");
                                        checkMarkImage.Source = new BitmapImage(newImage3);
                                        Instructions.Text += Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "The box will turn green when this is successfully completed.";
                                    }
                                }
                                if (Contains(rightHandValues, box1))
                                {
                                    dc.DrawRectangle(null, new Pen(Brushes.Green, 4), box1);
                                    showStartBox = false;
                                    checkMarkImage.Source = null;
                                    showCheckMark = false;
                                    if (!playSong) { Instructions.Text = "Well done!" + Environment.NewLine + "Now spend a little time practicing!"; }
                                    else
                                    {
                                        checkLeftHandStill = true;
                                        checkBothHandsStill = true;
                                        counterRight = 0;
                                    }
                                    goBack = true;
                                    boxDisappear = true;
                                    tempoBox = true;
                                    checkRightHandStill = false;
                                    stopwatch.Start();
                                }
                            }
                        }
                        if (counterRight < 15 && Math.Abs(prevOneRight.X - rightHand.X) < .08 && Math.Abs(prevOneRight.Y - rightHand.Y) < .08 && Math.Abs(prevTwoRight.X - rightHand.X) < .08 && Math.Abs(prevTwoRight.Y - rightHand.Y) < .08 && aboveHip == true)
                        {
                            counterRight++;
                        }
                        else if (counterRight < 15)
                        {
                            counterRight = 0;
                        }
                    }
                }
                if (checkLeftHandStill)
                {
                    if (leftHand.X != 0 && leftHip.X != 0)
                    {
                        if (leftHand.Y > leftHip.Y) { aboveHip = true; }
                        else { aboveHip = false; belowHipCounter++; }
                        if (belowHipCounter > 1)
                        {
                            if (counterLeft == 15)
                            {
                                if (checkBothHandsStill) { leftHandStill = true; }
                                if (!checkBothHandsStill || playSong)
                                {
                                    if (setLocation) { box1.Location = new Point(leftHandValues.X - Math.Abs(leftHipValues.X - rightHipValues.X) * .875,  10); setLocation = false; }
                                    box1.Size = new Size(Math.Abs(leftHipValues.X - rightHipValues.X) * 1.25, rightHipValues.Y);
                                    if (Contains(leftHandValues, box1) && boxDisappear == false)
                                    {
                                        dc.DrawRectangle(null, new Pen(Brushes.Green, 4), box1); 
                                        if (printText && !playSong) { Instructions.Text += Environment.NewLine + "See the green box? Move your hand up and down in this box to increase and decrease the volume."; }
                                        printText = false;
                                    }
                                    else
                                    {
                                        dc.DrawRectangle(null, new Pen(Brushes.Red, 4), box1);
                                        boxDisappear = true;
                                        stopwatch.Start();
                                    }
                                }
                            }
                            if (counterLeft < 15 && Math.Abs(prevOneLeft.X - leftHand.X) < .08 && Math.Abs(prevOneLeft.Y - leftHand.Y) < .08 && Math.Abs(prevTwoLeft.X - leftHand.X) < .08 && Math.Abs(prevTwoLeft.Y - leftHand.Y) < .08 && aboveHip == true)
                            {
                                counterLeft++;
                            }
                            else if (counterLeft < 15)
                            {
                                counterLeft = 0;
                            }
                        }
                    }
                }
                if (checkBothHandsStill)
                {
                    double yValue = neckValues.Y + Math.Abs(rightShoulderValues.Y - neckValues.Y) / 2;
                    box1.Size = new Size(Math.Abs(neckValues.Y - coreValues.Y) * 1.5, yValue - 10);
                    box1.Location = new Point(neckValues.X - Math.Abs(neckValues.Y - coreValues.Y) * .75, 10);
                    if (leftHandStill && rightHandStill && rightHandValues.Y < yValue && leftHandValues.Y < yValue) 
                    { 
                        dc.DrawRectangle(null, new Pen(Brushes.Green, 4), box1);
                    }
                    if (leftHandStill && rightHandStill)
                    {
                        if (Instructions.Text == "You stopped the music! Congratulations!" && musicStopped != true && !playSong) { Instructions.Text = "Awesome! Now, move your hands diagonally downward, into the red boxes."; }
                        box2.Size = new Size(Math.Abs(rightHipValues.X - leftHipValues.X) * 1.25, Math.Abs(rightHipValues.X - leftHipValues.X) * 1.25);
                        box2.Location = new Point(coreValues.X - Math.Abs(neckValues.Y - coreValues.Y) - box2.Size.Width, spineValues.Y);
                        box3.Size = box2.Size;
                        box3.Location = new Point(coreValues.X + Math.Abs(neckValues.Y - coreValues.Y), box2.Location.Y);
                        if (Math.Abs(rightHand.X - prevOneRight.X) < .1 && Math.Abs(leftHand.X - prevOneLeft.X) < .1 && Math.Abs(rightHand.Y - prevOneRight.Y) < .1 && Math.Abs(leftHand.Y - prevOneLeft.Y) < .1 && Contains(rightHandValues, box3) && Contains(leftHandValues, box2))
                        {
                            dc.DrawRectangle(null, new Pen(Brushes.Green, 4), box2);
                            dc.DrawRectangle(null, new Pen(Brushes.Green, 4), box3);
                            if (!playSong) { Instructions.Text = "You stopped the music! Congratulations!"; }
                            musicStopped = true;
                            boxDisappear = true;
                        }
                        else if (boxDisappear == false)
                        {
                            dc.DrawRectangle(null, new Pen(Brushes.Red, 4), box2);
                            dc.DrawRectangle(null, new Pen(Brushes.Red, 4), box3);
                        }
                    }
                    else { dc.DrawRectangle(null, new Pen(Brushes.Gray, 4), box1); }
                }
                if (boxDisappear == true && stopwatch.ElapsedMilliseconds > 800)
                {
                    counterLeft = 0;
                    counterRight = 0;
                    checkRightHandStill = false;
                    boxDisappear = false;
                    stopwatch.Reset();
                    belowHipCounter = 0;
                    showStartBox = false;
                    setLocation = true;
                    if (checkLeftHandStill == true && !playSong)
                    {
                        Instructions.Text = "Your hand was no longer inside the box of allowed values, which is why it disappeared." + Environment.NewLine + "To resume changing the volume, put your left hand below your hip. This resets the volume. You can then put your left hand back above your hip and leave it in the same place for about a second. You will see the green box again.";
                    }
                }

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
            prevTwoRight = prevOneRight;
            prevOneRight = rightHand;
            prevTwoLeft = prevOneLeft;
            prevOneLeft = leftHand;

            // Prevent drawing outside of our render area
            this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, RenderWidth, RenderHeight));
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
            DepthImagePoint depthPoint = Kinect.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
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

            Point j0 = this.SkeletonPointToScreen(joint0.Position);
            Point j1 = this.SkeletonPointToScreen(joint1.Position);
            drawingContext.DrawLine(drawPen, this.SkeletonPointToScreen(joint0.Position), this.SkeletonPointToScreen(joint1.Position));
        }
    }
}
