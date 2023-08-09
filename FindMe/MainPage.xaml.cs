namespace FindMe;

public partial class MainPage : ContentPage
{
    string _baseUrl = "https://bing.com/maps/default.aspx?cp=";
    public string UserName { get; set; }

    public MainPage()
	{
		InitializeComponent();
	}

	private async void OnFindMeClicked(object sender, EventArgs e)
	{
        isLoading.IsEnabled = true;
        isLoading.IsVisible = true;
        isLoading.IsRunning = true;
        var permission = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

		if (permission == PermissionStatus.Granted)
		{
			await ShareLocation();
		}
        else
        {
			await DisplayAlert("Permission Errror", "You have not granted the app permission to access your location.", "OK");

			var request = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (request == PermissionStatus.Granted)
            {
                await ShareLocation();
            }
			else
			{
                isLoading.IsEnabled = false;
                isLoading.IsVisible = false;
                isLoading.IsRunning = false;

                if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
				{
					await DisplayAlert("Location Required", "Location is required to share it. Please enable for this app in settings.", "OK");
				}
				else
				{
					await DisplayAlert("Location Required", "Location is required to share it. We'll ask agin next time.", "OK");
				}
			}
        }

        SemanticScreenReader.Announce(FindBtn.Text);
	}

	private async Task ShareLocation()
	{
		UserName = UsernameEntry.Text;

		var locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);

		var location = await Geolocation.GetLocationAsync(locationRequest);

		await Share.RequestAsync(new ShareTextRequest
		{
			Subject = "Find me!",
			Title = "Find me!",
			Text = $"{UserName} is sharing their location with you",
			Uri = $"{_baseUrl}{location.Latitude}~{location.Latitude}"
		});

        isLoading.IsEnabled = false;
        isLoading.IsVisible = false;
        isLoading.IsRunning = false;
    }
}

