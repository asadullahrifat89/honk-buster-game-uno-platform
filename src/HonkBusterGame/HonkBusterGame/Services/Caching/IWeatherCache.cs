using HonkBusterGame.DataContracts;
using System.Collections.Immutable;

namespace HonkBusterGame.Services.Caching
{
    public interface IWeatherCache
    {
        ValueTask<IImmutableList<WeatherForecast>> GetForecast(CancellationToken token);
    }
}