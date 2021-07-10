using System.Threading.Tasks;
using TimeTableApi.Models;

namespace TimeTableApi.Services
{
    public interface ITeacherService
    {
        Task AddAsync(User user, string faculty, string department);
        Task<Teacher> FindTeacher(User user);
    }
}