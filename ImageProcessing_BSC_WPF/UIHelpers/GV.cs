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
using System.Media;

namespace ImageProcessing_BSC_WPF
{
    public enum ErrorCode
    {
        Normal,
        No_picture_found,
        No_object_image,
        SearchSURF_Fail,
        SearchFFT_Fail,
        SearchColor_Fail
    }

    public enum DecoderEngine
    {
        Zxing,
        Cortex
    }

    public class Windows
    {
        public static MainWindow main = null;
    }

    public class GV
    {
        public static SoundPlayer CaptureSound = new SoundPlayer(System.Environment.CurrentDirectory + @"\Resources\camera-shutter-click-03.wav");

        public static DecoderEngine mDecoderEngine = DecoderEngine.Zxing;

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
        public static bool _MLSwitch;                           //turn on machine learning.
        public static bool _motionDetectSwitch;                 //turn on motion detection.

        // Color detection, multi points select
        public static int _remainColorPoints = 3;               //total interest points in color detection
        public static int _colorRegionSize = 10;                //pixels

        public static bool maxmized = false;
        public static Image<Bgr, byte> imgOriginal;
        public static Image<Bgr, byte> imgOriginal_pure;
        public static Image<Bgr, byte> imgOriginal_save;
        public static Image<Bgr, byte> imgProcessed;
        public static Image<Bgr, byte> OCROutputImg;
        public static Image<Bgr, byte> object_img = null;

        public static Graphics mGraphics;

        public static CameraConnection mCamera = new CameraConnection();
        public static CameraToImage_dll_x64.Windows.Conversion mConvert;

        public static Setting mSetting = new Setting();  //This will load the newest setting
    }
}
