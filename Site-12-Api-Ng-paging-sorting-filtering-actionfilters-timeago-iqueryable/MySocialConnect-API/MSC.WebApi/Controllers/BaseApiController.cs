using Microsoft.AspNetCore.Mvc;
using MSC.Core.ActionFilters;

namespace MSC.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
[ServiceFilter(typeof(LogUserActivityFilter))]
public class BaseApiController : ControllerBase
{

}
