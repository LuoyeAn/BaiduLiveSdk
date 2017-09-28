using System;
using UIKit;
using AVFoundation;
using System.Threading.Tasks;
using CoreGraphics;
using BaiduLiveApp.iOSBinding;

namespace Sample.iOS
{
    public partial class ViewController : UIViewController
    {

        VCSimpleSession session;

        UILabel stateLabel;
        UIButton closeButton;
        UIButton beautyButton;
        UIButton cameraButton;
        UIButton startLiveButton;

        public ViewController(IntPtr handle) : base(handle)
        {

            stateLabel = new UILabel(new CGRect(20, 20, 120, 40))
            {
                Text = "Not Recording",
                TextColor = UIColor.White,
                Font = UIFont.SystemFontOfSize(14)
            };

            closeButton = new UIButton(new CGRect(UIScreen.MainScreen.Bounds.Width - 10 - 44, 20, 50, 44));
            closeButton.SetTitle("Close", UIControlState.Normal);

            cameraButton = new UIButton(new CGRect(UIScreen.MainScreen.Bounds.Width - 54 * 2, 20, 50, 44));
            cameraButton.SetTitle("Camera Preview", UIControlState.Normal);

            beautyButton = new UIButton(new CGRect(UIScreen.MainScreen.Bounds.Width - 54 * 3, 20, 50, 44));
            beautyButton.SetTitle("Camera Beauty Open", UIControlState.Selected);
            beautyButton.SetTitle("Camera Beauty Closed", UIControlState.Normal);

            startLiveButton = new UIButton(new CGRect(30, UIScreen.MainScreen.Bounds.Height - 50, UIScreen.MainScreen.Bounds.Width - 10 - 44, 44));
            startLiveButton.Layer.CornerRadius = 22;
            startLiveButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            startLiveButton.SetTitle("Start Live", UIControlState.Normal);
            startLiveButton.TitleLabel.Font = UIFont.SystemFontOfSize(14);
            startLiveButton.BackgroundColor = new UIColor(50, 32, 245, 1);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            VCSimpleSessionConfiguration config = new VCSimpleSessionConfiguration
            {
                CameraOrientation = AVCaptureVideoOrientation.Portrait,
                VideoSize = UIScreen.MainScreen.Bounds.Size,
                Bitrate = 1200 * 1000,
                CameraDevice = VCCameraState.Front,
                ContinuousAutofocus = true,
                ContinuousExposure = false,
                Fps = 20
            };
            session = new VCSimpleSession(config)
            {
                AspectMode = VCAspectMode.Fill,
                Delegate = new CameraDelegate(startLiveButton)
            };
            
            View.BackgroundColor = UIColor.Clear;

            this.View.AddSubview(stateLabel);
            this.View.AddSubview(closeButton);
            this.View.AddSubview(beautyButton);
            this.View.AddSubview(cameraButton);
            this.View.AddSubview(startLiveButton);

            this.View.InsertSubview(session.PreviewView, 0);

            cameraButton.TouchUpInside += DidTappedCameraButton;
            beautyButton.TouchUpInside += DidTappedBeautyButton;
            startLiveButton.TouchUpInside += DidTappedStartLiveButton;
		}

        public override void ViewDidLayoutSubviews()
        {
            session.PreviewView.Frame = View.Bounds;
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

			cameraButton.TouchUpInside -= DidTappedCameraButton;
			beautyButton.TouchUpInside -= DidTappedBeautyButton;
			startLiveButton.TouchUpInside += DidTappedStartLiveButton;
        }

        void LiveSessionChanged(string stateChangeDescription, bool turnLiveOff)
        {
            stateLabel.Text = stateChangeDescription;

            if (turnLiveOff)
                ToggleLiveStream(false);
        }

        void DidTappedCameraButton(object sender, EventArgs e)
        {
            // TODO
        }

        void DidTappedBeautyButton(object sender, EventArgs e)
        {
			//session.BeautyLevel = !session.BeautyFace;
			//beautyButton.Selected = !session.BeautyFace;
        }

        void DidTappedStartLiveButton(object sender, EventArgs e)
        {
            startLiveButton.Selected = !startLiveButton.Selected;

            if (startLiveButton.Selected)
            {
                ToggleLiveStream(true);
			}
            else
            {
                ToggleLiveStream(false);  
            }
        }

        void ToggleLiveStream(bool startLive)
        {
            if (startLive)
            {
                startLiveButton.SetTitle("End Live", UIControlState.Normal); 
                session.StartRtmpSessionWithURL("rtmp://channel1-testr.channel.mediaservices.windows.net:1935/live/2463e36343b14b6ab48a7bcb40f0333e", "key");
            }
            else
            {
				startLiveButton.SetTitle("Start Live", UIControlState.Normal);
                session.EndRtmpSession();
            }
        }
    }

    public class CameraDelegate : VCSessionDelegate
    {
        private UIButton _button;
        public CameraDelegate(UIButton button)
        {
            _button = button;
        }
        public override void ConnectionStatusChanged(VCSessionState sessionState)
        {
            switch (sessionState)
            {
                case VCSessionState.PreviewStarted:
                    Console.WriteLine("Preview Started");
                    _button.SetTitle("Preview Started", UIControlState.Normal);
                    break;
                case VCSessionState.Starting:
                    Console.WriteLine("Starting");
                    _button.SetTitle("Staring", UIControlState.Normal);
                    break;
                case VCSessionState.Started:
                    Console.WriteLine("Started");
                    _button.SetTitle("Started", UIControlState.Normal);
                    break;
                case VCSessionState.Error:
                    Console.WriteLine("Error");
                    _button.SetTitle("Error", UIControlState.Normal);
                    break;
                case VCSessionState.Ended:
                    Console.WriteLine("Ended");
                    _button.SetTitle("End", UIControlState.Normal);
                    break;
                default:
                    break;
            };
        }
    }
}
