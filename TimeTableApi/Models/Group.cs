using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Models
{
    public class Group:Entity
    {
        public int GroupNumber { get; set; }
        public int Year { get; set; }
        public string Faculty { get; set; }
    }
}
