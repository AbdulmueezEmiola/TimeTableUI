using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeTableApi.Services
{
    public class CompareTime
    {
        public static int Compare(string first,string second)
        {
            var firstValue = first.Split(":").Select(x => int.Parse(x)).ToList();
            var secondValue = second.Split(":").Select(x => int.Parse(x)).ToList();
            if(firstValue[0]>secondValue[0] && firstValue[1] > secondValue[1])
            {
                return 1;
            }else if(firstValue[0] == secondValue[0] && firstValue[1] == secondValue[1])
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
