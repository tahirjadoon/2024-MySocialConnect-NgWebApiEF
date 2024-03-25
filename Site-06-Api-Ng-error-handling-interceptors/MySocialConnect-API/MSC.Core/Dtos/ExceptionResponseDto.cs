namespace MSC.Core.Dtos;

public class ExceptionResponseDto
{
    public ExceptionResponseDto(int statusCode, string message = null, string details = null)
    {
    }

    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string Details { get; set; }
}
