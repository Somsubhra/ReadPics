using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Devices;
using System.Windows.Media.Imaging;

namespace ReadPics.Pages
{
    public partial class CameraPage : PhoneApplicationPage
    {
        private PhotoCamera photoCamera;
        private WriteableBitmap wb;
        private static BitmapImage bmpImg;

        public CameraPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true)
            {
                photoCamera = new PhotoCamera(CameraType.Primary);

                photoCamera.Initialized += photoCamera_Initialized;
                photoCamera.AutoFocusCompleted += photoCamera_AutoFocusCompleted;
                photoCamera.CaptureImageAvailable += photoCamera_CaptureImageAvailable;
                photoCamera.CaptureCompleted += photoCamera_CaptureCompleted;

                ViewfinderBrush.SetSource(photoCamera);
            }
            else
            {
                // Camera not found
            }
        }

        void photoCamera_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            if (photoCamera.IsFlashModeSupported(FlashMode.Off))
            {
                try
                {
                    photoCamera.FlashMode = FlashMode.Off;
                }
                catch (Exception ex)
                {

                }
            }
        }

        void photoCamera_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate()
            {
                PhoneApplicationService.Current.State["wb"] = wb;
                NavigationService.Navigate(new Uri("/Pages/OutputPage.xaml", UriKind.Relative));
            });
        }

        void photoCamera_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            Dispatcher.BeginInvoke(delegate()
            {
                bmpImg = new BitmapImage();
                bmpImg.CreateOptions = BitmapCreateOptions.None;
                bmpImg.SetSource(e.ImageStream);

                wb = new WriteableBitmap(bmpImg);
            });
        }

        void photoCamera_AutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            photoCamera.CaptureImage();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            photoCamera.Dispose();
        }

        private void CameraTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (photoCamera != null)
            {
                try
                {
                    photoCamera.Focus();
                }
                catch (Exception ex)
                {
                    // Autofocus failed
                }
            }
        }
    }
}