﻿using CameraToImage_dll;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities_BSC_dll;

namespace ImageProcessing_BSC_WPF
{
    public class GF
    {
        public static void updateImgInfo()
        {
            // clear cropping rectangle
            if (ImageCropping.rect != null)
            {
                ImageCropping.rect.Width = 0;
                ImageCropping.rect.Height = 0;
            }
            if (GV.mCamera != null && GV.mCamera.IsConnected)
            {
                switch (GV._camSelected)
                {
                    case camType.WebCam:
                        Image<Bgr, byte> b = GV.mCamera.capture();
                        GV.imgHeight = b.Height;
                        GV.imgWidth = b.Width; break;
                    case camType.PointGreyCam:
                        Image<Bgr, byte> c = GV.mCamera.capture();
                        GV.imgHeight = c.Height;
                        GV.imgWidth = c.Width; break;
                }
            }
            else if (GV._pictureLoaded) // Static picture
            {
                GV.imgHeight = GV.imgOriginal.Height;
                GV.imgWidth = GV.imgOriginal.Width;
            }

            GV._zoomFactor = ImageCropping.zoomFactorCalculator(GV.imgWidth, GV.imgHeight, 4, 3, GV.mMainWindow.ibOriginal);
            GV.mMainWindow.TB_info_camera.Text = "Image size: (" + GV.imgWidth + "," + GV.imgHeight + ") " +
                                  "PictureBox size: (" + GV.mMainWindow.ibOriginal.ActualWidth.ToString("0.#") + "," +
                                  GV.mMainWindow.ibOriginal.ActualHeight.ToString("0.#") + ") " +
                                  "Zoom factor: " + GV._zoomFactor.ToString("0.##");
        }

    }
}
