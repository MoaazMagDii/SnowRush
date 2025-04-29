using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")] // https://Localhost:5001/api/{controllerName}
    [ApiController]
    public class BaseApiController : ControllerBase
    {
    }
}
