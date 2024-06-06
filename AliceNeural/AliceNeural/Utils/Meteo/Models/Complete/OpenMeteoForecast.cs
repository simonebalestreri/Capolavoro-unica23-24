using System.Text.Json.Serialization;

namespace AliceNeural.Utils.Meteo.Models.Complete
{
    // OpenMeteoForecast myDeserializedClass = JsonSerializer.Deserialize<OpenMeteoForecast>(myJsonResponse);
    public class Current
    {
        [JsonPropertyName("time")]
        public int? Time { get; set; }

        [JsonPropertyName("interval")]
        public int? Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double? Temperature2m { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public int? RelativeHumidity2m { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public double? ApparentTemperature { get; set; }

        [JsonPropertyName("is_day")]
        public int? IsDay { get; set; }

        [JsonPropertyName("precipitation")]
        public int? Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public int? Rain { get; set; }

        [JsonPropertyName("showers")]
        public int? Showers { get; set; }

        [JsonPropertyName("snowfall")]
        public int? Snowfall { get; set; }

        [JsonPropertyName("weather_code")]
        public int? WeatherCode { get; set; }

        [JsonPropertyName("cloud_cover")]
        public int? CloudCover { get; set; }

        [JsonPropertyName("pressure_msl")]
        public double? PressureMsl { get; set; }

        [JsonPropertyName("surface_pressure")]
        public double? SurfacePressure { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public double? WindSpeed10m { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public int? WindDirection10m { get; set; }

        [JsonPropertyName("wind_gusts_10m")]
        public double? WindGusts10m { get; set; }
    }

    public class CurrentUnits
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("interval")]
        public string? Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string? Temperature2m { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public string? RelativeHumidity2m { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public string? ApparentTemperature { get; set; }

        [JsonPropertyName("is_day")]
        public string? IsDay { get; set; }

        [JsonPropertyName("precipitation")]
        public string? Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public string? Rain { get; set; }

        [JsonPropertyName("showers")]
        public string? Showers { get; set; }

        [JsonPropertyName("snowfall")]
        public string? Snowfall { get; set; }

        [JsonPropertyName("weather_code")]
        public string? WeatherCode { get; set; }

        [JsonPropertyName("cloud_cover")]
        public string? CloudCover { get; set; }

        [JsonPropertyName("pressure_msl")]
        public string? PressureMsl { get; set; }

        [JsonPropertyName("surface_pressure")]
        public string? SurfacePressure { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public string? WindSpeed10m { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public string? WindDirection10m { get; set; }

        [JsonPropertyName("wind_gusts_10m")]
        public string? WindGusts10m { get; set; }
    }

    public class Daily
    {
        [JsonPropertyName("time")]
        public List<int>? Time { get; set; }

        [JsonPropertyName("weather_code")]
        public List<int>? WeatherCode { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public List<double>? Temperature2mMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<double>? Temperature2mMin { get; set; }

        [JsonPropertyName("apparent_temperature_max")]
        public List<double>? ApparentTemperatureMax { get; set; }

        [JsonPropertyName("apparent_temperature_min")]
        public List<double>? ApparentTemperatureMin { get; set; }

        [JsonPropertyName("sunrise")]
        public List<int>? Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public List<int>? Sunset { get; set; }

        [JsonPropertyName("daylight_duration")]
        public List<double>? DaylightDuration { get; set; }

        [JsonPropertyName("sunshine_duration")]
        public List<double>? SunshineDuration { get; set; }

        [JsonPropertyName("uv_index_max")]
        public List<double>? UvIndexMax { get; set; }

        [JsonPropertyName("uv_index_clear_sky_max")]
        public List<double>? UvIndexClearSkyMax { get; set; }

        [JsonPropertyName("precipitation_sum")]
        public List<double>? PrecipitationSum { get; set; }

        [JsonPropertyName("rain_sum")]
        public List<double>? RainSum { get; set; }

        [JsonPropertyName("showers_sum")]
        public List<double>? ShowersSum { get; set; }

        [JsonPropertyName("snowfall_sum")]
        public List<int>? SnowfallSum { get; set; }

        [JsonPropertyName("precipitation_hours")]
        public List<int>? PrecipitationHours { get; set; }

        [JsonPropertyName("precipitation_probability_max")]
        public List<int>? PrecipitationProbabilityMax { get; set; }

        [JsonPropertyName("wind_speed_10m_max")]
        public List<double>? WindSpeed10mMax { get; set; }

        [JsonPropertyName("wind_gusts_10m_max")]
        public List<double>? WindGusts10mMax { get; set; }

        [JsonPropertyName("wind_direction_10m_dominant")]
        public List<int>? WindDirection10mDominant { get; set; }

        [JsonPropertyName("shortwave_radiation_sum")]
        public List<double>? ShortwaveRadiationSum { get; set; }

        [JsonPropertyName("et0_fao_evapotranspiration")]
        public List<double>? Et0FaoEvapotranspiration { get; set; }
    }

    public class DailyUnits
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("weather_code")]
        public string? WeatherCode { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public string? Temperature2mMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public string? Temperature2mMin { get; set; }

        [JsonPropertyName("apparent_temperature_max")]
        public string? ApparentTemperatureMax { get; set; }

        [JsonPropertyName("apparent_temperature_min")]
        public string? ApparentTemperatureMin { get; set; }

        [JsonPropertyName("sunrise")]
        public string? Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public string? Sunset { get; set; }

        [JsonPropertyName("daylight_duration")]
        public string? DaylightDuration { get; set; }

        [JsonPropertyName("sunshine_duration")]
        public string? SunshineDuration { get; set; }

        [JsonPropertyName("uv_index_max")]
        public string? UvIndexMax { get; set; }

        [JsonPropertyName("uv_index_clear_sky_max")]
        public string? UvIndexClearSkyMax { get; set; }

        [JsonPropertyName("precipitation_sum")]
        public string? PrecipitationSum { get; set; }

        [JsonPropertyName("rain_sum")]
        public string? RainSum { get; set; }

        [JsonPropertyName("showers_sum")]
        public string? ShowersSum { get; set; }

        [JsonPropertyName("snowfall_sum")]
        public string? SnowfallSum { get; set; }

        [JsonPropertyName("precipitation_hours")]
        public string? PrecipitationHours { get; set; }

        [JsonPropertyName("precipitation_probability_max")]
        public string? PrecipitationProbabilityMax { get; set; }

        [JsonPropertyName("wind_speed_10m_max")]
        public string? WindSpeed10mMax { get; set; }

        [JsonPropertyName("wind_gusts_10m_max")]
        public string? WindGusts10mMax { get; set; }

        [JsonPropertyName("wind_direction_10m_dominant")]
        public string? WindDirection10mDominant { get; set; }

        [JsonPropertyName("shortwave_radiation_sum")]
        public string? ShortwaveRadiationSum { get; set; }

        [JsonPropertyName("et0_fao_evapotranspiration")]
        public string? Et0FaoEvapotranspiration { get; set; }
    }

    public class Hourly
    {
        [JsonPropertyName("time")]
        public List<int>? Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public List<double>? Temperature2m { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public List<int>? RelativeHumidity2m { get; set; }

        [JsonPropertyName("dew_point_2m")]
        public List<double>? DewPoint2m { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public List<double>? ApparentTemperature { get; set; }

        [JsonPropertyName("precipitation_probability")]
        public List<int>? PrecipitationProbability { get; set; }

        [JsonPropertyName("precipitation")]
        public List<double>? Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public List<double>? Rain { get; set; }

        [JsonPropertyName("showers")]
        public List<double>? Showers { get; set; }

        [JsonPropertyName("snowfall")]
        public List<int>? Snowfall { get; set; }

        [JsonPropertyName("snow_depth")]
        public List<int>? SnowDepth { get; set; }

        [JsonPropertyName("weather_code")]
        public List<int>? WeatherCode { get; set; }

        [JsonPropertyName("pressure_msl")]
        public List<double>? PressureMsl { get; set; }

        [JsonPropertyName("surface_pressure")]
        public List<double>? SurfacePressure { get; set; }

        [JsonPropertyName("cloud_cover")]
        public List<int>? CloudCover { get; set; }

        [JsonPropertyName("cloud_cover_low")]
        public List<int>? CloudCoverLow { get; set; }

        [JsonPropertyName("cloud_cover_mid")]
        public List<int>? CloudCoverMid { get; set; }

        [JsonPropertyName("cloud_cover_high")]
        public List<int>? CloudCoverHigh { get; set; }

        [JsonPropertyName("visibility")]
        public List<int>? Visibility { get; set; }

        [JsonPropertyName("evapotranspiration")]
        public List<double>? Evapotranspiration { get; set; }

        [JsonPropertyName("et0_fao_evapotranspiration")]
        public List<double>? Et0FaoEvapotranspiration { get; set; }

        [JsonPropertyName("vapour_pressure_deficit")]
        public List<double>? VapourPressureDeficit { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public List<double>? WindSpeed10m { get; set; }

        [JsonPropertyName("wind_speed_80m")]
        public List<double>? WindSpeed80m { get; set; }

        [JsonPropertyName("wind_speed_120m")]
        public List<double>? WindSpeed120m { get; set; }

        [JsonPropertyName("wind_speed_180m")]
        public List<double>? WindSpeed180m { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public List<int>? WindDirection10m { get; set; }

        [JsonPropertyName("wind_direction_80m")]
        public List<int>? WindDirection80m { get; set; }

        [JsonPropertyName("wind_direction_120m")]
        public List<int>? WindDirection120m { get; set; }

        [JsonPropertyName("wind_direction_180m")]
        public List<int>? WindDirection180m { get; set; }

        [JsonPropertyName("wind_gusts_10m")]
        public List<double>? WindGusts10m { get; set; }

        [JsonPropertyName("temperature_80m")]
        public List<double>? Temperature80m { get; set; }

        [JsonPropertyName("temperature_120m")]
        public List<double>? Temperature120m { get; set; }

        [JsonPropertyName("temperature_180m")]
        public List<double>? Temperature180m { get; set; }

        [JsonPropertyName("soil_temperature_0cm")]
        public List<double>? SoilTemperature0cm { get; set; }

        [JsonPropertyName("soil_temperature_6cm")]
        public List<double>? SoilTemperature6cm { get; set; }

        [JsonPropertyName("soil_temperature_18cm")]
        public List<double>? SoilTemperature18cm { get; set; }

        [JsonPropertyName("soil_temperature_54cm")]
        public List<double>? SoilTemperature54cm { get; set; }

        [JsonPropertyName("soil_moisture_0_to_1cm")]
        public List<double>? SoilMoisture0To1cm { get; set; }

        [JsonPropertyName("soil_moisture_1_to_3cm")]
        public List<double>? SoilMoisture1To3cm { get; set; }

        [JsonPropertyName("soil_moisture_3_to_9cm")]
        public List<double>? SoilMoisture3To9cm { get; set; }

        [JsonPropertyName("soil_moisture_9_to_27cm")]
        public List<double>? SoilMoisture9To27cm { get; set; }

        [JsonPropertyName("soil_moisture_27_to_81cm")]
        public List<double>? SoilMoisture27To81cm { get; set; }
    }

    public class HourlyUnits
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string? Temperature2m { get; set; }

        [JsonPropertyName("relative_humidity_2m")]
        public string? RelativeHumidity2m { get; set; }

        [JsonPropertyName("dew_point_2m")]
        public string? DewPoint2m { get; set; }

        [JsonPropertyName("apparent_temperature")]
        public string? ApparentTemperature { get; set; }

        [JsonPropertyName("precipitation_probability")]
        public string? PrecipitationProbability { get; set; }

        [JsonPropertyName("precipitation")]
        public string? Precipitation { get; set; }

        [JsonPropertyName("rain")]
        public string? Rain { get; set; }

        [JsonPropertyName("showers")]
        public string? Showers { get; set; }

        [JsonPropertyName("snowfall")]
        public string? Snowfall { get; set; }

        [JsonPropertyName("snow_depth")]
        public string? SnowDepth { get; set; }

        [JsonPropertyName("weather_code")]
        public string? WeatherCode { get; set; }

        [JsonPropertyName("pressure_msl")]
        public string? PressureMsl { get; set; }

        [JsonPropertyName("surface_pressure")]
        public string? SurfacePressure { get; set; }

        [JsonPropertyName("cloud_cover")]
        public string? CloudCover { get; set; }

        [JsonPropertyName("cloud_cover_low")]
        public string? CloudCoverLow { get; set; }

        [JsonPropertyName("cloud_cover_mid")]
        public string? CloudCoverMid { get; set; }

        [JsonPropertyName("cloud_cover_high")]
        public string? CloudCoverHigh { get; set; }

        [JsonPropertyName("visibility")]
        public string? Visibility { get; set; }

        [JsonPropertyName("evapotranspiration")]
        public string? Evapotranspiration { get; set; }

        [JsonPropertyName("et0_fao_evapotranspiration")]
        public string? Et0FaoEvapotranspiration { get; set; }

        [JsonPropertyName("vapour_pressure_deficit")]
        public string? VapourPressureDeficit { get; set; }

        [JsonPropertyName("wind_speed_10m")]
        public string? WindSpeed10m { get; set; }

        [JsonPropertyName("wind_speed_80m")]
        public string? WindSpeed80m { get; set; }

        [JsonPropertyName("wind_speed_120m")]
        public string? WindSpeed120m { get; set; }

        [JsonPropertyName("wind_speed_180m")]
        public string? WindSpeed180m { get; set; }

        [JsonPropertyName("wind_direction_10m")]
        public string? WindDirection10m { get; set; }

        [JsonPropertyName("wind_direction_80m")]
        public string? WindDirection80m { get; set; }

        [JsonPropertyName("wind_direction_120m")]
        public string? WindDirection120m { get; set; }

        [JsonPropertyName("wind_direction_180m")]
        public string? WindDirection180m { get; set; }

        [JsonPropertyName("wind_gusts_10m")]
        public string? WindGusts10m { get; set; }

        [JsonPropertyName("temperature_80m")]
        public string? Temperature80m { get; set; }

        [JsonPropertyName("temperature_120m")]
        public string? Temperature120m { get; set; }

        [JsonPropertyName("temperature_180m")]
        public string? Temperature180m { get; set; }

        [JsonPropertyName("soil_temperature_0cm")]
        public string? SoilTemperature0cm { get; set; }

        [JsonPropertyName("soil_temperature_6cm")]
        public string? SoilTemperature6cm { get; set; }

        [JsonPropertyName("soil_temperature_18cm")]
        public string? SoilTemperature18cm { get; set; }

        [JsonPropertyName("soil_temperature_54cm")]
        public string? SoilTemperature54cm { get; set; }

        [JsonPropertyName("soil_moisture_0_to_1cm")]
        public string? SoilMoisture0To1cm { get; set; }

        [JsonPropertyName("soil_moisture_1_to_3cm")]
        public string? SoilMoisture1To3cm { get; set; }

        [JsonPropertyName("soil_moisture_3_to_9cm")]
        public string? SoilMoisture3To9cm { get; set; }

        [JsonPropertyName("soil_moisture_9_to_27cm")]
        public string? SoilMoisture9To27cm { get; set; }

        [JsonPropertyName("soil_moisture_27_to_81cm")]
        public string? SoilMoisture27To81cm { get; set; }
    }

    public class OpenMeteoForecast
    {
        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double? GenerationtimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public int? UtcOffsetSeconds { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("timezone_abbreviation")]
        public string? TimezoneAbbreviation { get; set; }

        [JsonPropertyName("elevation")]
        public double? Elevation { get; set; }

        [JsonPropertyName("current_units")]
        public CurrentUnits? CurrentUnits { get; set; }

        [JsonPropertyName("current")]
        public Current? Current { get; set; }

        [JsonPropertyName("hourly_units")]
        public HourlyUnits? HourlyUnits { get; set; }

        [JsonPropertyName("hourly")]
        public Hourly? Hourly { get; set; }

        [JsonPropertyName("daily_units")]
        public DailyUnits? DailyUnits { get; set; }

        [JsonPropertyName("daily")]
        public Daily? Daily { get; set; }
    }

}
