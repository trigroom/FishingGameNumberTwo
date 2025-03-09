public struct GlobalTimeComponent
{
    public int currentDayTime;
    public int currentDay;
    public bool isNight;
    public bool lightStateIsChanged;
    public float changeGloabalLightTime;
    public float currentGlobalLightIntensity { get; set; }

    public float nightLightIntensity;
    public bool goToLightNight;

    public int levelsToRain;
    public WeatherType currentWeatherType { get; set; }

    public float lastRainDropTime;

    public float currentThunderLight;
    public bool thuderIsLighting;
    public float lastThunderTime;

    public enum WeatherType
    {
        none,
        rain,
        thunderstorm
    }
    // public float minutesToTimerTextCount;
}
