using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTableApi.Context;
using TimeTableApi.Models;
using TimeTableApi.Services;

namespace TimeTableApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LessonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Lessons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessons()
        {
            return await _context.Lessons.ToListAsync();
        }

        [HttpGet("group/{id}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> FilterLessonsByGroup(int id)
        {
            var lessons = await _context.Lessons.Include(x=>x.Classroom).Include(x=>x.Teacher).Include(x=>x.Group).Where(x => x.GroupId == id).ToListAsync();
            return lessons;
        }

        [HttpGet("teacher/{id}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> FilterLessonsByTeacher(int id)
        {
            var lessons = await _context.Lessons.Include(x => x.Classroom).Include(x => x.Teacher).Include(x => x.Group).Where(x => x.TeacherId == id).ToListAsync();
            return lessons;
        }

        [HttpGet("classroom/{id}")]
        public async Task<ActionResult<IEnumerable<Lesson>>> FilterLessonsByClassroom(int id)
        {
            var lessons = await _context.Lessons.Include(x => x.Classroom).Include(x => x.Teacher).Include(x => x.Group).Where(x => x.ClassroomId == id).ToListAsync();
            return lessons;
        }


        // GET: api/Lessons/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);

            if (lesson == null)
            {
                return NotFound();
            }

            return lesson;
        }

        // PUT: api/Lessons/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLesson(int id, Lesson lesson)
        {
            if (id != lesson.Id)
            {
                return BadRequest();
            }

            _context.Entry(lesson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Lessons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lesson>> PostLesson(Lesson lesson)
        {
            List<string> errors = new List<string>();
            var sameClassroom = _context.Lessons.FirstOrDefault(x => x.ClassroomId == lesson.ClassroomId && 
            (   CompareTime.Compare(x.StartTime, lesson.StartTime) > -1 && CompareTime.Compare(x.EndTime, lesson.EndTime) < 1) && x.Week == lesson.Week && x.Day == lesson.Day);
            if(sameClassroom!= null)
            {
                errors.Add("The classroom is in use within that time period");
            }
            var sameGroup = _context.Lessons.FirstOrDefault(x => x.GroupId== lesson.GroupId &&
                (CompareTime.Compare(x.StartTime, lesson.StartTime) > -1 && CompareTime.Compare(x.EndTime, lesson.EndTime) < 1) && x.Week == lesson.Week && x.Day == lesson.Day);
            if (sameGroup != null)
            {
                errors.Add("The classroom is in use within that time period");
            }
            var sameTeacher = _context.Lessons.FirstOrDefault(x => x.TeacherId == lesson.TeacherId &&
                (CompareTime.Compare(x.StartTime, lesson.StartTime) > -1 && CompareTime.Compare(x.EndTime, lesson.EndTime) < 1) && x.Week == lesson.Week && x.Day == lesson.Day);
            if (sameTeacher != null)
            {
                errors.Add("The classroom is in use within that time period");
            }
            
            if(errors.Count > 0)
            {
                return BadRequest(errors);
            }
            else
            {
                _context.Lessons.Add(lesson);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetLesson", new { id = lesson.Id }, lesson);
            }
        }

        
        // DELETE: api/Lessons/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound();
            }

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }

    }
}
