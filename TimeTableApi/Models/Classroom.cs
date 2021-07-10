using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Models
{
    public class Classroom:Entity
    {
        public int roomNumber { get; set; }
        public string Department { get; set; }
        public string Faculty { get; set; }
        public string BuildingNumber { get; set; }
    }
}
