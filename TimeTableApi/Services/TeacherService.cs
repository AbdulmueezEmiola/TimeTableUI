using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTableApi.Context;
using TimeTableApi.Models;

namespace TimeTableApi.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _context;
        public TeacherService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(User user,string faculty,string department)
        {
            Teacher teacher = new Teacher
            {
                UserName = user.UserName,
                Department = faculty,
                Faculty = department,
                UserId = user.Id
            };
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task<Teacher> FindTeacher(User user)
        {
            Teacher teacher = await _context.Teachers.FirstOrDefaultAsync(x => x.UserId == user.Id);
            return teacher;
        }
    }
}
