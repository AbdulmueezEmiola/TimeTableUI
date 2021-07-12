using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

            List<string> errors = new List<string>();
            if (CompareTime.Compare(lesson.EndTime, lesson.StartTime) < 1)
            {
                errors.Add("End Time must be greater than start time");
                return BadRequest(errors);
            }
            if (lesson.Week == 3)
            {
                errors.Add("Choose either even or odd weeks");
                return BadRequest(errors);
            }
            else
            {
                errors = PutLessonPerWeek(lesson);

                if (errors.Count > 0)
                {
                    return BadRequest(errors);
                }                
                _context.Entry(lesson).State = EntityState.Modified;
            }

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
        [Authorize(Roles ="Teacher")]
        [HttpPost]
        public async Task<ActionResult<Lesson>> PostLesson(Lesson lesson)
        {
            List<string> errors = new List<string>();
            if(CompareTime.Compare(lesson.EndTime,lesson.StartTime)<1)
            {
                errors.Add("End Time must be greater than start time");
                return BadRequest(errors);
            }
            if (lesson.Week == 3)
            {
                Lesson lessonOdd = new Lesson(lesson);
                lessonOdd.Week = 1;
                errors = errors.Concat(PostLessonPerWeek(lessonOdd)).ToList();

                Lesson lessonEven = new Lesson(lesson);
                lessonEven.Week = 2;
                errors = errors.Concat(PostLessonPerWeek(lessonEven)).ToList();

                if (errors.Count > 0)
                {
                    return BadRequest(errors);
                }
                else
                {
                    _context.Lessons.Add(lessonEven);
                    _context.Lessons.Add(lessonOdd);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetLesson", new { evenId = lessonEven.Id,oddId = lessonOdd.Id }, lesson);
                }
            }
            else
            {
                errors = PostLessonPerWeek(lesson);
                if (errors.Count > 0)
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
            
            
        }

        private List<string> PostLessonPerWeek(Lesson lesson)
        {
            List<string> errors = new List<string>();
            var sameClassroom = _context.Lessons.AsEnumerable().FirstOrDefault(x => x.ClassroomId == lesson.ClassroomId && CompareTime.CheckTimeOverlap(x, lesson));
            var weekType = lesson.Week == 1? "Odd week" : "Even week";
            if (sameClassroom != null)
            {
                errors.Add("The classroom is in use within that time period "+weekType);
            }
            var sameGroup = _context.Lessons.AsEnumerable().FirstOrDefault(x => x.GroupId == lesson.GroupId && CompareTime.CheckTimeOverlap(x, lesson));
            if (sameGroup != null)
            {
                errors.Add("The group has a class within that time period "+weekType);
            }
            var sameTeacher = _context.Lessons.AsEnumerable().FirstOrDefault(x => x.TeacherId == lesson.TeacherId && CompareTime.CheckTimeOverlap(x, lesson));
            if (sameTeacher != null)
            {
                errors.Add("The teacher has a class within that time period "+weekType);
            }
            return errors;
        }

        private List<string> PutLessonPerWeek(Lesson lesson)
        {
            List<string> errors = new List<string>();
            var sameClassroom = _context.Lessons.AsNoTracking().AsEnumerable().FirstOrDefault(x => x.ClassroomId == lesson.ClassroomId && CompareTime.CheckTimeOverlap(x, lesson) && lesson.Id != x.Id);
            var weekType = lesson.Week == 1 ? "Odd week" : "Even week";
            if (sameClassroom != null)
            {
                errors.Add("The classroom is in use within that time period " + weekType);
            }
            var sameGroup = _context.Lessons.AsNoTracking().AsEnumerable().FirstOrDefault(x => x.GroupId == lesson.GroupId && CompareTime.CheckTimeOverlap(x, lesson) && lesson.Id != x.Id);
            if (sameGroup != null)
            {
                errors.Add("The group has a class within that time period " + weekType);
            }
            var sameTeacher = _context.Lessons.AsNoTracking().AsEnumerable().FirstOrDefault(x => x.TeacherId == lesson.TeacherId && CompareTime.CheckTimeOverlap(x, lesson) && lesson.Id != x.Id);
            if (sameTeacher != null)
            {
                errors.Add("The teacher has a class within that time period " + weekType);
            }
            return errors;
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
