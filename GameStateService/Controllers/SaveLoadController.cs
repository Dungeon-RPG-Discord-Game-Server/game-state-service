using GameStateService.Azure;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using GameStateService.Models;
using GameStateService.Services;
using GameStateService.Utils;
using GameStateService.Azure;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaveLoadController : ControllerBase
    {
        private readonly CosmosDbWrapper _cosmosDbWrapper;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;

        public SaveLoadController(IConfiguration configuration)
        {
            _configuration = configuration;
            if (null == _configuration)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            string serviceName = configuration["Logging:ServiceName"];
            _logger = new Logger(serviceName);
            _cosmosDbWrapper = new CosmosDbWrapper(configuration);
        }

    }

    public class LoadResponse
    {
        public string UserId { get; set; }
        public int SelectedOption { get; set; }
    }
}
