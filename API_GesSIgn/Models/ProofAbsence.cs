
﻿using API_GesSIgn.Migrations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_GesSIgn.Models
{
    public class ProofAbsence
    {
        [Key]
        public int ProofAbsence_Id { get; set; }

        public string ProofAbsence_UrlFile { get; set; }

        /// <summary>
        /// 1 -> Pas accepté
        /// 2 -> accepté
        /// 3 -> en attente de traitement
        /// </summary>
        public int ProofAbsence_Status { get; set; }

        public string ProofAbsence_Commentaire { get; set; }
    }
}
