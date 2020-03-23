using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android;
using Plugin.Media;
using Android.Graphics;
using System.IO;
using Android.Content;

namespace simple_Camera
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        Button btnDoPhoto, btnSettings, check;
        ImageView ImageViewLastPhoto;
        TextView txtPath;

        readonly string[] permissionGroup =
        {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            btnDoPhoto = (Button)FindViewById(Resource.Id.btnDoPhoto);
            btnSettings = (Button)FindViewById(Resource.Id.btnSettings);
            ImageViewLastPhoto = (ImageView)FindViewById(Resource.Id.ImageViewLastPhoto);
            txtPath = (TextView)FindViewById(Resource.Id.txtPath);
            check = (Button)FindViewById(Resource.Id.check);

            check.Click += checkFunction;
            btnDoPhoto.Click += DoPhoto_Click;
            btnSettings.Click += Settings_Click;
            RequestPermissions(permissionGroup, 0);
        }

        private void checkFunction(object sender, EventArgs e)
        {
            /*Toast.MakeText(this, "SD card path: " + GetBaseFolderPath(true), ToastLength.Long).Show();
            Toast.MakeText(this, "Phone path: " + GetBaseFolderPath(false), ToastLength.Long).Show();*/
            if(Directory.Exists(MainActivity.GetBaseFolderPath(true) + "/CheckThisOutOnSD"))
            {
                Toast.MakeText(this, "WORKED", ToastLength.Long).Show();
            }
        }

        public static string GetBaseFolderPath(bool getSDPath = false)
        {
            string baseFolderPath = "";

            try
            {
                Context context = Application.Context;
                Java.IO.File[] dirs = context.GetExternalFilesDirs(null);

                foreach (Java.IO.File folder in dirs)
                {
                    bool IsRemovable = Android.OS.Environment.InvokeIsExternalStorageRemovable(folder);
                    bool IsEmulated = Android.OS.Environment.InvokeIsExternalStorageEmulated(folder);

                    if (getSDPath ? IsRemovable && !IsEmulated : !IsRemovable && IsEmulated)
                        baseFolderPath = folder.Path;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetBaseFolderPath caused the follwing exception: {0}", ex);
            }

            return baseFolderPath;
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(this, typeof(SettingsActivity)));
        }

        private void DoPhoto_Click(object sender, EventArgs e)
        {
            TakePhoto();
        }

        async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 40,
                Name = "myimage.jpg",
                Directory = "newDirectory",
                SaveToAlbum = true
            });

            if (file == null)
                return;

            //Check the last taken photo
            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            //Toast.MakeText(this, file.ToString(), ToastLength.Long).Show();
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            ImageViewLastPhoto.SetImageBitmap(bitmap);
            txtPath.Text = file.Path;
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}