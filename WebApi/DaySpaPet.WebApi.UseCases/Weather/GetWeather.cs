using DaySpaPet.WebApi.Core.Domain.Weather;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaySpaPet.WebApi.Core.Domain.Weather;

public sealed class GetWeather {
  public static async ValueTask<IQueryable<WeatherForecast>> Execute(DateOnly date) {
    // TODO: Replace with use of infrastructure layer via dependency inversion

    // Simulate asynchronous loading to demonstrate a loading indicator
    await Task.Delay(500);

    var startDate = DateOnly.FromDateTime(DateTime.Now);
    var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
#pragma warning disable CA5394 // Do not use insecure randomness
    var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast {
      Date = startDate.AddDays(index),
      TemperatureC = Random.Shared.Next(-20, 55),
      Summary = summaries[Random.Shared.Next(summaries.Length)]
    }).AsQueryable();
#pragma warning restore CA5394 // Do not use insecure randomness
    return forecasts;
  }
}