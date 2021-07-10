using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Models.Constants
{
    public class Authorization
    {
        public enum Roles
        {
            Administrator,
            Teacher,
            User
        }
        public const string default_username = "user";
        public const string default_email = "user@timetableapi.com";
        public const string default_password = "Pa$$w0rd.";
        public const Roles default_role = Roles.User;
    }
}
