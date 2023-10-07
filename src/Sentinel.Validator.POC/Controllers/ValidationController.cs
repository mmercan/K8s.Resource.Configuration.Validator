using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Sentinel.Validator.POC.Repo;

namespace Sentinel.Validator.POC.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValidationController : ControllerBase
    {
        private readonly ILogger<ValidationController> _logger;
        private readonly ValidationRepo _validationRepo;

        public ValidationController(ILogger<ValidationController> logger, ValidationRepo validationRepo)
        {
            _logger = logger;
            _validationRepo = validationRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_validationRepo.ValidationModels);
        }
    }
}