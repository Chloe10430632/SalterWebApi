namespace TripServiceHelper.Models.DTOs;

public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";

    public static ServiceResult Success(string message = "成功")
        => new() { IsSuccess = true, Message = message };

    public static ServiceResult Fail(string message)
        => new() { IsSuccess = false, Message = message };
}

// 泛型版本 需要回傳資料時使用
public class ServiceResult<T> 
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";
    public T? Data { get; set; }

    public static ServiceResult<T> Success(T data, string message = "成功")
        => new() { IsSuccess = true, Message = message, Data = data };

    public static ServiceResult<T> Fail(string message)
        => new() { IsSuccess = false, Message = message };
}