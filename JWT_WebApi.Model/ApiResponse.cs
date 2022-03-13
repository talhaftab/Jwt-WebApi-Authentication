using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JWT.WebApi.Model
{
    public class ApiResponse<T>
    {
        public T? Content { get; set; }
        public string? Message { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}
