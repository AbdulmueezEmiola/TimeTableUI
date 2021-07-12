using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Models
{
    public class Lesson : Entity
    {
        public Lesson()
        {

        }
        public Lesson(Lesson lesson)
        {
            CourseName = lesson.CourseName;
            StartTime = lesson.StartTime;
            EndTime = lesson.EndTime;
            Week = lesson.Week;
            LessonType = lesson.LessonType;
            Day = lesson.Day;
            Teacher = lesson.Teacher;
            TeacherId = lesson.TeacherId;
            ClassroomId = lesson.ClassroomId;
            Classroom = lesson.Classroom;
            GroupId = lesson.GroupId;
            Group = lesson.Group;
        }
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
