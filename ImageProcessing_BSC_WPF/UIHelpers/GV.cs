﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCV_BSC_dll_x64;
using System.Drawing;
using Utilities_BSC_dll_x64;
using ZXing;
using Emgu.CV;
using Emgu.CV.Structure;
using OpenCV_BSC_dll_x64.Windows;
using CameraToImage_dll_x64;

namespace ImageProcessing_BSC_WPF
{
    public enum ErrorCode
    {
        Normal,
        No_picture_found,
        No_object_image,
        SearchFFT_Fail,
        SearchColor_Fail
    }

    public class Windows
    {
        public static MainWindow main = null;
    }

    public class GV
    {
        public static int _pictureBoxWidthRatio = 4;
        public static int _pictureBoxHeightRatio = 3;

        public static bool _camConnectAtStartup;
        public static camType _camSelected;

        public static bool _cameraConnected = false;
        public static bool _pictureLoaded = false;

        public static int imgWidth = 0, imgHeight = 0;
        public static double _zoomFactor = 0;
        public static ErrorCode _err;

        public static bool _findMinSwitch;
        public static bool _findCenterSwitch;
        public static bool _decodeSwitch;                       //turn on code decoding.
        public static bool _OCRSwitch;                          //turn on OCR decoding.
        public static bool _MLSwitch;                          //turn on machine learning.

        public static bool maxmized = false;
        public static Image<Bgr, byte> imgOriginal;
        public static Image<Bgr, byte> imgOriginal_save;
        public static Image<Bgr, byte> imgProcessed;
        public static Image<Bgr, byte> OCROutputImg;
        public static Image<Bgr, byte> object_img = null;

        public static Graphics mGraphics;

        public static CameraConnection mCamera = null;
        public static CameraToImage_dll_x64.Windows.Conversion mConvert;

        public static Setting mSetting = new Setting();  //This will load the newest setting
    }
}