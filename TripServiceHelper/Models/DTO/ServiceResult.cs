namespace TripServiceHelper.Models.DTOs;

public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";
    public int Code { get; set; } = 200;

    public static ServiceResult Success(string message = "成功")
        => new() { IsSuccess = true, Code = 200, Message = message };

    public static ServiceResult Fail(string message, int code = 400)
        => new() { IsSuccess = false, Code = code, Message = message };
}

//泛型版本 需要回傳資料時使用
public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";
    public int Code { get; set; } = 200;
    public T? Data { get; set; }

    public static ServiceResult<T> Success(T data, string message = "成功")
        => new() { IsSuccess = true, Code = 200, Message = message, Data = data };

    public static ServiceResult<T> Fail(string message, int code = 400)
        => new() { IsSuccess = false, Code = code, Message = message };
}