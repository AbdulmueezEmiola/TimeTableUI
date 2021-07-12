using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Models
{
    [Index(nameof(UserId),IsUnique = true)]

    public class Teacher:Entity
    {
        public string UserName { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string UserId { get; set; }
    }
}
