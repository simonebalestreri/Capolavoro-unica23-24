using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Intent;
using AliceNeural.Utils;
using AliceNeural.Models;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using System.Net.Http.Json;

using AliceNeural.Utils.Meteo;
using HttpProxyControl;
namespace AliceNeural
{
	public partial class MainPage : ContentPage
	{
		static readonly HttpClient _client = HttpProxyControl.HttpProxyHelper.CreateHttpClient(setProxy: true);
		SpeechRecognizer? speechRecognizer;
		IntentRecognizer? intentRecognizerByPatternMatching;
		IntentRecognizer? intentRecognizerByCLU;
		SpeechSynthesizer? speechSynthesizer;
		TaskCompletionSourceManager<int>? taskCompletionSourceManager;
		AzureCognitiveServicesResourceManager? serviceManager;
		bool buttonToggle = false;
		Brush? buttonToggleColor;
		private static readonly JsonSerializerOptions? jsonSerializationOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };
		public MainPage()
		{
			InitializeComponent();
			serviceManager = new AzureCognitiveServicesResourceManager("MyResponder", "MyDeployment");
			taskCompletionSourceManager = new TaskCompletionSourceManager<int>();
			(intentRecognizerByPatternMatching, speechSynthesizer, intentRecognizerByCLU) =
				ConfigureContinuousIntentPatternMatchingWithMicrophoneAsync(
					serviceManager.CurrentSpeechConfig,
					serviceManager.CurrentCluModel,
					serviceManager.CurrentPatternMatchingModel,
					taskCompletionSourceManager);
			speechRecognizer = new SpeechRecognizer(serviceManager.CurrentSpeechConfig);
		}
		protected override async void OnDisappearing()
		{
			base.OnDisappearing();

			if (speechSynthesizer != null)
			{
				await speechSynthesizer.StopSpeakingAsync();
				speechSynthesizer.Dispose();
			}

			if (intentRecognizerByPatternMatching != null)
			{
				await intentRecognizerByPatternMatching.StopContinuousRecognitionAsync();
				intentRecognizerByPatternMatching.Dispose();
			}

			if (intentRecognizerByCLU != null)
			{
				await intentRecognizerByCLU.StopContinuousRecognitionAsync();
				intentRecognizerByCLU.Dispose();
			}
		}

		private async void ContentPage_Loaded(object sender, EventArgs e)
		{
			await CheckAndRequestMicrophonePermission();
		}

		private async Task<PermissionStatus> CheckAndRequestMicrophonePermission()
		{
			PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
			if (status == PermissionStatus.Granted)
			{
				return status;
			}
			if (Permissions.ShouldShowRationale<Permissions.Microphone>())
			{
				// Prompt the user with additional information as to why the permission is needed
				await DisplayAlert("Permission required", "Microphone permission is necessary", "OK");
			}
			status = await Permissions.RequestAsync<Permissions.Microphone>();
			return status;
		}

		private static async Task ContinuousIntentPatternMatchingWithMicrophoneAsync(
			IntentRecognizer intentRecognizer, TaskCompletionSourceManager<int> stopRecognition)
		{
			await intentRecognizer.StartContinuousRecognitionAsync();
			// Waits for completion. Use Task.WaitAny to keep the task rooted.
			Task.WaitAny(new[] { stopRecognition.TaskCompletionSource.Task });
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		/// <param name="cluModel"></param>
		/// <param name="patternMatchingModelCollection"></param>
		/// <param name="stopRecognitionManager"></param>
		/// <returns>una tupla contentente nell'ordine un intent recognizer basato su Patter Matching, un sintetizzatore vocale e un intent recognizer basato su un modello di Conversational Language Understanding </returns>
		private static (IntentRecognizer, SpeechSynthesizer, IntentRecognizer) ConfigureContinuousIntentPatternMatchingWithMicrophoneAsync(
			SpeechConfig config,
			ConversationalLanguageUnderstandingModel cluModel,
			LanguageUnderstandingModelCollection patternMatchingModelCollection,
			TaskCompletionSourceManager<int> stopRecognitionManager)
		{
			//creazione di un intent recognizer basato su pattern matching
			var intentRecognizerByPatternMatching = new IntentRecognizer(config);
			intentRecognizerByPatternMatching.ApplyLanguageModels(patternMatchingModelCollection);

			//creazione di un intent recognizer basato su CLU
			var intentRecognizerByCLU = new IntentRecognizer(config);
			var modelsCollection = new LanguageUnderstandingModelCollection { cluModel };
			intentRecognizerByCLU.ApplyLanguageModels(modelsCollection);

			//creazione di un sitetizzatore vocale
			var synthesizer = new SpeechSynthesizer(config);

			//gestione eventi
			intentRecognizerByPatternMatching.Recognized += async (s, e) =>
			{
				switch (e.Result.Reason)
				{
					case ResultReason.RecognizedSpeech:
						Debug.WriteLine($"PATTERN MATCHING - RECOGNIZED SPEECH: Text= {e.Result.Text}");
						break;
					case ResultReason.RecognizedIntent:
						{
							Debug.WriteLine($"PATTERN MATCHING - RECOGNIZED INTENT: Text= {e.Result.Text}");
							Debug.WriteLine($"       Intent Id= {e.Result.IntentId}.");
							if (e.Result.IntentId == "Ok")
							{
								Debug.WriteLine("Stopping current speaking if any...");
								await synthesizer.StopSpeakingAsync();
								Debug.WriteLine("Stopping current intent recognition by CLU if any...");
								await intentRecognizerByCLU.StopContinuousRecognitionAsync();
								await HandleOkCommand(synthesizer, intentRecognizerByCLU).ConfigureAwait(false);
							}
							else if (e.Result.IntentId == "Stop")
							{
								Debug.WriteLine("Stopping current speaking...");
								await synthesizer.StopSpeakingAsync();
							}
						}

						break;
					case ResultReason.NoMatch:
						Debug.WriteLine($"NOMATCH: Speech could not be recognized.");
						var noMatch = NoMatchDetails.FromResult(e.Result);
						switch (noMatch.Reason)
						{
							case NoMatchReason.NotRecognized:
								Debug.WriteLine($"PATTERN MATCHING - NOMATCH: Speech was detected, but not recognized.");
								break;
							case NoMatchReason.InitialSilenceTimeout:
								Debug.WriteLine($"PATTERN MATCHING - NOMATCH: The start of the audio stream contains only silence, and the service timed out waiting for speech.");
								break;
							case NoMatchReason.InitialBabbleTimeout:
								Debug.WriteLine($"PATTERN MATCHING - NOMATCH: The start of the audio stream contains only noise, and the service timed out waiting for speech.");
								break;
							case NoMatchReason.KeywordNotRecognized:
								Debug.WriteLine($"PATTERN MATCHING - NOMATCH: Keyword not recognized");
								break;
						}
						break;

					default:
						break;
				}
			};
			intentRecognizerByPatternMatching.Canceled += (s, e) =>
			{
				Debug.WriteLine($"PATTERN MATCHING - CANCELED: Reason={e.Reason}");

				if (e.Reason == CancellationReason.Error)
				{
					Debug.WriteLine($"PATTERN MATCHING - CANCELED: ErrorCode={e.ErrorCode}");
					Debug.WriteLine($"PATTERN MATCHING - CANCELED: ErrorDetails={e.ErrorDetails}");
					Debug.WriteLine($"PATTERN MATCHING - CANCELED: Did you update the speech key and location/region info?");
				}
				stopRecognitionManager.TaskCompletionSource.TrySetResult(0);
			};
			intentRecognizerByPatternMatching.SessionStopped += (s, e) =>
			{
				Debug.WriteLine("\n    Session stopped event.");
				stopRecognitionManager.TaskCompletionSource.TrySetResult(0);
			};

			return (intentRecognizerByPatternMatching, synthesizer, intentRecognizerByCLU);

		}
		private static async Task HandleOkCommand(SpeechSynthesizer synthesizer, IntentRecognizer intentRecognizer)
		{
			await synthesizer.SpeakTextAsync("Sono in ascolto");
			//avvia l'intent recognition su Azure
			string? jsonResult = await RecognizeIntentAsync(intentRecognizer);
			if (jsonResult != null)
			{
				//process jsonResult
				//deserializzo il json
				CLUResponse cluResponse = JsonSerializer.Deserialize<CLUResponse>(jsonResult, jsonSerializationOptions) ?? new CLUResponse();
				//await synthesizer.SpeakTextAsync($"La tua richiesta è stata {cluResponse.Result?.Query}");
				var topIntent = cluResponse.Result?.Prediction?.TopIntent;

				if (topIntent != null)
				{
					switch (topIntent)
					{
						case string intent when intent.Contains("Wiki"):
							//await synthesizer.SpeakTextAsync("Vuoi fare una ricerca su Wikipedia");

							string mainSearchItem = cluResponse.Result?.Prediction?.Entities?.FirstOrDefault(e => e.Category.Contains("MainSearchItem")).Text;
							string subSearchItem = default;
							foreach (var item in cluResponse.Result?.Prediction?.Entities)
							{
								if (item.Category.Contains("SubSearchItem"))
								{
									subSearchItem = item.Text;
								}
							}

							await WikiResponder(synthesizer, cluResponse, mainSearchItem, subSearchItem, _client);
							break;

						case string intent when intent.Contains("Weather"):
							//await synthesizer.SpeakTextAsync("Vuoi sapere come è il tempo");
							await Meteoo(cluResponse, synthesizer);
							break;
						case string intent when intent.Contains("Places"):
							//await synthesizer.SpeakTextAsync("Vuoi informazioni geolocalizzate");
							string intentRecognize = cluResponse.Result?.Prediction?.TopIntent;
							await PlaceResponder(synthesizer, cluResponse, intentRecognize);
							break;
						case string intent when intent.Contains("None"):
							await synthesizer.SpeakTextAsync("Non ho capito");
							break;
					}

				}
				//determino l'action da fare, eventualmente effettuando una richiesta GET su un endpoint remoto scelto in base al topScoringIntent
				//ottengo il risultato dall'endpoit remoto
				//effettuo un text to speech per descrivere il risultato
			}
			else
			{
				//è stato restituito null - ciò si verifica, ad esempio, quando il processo è interrotto prima di ottenere la risposta dal server
				Debug.WriteLine("Non è stato restituito nulla dall'intent reconition sul server");
			}
		}

		public static async Task<string?> RecognizeIntentAsync(IntentRecognizer recognizer)
		{
			// Starts recognizing.
			Debug.WriteLine("Say something...");

			// Starts intent recognition, and returns after a single utterance is recognized. The end of a
			// single utterance is determined by listening for silence at the end or until a maximum of 15
			// seconds of audio is processed.  The task returns the recognition text as result. 
			// Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
			// shot recognition like command or query. 
			// For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
			var result = await recognizer.RecognizeOnceAsync();
			string? languageUnderstandingJSON = null;

			// Checks result.
			switch (result.Reason)
			{
				case ResultReason.RecognizedIntent:
					Debug.WriteLine($"RECOGNIZED: Text={result.Text}");
					Debug.WriteLine($"    Intent Id: {result.IntentId}.");
					languageUnderstandingJSON = result.Properties.GetProperty(PropertyId.LanguageUnderstandingServiceResponse_JsonResult);
					Debug.WriteLine($"    Language Understanding JSON: {languageUnderstandingJSON}.");
					CLUResponse cluResponse = JsonSerializer.Deserialize<CLUResponse>(languageUnderstandingJSON, jsonSerializationOptions) ?? new CLUResponse();
					Debug.WriteLine("Risultato deserializzato:");
					Debug.WriteLine($"kind: {cluResponse.Kind}");
					Debug.WriteLine($"result.query: {cluResponse.Result?.Query}");
					Debug.WriteLine($"result.prediction.topIntent: {cluResponse.Result?.Prediction?.TopIntent}");
					Debug.WriteLine($"result.prediction.Intents[0].Category: {cluResponse.Result?.Prediction?.Intents?[0].Category}");
					Debug.WriteLine($"result.prediction.Intents[0].ConfidenceScore: {cluResponse.Result?.Prediction?.Intents?[0].ConfidenceScore}");
					Debug.WriteLine($"result.prediction.entities: ");
					cluResponse.Result?.Prediction?.Entities?.ForEach(s => Debug.WriteLine($"\tcategory = {s.Category}; text= {s.Text};"));
					break;
				case ResultReason.RecognizedSpeech:
					Debug.WriteLine($"RECOGNIZED: Text={result.Text}");
					Debug.WriteLine($"    Intent not recognized.");
					break;
				case ResultReason.NoMatch:
					Debug.WriteLine($"NOMATCH: Speech could not be recognized.");
					break;
				case ResultReason.Canceled:
					var cancellation = CancellationDetails.FromResult(result);
					Debug.WriteLine($"CANCELED: Reason={cancellation.Reason}");

					if (cancellation.Reason == CancellationReason.Error)
					{
						Debug.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
						Debug.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
						Debug.WriteLine($"CANCELED: Did you update the subscription info?");
					}
					break;
			}
			return languageUnderstandingJSON;
		}
		private async void OnRecognitionButtonClicked2(object sender, EventArgs e)
		{
			if (serviceManager != null && taskCompletionSourceManager != null)
			{
				buttonToggle = !buttonToggle;
				if (buttonToggle)
				{
					MainThread.BeginInvokeOnMainThread(() =>
					{
						buttonToggleColor = RecognizeSpeechBtn.Background;
						RecognizeSpeechBtn.Background = Colors.Yellow;
					});


					//creo le risorse
					//su un dispositivo mobile potrebbe succedere che cambiando rete cambino i parametri della rete, ed in particolare il proxy
					//In questo caso, per evitare controlli troppo complessi, si è scelto di ricreare lo speechConfig ad ogni richiesta se cambia il proxy
					if (serviceManager.ShouldRecreateSpeechConfigForProxyChange())
					{
						(intentRecognizerByPatternMatching, speechSynthesizer, intentRecognizerByCLU) =
					   ConfigureContinuousIntentPatternMatchingWithMicrophoneAsync(
						   serviceManager.CurrentSpeechConfig,
						   serviceManager.CurrentCluModel,
						   serviceManager.CurrentPatternMatchingModel,
						   taskCompletionSourceManager);
					}

					_ = Task.Factory.StartNew(async () =>
					{
						//bisogna sempre verificare di avere il premesso prima di usare una risorsa
						//che richiede il permesso dell'utente
						PermissionStatus status = await CheckAndRequestMicrophonePermission();
						if (status == PermissionStatus.Granted)
						{
							taskCompletionSourceManager.TaskCompletionSource = new TaskCompletionSource<int>();
							await ContinuousIntentPatternMatchingWithMicrophoneAsync(
								intentRecognizerByPatternMatching!, taskCompletionSourceManager)
							.ConfigureAwait(false);
						}
					});
				}
				else
				{
					MainThread.BeginInvokeOnMainThread(() =>
					{
						RecognizeSpeechBtn.Background = buttonToggleColor;
					});
					//la doppia chiamata di StopSpeakingAsync è un work-around a un problema riscontrato in alcune situazioni:
					//se si prova a fermare il task mentre il sintetizzatore sta parlando, in alcuni casi si verifica un'eccezione. 
					//Con il doppio StopSpeakingAsync non succede.
					await speechSynthesizer!.StopSpeakingAsync();
					await speechSynthesizer.StopSpeakingAsync();
					await intentRecognizerByCLU!.StopContinuousRecognitionAsync();
					await intentRecognizerByPatternMatching!.StopContinuousRecognitionAsync();
					//speechSynthesizer.Dispose();
					//intentRecognizerByPatternMatching.Dispose();
				}
			}
		}
		private async void OnRecognitionButtonClicked(object sender, EventArgs e)
		{
			try
			{
				PermissionStatus status = await CheckAndRequestMicrophonePermission();
				if (status == PermissionStatus.Granted)
				{
					//accedo ai servizi
					//AzureCognitiveServicesResourceManager serviceManager =(Application.Current as App).AzureCognitiveServicesResourceManager;
					// Creates a speech recognizer using microphone as audio input.
					// Starts speech recognition, and returns after a single utterance is recognized. The end of a
					// single utterance is determined by listening for silence at the end or until a maximum of 15
					// seconds of audio is processed.  The task returns the recognition text as result.
					// Note: Since RecognizeOnceAsync() returns only a single utterance, it is suitable only for single
					// shot recognition like command or query.
					// For long-running multi-utterance recognition, use StartContinuousRecognitionAsync() instead.
					var result = await speechRecognizer!.RecognizeOnceAsync().ConfigureAwait(false);

					// Checks result.
					StringBuilder sb = new();
					if (result.Reason == ResultReason.RecognizedSpeech)
					{
						sb.AppendLine($"RECOGNIZED: Text={result.Text}");
						await speechSynthesizer!.SpeakTextAsync(result.Text);
					}
					else if (result.Reason == ResultReason.NoMatch)
					{
						sb.AppendLine($"NOMATCH: Speech could not be recognized.");
					}
					else if (result.Reason == ResultReason.Canceled)
					{
						var cancellation = CancellationDetails.FromResult(result);
						sb.AppendLine($"CANCELED: Reason={cancellation.Reason}");

						if (cancellation.Reason == CancellationReason.Error)
						{
							sb.AppendLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
							sb.AppendLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
							sb.AppendLine($"CANCELED: Did you update the subscription info?");
						}

					}
					UpdateUI(sb.ToString());
				}

			}
			catch (Exception ex)
			{
				UpdateUI("Exception: " + ex.ToString());
			}
		}
		private void UpdateUI(String message)
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				RecognitionText.Text = message;
			});
		}
		static async Task WikiResponder(SpeechSynthesizer synthesizer, CLUResponse cluResponse, string mainSearchItem, string subSearchItem, HttpClient _client)
		{
			await synthesizer.SpeakTextAsync($"\nInformazioni per {mainSearchItem}");
			string wikiUrl = $"https://it.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&exsectionformat=plain&redirects=1&titles={mainSearchItem}";
			//risposta wikipedia come stringa
			string wikiSummaryJSON = await _client.GetStringAsync(wikiUrl);
			string summary = AliceNeural.Utils.WikitextHelper.ExtractSummaryFromJSON(wikiSummaryJSON);
			await synthesizer.SpeakTextAsync(summary);
		}
		static async Task PlaceResponder(SpeechSynthesizer synthesizer, CLUResponse cluResponse, string intent)
		{
			switch (intent)
			{
				case "Places.FindPlaces":
					break;
				case "Places.GetDistance":
					List<Location> places = new();
					foreach (var item in cluResponse.Result?.Prediction?.Entities)
					{
						if (item.Category.Contains("AbsoluteLocation"))
						{
							places.Add(await PlaceUtils.GetLocation(item.Text));
						}
						if (item.Category.Contains("PlaceName"))
						{
							places.Add(await PlaceUtils.GetLocation(item.Text));
						}

					}
					Location place1 = default;
					Location place2 = default;
					switch (places.Count())
					{
						case 0:
							Debug.Write("***\n errore \n ***");
							break;
						case 1:
							place1 = places[0];
							place2 = await PlaceUtils.GetCurrentLocation();
							break;
						default:
							place1 = places[0];
							place2 = places[1];
							break;
					}
					int kilometers = (int)Location.CalculateDistance(place1, place2, DistanceUnits.Kilometers);
					string place1Name = PlaceUtils.GetGeocodeReverseData(place1.Latitude, place1.Longitude).Result;
					string place2Name = PlaceUtils.GetGeocodeReverseData(place2.Latitude, place2.Longitude).Result;

					await synthesizer.SpeakTextAsync($"\n{place1Name} dista {kilometers} chilometri da {place2Name}");
					break;
			}
		}

		public static async Task Meteoo(CLUResponse cluResponse, SpeechSynthesizer synthesizer)
		{
			try
			{
				var primoIntent = cluResponse.Result?.Prediction?.Intents?[0].Category;
				var entities = cluResponse.Result?.Prediction?.Entities;
				if (primoIntent != null)
				{
					switch (primoIntent)
					{
						case string intent when intent == "Weather.CheckWeatherValue":
							//che tempo fa a Seregno
							var placeEntityCWV = entities?.Where(l => l.Category == "Places.AbsoluteLocation" || l.Category == "Places.PlaceName").FirstOrDefault();
							var whenEntityCWV = entities?.Where(q => q.Category == "datetimeV2").FirstOrDefault();
							Debug.WriteLine(whenEntityCWV);
							string placeCWV = placeEntityCWV?.Text != null ? placeEntityCWV.Text : "Monticello Brianza";

							StringBuilder weatherSpeechCWV = new();
							(double? lat, double? lon)? geoCWV = await Utils.Meteo.Meteo.GeocodeByOpenMeteo(_client, placeCWV);
							if (geoCWV != null)
							{
								FormattableString addressUrlFormattableCWV = $"https://api.open-meteo.com/v1/forecast?latitude={geoCWV?.lat}&longitude={geoCWV?.lon}&current=temperature_2m,weather_code,wind_speed_10m,wind_direction_10m&hourly=temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,weather_code,wind_speed_10m,wind_direction_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min&timeformat=unixtime&timezone=auto";
								string addressUrlCWV = FormattableString.Invariant(addressUrlFormattableCWV);
								var responseCWV = await _client.GetAsync($"{addressUrlCWV}");
								if (responseCWV.IsSuccessStatusCode)
								{
									Utils.Meteo.Models.Complete.OpenMeteoForecast? forecastCWV = await responseCWV.Content.ReadFromJsonAsync<Utils.Meteo.Models.Complete.OpenMeteoForecast>();
									if (forecastCWV != null)
									{
										int dayCWV = 0;
										string giornoCWV = string.Empty;
										if (whenEntityCWV.Text != null)
										{
											if (whenEntityCWV.Text == "oggi")
											{
												dayCWV = 0;
												giornoCWV = "Oggi";
											}
											else if (whenEntityCWV.Text == "domani")
											{
												dayCWV = 1;
												giornoCWV = "Domani";
											}
											else if (whenEntityCWV.Text == "dopodomani")
											{
												dayCWV = 2;
												giornoCWV = "Dopodomani";
											}
										}
										weatherSpeechCWV.Append($"{giornoCWV}");
										weatherSpeechCWV.Append($" le previsioni danno {Utils.Meteo.Meteo.Display(Utils.Meteo.Meteo.WMOCodesIntIT(forecastCWV?.Daily?.WeatherCode?[dayCWV]), string.Empty)}");
										weatherSpeechCWV.Append($" con una temperatura massima {Utils.Meteo.Meteo.Display(forecastCWV?.Daily?.Temperature2mMax?[dayCWV], string.Empty)} °C");
										weatherSpeechCWV.Append($" e minima {Utils.Meteo.Meteo.Display(forecastCWV?.Daily?.Temperature2mMin?[dayCWV], string.Empty)} °C");
									}
									else
									{
										weatherSpeechCWV.Append($"Il giorno richiesto dista troppo tempo per poter fare una previsione");
									}

									await synthesizer.SpeakTextAsync(weatherSpeechCWV.ToString());
								}
							}
							break;
						case string intent when intent == "Weather.QueryWeather":
							//ci sarà vento domani a Milano?
							var placeEntityQW = entities?.Where(l => l.Category == "Places.AbsoluteLocation" || l.Category == "Places.PlaceName").FirstOrDefault();
							var weatherConditionEntityQW = entities?.Where(c => c.Category == "Weather.WeatherCondition").FirstOrDefault();
							var whenEntityQW = entities?.Where(q => q.Category == "datetimeV2").FirstOrDefault();
							string placeQW = placeEntityQW?.Text != null ? placeEntityQW.Text : "Monticello Brianza";

							StringBuilder weatherSpeechQW = new();
							(double? lat, double? lon)? geoQW = await Utils.Meteo.Meteo.GeocodeByOpenMeteo(_client, placeQW);
							if (geoQW != null)
							{
								FormattableString addressUrlFormattableQW = $"https://api.open-meteo.com/v1/forecast?latitude={geoQW?.lat}&longitude={geoQW?.lon}&current=temperature_2m,weather_code,wind_speed_10m,wind_direction_10m&hourly=temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,weather_code,wind_speed_10m,wind_direction_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min&timeformat=unixtime&timezone=auto";
								string addressUrlQW = FormattableString.Invariant(addressUrlFormattableQW);
								var responseQW = await _client.GetAsync($"{addressUrlQW}");
								if (responseQW.IsSuccessStatusCode)
								{
									Utils.Meteo.Models.Complete.OpenMeteoForecast forecastQW = await responseQW.Content.ReadFromJsonAsync<Utils.Meteo.Models.Complete.OpenMeteoForecast>();
									if (forecastQW != null)
									{
										int dayQW = 0;
										string giornoQW = string.Empty;
										if (whenEntityQW.Text != null)
										{
											if (whenEntityQW.Text == "oggi")
											{
												dayQW = 0;
												giornoQW = "Oggi";
											}
											else if (whenEntityQW.Text == "domani")
											{
												dayQW = 1;
												giornoQW = "Domani";
											}
											else if (whenEntityQW.Text == "dopodomani")
											{
												dayQW = 2;
												giornoQW = "Dopodomani";
											}
										}
										var richiestaQW = Utils.Meteo.Meteo.Display(Utils.Meteo.Meteo.WMOCodesIntIT(forecastQW?.Daily?.WeatherCode?[dayQW]), string.Empty);
										bool affermativo = false;
										#region VarieCasisitiche
										if (forecastQW?.Daily?.WeatherCode?[dayQW] == 0 || forecastQW?.Daily?.WeatherCode?[dayQW] == 1 && weatherConditionEntityQW.Text.Equals("bello") || weatherConditionEntityQW.Text.Equals("bello"))
										{
											affermativo = true;
										}
										if (forecastQW?.Daily?.WeatherCode?[dayQW] == 2 || forecastQW?.Daily?.WeatherCode?[dayQW] == 3 && weatherConditionEntityQW.Text.Equals("nuvoloso") || weatherConditionEntityQW.Text.Equals("copero"))
										{
											affermativo = true;
										}
										if (forecastQW?.Daily?.WeatherCode?[dayQW] == 45 || forecastQW?.Daily?.WeatherCode?[dayQW] == 48 && weatherConditionEntityQW.Text.Equals("brina") || weatherConditionEntityQW.Text.Equals("nebbia"))
										{
											affermativo = true;
										}
										if (forecastQW?.Daily?.WeatherCode?[dayQW] >= 51 && forecastQW?.Daily?.WeatherCode?[dayQW] <= 67 || forecastQW?.Daily?.WeatherCode?[dayQW] >= 80 && forecastQW?.Daily?.WeatherCode?[dayQW] <= 82 && weatherConditionEntityQW.Text.Equals("Pioverà") || weatherConditionEntityQW.Text.Equals("pioggia"))
										{
											affermativo = true;
										}
										if (forecastQW?.Daily?.WeatherCode?[dayQW] >= 71 && forecastQW?.Daily?.WeatherCode?[dayQW] <= 77 || forecastQW?.Daily?.WeatherCode?[dayQW] >= 85 && forecastQW?.Daily?.WeatherCode?[dayQW] <= 86 && weatherConditionEntityQW.Text.Equals("nevicherà") || weatherConditionEntityQW.Text.Equals("neve"))
										{
											affermativo = true;
										}
										if (forecastQW?.Daily?.WeatherCode?[dayQW] == 95 || forecastQW?.Daily?.WeatherCode?[dayQW] == 99 && weatherConditionEntityQW.Text.Equals("temporale") || weatherConditionEntityQW.Text.Equals("Pioverà"))
										{
											affermativo = true;
										}
										#endregion
										if (affermativo == true)
										{
											weatherSpeechQW.Append($"Sì {giornoQW} a {placeQW} le previsioni danno {richiestaQW}");
										}
										else
										{
											weatherSpeechQW.Append($"{giornoQW} a {placeQW} le previsioni danno {richiestaQW}");
										}
									}
									else
									{
										weatherSpeechQW.Append($"Il giorno richiesto dista troppo tempo per poter fare una previsione");
									}

									await synthesizer.SpeakTextAsync(weatherSpeechQW.ToString());
								}
							}
							break;
						case string intent when intent == "Weather.CheckWeatherTime":
							//Quando inizierà a piovere domani?
							var placeEntityCWT = entities?.Where(l => l.Category == "Places.AbsoluteLocation" || l.Category == "Places.PlaceName").FirstOrDefault();
							var weatherConditionEntityCWT = entities?.Where(c => c.Category == "Weather.WeatherCondition").FirstOrDefault();
							var whenEntityCWT = entities?.Where(q => q.Category == "datetimeV2").FirstOrDefault();
							Debug.WriteLine(weatherConditionEntityCWT);
							string placeCWT = placeEntityCWT?.Text != null ? placeEntityCWT.Text : "Monticello Brianza";
							StringBuilder weatherSpeechCWT = new();
							(double? lat, double? lon)? geoCWT = await Utils.Meteo.Meteo.GeocodeByOpenMeteo(_client, placeCWT);
							if (geoCWT != null)
							{
								FormattableString addressUrlFormattableCWT = $"https://api.open-meteo.com/v1/forecast?latitude={geoCWT?.lat}&longitude={geoCWT?.lon}&current=temperature_2m,weather_code,wind_speed_10m,wind_direction_10m&hourly=temperature_2m,relative_humidity_2m,dew_point_2m,apparent_temperature,precipitation_probability,precipitation,rain,showers,weather_code,wind_speed_10m,wind_direction_10m&daily=weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min&timeformat=unixtime&timezone=auto";
								string addressUrlCWT = FormattableString.Invariant(addressUrlFormattableCWT);
								var responseCWT = await _client.GetAsync($"{addressUrlCWT}");
								if (responseCWT.IsSuccessStatusCode)
								{
									Utils.Meteo.Models.Complete.OpenMeteoForecast? forecastCWT = await responseCWT.Content.ReadFromJsonAsync<Utils.Meteo.Models.Complete.OpenMeteoForecast>();
									if (forecastCWT != null)
									{
										int dayCWT = 0;
										string giornoCWT = string.Empty;
										if (whenEntityCWT.Text != null)
										{
											if (whenEntityCWT.Text == "oggi")
											{
												dayCWT = 0;
												giornoCWT = "oggi";
											}
											else if (whenEntityCWT.Text == "domani")
											{
												dayCWT = 1;
												giornoCWT = "domani";
											}
											else if (whenEntityCWT.Text == "dopodomani")
											{
												dayCWT = 2;
												giornoCWT = "dopodomani";
											}
										}
										var richiestaCWT = Utils.Meteo.Meteo.Display(Utils.Meteo.Meteo.WMOCodesIntIT(forecastCWT?.Current?.WeatherCode), string.Empty);
										bool affermativo = false;
										#region VarieCasisitiche
										if (forecastCWT?.Daily?.WeatherCode?[dayCWT] == 0 || forecastCWT?.Daily?.WeatherCode?[dayCWT] == 1 && weatherConditionEntityCWT.Text.Equals("sereno") || weatherConditionEntityCWT.Text.Equals("limpido"))
										{
											affermativo = true;
										}
										if (forecastCWT?.Daily?.WeatherCode?[dayCWT] == 2 || forecastCWT?.Daily?.WeatherCode?[dayCWT] == 3 && weatherConditionEntityCWT.Text.Equals("nuvoloso") || weatherConditionEntityCWT.Text.Equals("copero"))
										{
											affermativo = true;
										}
										if (forecastCWT?.Daily?.WeatherCode?[dayCWT] == 45 || forecastCWT?.Daily?.WeatherCode?[dayCWT] == 48 && weatherConditionEntityCWT.Text.Equals("brinare") || weatherConditionEntityCWT.Text.Equals("nebbia"))
										{
											affermativo = true;
										}
										if (forecastCWT?.Daily?.WeatherCode?[dayCWT] >= 51 && forecastCWT?.Daily?.WeatherCode?[dayCWT] <= 67 || forecastCWT?.Daily?.WeatherCode?[dayCWT] >= 80 && forecastCWT?.Daily?.WeatherCode?[dayCWT] <= 82 && weatherConditionEntityCWT.Text.Equals("piovere") || weatherConditionEntityCWT.Text.Equals("pioggia") || weatherConditionEntityCWT.Text.Equals("piovere"))
										{
											affermativo = true;
										}
										if (forecastCWT?.Daily?.WeatherCode?[dayCWT] >= 71 && forecastCWT?.Daily?.WeatherCode?[dayCWT] <= 77 || forecastCWT?.Daily?.WeatherCode?[dayCWT] >= 85 && forecastCWT?.Daily?.WeatherCode?[dayCWT] <= 86 && weatherConditionEntityCWT.Text.Equals("nevichare") || weatherConditionEntityCWT.Text.Equals("neve"))
										{
											affermativo = true;
										}
										if (forecastCWT?.Daily?.WeatherCode?[dayCWT] == 95 || forecastCWT?.Daily?.WeatherCode?[dayCWT] == 99 && weatherConditionEntityCWT.Text.Equals("temporale") || weatherConditionEntityCWT.Text.Equals("piovere") || weatherConditionEntityCWT.Text.Equals("pioverà"))
										{
											affermativo = true;
										}
										#endregion
										if (affermativo == true)
										{
											affermativo = false;
											int? numeroPrevisioni = forecastCWT.Hourly.Time?.Count;
											int index = 0;
											if (numeroPrevisioni > 0)
											{
												for (int i = 0; i < numeroPrevisioni; i++)
												{
													if (affermativo == false)
													{

														if (forecastCWT?.Hourly?.WeatherCode?[i] == 0 || forecastCWT?.Hourly?.WeatherCode?[i] == 1 && weatherConditionEntityCWT.Text.Equals("sereno") || weatherConditionEntityCWT.Text.Equals("limpido"))
														{
															affermativo = true;
															index = i;
														}
														if (forecastCWT?.Hourly?.WeatherCode?[i] == 2 || forecastCWT?.Hourly?.WeatherCode?[i] == 3 && weatherConditionEntityCWT.Text.Equals("nuvoloso") || weatherConditionEntityCWT.Text.Equals("copero"))
														{
															affermativo = true;
															index = i;
														}
														if (forecastCWT?.Hourly?.WeatherCode?[i] == 45 || forecastCWT?.Hourly?.WeatherCode?[i] == 48 && weatherConditionEntityCWT.Text.Equals("brinare") || weatherConditionEntityCWT.Text.Equals("nebbia"))
														{
															affermativo = true;
															index = i;
														}
														if (forecastCWT?.Hourly?.WeatherCode?[i] >= 51 && forecastCWT?.Hourly?.WeatherCode?[i] <= 67 || forecastCWT?.Hourly?.WeatherCode?[i] >= 80 && forecastCWT?.Hourly?.WeatherCode?[i] <= 82 && weatherConditionEntityCWT.Text.Equals("piovere") || weatherConditionEntityCWT.Text.Equals("pioggia") || weatherConditionEntityCWT.Text.Equals("piovere"))
														{
															affermativo = true;
															index = i;
														}
														if (forecastCWT?.Hourly?.WeatherCode?[i] >= 71 && forecastCWT?.Hourly?.WeatherCode?[i] <= 77 || forecastCWT?.Hourly?.WeatherCode?[i] >= 85 && forecastCWT?.Hourly?.WeatherCode?[i] <= 86 && weatherConditionEntityCWT.Text.Equals("nevichare") || weatherConditionEntityCWT.Text.Equals("neve"))
														{
															affermativo = true;
															index = i;
														}
														if (forecastCWT?.Hourly?.WeatherCode?[i] == 95 || forecastCWT?.Hourly?.WeatherCode?[i] == 99 && weatherConditionEntityCWT.Text.Equals("temporale") || weatherConditionEntityCWT.Text.Equals("piovere") || weatherConditionEntityCWT.Text.Equals("pioverà"))
														{
															affermativo = true;
															index = i;
														}
													}
												}
												if (affermativo == true)
												{
													weatherSpeechCWT.Append($"Le previsioni danno {Utils.Meteo.Meteo.Display(Utils.Meteo.Meteo.WMOCodesIntIT(forecastCWT.Hourly.WeatherCode?[index]), "")} a {placeCWT} in data e ora {forecastCWT.Hourly.Time?[index]}");
												}
											}
										}
										else
										{
											weatherSpeechCWT.Append($"Nei prossimi sette giorni le previsioni non {weatherConditionEntityCWT.Text}, le previsioni danno {richiestaCWT}");
										}
									}
									else
									{
										weatherSpeechCWT.Append($"Il giorno richiesto dista troppo tempo per poter fare una previsione");
									}

									await synthesizer.SpeakTextAsync(weatherSpeechCWT.ToString());
								}
							}
							break;
					}
				}
			}
			catch
			{
				await synthesizer.SpeakTextAsync("Nessuna previsione");
			}
		}
	}
}