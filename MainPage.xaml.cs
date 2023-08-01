using System.Diagnostics;
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
        ChangeControlButtonAccessability();
        try
        {
            FileResult fileResult = await FilePicker.PickAsync();
            if (fileResult != null)
            {
                await FileSender.Send(ChangeProgressValue, GetSelectedClient(), fileResult.FullPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            ChangeControlButtonAccessability();
        }
    }
    private async void OnClickReceive(object sender, EventArgs e)
    {
        ChangeControlButtonAccessability();
        string downloadFolderPath = GetDownloadsFolderPath();
        try
        {
            await FileReceiver.Receive(ChangeProgressValue, downloadFolderPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            ChangeControlButtonAccessability();
        }
    }
    private void ChangeProgressValue(double value)
    {
        progress.Progress = value;
        progressText.Text = $"{value:P2}";
    }

    private void ChangeControlButtonAccessability()
    {
        progressText.IsVisible = !progressText.IsVisible;
        progress.IsVisible = !progress.IsVisible;
        SendBtn.IsEnabled = !SendBtn.IsEnabled;
        ReceiveBtn.IsEnabled = !ReceiveBtn.IsEnabled;
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