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
        public static string PHONE_PATH_PICTURES = GetBaseFolderPath(false) + "/Pictures";
        public static string SD_PATH_PICTURES = GetBaseFolderPath(true) + "/Pictures";
        public static string FOLDER_NAME = "PrzegladyZdjecia";

        Button btnDoPhoto, btnSettings, check;
        //ImageView ImageViewLastPhoto;
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
            SetContentView(Resource.Layout.activity_main);

            btnDoPhoto = (Button)FindViewById(Resource.Id.btnDoPhoto);
            //btnSettings = (Button)FindViewById(Resource.Id.btnSettings);
            //ImageViewLastPhoto = (ImageView)FindViewById(Resource.Id.ImageViewLastPhoto);
            //txtPath = (TextView)FindViewById(Resource.Id.txtPath);
            check = (Button)FindViewById(Resource.Id.check);
            RadioButton radioPhone = FindViewById<RadioButton>(Resource.Id.radioPhone);
            RadioButton radioSD = FindViewById<RadioButton>(Resource.Id.radioSD);

            radioPhone.Click += RadioButtonClick;
            radioSD.Click += RadioButtonClick;

            check.Click += checkFunction;
            btnDoPhoto.Click += DoPhoto_Click;
            //btnSettings.Click += Settings_Click;
            RequestPermissions(permissionGroup, 0);
        }

        private void RadioButtonClick(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            //Toast.MakeText(this, rb.Text, ToastLength.Short).Show();
            if(rb.Text.Equals("SD CARD"))
            {
                ChangeFolderForPhotos(PHONE_PATH_PICTURES + "/" + FOLDER_NAME, SD_PATH_PICTURES + "/" + FOLDER_NAME);
            } else if(rb.Text.Equals("PHONE"))
            {
                ChangeFolderForPhotos(SD_PATH_PICTURES + "/" + FOLDER_NAME, PHONE_PATH_PICTURES + "/" + FOLDER_NAME);
            } else
            {
                Toast.MakeText(this, "ELSE?", ToastLength.Long).Show();
            }
        }

        private void ChangeFolderForPhotos(string sourceDir, string destDir)
        {
            if (Directory.Exists(sourceDir))
            {
                Toast.MakeText(this, "Source Dir Exists...", ToastLength.Long).Show();
                try
                {
                    /*string sourceDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString() + "/PrzegladyZdjecia";
                    string destDir = GetBaseFolderPath(true) + "/CheckThisOutOnSD";*/
                    string[] picList = Directory.GetFiles(sourceDir, "*.jpg");

                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    foreach (string f in picList)
                    {
                        //Toast.MakeText(this, f, ToastLength.Long).Show();
                        string fName = f.Substring(sourceDir.Length + 1);
                        System.IO.File.Copy(System.IO.Path.Combine(sourceDir, fName), System.IO.Path.Combine(destDir, fName), true);
                    }

                    foreach (string f in picList)
                    {
                        System.IO.File.Delete(f);
                        //Toast.MakeText(this, "Usuwamy " + f, ToastLength.Long).Show();
                    }

                }
                catch (DirectoryNotFoundException dirNotFound)
                {
                    Toast.MakeText(this, dirNotFound.ToString(), ToastLength.Long).Show();
                }
                //Copy(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString() + "/newDirectory", Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString() + "/CheckThis");
                Toast.MakeText(this, "DONE", ToastLength.Long).Show();
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
                Directory = FOLDER_NAME,
                //It is important to set true to SaveToAlbum!!!
                SaveToAlbum = true
            });

            if (file == null)
                return;

            //Check the last taken photo
            //byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            //Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            //ImageViewLastPhoto.SetImageBitmap(bitmap);
            //txtPath.Text = file.Path;
            
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}