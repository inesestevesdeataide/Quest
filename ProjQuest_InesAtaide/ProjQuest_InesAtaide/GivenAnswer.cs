using System;
namespace ProjQuest_InesAtaide;

public class GivenAnswer
{
    public int QuestionId { get; set; }
    public List<String> GivenAnswers { get; set; }
    public bool IsRight { get; set; }

    public GivenAnswer()
    {
    }

    public GivenAnswer(int questionId, List<String> givenAnswers, bool isRight)
    {
        QuestionId = questionId;
        GivenAnswers = givenAnswers;
        IsRight = isRight;
    }
}

