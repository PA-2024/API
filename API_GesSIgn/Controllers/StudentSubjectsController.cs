using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class StudentSubjectsController : ControllerBase
{
    private readonly MonDbContext _context;

    public StudentSubjectsController(MonDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// POST: api/StudentSubjects
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [RoleRequirement("Gestion Ecole")]
    public async Task<IActionResult> AddStudentToSubject([FromBody] StudentSubjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (await _context.StudentSubjects.AnyAsync(ss => ss.StudentSubject_StudentId == request.Student_Id && ss.StudentSubject_SubjectId == request.Subject_Id))
        {
            return StatusCode(409);
        }

        var studentSubject = new StudentSubject
        {
            StudentSubject_StudentId = request.Student_Id,
            StudentSubject_SubjectId = request.Subject_Id
        };

        _context.StudentSubjects.Add(studentSubject);
        await _context.SaveChangesAsync();

        var sh = await _context.SubjectsHour
                .Include(s => s.SubjectsHour_Subjects)
                .Where(s => s.SubjectsHour_Subjects.Subjects_Id == request.Subject_Id && s.SubjectsHour_DateStart > DateTime.Now.AddDays(-1))
                .ToListAsync();
        if (sh.Count > 0)
        {
            List<Presence> presences  = new List<Presence>();
            foreach (var s in sh)
            {
                var presence = new List<Presence>();
                presences.Add(new Presence
                {
                    Presence_Student_Id = request.Student_Id,
                    Presence_SubjectsHour_Id = s.SubjectsHour_Id,
                });
            }
            _context.Presences.AddRange(presences);
            await _context.SaveChangesAsync();
        }

        return StatusCode(201); 
    }

    /// <summary>
    /// POST: api/StudentSubjects/List
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("List")]
    [RoleRequirement("Gestion Ecole")]
    public async Task<IActionResult> AddStudentsToSubject([FromBody] AddStudentsToSubjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        List<StudentSubject> studentSubjects = new List<StudentSubject>();
        
        foreach (var studentId in request.StudentIds)
        {
            if (await _context.StudentSubjects.AnyAsync(ss => ss.StudentSubject_StudentId == studentId && ss.StudentSubject_SubjectId == request.Subject_Id))
            {
                return StatusCode(409);
            }
            StudentSubject add = new StudentSubject();
            add.StudentSubject_StudentId = studentId;
            add.StudentSubject_SubjectId = request.Subject_Id;

            studentSubjects.Add(add);

        }
        _context.StudentSubjects.AddRange(studentSubjects);
        await _context.SaveChangesAsync();

        var sh = await _context.SubjectsHour
               .Include(s => s.SubjectsHour_Subjects)
               .Where(s => s.SubjectsHour_Subjects.Subjects_Id == request.Subject_Id && s.SubjectsHour_DateStart > DateTime.Now.AddDays(-1))
               .ToListAsync();
        if (sh.Count > 0)
        {
            List<Presence> presences = new List<Presence>();
            foreach (var s in sh)
            {
                var presence = new List<Presence>();
                foreach (var studentId in request.StudentIds)
                {
                    presences.Add(new Presence
                    {
                        Presence_Student_Id = studentId,
                        Presence_SubjectsHour_Id = s.SubjectsHour_Id,
                    });
                }
            }
            _context.Presences.AddRange(presences);
            await _context.SaveChangesAsync();
        }


        return StatusCode(201);
    }

    /// <summary>
    /// DELETE: api/StudentSubjects
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> RemoveStudentFromSubject([FromBody] StudentSubjectRequest request)
    {
        var existingStudentSubject = await _context.StudentSubjects
            .FirstOrDefaultAsync(ss => ss.StudentSubject_StudentId == request.Student_Id && ss.StudentSubject_SubjectId == request.Subject_Id);

        if (existingStudentSubject == null)
        {
            return NotFound();
        }

        _context.StudentSubjects.Remove(existingStudentSubject);
        await _context.SaveChangesAsync();

        return StatusCode(204);
    }
}
