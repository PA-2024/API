namespace API_GesSIgn.Models.Response
{
    public class ProofAbsenceDetailsResponse
    {
        public  ProofAbsenceResponse proofAbsenceResponse { get; set; }

        public string Subject_Name { get; set; } 

        public DateTime SubjectHour_DateStart { get; set; }

        public DateTime SubjectHour_DateEnd { get; set; }

        public StudentSimplifyDto student { get; set; }

        public int Presence_id { get; set; }

        public static ProofAbsenceDetailsResponse FromProofAbsence(Presence presence, string name, DateTime start, DateTime end, Student student)
        {
            ProofAbsenceDetailsResponse res = new ProofAbsenceDetailsResponse();
            res.proofAbsenceResponse = ProofAbsenceResponse.FromProofAbsence(presence.Presence_ProofAbsence);
            res.Subject_Name = name;
            res.SubjectHour_DateStart = start;
            res.SubjectHour_DateEnd = end;
            res.student = StudentSimplifyDto.FromStudent(student);
            res.Presence_id = presence.Presence_Id;
            return res;
        }

    }

    public class ProofAbsenceResponse
    {
        public int ProofAbsence_Id { get; set; }

        public string ProofAbsence_UrlFile { get; set; }

        /// <summary>
        /// 1 -> Refusé
        /// 2 -> accepté
        /// 3 -> en attente de traitement
        /// </summary>
        public string ProofAbsence_Status { get; set; }

        public string ProofAbsence_SchoolCommentaire { get; set; }

        public string ProofAbsence_ReasonAbscence { get; set; }

        public static ProofAbsenceResponse FromProofAbsence(ProofAbsence proofAbsence)
        {
            ProofAbsenceResponse res = new ProofAbsenceResponse();
            res.ProofAbsence_Id = proofAbsence.ProofAbsence_Id;
            res.ProofAbsence_UrlFile = proofAbsence.ProofAbsence_UrlFile;
            if (proofAbsence.ProofAbsence_Status == 1)
            {
                res.ProofAbsence_Status = "Refusé";
            }
            else if (proofAbsence.ProofAbsence_Status == 2)
            {
                res.ProofAbsence_Status = "Accepté";
            }
            else
            {
                res.ProofAbsence_Status = "En attente de traitement";
            }
            res.ProofAbsence_SchoolCommentaire = proofAbsence.ProofAbsence_SchoolCommentaire;
            res.ProofAbsence_ReasonAbscence = proofAbsence.ProofAbsence_ReasonAbscence;
            return res;
        }

    }


}
