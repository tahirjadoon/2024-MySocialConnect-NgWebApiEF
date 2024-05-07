using System.Net;

namespace MSC.Core.Dtos;

public class BusinessResponse
{
    public BusinessResponse()
    {
    }

    public BusinessResponse(HttpStatusCode httpStatusCode)
    {
        HttpStatusCode = httpStatusCode;
        Message = string.Empty;
    }

    public BusinessResponse(HttpStatusCode httpStatusCode, string message)
    {
        HttpStatusCode = httpStatusCode;
        Message = message;
    }

    public HttpStatusCode HttpStatusCode { get; set; }
    public string Message { get; set; }
}
