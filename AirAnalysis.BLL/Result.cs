using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAnalysis.BLL
{
    public class Result<T>
    {
        public T? Data { get; set; }
        public string? Error { get; set; }
        public bool IsSuccess => Error == null;
    }

}
