using System;
namespace ProjQuest_InesAtaide.UserInterface;

public class UiUtils
{
    // user output
    public static void PrintListQuestions(List<Question> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];

            Console.WriteLine(@$"
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
#{i + 1} - {item.Name}                                   
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
");
            PrintListAnswers(item.Answers, true);
        }
    }

    public static void PrintListAnswers(List<AvailableAnswer> list, bool showCorrect)
    {
        for (int i = 0; i < list.Count; i++)
        {
            string correctIncorrect = "";

            if (showCorrect)
            {
                correctIncorrect = list[i].IsCorrect ? " - CORRECT" : " - INCORRECT";
            }

            Console.WriteLine($"{i + 1}) {list[i].Answer} {correctIncorrect}");
        }
    }

    public static void PrintListString(List<String> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            Console.WriteLine($"{i + 1} - {item}");
        }
    }

}