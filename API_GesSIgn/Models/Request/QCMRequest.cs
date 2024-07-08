namespace API_GesSIgn.Models.Request
{
    /// <summary>
    /// classe de requête pour les QCM
    /// </summary>
    public class CreateQCMRequest
    {
        public string Title { get; set; }

        public int SubjectHour_id { get; set; } 

        public List<CreateQuestionRequest>? Questions { get; set; }
    }

    public class CreateQuestionRequest
    {

        public string Text { get; set; }

        public required List<CreateOptionRequest> Options { get; set; }
    }

    public  class CreateOptionRequest
    {
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
    }
}
