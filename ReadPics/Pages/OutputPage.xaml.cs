using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using WindowsPreview.Media.Ocr;
using System.IO;

namespace ReadPics.Pages
{
    public partial class OutputPage : PhoneApplicationPage
    {
        private WriteableBitmap wb;

        private OcrEngine ocrEngine;

        public OutputPage()
        {
            InitializeComponent();
            ocrEngine = new OcrEngine(OcrLanguage.English);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            wb = (WriteableBitmap)PhoneApplicationService.Current.State["wb"];
            this.ResizeImage(ref wb);
            GetTextFromImage();
        }

        private async void GetTextFromImage()
        {
            if (wb.PixelHeight < 40 || wb.PixelHeight > 2600 || wb.PixelWidth < 40 || wb.PixelWidth > 2600)
            {
                OutputBox.Text = "Image Dimensions must be between 40 px and 2600px. It is currently " + wb.PixelWidth.ToString() + "x" + wb.PixelHeight.ToString();
                return;
            }

            var ocrResult = await ocrEngine.RecognizeAsync((uint)wb.PixelHeight, (uint)wb.PixelWidth, this.intArrToByteArr(wb.Pixels));

            if (ocrResult.Lines != null)
            {
                string recognizedText = "";

                foreach (var line in ocrResult.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        recognizedText += word.Text + " ";
                    }

                    recognizedText += Environment.NewLine;
                }

                OutputBox.Text = recognizedText;
            }
            else
            {
                OutputBox.Text = "Could not read anything!";
            }
        }

        private byte[] intArrToByteArr(int[] intArray)
        {
            byte[] result = new byte[intArray.Length * sizeof(int)];
            Buffer.BlockCopy(intArray, 0, result, 0, result.Length);
            return result;
        }

        private void ResizeImage(ref WriteableBitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            int h, w;
            if (bmp.PixelWidth > bmp.PixelHeight)
            {
                double aspRatio = bmp.PixelWidth / (double)bmp.PixelHeight;
                double hh, ww;
                hh = (2400.0 / aspRatio);
                ww = hh * aspRatio;
                h = (int)hh;
                w = (int)ww;
            }
            else
            {
                double aspRatio = bmp.PixelHeight / (double)bmp.PixelWidth;
                double hh, ww;
                hh = (2400.0 / aspRatio);
                ww = hh * aspRatio;
                h = (int)hh;
                w = (int)ww;
            }
            bmp.SaveJpeg(ms, w, h, 0, 100);
            bmp.SetSource(ms);
        }
    }
}