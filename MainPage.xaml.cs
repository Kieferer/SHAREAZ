using System.Diagnostics;
#if ANDROID
using Android.Service.Voice;
#endif
using SHAREAZ.Services;
namespace SHAREAZ;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
    }

	private async void  OnClickSend(object sender, EventArgs e)
	{
        try
        {
            FileResult fileResult = await FilePicker.PickAsync();
            if (fileResult != null)
            {
                Debug.WriteLine("Selected file: " + fileResult.FullPath);
                FileSender.Send(GetSelectedClient(), fileResult.FullPath);
                // You can use fileResult.FileName and fileResult.Stream to access the selected file's information and data.
                // Example: string fileName = fileResult.FileName; Stream fileStream = await fileResult.OpenReadAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            // Handle any errors that might occur during file selection.
        }
    }
    private void OnClickReceive(object sender, EventArgs e)
    {
        string downloadFolderPath = GetDownloadsFolderPath();
        try
        {
            FileReceiver.Receive(downloadFolderPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private string GetDownloadsFolderPath()
    {
        #if ANDROID
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;
        #elif WINDOWS
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        #else
                throw new PlatformNotSupportedException("Platform not supported.");
        #endif
    }

    private string GetSelectedClient()
    {
        return clientIP.GetItemsAsArray()[clientIP.SelectedIndex];
    }
}

