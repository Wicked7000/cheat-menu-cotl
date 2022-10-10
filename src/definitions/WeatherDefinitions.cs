using HarmonyLib;

namespace CheatMenu;

[CheatCategory(CheatCategoryEnum.WEATHER)]
public class WeatherDefinitions : IDefinition{
    [CheatDetails("Weather: Rain", "Set weather to raining")]
    public static void WeatherRain(){
        WeatherController.Instance.SetRain();
    }

    [CheatDetails("Weather: Windy", "Set weather to windy")]
    public static void WeatherWindy(){
        WeatherController.Instance.SetWind();
    }

    [CheatDetails("Weather: Clear", "Set weather to clear")]
    public static void WeatherClear(){
        WeatherController.isRaining = false;
        Traverse.Create(WeatherController.Instance).Field("isRaining").SetValue(false);
        Traverse.Create(WeatherController.Instance).Field("RainIntensity").SetValue(0f);
        Traverse.Create(WeatherController.Instance).Field("windSpeed").SetValue(0f);
        Traverse.Create(WeatherController.Instance).Field("windDensity").SetValue(0f);
        Traverse.Create(WeatherController.Instance).Field("IsActive").SetValue(false);
        Traverse.Create(WeatherController.Instance).Field("weatherChanged").SetValue(true);
        WeatherController.Instance.CheckWeather();
    }
}