using System;
namespace ProjQuest_InesAtaide;

public class Quiz
{
    public string Student { get; set; }
    public DateTime Date { get; set; }
    public Question[] Questions { get; set; }
    public GivenAnswer[] Answers { get; set; }
    public double Score { get; set; }

    public Quiz()
    {
    }

    public Quiz(string student, Question[] questions, GivenAnswer[] answers, double score)
    {
        Student = student;
        Date = DateTime.Now;
        Questions = questions;
        Answers = answers;
        Score = score;
    }
}
