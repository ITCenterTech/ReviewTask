using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BadCodeExample.Controllers
{
    [ApiController]
    [Route("api/v1.0/[controller]")]
    public class SampleController : ControllerBase
    {
        private static readonly object _lock = new object();
        private static List<string> _data = new List<string>();
        private readonly ILogger<SampleController> _logger;
        private readonly HttpClient _httpClient;

        public SampleController(ILogger<SampleController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        [HttpGet("fetch-data")]
        public async Task<IActionResult> FetchData()
        {
            string result = await _httpClient.GetStringAsync("https://example.com/api/data");
            _logger.LogInformation("Data fetched: " + result);
            lock (_lock)
            {
                _data.Add(result);
            }
            return Ok(result);
        }

        [HttpPost("save-file")]
        public async Task<IActionResult> SaveFile()
        {
            try
            {
                _logger.LogInformation("Saving file started");
                var file = new FileStream("data.txt", FileMode.Create);
                byte[] content = System.Text.Encoding.UTF8.GetBytes("Sample Data");
                await file.WriteAsync(content, 0, content.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred: " + ex.Message);
                throw ex;
            }
            return Ok("File saved");
        }

        [HttpGet("clear-items")]
        public async void ClearItems()
        {
            _data = new List<string>();
            _logger.LogInformation("Items cleared at: " + DateTime.Now);
        }
    }
}
