using System;
using System.Collections.Generic;
using System.Text;

namespace DT.Domain.DTO
{
    public class ServiceResponse
    {
        public static ServiceResponse SuccessResponse() => SuccessResponse(null);

        public static ServiceResponse SuccessResponse(object payload) => new ServiceResponse { success = true, data = payload };

        public static ServiceResponse SuccessResponse(string message, object payload) => new ServiceResponse { success = true, data = payload, message = message };

        public static ServiceResponse ErrorResponse(string message) => new ServiceResponse { success = false, message = message, data = null };

        public static ServiceResponse ErrorResponse(Exception exception) => ErrorResponse(exception.Message);

        public object data { get; set; }
        public string message { get; set; }
        public bool success { get; set; }
        public List<ErrorMessage> errorList { get; set; }
    }

    public class ErrorMessage
    {
        public string error { get; set; }
        public string value { get; set; }
    }
}
