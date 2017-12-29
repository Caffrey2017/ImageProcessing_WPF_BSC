﻿using CNTK;
using Emgu.CV;
using Emgu.CV.Structure;
using mUserControl_BSC_dll;
using mUserControl_BSC_dll.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Utilities_BSC_dll_x64;

namespace ImageProcessing_BSC_WPF.Modules.MachineLearning
{
  
    class ResNet
    {
        public static bool IsModelLoaded = false;

        public static string OutputString;
        public static double OutputProbablility;
        public static List<double> resultList = new List<double>();


        private static Image<Bgr, byte> _ImgOriginal;
        private static DeviceDescriptor device;               //GPU to Use
        private static Function modelFunc;

        private static double timeSpent;
        
        private static BackgroundWorker MLRoutine = new BackgroundWorker();


        public static void MLSetup()
        {
            device = DeviceDescriptor.GPUDevice(0);

            MLRoutine.DoWork += new DoWorkEventHandler(MLRoutine_doWork);
            MLRoutine.ProgressChanged += new ProgressChangedEventHandler(MLRoutine_ProgressChanged);
            MLRoutine.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MLRoutine_WorkerCompleted);
            MLRoutine.WorkerReportsProgress = true;
        }

        public static void startMLRoutine()
        {
            if (!MLRoutine.IsBusy)
            {
                MLRoutine.RunWorkerAsync();
            }
        }

        private static void MLRoutine_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void MLRoutine_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case (int)ErrCode.ModelNotExists:
                    mNotification.Show("Model not exists"); break;
                case (int)ErrCode.LoadingError:
                    mNotification.Show("Cannot load model"); break;
            }
        }

        private static void MLRoutine_doWork(object sender, DoWorkEventArgs e)
        {
            EvaluationSingleImage(GV.imgOriginal);
        }



        /// <summary>
        /// This is a one time process
        /// </summary>
        /// <param name="device"></param>
        public static void LoadModel(string modelFile)
        {
            // Load the model.
            string modelFilePath = Environment.CurrentDirectory + @"\Modules\MachineLearning\TrainedModels\" + modelFile;

            if (!File.Exists(modelFilePath))
            {
                mNotification.Show("Model not exists");
                return;
            }

            try
            {
                modelFunc = Function.Load(modelFilePath, device);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
                IsModelLoaded = false;
            }
            IsModelLoaded = true;
        }

        /// <summary>
        /// The example shows
        /// - how to load model.
        /// - how to prepare input data for a single sample.
        /// - how to prepare input and output data map.
        /// - how to evaluate a model.
        /// - how to retrieve evaluation result and retrieve output data in dense format.
        /// </summary>
        /// <param name="device">Specify on which device to run the evaluation.</param>
        public static void EvaluationSingleImage(Image<Bgr, byte> Img)
        {
            if (Img == null) return;

            DateTime startTime = DateTime.Now;
            if (!IsModelLoaded)
            {
                MLRoutine.ReportProgress((int)ErrCode.LoadingError);
                return;
            }
            _ImgOriginal = Img.Copy();
            try
            {
                // Get input variable. The model has only one single input.
                // The same way described above for output variable can be used here to get input variable by name.
                Variable inputVar = modelFunc.Arguments.Single();

                // Get shape data for the input variable
                NDShape inputShape = inputVar.Shape;
                int imageWidth = inputShape[0];
                int imageHeight = inputShape[1];
                //int imageChannels = inputShape[2];
                //int imageSize = inputShape.TotalSize;

                // The model has only one output.
                // If the model have more than one output, use the following way to get output variable by name.
                // Variable outputVar = modelFunc.Outputs.Where(variable => string.Equals(variable.Name, outputName)).Single();
                Variable outputVar = modelFunc.Output;

                var inputDataMap = new Dictionary<Variable, Value>();
                var outputDataMap = new Dictionary<Variable, Value>();

                Bitmap bmp = _ImgOriginal.CopyBlank().ToBitmap();
                if (_ImgOriginal == null)
                {
                    mNotification.Show("No image");
                    return;
                }
                else
                    bmp = _ImgOriginal.Copy().ToBitmap();

                var resized = bmp.Resize((int)imageWidth, (int)imageHeight, true);
                List<float> resizedCHW = resized.ParallelExtractCHW();

                // Create input data map
                var inputVal = Value.CreateBatch(inputVar.Shape, resizedCHW, device);
                inputDataMap.Add(inputVar, inputVal);

                // Create ouput data map. Using null as Value to indicate using system allocated memory.
                // Alternatively, create a Value object and add it to the data map.
                outputDataMap.Add(outputVar, null);

                // Start evaluation on the device
                modelFunc.Evaluate(inputDataMap, outputDataMap, device);

                // Get evaluate result as dense output
                var outputVal = outputDataMap[outputVar];
                var outputData = outputVal.GetDenseData<float>(outputVar);
                
                timeSpent = (DateTime.Now - startTime).TotalMilliseconds;
                //printOutput(outputVar.Shape.TotalSize, outputData);
                showResult(outputVar.Shape.TotalSize, outputData, OutputProbablility);
            }
            catch (Exception ex)
            {
                //Windows.main.listBox.Items.Add("Error: {0}\nCallStack: {1}\n Inner Exception: {2}");
                throw ex;
            }
        }

        /*
        /// <summary>
        /// Print out the evalaution results.
        /// </summary>
        /// <typeparam name="T">The data value type</typeparam>
        /// <param name="sampleSize">The size of each sample.</param>
        /// <param name="outputBuffer">The evaluation result data.</param>
        internal static void printOutput<T>(int sampleSize, IList<IList<T>> outputBuffer)
        {
            Windows.main.listBox.Items.Add("\nThe number of sequences in the batch: " + outputBuffer.Count);
            int seqNo = 0;
            int outputSampleSize = sampleSize;
            foreach (var seq in outputBuffer)
            {
                if (seq.Count % outputSampleSize != 0)
                {
                    throw new ApplicationException("The number of elements in the sequence is not a multiple of sample size");
                }

                Windows.main.listBox.Items.Add(String.Format("\nSequence {0} contains {1} samples.", seqNo++, seq.Count / outputSampleSize));
                int i = 0;
                int sampleNo = 0;
                foreach (var element in seq)
                {
                    if (i++ % outputSampleSize == 0)
                    {
                        Windows.main.listBox.Items.Add(String.Format("\n    sample {0}: ", sampleNo));
                    }
                    Windows.main.listBox.Items.Add(element + "(" + (CIFAR10)(i-1) + ")");
                    if (i % outputSampleSize == 0)
                    {
                        Windows.main.listBox.Items.Add("." + " (" + timeSpent + " ms)");
                        sampleNo++;
                    }
                    else
                    {
                        Windows.main.listBox.Items.Add(",");
                    }
                }
            }
        }
        */

        private static int predictResult<T>(int sampleSize, IList<IList<T>> outputBuffer, out double probability)
        {
            resultList.Clear();
            probability = 0;
            int outputSampleSize = sampleSize;
            // This is for 1 image
            foreach (var seq in outputBuffer)
            {
                foreach (var element in seq)
                {
                    resultList.Add(Convert.ToDouble(element));
                }
            }
            int maxIndex = resultList.IndexOf(resultList.Max());
            probability = resultList.Max();                          //Get the max probablitity value
            return maxIndex;
        }


        private static void showResult<T>(int sampleSize, IList<IList<T>> outputBuffer, double outputValue)
        {
            int predictedIndex = 0;
            switch (MLCore.MLTrainedDataSetSelectedIndex)
            {
                case 0:
                    predictedIndex = predictResult<T>(sampleSize, outputBuffer, out outputValue);
                    OutputString = DataSet.labelSet[0][predictedIndex];
                    OutputProbablility = outputValue;
                    break;
                case 1:
                    predictedIndex = predictResult<T>(sampleSize, outputBuffer, out outputValue);
                    OutputString = DataSet.labelSet[1][predictedIndex];
                    OutputProbablility = outputValue;
                    break;
                case 2:
                    predictedIndex = predictResult<T>(sampleSize, outputBuffer, out outputValue);
                    OutputString = DataSet.labelSet[2][predictedIndex];
                    OutputProbablility = outputValue;
                    break;
            }
        }


    }
}
