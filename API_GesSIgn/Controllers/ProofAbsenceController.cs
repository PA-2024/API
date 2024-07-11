﻿using API_GesSIgn.Models;
using API_GesSIgn.Models.Request;
using API_GesSIgn.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_GesSIgn.Controllers
{
    public class ProofAbsenceController : ControllerBase 
    {
        private readonly MonDbContext _context;

        public ProofAbsenceController(MonDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateProofAbsence/{Presence_Id}")]
        public async Task<ActionResult<ProofAbsence>> CreateProofAbsence(int Presence_Id, ProofAbsence proofAbsenceRequest)
        {
            var presence = await _context.Presences
                .FirstOrDefaultAsync(m => m.Presence_Id == Presence_Id && !m.Presence_Is);

            if (presence == null)
            {
                return NotFound();
            }
            _context.ProofAbsences.Add(proofAbsenceRequest);
            await _context.SaveChangesAsync();

            presence.Presence_ProofAbsence.ProofAbsence_Id = proofAbsenceRequest.ProofAbsence_Id;
            _context.Presences.Update(presence);


            await _context.SaveChangesAsync();
            
            return Ok("Justificatif d'absence ajouté");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProofAbsence(int id, ProofAbsenceRequest proofAbsenceRequest)
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
        public async Task<ActionResult<IEnumerable<ProofAbsenceDetailsResponse>>> GetProofAbsence()
        {
            var schoolIdClaim = User.FindFirst("SchoolId")?.Value;
            if (string.IsNullOrEmpty(schoolIdClaim))
            {
                return BadRequest("School ID not found in token.");
            }



            var allData = await _context.ProofAbsences.ToListAsync();
            return Ok();
        }
    }
}
