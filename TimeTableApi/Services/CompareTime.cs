using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTableApi.Models;

namespace TimeTableApi.Services
{
    public class CompareTime
    {
        public static int Compare(string first,string second)
        {
            var firstValue = first.Split(":").Select(x => int.Parse(x)).ToList();
            var secondValue = second.Split(":").Select(x => int.Parse(x)).ToList();
            if(firstValue[0]>secondValue[0])
            {
                return 1;
            }
            else if (firstValue[0] == secondValue[0] && firstValue[1] > secondValue[1])
            {
                return 1;
            }
            else if(firstValue[0] == secondValue[0] && firstValue[1] == secondValue[1])
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public static bool CheckTimeOverlap(Lesson lessonA, Lesson lessonB)
        {
            if (lessonA.Week != lessonB.Week)
            {
                return false;
            }else if(lessonA.Day != lessonB.Day)
            {
                return false;
            }
            else if(Compare(lessonA.StartTime,lessonB.StartTime) == -1 && Compare(lessonA.EndTime, lessonB.StartTime) == -1)
            {
                return false;
            }else if(Compare(lessonA.StartTime, lessonB.EndTime) == 1 && Compare(lessonA.StartTime, lessonB.EndTime) == 1)
            {
                return false;
            }
            return true;             
        }
    }
}
