using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AliceNeural.Utils
{
	public static class PlaceUtils
	{
		private static CancellationTokenSource _cancelTokenSource;
		private static bool _isCheckingLocation;

		public static async Task<Location?> GetCurrentLocation()
		{
			try
			{
				_isCheckingLocation = true;

				GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

				_cancelTokenSource = new CancellationTokenSource();

				Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);
				_isCheckingLocation = false;
				return location;
			}
			// Catch one of the following exceptions:
			//   FeatureNotSupportedException
			//   FeatureNotEnabledException
			//   PermissionException
			catch (Exception ex)
			{
				// Unable to get location
				_isCheckingLocation = false;
				return null;
			}

		}

		internal static void CancelRequest()
		{
			if (_isCheckingLocation && _cancelTokenSource != null && _cancelTokenSource.IsCancellationRequested == false)
				_cancelTokenSource.Cancel();
		}

		public static async Task<string> GetGeocodeReverseData(double latitude, double longitude)
		{
			IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(latitude, longitude);

			Placemark placemark = placemarks?.FirstOrDefault();

			string luogo = placemark.Locality.ToString();

			return luogo;
		}

		public static async Task<Location?> GetLocation(string address)
		{
			IEnumerable<Location> locations = await Geocoding.Default.GetLocationsAsync(address);

			Location location = locations?.FirstOrDefault();

			return location;
		}
	}
}
