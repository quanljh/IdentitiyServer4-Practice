using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("[controller]")]
[Authorize]
public class NoPolicyController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return new JsonResult(User.Claims.Select(o=> new {o.Type, o.Value}));
    }
}