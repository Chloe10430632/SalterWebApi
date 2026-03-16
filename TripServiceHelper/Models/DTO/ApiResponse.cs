namespace TripServiceHelper.Models.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int Code { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Details { get; set; }

    public static ApiResponse<T> Ok(T data, string message = "成功")
        => new() { Success = true, Code = 200, Message = message, Data = data };

    public static ApiResponse<T> Fail(string message, int code = 400, List<string>? details = null)
        => new() { Success = false, Code = code, Message = message, Details = details };
}