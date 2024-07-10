using Microsoft.AspNetCore.Mvc;

namespace API_GesSIgn.Models.Response
{
    public class QcmResultResponce { 

        public int QcmResult_Id { get; set; }

        public string QcmResult_Title { get; set; }

        public StudentSimplifyDto QcmResult_Student { get; set; }

        public int QcmResult_Score { get; set; }



    }

    public class QcmResultDetails
    {
        public StudentSimplifyDto QcmResult_Student { get; set; }


        public string QcmResultDetails_Answer { get; set; }


        public int QcmResultDetails_Question_id { get; set; }
    }







}
