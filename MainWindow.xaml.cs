﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Collections.Generic;
using WpfApplication4;


namespace WpfTouchFrameSample
{
    public partial class MainWindow : Window
    {
        private int countTouches = 0;
        private int countDistances = 0;
        private double[,] position = new double[8, 2];

        // private ArrayList vectorList = new ArrayList();
        // map with all point objects
        // private Dictionary<int, Point> pointMap = new Dictionary<int, Point>();
        private Dictionary<int, TouchPoint> touchPointMap = new Dictionary<int, TouchPoint>();

        // map with all distances between the points
        private Dictionary<Array, double> distanceMap = new Dictionary<Array, double>();


        public MainWindow()
        {
            InitializeComponent();
        }

        public delegate void UpdateTextCallback(string text);

        TouchPoint _touchPoint;

        void OnTouchDown(object sender, TouchEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            if (element == null) return;

            element.CaptureTouch(e.TouchDevice);
            _touchPoint = e.TouchDevice.GetTouchPoint(this.canvas1);

            // retrieve coordination of single point
            //Point point = new Point(_touchPoint.Position.X, _touchPoint.Position.Y);

            // add point to map
            //pointMap.Add(countTouches, point);
            touchPointMap.Add(countTouches, _touchPoint);

            // only calculate distance, if there is more than 1 touch point
            if (countTouches > 1)
            {
                // get one touch point
                // then calculate distance with the other touch points
                // create maps for touch points and distances
                for (int i = 0; i < countTouches; i++)
                {
                    for (int j = i + 1; j < countTouches; j++)
                    {

                        // declare single-dimensional array for index i, j
                        int[] aIndex = new int[2];

                        // save index i, j as values
                        // later compare with keys of touch point map
                        aIndex[0] = i;
                        aIndex[1] = j;

                        // add distance to map
                        // {array} aIndex as key
                        distanceMap.Add(aIndex, calcDistance(touchPointMap[i], touchPointMap[j]));
                    }
                }
                // here to continue... 

            }
            else
            {
                // only one touch point...
            }

            // count touch point, after calculating distance
            countTouches++;

            Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), countTouches.ToString());

            xy.AppendText(countTouches.ToString() + " X " + _touchPoint.Position.X.ToString() + " Y " + _touchPoint.Position.Y.ToString() + "\n");


            // LOG distances
            Console.WriteLine("OnTouchDown...");

            int counter = 1;
            foreach (var key in distanceMap)
            {
                
                Console.WriteLine("Distance " + counter + ": " + distanceMap[key.Key]);
                counter++;
            }
            e.Handled = true;
        }

        void OnTouchUp(object sender, TouchEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;

            if (el == null) return;

            // get touch point
            TouchPoint touchPoint = e.GetTouchPoint(el);

            // get xy coordinates of touch point
            Point point = new Point(touchPoint.Position.X, touchPoint.Position.Y);

            Rect bounds = new Rect(new Point(0, 0), el.RenderSize);

            // remove touch point from point map
            foreach (var key in touchPointMap)
            {
                if (touchPointMap.ContainsKey(key.Key))
                {
                    if (touchPointMap[key.Key].Equals(touchPoint))
                    {
                        touchPointMap.Remove(key.Key);
                        countTouches--;
                        // remove distance from distance map
                        // {array} aKey array with indexes of touch points
                        foreach (var aKey in distanceMap)
                        {
                            if (distanceMap.ContainsKey(aKey.Key))
                            {
   
                                for (int i = 0; i < aKey.Key.Length; i++)
                                {
                                    if (key.Key == (int)aKey.Key.GetValue(i))
                                    {
                                        distanceMap.Remove(aKey.Key);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (bounds.Contains(touchPoint.Position))
            {
               // countTouches--;
                Nummer.Dispatcher.Invoke(new UpdateTextCallback(this.UpdateText), countTouches.ToString());
            }

            // LOG distances
            Console.WriteLine("OnTouchUp...");

            int counter = 1;
            foreach (var key in distanceMap)
            {
                Console.WriteLine("Distance " + counter + ": " + distanceMap[key.Key]);
                counter++;
            }

            el.ReleaseTouchCapture(e.TouchDevice);

            e.Handled = true;
        }

        private void UpdateText(string message)
        {
            // hier evtl. den calculator aufrufen
            Nummer.Document.Blocks.Clear();
            Nummer.AppendText(message);
            // xy.Document.Blocks.Clear();
        }

        // Function calculates distance between two touch points
        // returns {double} distance
        private double calcDistance(TouchPoint p1, TouchPoint p2)
        {
            countDistances++;
            return Math.Sqrt(Math.Pow(p2.Position.X - p1.Position.X, 2) + Math.Pow(p2.Position.Y - p1.Position.Y, 2));
        }
    }
}