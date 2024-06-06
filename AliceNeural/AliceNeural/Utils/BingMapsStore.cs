using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AliceNeural.Utils
{
	public class BingMapsStore
	{
		[JsonPropertyName("api_key")]
		public string APIKeyValue { get; set; } = string.Empty;
	}
}
