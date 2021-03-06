﻿using System;
using OpenCV_BSC_dll_x64;
using Emgu.CV;
using System.Drawing;
using Emgu.CV.Structure;
using mUserControl_BSC_dll;
using OpenCV_BSC_dll_x64.FeatureDetection;
using mUserControl_BSC_dll.UserControls;

namespace ImageProcessing_BSC_WPF.Modules
{
    /// <summary>
    /// This is used to restrict the target img not going outside the view field
    /// </summary>
    public class FindMinDistance
    {
        static double[] contourArea;
        static Rectangle[] contourRect;
        static Point[] contourPoints;
        static private Image<Bgr, byte> originalImage;

        public static void findMinDistance()
        {
            int threshold = 150;
            originalImage = GV.imgOriginal;
            originalImage.Draw(new System.Drawing.Rectangle() { X = threshold - 4, Y = 0, Width = 2, Height = originalImage.Height }, new Bgr(0, 0, 255), 2);

            int dis = findMinDistance(originalImage);
            if (dis > threshold)
                BindManager.BindMngr.GMessage.value = dis.ToString();
            else if (dis != 0)
                BindManager.BindMngr.GMessage.value = "Hit the wall!";
            GV.imgOriginal = originalImage;
        }


        public static int findMinDistance(Image<Bgr, Byte> imgOriginal)
        {
            ContourDetection.contourDetection(imgOriginal, false, false, "", out contourArea, out contourRect, out contourPoints);

            int minVal = imgOriginal.Width;
            foreach (Point p in contourPoints)
            {
                if (p.X < minVal)
                    minVal = p.X;
            }
            return minVal;
        }
    }
}
