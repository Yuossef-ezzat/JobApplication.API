using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Abstractions.ResultPattern
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public static readonly Error None = new(0, string.Empty);

        public Error(int code, string message)
        {
            Message = message;
            Code = code;
        }
    }
}
