using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Kinect;

namespace Orchestra
{
    public class TempoGesture
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
                return false;
            }

            public int contains(X item)
            {
                for (int index = 0; index < capacity; index++) 
                {
                    if (queueStorage[(firstElementIndex + capacity - index - 1) % capacity].Equals(item))
                    {
                        return index;
                    }
                }
                return -1;
            }
        }

        public Int32 counter;
        public String seeking;
        public float prevYOne;
        public float prevYTwo;
        public float prevXOne;
        public float prevXTwo;
        public float rightHandY;
        public float rightHandX;
        public float threshold;
        public Stopwatch stopwatch;
        public Boolean conducting;
        public CircularQueue<string> recentEvents;
        public float headX;
        public float headY;

        public TempoGesture()
        {
            Dispatch.SkeletonMoved += this.SkeletonMoved;
            stopwatch = new Stopwatch();
            recentEvents = new CircularQueue<string>(20);
            counter = 0;
            seeking = "START";
            threshold = .002F;
        }

        ~TempoGesture()
        {
            Dispatch.SkeletonMoved -= this.SkeletonMoved;
        }

        void SkeletonMoved(Skeleton skel)
        {
            foreach (Joint joint in skel.Joints)
            {
                if (joint.JointType == JointType.HandRight)
                {
                    rightHandY = joint.Position.Y;
                    rightHandX = joint.Position.X;
                    Console.WriteLine(rightHandX + "\t" + rightHandY);
                    //Console.WriteLine(prevYOne);
                }
                // As the starting motion requires the right hand to be close to the 
                // head, grab the head coordinates if conducting has not yet begun.
                if (seeking.Equals("START"))
                {
                    if (joint.JointType == JointType.Head)
                    {
                        headY = joint.Position.Y;
                        headX = joint.Position.X;
                    }
                }
            }
            switch (seeking)
            {
                case "START":
                    {
                        if (true) // FIXME!!!
                        {

                        }
                        break;
                    }
                case "MINIMUM":
                    {
                        if (prevYTwo < (prevYOne - threshold) && prevYOne < (rightHandY - threshold))
                        {
                            //Dispatch.TriggerBeat(counter % 4 + 1);
                            seeking = "MAXIMUM";
                            counter++;
                            break;
                        }
                        break;
                    }
                case "MAXIMUM":
                    {
                        if (prevYTwo > (prevYOne + threshold) && prevYOne > (rightHandY + threshold))
                        {
                            //Console.WriteLine((counter % 4) + 1);
                            seeking = "MINIMUM";
                            break;
                        }
                        break;
                    }


            }
            prevYTwo = prevYOne;
            prevXTwo = prevXOne;
            prevYOne = rightHandY;
            prevXOne = rightHandX;
        }
    }
}