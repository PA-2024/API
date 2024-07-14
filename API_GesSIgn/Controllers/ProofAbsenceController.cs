using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_GesSIgn.Controllers
{
    public class ProofAbsenceController : ControllerBase
    {
        private readonly MonDbContext _context;

        public ProofAbsenceController(MonDbContext context)
        {
            _context = context;
        }

        [RoleRequirement("Eleve")]
        [HttpPost("CreateProofAbsence/{Presence_Id}")]
        public async Task<ActionResult<ProofAbsence>> CreateProofAbsence(int Presence_Id, [FromBody] CreateProofAbsenceRequest Request)
        {
            var presence = await _context.Presences
                .FirstOrDefaultAsync(m => m.Presence_Id == Presence_Id && !m.Presence_Is);

            if (presence == null)
            {
                return NotFound();
            }
            ProofAbsence proofAbsenceRequest = new ProofAbsence
            {
                ProofAbsence_ReasonAbscence = Request.ProofAbsence_StudentComment,
                ProofAbsence_UrlFile = Request.ProofAbsence_UrlFile,
                ProofAbsence_Status = 3
            };

            _context.ProofAbsences.Add(proofAbsenceRequest);
            await _context.SaveChangesAsync();

            presence.Presence_ProofAbsence = proofAbsenceRequest;
            _context.Presences.Update(presence);


            await _context.SaveChangesAsync();

            return Ok("Justificatif d'absence ajouté");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProofAbsence(int id, [FromBody] ProofAbsenceRequest proofAbsenceRequest)
        {
            var proofAbsence = await _context.ProofAbsences
                .FirstOrDefaultAsync(m => m.ProofAbsence_Id == id);

            if (proofAbsence == null)
            {
                return NotFound();
            }

            proofAbsence.ProofAbsence_SchoolCommentaire = proofAbsenceRequest.ProofAbsence_SchoolComment;
            proofAbsence.ProofAbsence_Status = proofAbsenceRequest.ProofAbsence_Status;

            _context.ProofAbsences.Update(proofAbsence);

            if (proofAbsenceRequest.Presence_id == null)
            {
                return Ok("Justificatif d'absence modifié");
            }
            foreach (var p_id in proofAbsenceRequest.Presence_id)
            {
                var presence = await _context.Presences
                .FirstOrDefaultAsync(m => m.Presence_Id == p_id && !m.Presence_Is);

                if (presence == null)
                {
                    return NotFound("Erreur dans le liste des présence");
                }
                presence.Presence_ProofAbsence.ProofAbsence_Id = proofAbsenceRequest.ProofAbsence_Id;
                _context.Presences.Update(presence);

                await _context.SaveChangesAsync();
            }
            return Ok("Justificatif d'absence modifié");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [RoleRequirement("Gestion Ecole")]
        [HttpGet("GetProofAbsenceAll")]
        public async Task<ActionResult<IEnumerable<ProofAbsenceDetailsResponse>>> GetProofAbsence(int? Student_id = null)
        {
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }
            List<Presence> data = new List<Presence>();

            if (Student_id == null)
                data = await _context.Presences
                    .Include(s => s.Presence_SubjectsHour)
                    .ThenInclude(s => s.SubjectsHour_Subjects)
                    .ThenInclude(s => s.Subjects_User) //teacher 
                    .Include(s => s.Presence_ProofAbsence)
                    .Include(s => s.Presence_Student)
                    .ThenInclude(s => s.Student_User)
                    .Where(s => s.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_User.User_School_Id == int.Parse(schoolIdClaim)
                    && s.Presence_ProofAbsence_Id != null)
                    .ToListAsync();
            else
                data = await _context.Presences
                    .Include(s => s.Presence_SubjectsHour)
                    .ThenInclude(s => s.SubjectsHour_Subjects)
                    .ThenInclude(s => s.Subjects_User) //teacher 
                    .Include(s => s.Presence_ProofAbsence)
                    .Include(s => s.Presence_Student)
                    .ThenInclude(s => s.Student_User)
                    .Where(s => s.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_User.User_School_Id == int.Parse(schoolIdClaim) && s.Presence_Student_Id == Student_id
                     && s.Presence_ProofAbsence_Id != null)
                    .ToListAsync();

            List<ProofAbsenceDetailsResponse> proofAbsenceDetails = new List<ProofAbsenceDetailsResponse>();

            foreach (var item in data)
            {
                ProofAbsenceDetailsResponse add = ProofAbsenceDetailsResponse.FromProofAbsence(item,
                    item.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_Name, item.Presence_SubjectsHour.SubjectsHour_DateStart,
                    item.Presence_SubjectsHour.SubjectsHour_DateEnd, item.Presence_Student
                    );
                proofAbsenceDetails.Add(add);
            }


            return Ok(proofAbsenceDetails);
        }

        [RoleRequirement("Eleve")]
        [HttpGet("GetProofAbsenceAll/student")]
        public async Task<ActionResult<IEnumerable<ProofAbsenceDetailsResponse>>> GetProofAbsenceForStudent()
        {
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            List<Presence> data = new List<Presence>();

           
            data = await _context.Presences
                    .Include(s => s.Presence_SubjectsHour)
                    .ThenInclude(s => s.SubjectsHour_Subjects)
                    .ThenInclude(s => s.Subjects_User) //teacher 
                    .Include(s => s.Presence_ProofAbsence)
                    .Include(s => s.Presence_Student)
                    .ThenInclude(s => s.Student_User)
                    .Where(s => s.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_User.User_School_Id == int.Parse(schoolIdClaim) 
                    && s.Presence_Student.Student_User.User_Id == userId
                    && s.Presence_ProofAbsence_Id != null)
                    .ToListAsync();

            List<ProofAbsenceDetailsResponse> proofAbsenceDetails = new List<ProofAbsenceDetailsResponse>();

            foreach (var item in data)
            {
                ProofAbsenceDetailsResponse add = ProofAbsenceDetailsResponse.FromProofAbsence(item,
                    item.Presence_SubjectsHour.SubjectsHour_Subjects.Subjects_Name, item.Presence_SubjectsHour.SubjectsHour_DateStart,
                    item.Presence_SubjectsHour.SubjectsHour_DateEnd, item.Presence_Student
                    );
                proofAbsenceDetails.Add(add);
            }


            return Ok(proofAbsenceDetails);
        }
    }
}
