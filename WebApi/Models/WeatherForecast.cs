using System.Collections;
using Microsoft.AspNetCore.Mvc;

namespace CodeBlocksMiddleware
{
    public class WeatherForecast : IEnumerable
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}