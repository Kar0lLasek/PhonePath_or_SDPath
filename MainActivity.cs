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
using Android.Provider;
using Android.Util;

namespace simple_Camera
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static string PHONE_PATH_PICTURES = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString();
        public static string SD_PATH_PICTURES = GetBaseFolderPath(true) + "/Pictures";
        public static string FOLDER_NAME = "PrzegladyZdjecia";

        Button btnDoPhoto, check;
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
            SetContentView(Resource.Layout.activity_main);

            btnDoPhoto = (Button)FindViewById(Resource.Id.btnDoPhoto);
            check = (Button)FindViewById(Resource.Id.check);
            RadioButton radioPhone = FindViewById<RadioButton>(Resource.Id.radioPhone);
            RadioButton radioSD = FindViewById<RadioButton>(Resource.Id.radioSD);
            ImageViewLastPhoto = (ImageView)FindViewById(Resource.Id.ImageViewLastPhoto);
            txtPath = (TextView)FindViewById(Resource.Id.txtPath);

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

        /*private void saveBitmap(Context context, Bitmap bitmap,
                        Bitmap.CompressFormat format, String mimeType,
                        String displayName)
        {
            String relativeLocation = Android.OS.Environment.DirectoryPictures;

            ContentValues  contentValues = new ContentValues();
            
            contentValues.Put(MediaStore.MediaColumns.DisplayName, displayName);
            contentValues.Put(MediaStore.MediaColumns.MimeType, mimeType);
            contentValues.Put(MediaStore.MediaColumns.RelativePath, relativeLocation);

            ContentResolver resolver = context.ContentResolver;

            OutputStream stream = null;
            Uri uri = null;

            try
            {
                Uri contentUri = MediaStore.Images.Media.ExternalContentUri;
                uri = resolver.Insert(contentUri, contentValues);

                if (uri == null)
                {
                    throw new System.IO.IOException("Failed to create new MediaStore record.");
                }

                stream = resolver.OpenOutputStream(uri);

                if (stream == null)
                {
                    throw new System.IO.IOException("Failed to get output stream.");
                }

                if (bitmap.Compress(format, 95, stream) == false)
                {
                    throw new System.IO.IOException("Failed to save bitmap.");
                }
            }
            catch (System.IO.IOException e)
            {
                if (uri != null)
                {
                    // Don't leave an orphan entry in the MediaStore
                    resolver.Delete(uri, null, null);
                }

                throw e;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }*/

        /*private void saveImage(Bitmap bitmap, String name)
        {
            OutputStream fos;
            if (Build.VERSION.SdkInt >= Build.VERSION_CODES.Q) {
                ContentResolver resolver = ContentResolver;
                ContentValues contentValues = new ContentValues();
                contentValues.Put(MediaStore.MediaColumns.DisplayName, name + ".jpg");
                contentValues.Put(MediaStore.MediaColumns.MimeType, "image/jpg");
                contentValues.Put(MediaStore.MediaColumns.RelativePath, Environment.DIRECTORY_PICTURES);
                Uri imageUri = resolver.Insert(MediaStore.Images.Media., contentValues);
                fos = resolver.OpenOutputStream(Java.Util.Objects.requireNonNull(imageUri));
            } else {
                String imagesDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString();
                Java.IO.File image = new Java.IO.File(imagesDir, name + ".jpg");
            fos = new FileOutputStream(image);
        }
        bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, fos);
            Objects.requireNonNull(fos).close();
        }*/

        void ExportBitmapAsJPG(Bitmap bitmap, string sdCardPath)
        {
            if (!Directory.Exists(sdCardPath))
                Directory.CreateDirectory(sdCardPath);
            var filePath = System.IO.Path.Combine(sdCardPath, "test.jpg");
            var stream = new FileStream(filePath, FileMode.Create);
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();
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
            } else
            {
                Toast.MakeText(this, "Nie ma takiej ścieżki", ToastLength.Short).Show();
            }
        }

        private void checkFunction(object sender, EventArgs e)
        {
            /*Toast.MakeText(this, "SD card path: " + GetBaseFolderPath(true), ToastLength.Long).Show();
            Toast.MakeText(this, "Phone path: " + GetBaseFolderPath(false), ToastLength.Long).Show();*/
            if(Directory.Exists(SD_PATH_PICTURES))
            {
                Toast.MakeText(this, "SD DZIAŁa", ToastLength.Long).Show();
            } else
            {
                Toast.MakeText(this, "NIE", ToastLength.Long).Show();
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

            
            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            //Toast.MakeText(this, file.ToString(), ToastLength.Long).Show();
            Bitmap bitmap = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            ImageViewLastPhoto.SetImageBitmap(bitmap);
            txtPath.Text = file.Path;

            ExportBitmapAsJPG(bitmap, SD_PATH_PICTURES + "/" + FOLDER_NAME);
            if(Directory.Exists(SD_PATH_PICTURES + "/" + FOLDER_NAME))
                Toast.MakeText(this, "TAK SD", ToastLength.Long).Show();
            else
                Toast.MakeText(this, "NIE SD", ToastLength.Long).Show();

            ExportBitmapAsJPG(bitmap, PHONE_PATH_PICTURES + "/" + FOLDER_NAME);
            /*if (Directory.Exists(PHONE_PATH_PICTURES + "/" + FOLDER_NAME))
                Toast.MakeText(this, "TAK PHONE", ToastLength.Long).Show();
            else
                Toast.MakeText(this, "NIE PHONE", ToastLength.Long).Show();*/

            /*if(Directory.Exists(file.Path))
                Toast.MakeText(this, "ISTNIEJE", ToastLength.Long).Show();
            else Toast.MakeText(this, "NIE ISTNIEJE", ToastLength.Long).Show();

            if(Directory.Exists(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString()))
            {
                string sourceDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).ToString();
                Toast.MakeText(this, "Ciekawe", ToastLength.Long).Show();
                *//*string[] picList = Directory.GetFiles(sourceDir, "*.jpg");
                string[] dirList = Directory.GetDirectories(sourceDir, "*");
                string showDirs = "";
                foreach (string dir in dirList)
                    showDirs += dir + "\n";
                Log.Debug("ShowDirs: ", showDirs);
                string showFiles = "";
                foreach (string pic in picList)
                    showFiles += pic + "\n";
                Log.Debug("ShowFiles: ", showFiles);*//*
            } else
            {
                Toast.MakeText(this, "NIE Ciekawe", ToastLength.Long).Show();
            }*/
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}