﻿using System.Net.WebSockets;
using System.Runtime.CompilerServices;

namespace API_GesSIgn.Models.Response
{
    public class QCMDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<QuestionDto> Questions { get; set; }

    }

    public class StudentQcm
    {
        public string Student_Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public WebSocket webSocket { get; set; }

        public static StudentQcm FromStudent(Student student)
        {
            StudentQcm res = new StudentQcm(student.Student_Id.ToString(), student.Student_User.User_firstname + " " + student.Student_User.User_lastname);
            res.Score = 0;
            return res;
        }

        public StudentQcm(string Student_Id, string name)
        {
            this.Student_Id = Student_Id;
            this.Name = name;
            webSocket = null;
        }
    }

    public class OptionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public static OptionDto FromOption(OptionQcm option)
        {
            OptionDto res = new OptionDto();
            res.Id = option.OptionQcm_id;
            
            res.Text = option.OptionQcm_Text;
            return res;
        }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<OptionDto> Options { get; set; }

        public List<int> CorrectOption { get; set; }

        public static QuestionDto FromQuestion(Question question, List<OptionQcm> options)
        {
            QuestionDto res = new QuestionDto();
            res.Id = question.Question_Id;
            res.Text = question.Question_Text;
            res.Options = new List<OptionDto>();
            res.CorrectOption = new List<int>();

            foreach (OptionQcm option in options)
            {
                res.Options.Add(OptionDto.FromOption(option));
                if (option.OptionQcm_IsCorrect)
                {
                    res.CorrectOption.Add(option.OptionQcm_id);
                }
            }
            return res;
        }

    }
}
