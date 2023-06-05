using System;
namespace ProjQuest_InesAtaide;

public class Test
{
    private static int _counter;

    public int Id { get; set; }
    public List<string> Student { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<Question> Questions { get; set; }
    public bool HasRandomQuestions { get; set; }
    public bool HasRandomAnswers { get; set; }

    public Test()
    {
        _counter++;
    }

    public Test(List<string> student, DateTime startDate, DateTime endDate, bool hasRandomQuestions, bool hasrandomAnswers, List<Question> questions) : this()
    {
        Id = _counter;
        Student = student;
        StartDate = startDate;
        EndDate = endDate;
        HasRandomQuestions = hasRandomQuestions;
        HasRandomAnswers = hasrandomAnswers;
        Questions = questions;
    }
}