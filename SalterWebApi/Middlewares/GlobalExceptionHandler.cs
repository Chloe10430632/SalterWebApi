using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace SalterWebApi.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        //加入環境判斷
        private readonly IHostEnvironment? _env;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        // 注入 Logger 紀錄錯誤，注入 Environment 判斷開發/正式環境
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception, CancellationToken cancellationToken)
        {
            // 1. 紀錄錯誤到後端終端機/日誌，方便除錯
            _logger.LogError(exception, $"捕捉到未處理的異常: {exception. Message}", exception.Message);


            // 根據異常類型決定狀態碼與標題
            var (statusCode, title) = exception switch
            {
                // 當 Service 拋出 KeyNotFoundException 時，回傳 404
                KeyNotFoundException => (StatusCodes.Status404NotFound, "找不到資源"),

                // 當參數錯誤時（例如格式不對），回傳 400
                ArgumentException => (StatusCodes.Status400BadRequest, "請確認請求的格式正確"),

                // 其他未預期的錯誤，回傳 500
                _ => (StatusCodes.Status500InternalServerError, "伺服器發生內部錯誤")
            };

            // 2. 建立符合 RFC 7807 標準的錯誤物件
            var problemDetails = new ProblemDetails
            {
                Title = title,
                Status = statusCode, // 預設 500 錯誤
                Instance = httpContext.Request.Path,
                // 如果是開發環境，秀出詳細報錯；正式環境則給模糊提示（安全性考量）
                Detail = _env.IsDevelopment() ? exception.Message : "請稍後再試，或聯絡系統管理員。"
            };

            // 3. 設定 Response Header
            httpContext.Response.StatusCode = statusCode;

            // 4. 將物件轉為 JSON 回傳給 Angular 前端
            await httpContext.Response
                .WriteAsJsonAsync(problemDetails, cancellationToken);

            // 回傳 true 告知系統：我們已經處理好這個錯誤了，不用再往外丟
            return true;
        }
    }

}

