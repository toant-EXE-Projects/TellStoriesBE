using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.ResponseModel
{
    public class APIResponse<T> where T : class
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        
        public APIResponse()
        {
            Errors = new List<string>();
        }


        public static APIResponse<T> SuccessResponse(T data, string message = "")
        {
            return new APIResponse<T> { Success = true, Data = data, Message = message};
        }
        public static APIResponse<object> SuccessResponse(object? data = null, string message = "")
        {
            return new APIResponse<object> { Success = true, Data = data, Message = message };
        }

        public static APIResponse<T> ErrorResponse(List<string> errors, string message = "")
        {
            return new APIResponse<T> { Success = false, Errors = errors, Message = message};
        }

        public static APIResponse<object> ErrorResponse(string message = "")
        {
            return new APIResponse<object>
            {
                Success = false,
                Data = null,
                Message = message,
                Errors = new List<string>()
            };
        }
    }
}
