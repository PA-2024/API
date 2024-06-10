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
    public async Task<IActionResult> AddStudentToSubject([FromBody] StudentSubjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var studentSubject = new StudentSubject
        {
            StudentSubject_StudentId = request.Student_Id,
            StudentSubject_SubjectId = request.Subject_Id
        };

        _context.StudentSubjects.Add(studentSubject);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetStudentSubject", new { studentId = studentSubject.StudentSubject_StudentId, subjectId = studentSubject.StudentSubject_SubjectId }, studentSubject);
    }

    /// <summary>
    /// POST: api/StudentSubjects/List
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("List")]
    public async Task<IActionResult> AddStudentsToSubject([FromBody] AddStudentsToSubjectRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var studentSubjects = request.StudentIds.Select(studentId => new StudentSubject
        {
            StudentSubject_StudentId = studentId,
            StudentSubject_SubjectId = request.Subject_Id
        }).ToList();

        _context.StudentSubjects.AddRange(studentSubjects);
        await _context.SaveChangesAsync();

        return Ok(studentSubjects);
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

        return NoContent();
    }
}
