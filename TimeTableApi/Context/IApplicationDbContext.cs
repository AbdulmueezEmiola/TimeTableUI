using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TimeTableApi.Models;

namespace TimeTableApi.Context
{
    public interface IApplicationDbContext
    {

        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Lesson> Lessons { get; set; }

        public DbSet<Teacher> Teachers { get; set; }
    }
}