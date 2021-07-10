using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Models
{
    public class Create
    {
        public List<Classroom> Classrooms { get; set; }
        public List<Group> Groups { get; set; }
    }
}
