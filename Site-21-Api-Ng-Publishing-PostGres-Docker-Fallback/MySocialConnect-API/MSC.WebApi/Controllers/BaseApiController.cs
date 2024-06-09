using Microsoft.AspNetCore.Mvc;
using MSC.Core.ActionFilters;

namespace MSC.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(LogUserActivityFilter))]
public class BaseApiController : ControllerBase
{

}
