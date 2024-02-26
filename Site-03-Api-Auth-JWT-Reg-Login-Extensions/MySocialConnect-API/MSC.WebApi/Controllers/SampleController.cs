using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MSC.Core.Dtos.Helper;
using MSC.Core.Extensions;

namespace MSC.WebApi.Controller;

// /api/sample
public class SampleController : BaseApiController
{
    private readonly IOptions<EnvConfig> _envConfig;
    private readonly IConfiguration _config;

    public SampleController(IOptions<EnvConfig> envConfig, IConfiguration config)
    {
        _envConfig = envConfig;
        _config = config;
    }

    [HttpPost("sample1")]
    public async Task<ActionResult<bool>> Sample1()
    {
        //IConfiguration
        var config = _config;
        //using Ioptions
        var howdy = _envConfig;
        return Ok(true);
    }

    [HttpPost("sample2")]
    public async Task<ActionResult<bool>> Sample2()
    {
        //IConfiguration with extension
        var config = _config;
        var tokenKey = _config.GetTokenKey();
        //using Ioptions
        var howdy = _envConfig;
        return Ok(true);
    }
}
