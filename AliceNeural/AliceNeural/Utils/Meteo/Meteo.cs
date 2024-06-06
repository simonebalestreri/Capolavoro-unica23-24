using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using AliceNeural.Utils.Meteo.Models;

namespace AliceNeural.Utils.Meteo
{
	
		public static class Meteo
		{
			/// <summary>
			/// Stampa value se questo non è nullo. Nel caso in cui value sia nullo stampa stringIfNull. Se stringIfNull è null viene usata string.Empty
			/// </summary>
			/// <param name="value">Valore da stampare</param>
			/// <param name="stringIfNull">Stringa da usare se value è null</param>
			/// <returns>Restituisce una stringa da stampare</returns>
			public static string Display(object? value, string? stringIfNull)
			{
				if (value is null)
				{
					if (stringIfNull is null)
					{
						return string.Empty;
					}
					return stringIfNull;
				}
				else
				{
					if (stringIfNull is null)
					{
						return value.ToString() ?? string.Empty;
					}
					return value.ToString() ?? stringIfNull;
				}
			}
			/// <summary>
			/// https://stackoverflow.com/questions/249760/how-can-i-convert-a-unix-timestamp-to-datetime-and-vice-versa
			/// https://stackoverflow.com/a/250400
			/// https://www.epochconverter.com/
			/// </summary>
			/// <param name="unixTimeStamp"></param>
			/// <returns></returns>
			public static DateTime? UnixTimeStampToDateTime(double? unixTimeStamp)
			{
				// Unix timestamp is seconds past epoch
				if (unixTimeStamp != null)
				{
					DateTime dateTime = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
					dateTime = dateTime.AddSeconds((double)unixTimeStamp).ToLocalTime();
					return dateTime;
				}
				return null;
			}
			/// <summary>
			/// Geolocalizza un indirizzo con i parametri forniti, usando il servizio di geolocalizzazione di open-meteo
			/// https://open-meteo.com/en/docs/geocoding-api
			/// </summary>
			/// <param name="client">l'HttpClient utilizzato per effettuare le richieste all'endpoint delle api di geolocalizzazione </param>
			/// <param name="name">nome della località</param>
			/// <param name="language">lingua in cui viene presentato il risultato. Codice di due lettere</param>
			/// <param name="count">Numero di risultati restituiti. Il valore massimo è 100 e il minimo è 1</param>
			/// <returns></returns>
			public static async Task<(double? lat, double? lon)?> GeocodeByOpenMeteo(HttpClient client, string? name, string? language = "it", int count = 1)
			{
				string? nameEncoded = HttpUtility.UrlEncode(name);
				string geocodingUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={nameEncoded}&count={count}&language={language}";
				try
				{
					HttpResponseMessage responseGeocoding = await client.GetAsync($"{geocodingUrl}");
					if (responseGeocoding.IsSuccessStatusCode)
					{
						OpenMeteoGeocoding? geocodingResult = await responseGeocoding.Content.ReadFromJsonAsync<OpenMeteoGeocoding>();
						if (geocodingResult != null && geocodingResult.Results?.Count > 0)
						{
							return (geocodingResult.Results[0].Latitude, geocodingResult.Results[0].Longitude);
						}
					}
					return null;
				}
				catch (Exception ex)
				{
					if (ex is HttpRequestException || ex is ArgumentException)
					{
						Debug.WriteLine(ex.Message + "\nIl recupero dei dati dal server non è riuscito");
					}
				}
				return null;
			}
			/// <summary>
			/// Restituisce la descrizione testuale della previsione meteo a partire dal codice di previsione
			/// </summary>
			/// <param name="code">Codice di previsione meteo</param>
			/// <returns></returns>
			public static string? WMOCodesInt(int? code)
			{
				string? result = code switch
				{
					0 => "clear sky",
					1 => "mainly clear",
					2 => "partly cloudy",
					3 => "overcast",
					45 => "fog",
					48 => "depositing rime fog",
					51 => "drizzle: light intensity",
					53 => "drizzle: moderate intensity",
					55 => "drizzle: dense intensity",
					56 => "freezing drizzle: light intensity",
					57 => "freezing drizzle: dense intensity",
					61 => "rain: slight intensity",
					63 => "rain: moderate intensity",
					65 => "rain: heavy intensity",
					66 => "freezing rain: light intensity",
					67 => "freezing rain: heavy intensity",
					71 => "snow fall: slight intensity",
					73 => "snow fall: moderate intensity",
					75 => "snow fall: heavy intensity",
					77 => "snow grains",
					80 => "rain showers: slight",
					81 => "rain showers: moderate",
					82 => "rain showers: violent",
					85 => "snow showers slight",
					86 => "snow showers heavy",
					95 => "thunderstorm: slight or moderate",
					96 => "thunderstorm with slight hail",
					99 => "thunderstorm with heavy hail",
					_ => null,
				};
				return result;
			}
			/// <summary>
			/// Restituisce la descrizione testuale della previsione meteo a partire dal codice di previsione in italiano
			/// </summary>
			/// <param name="code">Codice di previsione meteo</param>
			/// <returns></returns>
			public static string? WMOCodesIntIT(int? code)
			{
				string? result = code switch
				{
					0 => "cielo sereno",
					1 => "prevalentemente limpido",
					2 => "parzialmente nuvoloso",
					3 => "coperto",
					45 => "nebbia",
					48 => "nebbia con brina",
					51 => "pioggerellina di scarsa intensità",
					53 => "pioggerellina di moderata intensità",
					55 => "pioggerellina intensa",
					56 => "pioggerellina gelata di scarsa intensità",
					57 => "pioggerellina gelata intensa",
					61 => "pioggia di scarsa intensità",
					63 => "pioggia di moderata intensità",
					65 => "pioggia molto intensa",
					66 => "pioggia gelata di scarsa intensità",
					67 => "pioggia gelata intensa",
					71 => "nevicata di lieve entità",
					73 => "nevicata di media entità",
					75 => "nevicata intensa",
					77 => "granelli di neve",
					80 => "deboli rovesci di pioggia",
					81 => "moderati rovesci di pioggia",
					82 => "violenti rovesci di pioggia",
					85 => "leggeri rovesci di neve",
					86 => "pesanti rovesci di neve",
					95 => "temporale lieve o moderato",
					96 => "temporale con lieve grandine",
					99 => "temporale con forte grandine",
					_ => null,
				};
				return result;
			}
			public static string ExtractSummaryFromJSON(string wikiSummary)
			{
				using JsonDocument document = JsonDocument.Parse(wikiSummary);
				JsonElement root = document.RootElement;
				JsonElement query = root.GetProperty("query");
				JsonElement pages = query.GetProperty("pages");
				//per prendere il primo elemento dentro pages, creo un enumeratore delle properties
				JsonElement.ObjectEnumerator enumerator = pages.EnumerateObject();
				//quando si crea un enumeratore su una collection, bisogna farlo avanzare di una posizione per portarlo sul primo elemento della collection
				//il primo elemento corrisponde all'id della pagina all'interno dell'oggetto pages
				if (enumerator.MoveNext())
				{
					//accedo all'elemento
					JsonElement targetJsonElem = enumerator.Current.Value;
					if (targetJsonElem.TryGetProperty("extract", out JsonElement extract))
					{
						return extract.GetString() ?? string.Empty;
					}
				}
				return string.Empty;
			}

		}
}
