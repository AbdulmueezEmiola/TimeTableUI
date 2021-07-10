using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Models
{
    public class Lesson : Entity
    {
        public string CourseName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int Week { get; set; }
        public LessonType LessonType { get; set; }

        public Day Day { get; set; }
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; }

        [ForeignKey("Classroom")]
        public int ClassroomId { get; set; }
        public virtual Classroom Classroom { get; set; }

        [ForeignKey("Group")]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; }


    }
    public enum LessonType { LABORATORY, LECTURE, PRACTICAL }   
    public enum Day { Monday,Tuesday,Wednesday,Thursday,Friday,Saturday}

}
