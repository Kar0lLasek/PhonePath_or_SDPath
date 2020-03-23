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
        public static string PHONE_PATH_PICTURES = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString();
        public static string SD_PATH_PICTURES = GetBaseFolderPath(true) + "/Pictures";
        public static string FOLDER_NAME = "PrzegladyZdjecia";

        Button btnDoPhoto, check;

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
            SetContentView(Resource.Layout.activity_main);

            btnDoPhoto = (Button)FindViewById(Resource.Id.btnDoPhoto);
            check = (Button)FindViewById(Resource.Id.check);
            RadioButton radioPhone = FindViewById<RadioButton>(Resource.Id.radioPhone);
            RadioButton radioSD = FindViewById<RadioButton>(Resource.Id.radioSD);

            radioPhone.Click += RadioButtonClick;
            radioSD.Click += RadioButtonClick;

            check.Click += checkFunction;
            btnDoPhoto.Click += DoPhoto_Click;
            RequestPermissions(permissionGroup, 0);
        }

        private void RadioButtonClick(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            //Toast.MakeText(this, rb.Text, ToastLength.Short).Show();
            if(SD_PATH_PICTURES.Equals(null) || SD_PATH_PICTURES.Equals("") || SD_PATH_PICTURES.Equals("/Pictures"))
            {
                Toast.MakeText(this, "You don't have SD card", ToastLength.Long).Show();
                
            } else
            {
                //Toast.MakeText(this, SD_PATH_PICTURES, ToastLength.Long).Show(); 
                if (rb.Text.Equals("SD CARD"))
                {
                    ChangeFolderForPhotos(PHONE_PATH_PICTURES + "/" + FOLDER_NAME, SD_PATH_PICTURES + "/" + FOLDER_NAME);
                }
                else if (rb.Text.Equals("PHONE"))
                {
                    ChangeFolderForPhotos(SD_PATH_PICTURES + "/" + FOLDER_NAME, PHONE_PATH_PICTURES + "/" + FOLDER_NAME);
                }
                else
                {
                    Toast.MakeText(this, "ELSE?", ToastLength.Long).Show();
                }
            }

            
        }

        private void ChangeFolderForPhotos(string sourceDir, string destDir)
        {
            if (Directory.Exists(sourceDir))
            {
                Toast.MakeText(this, "Source Dir Exists...", ToastLength.Short).Show();
                try
                {
                    string[] picList = Directory.GetFiles(sourceDir, "*.jpg");

                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    foreach (string f in picList)
                    {
                        string fName = f.Substring(sourceDir.Length + 1);
                        System.IO.File.Copy(System.IO.Path.Combine(sourceDir, fName), System.IO.Path.Combine(destDir, fName), true);
                    }

                    foreach (string f in picList)
                    {
                        System.IO.File.Delete(f);
                    }

                }
                catch (DirectoryNotFoundException dirNotFound)
                {
                    Toast.MakeText(this, dirNotFound.ToString(), ToastLength.Long).Show();
                }
                Toast.MakeText(this, "DONE", ToastLength.Short).Show();
            }
        }

        private void checkFunction(object sender, EventArgs e)
        {
            Toast.MakeText(this, "SD card path: " + GetBaseFolderPath(true), ToastLength.Long).Show();
            Toast.MakeText(this, "Phone path: " + GetBaseFolderPath(false), ToastLength.Long).Show();
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
                Directory = FOLDER_NAME,
                //It is important to set true to SaveToAlbum!!!
                SaveToAlbum = true
            });

            if (file == null)
                return;
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}