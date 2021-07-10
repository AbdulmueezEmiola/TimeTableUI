using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Models
{
    public class AuthenticationModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public int? TeacherId {get;set;}
    }
}
