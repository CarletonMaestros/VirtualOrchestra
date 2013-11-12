using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Orchestra
{
    public class TempoGestureII
    {
        public class CircularQueue<X>
        {
            private X[] queueStorage;
            private int firstElementIndex;
            private int capacity;
            private int numElements;

            public CircularQueue(int size)
            {
                queueStorage = new X[size];
                capacity = size;
                firstElementIndex = 0;
            }

            public void enqueue(X element)
            {
                if (numElements < capacity)
                {
                    queueStorage[firstElementIndex + numElements] = element;
                    numElements++;
                }
                else if (numElements == capacity)
                {
                    queueStorage[firstElementIndex] = element;
                    firstElementIndex = (firstElementIndex + 1) % capacity;
                }
            }

            public X get(int index)
            {
                return queueStorage[(firstElementIndex + capacity - index - 1) % capacity];
            }


            public Boolean isFull()
            {
                if (numElements == capacity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int seeking;
        public float X;
        public float Y;
        public float headX;
        public float headY;
        public CircularQueue<float> recentX;
        public CircularQueue<float> recentY;
        public float changeInX;
        public float changeInY;
        public float combinedChange;
        public float changeInXSmoothed;
        public float changeInYSmoothed;
        public float combinedChangeSmoothed;
        public float threshold;
        public float weight;
        public int numXSaved;
        public int numYSaved;
        Boolean conducting;
        int counter;

        public TempoGestureII()
        {
            Dispatch.SkeletonMoved += this.SkeletonMoved;
            numXSaved = 2;
            numYSaved = 2;
            weight = 2;
            recentX = new CircularQueue<float>(numXSaved);
            recentY = new CircularQueue<float>(numYSaved);
            conducting = true;
            counter = 0;
        }

        ~TempoGestureII()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }

        private void SkeletonMoved(float time, Skeleton skel)
        {
            foreach (Joint joint in skel.Joints)
            {
                // Grab the hand coordinates.
                if (joint.JointType == JointType.HandRight)
                {
                    Y = joint.Position.Y;
                    /*if (Y < .6) 
                    {
                        Console.WriteLine(Y);
                    }*/
                    X = joint.Position.X;
                }
                // As the starting motion requires the right hand to be close to the 
                // head, grab the head coordinates if conducting has not yet begun.
                if (!conducting)
                {
                    if (joint.JointType == JointType.Head)
                    {
                        headY = joint.Position.Y;
                        headX = joint.Position.X;
                    }
                }

            }
            // When there are atleast max(numXSaved, numYSaved) frames 
            // of data, begin looking for gestures.  
            if (recentX.isFull() && recentY.isFull())
            {
                changeInX = X - recentX.get(0);
                changeInY = Y - recentY.get(0);
                combinedChange = (float)Math.Sqrt(Math.Pow(changeInX, 2) + Math.Pow(changeInY, 2));
                changeInXSmoothed = (changeInXSmoothed + weight * changeInX) / (weight + 1); 
                changeInYSmoothed = (changeInYSmoothed + weight * changeInY) / (weight + 1);
                combinedChangeSmoothed = (float)Math.Sqrt(Math.Pow(changeInXSmoothed, 2) + Math.Pow(changeInYSmoothed, 2));
                Console.WriteLine(Y + "\t" + changeInY + "\t" + changeInYSmoothed);
                /*if (conducting)
                {
                    //Console.WriteLine(recentY.get(4) + "\t" + recentY.get(3) + "\t" + recentY.get(2) + "\t" + recentY.get(1) + "\t" + recentY.get(0));
                    if (recentY.diff(numYSaved - 1, 1) > .09 && recentY.diff(1, 2) < 0 && recentY.diff(1, 2) > -.05 && recentY.diff(0, 1) < 0 && recentY.diff(0, 1) > -.04 && Math.Abs(Y - recentY.get(0)) < .01)
                    {
                        //Console.WriteLine(0);
                        //Dispatch.TriggerBeat(1);
                        if (counter % 4 == 0)
                        {
                            Console.WriteLine("\n>>>");
                        }
                        if (counter % 4 == 1)
                        {
                            Console.WriteLine("\n>>>>>>>>>>");
                        }
                        if (counter % 4 == 2)
                        {
                            Console.WriteLine("\n>>>>>>>>>>>>>>>>>");
                        }
                        if (counter % 4 == 3)
                        {
                            Console.WriteLine("\n>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                        }
                        counter++;
                    }
                } */
            }
            recentX.enqueue(X);
            recentY.enqueue(Y);
            
        }
    }
}