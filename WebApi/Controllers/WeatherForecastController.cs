using CodeBlocksMiddleware;
using Microsoft.AspNetCore.Mvc;
 

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {

            // Simulate a file operation that could cause an IOException
            ReadFile("non_existent_file.txt");
            return Ok();

           
                // Catch and handle the IOException
                //Console.WriteLine("An IOException occurred: " + ex.Message);
                //return BadRequest(ex.Message);
            
        }

        public static void ReadFile(string filePath)
        {
           
                // Try to open and read the file
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line = sr.ReadLine();
                    Console.WriteLine("File content: " + line);
                }
            
                // If an IOException occurs during the file operation, rethrow it
                //throw new IOException("Error reading file: " + filePath, ex);
            
        }
    }
}