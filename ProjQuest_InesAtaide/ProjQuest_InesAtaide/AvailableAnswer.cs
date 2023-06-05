using System;
namespace ProjQuest_InesAtaide;

public class AvailableAnswer
{
    public string Answer { get; set; }
    public bool IsCorrect { get; set; }

    public AvailableAnswer()
    {
    }

    public AvailableAnswer(string answer, bool isCorrect)
    {
        Answer = answer;
        IsCorrect = isCorrect;
    }
}