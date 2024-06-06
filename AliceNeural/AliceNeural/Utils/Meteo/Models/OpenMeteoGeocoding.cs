﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AliceNeural.Utils.Meteo.Models
{
    // OpenMeteoGeocoding myDeserializedClass = JsonSerializer.Deserialize<OpenMeteoGeocoding>(myJsonResponse);
    public class Result
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("elevation")]
        public double? Elevation { get; set; }

        [JsonPropertyName("feature_code")]
        public string? FeatureCode { get; set; }

        [JsonPropertyName("country_code")]
        public string? CountryCode { get; set; }

        [JsonPropertyName("admin1_id")]
        public int? Admin1Id { get; set; }

        [JsonPropertyName("admin3_id")]
        public int? Admin3Id { get; set; }

        [JsonPropertyName("admin4_id")]
        public int? Admin4Id { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("population")]
        public int? Population { get; set; }

        [JsonPropertyName("postcodes")]
        public List<string>? Postcodes { get; set; }

        [JsonPropertyName("country_id")]
        public int? CountryId { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("admin1")]
        public string? Admin1 { get; set; }

        [JsonPropertyName("admin3")]
        public string? Admin3 { get; set; }

        [JsonPropertyName("admin4")]
        public string? Admin4 { get; set; }

        [JsonPropertyName("admin2_id")]
        public int? Admin2Id { get; set; }

        [JsonPropertyName("admin2")]
        public string? Admin2 { get; set; }
    }

    public class OpenMeteoGeocoding
    {
        [JsonPropertyName("results")]
        public List<Result>? Results { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double? GenerationtimeMs { get; set; }
    }


}
