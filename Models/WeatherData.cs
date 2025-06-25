namespace HellowWebAppSR.Models
{
    public class WeatherData
    {
        public string? Name { get; set; }
        public MainData? Main { get; set; }
        public List<WeatherDescription>? Weather { get; set; }
        public string? Description { get; set; } // If needed elsewhere
    }

    public class MainData
    {
        public double Temp { get; set; }
        public double FeelsLike { get; set; } // Use FeelsLike, not Feels_Like
        public double TempMin { get; set; }
        public double TempMax { get; set; }
        public int Humidity { get; set; } // Add this property
    }

    public class WeatherDescription
    {
        public string? Description { get; set; }
    }
}