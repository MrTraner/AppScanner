using Android.App;
using Android.Widget;
using Android.OS;

using Android.Support.V7.App;
using Android.Views;
using Android.Gms.Vision.Barcodes;
using Android.Gms.Vision;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Android;
using System;
using Android.Content.PM;
using static Android.Gms.Vision.Detector;
using Android.Util;
using Android.Content;

namespace AppScanner
{
    [Activity(Label = "AppScanner", MainLauncher = true, Theme = "@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity, ISurfaceHolderCallback, IProcessor
    {
        private SurfaceView previewCamara;
        private TextView txtResultado;
        private BarcodeDetector detector;
        private CameraSource camara;
        private const int ID_SolicitudPermisoCamara = 1001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            previewCamara = FindViewById<SurfaceView>(Resource.Id.previewCamara);
            txtResultado = FindViewById<TextView>(Resource.Id.txtResultado);
            detector = new BarcodeDetector.Builder(this).SetBarcodeFormats(BarcodeFormat.QrCode).Build();
            camara = new CameraSource.Builder(this, detector).SetRequestedPreviewSize(1280, 720).Build();

            previewCamara.Holder.AddCallback(this);
            detector.SetProcessor(this);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case ID_SolicitudPermisoCamara:
                    if(grantResults[0] == Permission.Granted)
                    {
                        if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                        {
                            ActivityCompat.RequestPermissions(this, new string[] {
                                Manifest.Permission.Camera
                            }, ID_SolicitudPermisoCamara);
                            return;
                        }
                        try
                        {
                            camara.Start(previewCamara.Holder);
                        }
                        catch (InvalidOperationException)
                        {

                        }
                    }
                break;
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if(ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[] {
                    Manifest.Permission.Camera
                }, ID_SolicitudPermisoCamara);
                return;
            }
            try
            {
                camara.Start(previewCamara.Holder);
            }
            catch (InvalidOperationException)
            {

            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            camara.Stop();
        }

        public void ReceiveDetections(Detections detections)
        {
            SparseArray qrcodes = detections.DetectedItems;
            if(qrcodes.Size() != 0)
            {
                txtResultado.Post(() => {
                    Vibrator vibrador = (Vibrator)GetSystemService(Context.VibratorService);
                    vibrador.Vibrate(1000);
                    txtResultado.Text = ((Barcode)qrcodes.ValueAt(0)).RawValue;
                });
            }
        }

        public void Release()
        {
            throw new NotImplementedException();
        }
    }
}

