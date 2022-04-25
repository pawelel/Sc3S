namespace Sc3S.Helpers;

public class ServiceResponse<T>
{
    public ServiceResponse(bool success, T? data, string message)
    {
        Success = success;
        Data = data;
        Message = message;
    }

    public bool Success { get; set; } = true;
    public T? Data { get; set; } 
    public string Message { get; set; }
}
public class ServiceResponse
{
    public ServiceResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; set; } = true;
    public string Message { get; set; } 
}
