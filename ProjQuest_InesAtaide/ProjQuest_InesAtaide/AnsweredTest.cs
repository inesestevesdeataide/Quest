using System;
namespace ProjQuest_InesAtaide;

public class AnsweredTest
{
    public int TestId { get; set; }
    public string Student { get; set; }
    public List<GivenAnswer> GivenAnswers { get; set; }
    public double Score { get; set; }

    public AnsweredTest()
    {
    }

    public AnsweredTest(int testId, string username, List<GivenAnswer> givenAnswers, double score) : this()
    {
        TestId = testId;
        Student = username;
        GivenAnswers = givenAnswers;
        Score = score;
    }
}

