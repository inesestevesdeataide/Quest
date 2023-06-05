using System;
using Quest.Utils;
namespace ProjQuest_InesAtaide.UserInterface;

// do lado do aluno
public class UiAnswer
{
    public static GivenAnswer AnswerQuestion(Question question)
    {
        string strType = "";
        string ruleByType = "";
        string exType = "";

        List<String> givenAnswers = new List<String>();

        if (question.Type == 1)
        {
            strType = "CheckBox";
            ruleByType = "one or many correct answers";
            exType = @"If you think there are two or more correct answers please separate their numeric references using a space.
Otherwise, insert the corresponding number only.

    -> For example, if both 2 and 3 are correct, you must type '2 3':";
        }
        else if (question.Type == 2)
        {
            strType = "DropDown";
            ruleByType = "only one correct answer";
            exType = "Type the number that matches your selected answer.";
        }
        else
        {
            strType = "YesNo";
            ruleByType = "'Yes', if you agree with the statement, or 'No', if you consider it wrong";
        }

        if (question.Type == 1)
        {
            string str = "";
            bool isValid = false;
            List<int> answerIndexes = new List<int>();

            while (!isValid)
            {
                answerIndexes = new List<int>();
                PrintQuestionAndAnswers(question, strType, ruleByType, exType);

                str = Console.ReadLine().Trim();

                if (str == "")
                {
                    Ui.ClearAndWriteLine("-> You must insert some answer.\n   " +
                        "Try again:\n");
                    isValid = false;
                    continue;
                }

                string[] splittedAnswers = str.Split(' ');

                foreach (var split in splittedAnswers)
                {
                    int.TryParse(split, out int answerIndex);

                    if (answerIndex <= 0 || answerIndex > question.Answers.Count)
                    {
                        Ui.ClearAndWriteLine($"-> Your answer must be between 1 " +
                            $"and {question.Answers.Count}.\n   '{split}' " +
                            $"is not a valid option.\n   Try again:\n");
                        isValid = false;
                        break;
                    }
                    else if (answerIndexes.Contains(answerIndex - 1))
                    {
                        Ui.ClearAndWriteLine($"-> You can not repeat answer " +
                            $"'{answerIndex}'.\n   Try again:\n");
                        isValid = false;
                        break;
                    }
                    else
                    {
                        int realAnswerIndex = answerIndex - 1;
                        answerIndexes.Add(realAnswerIndex);
                        isValid = true;
                    }
                }
            }

            answerIndexes.Sort();

            foreach (var item in answerIndexes)
            {
                givenAnswers.Add(question.Answers[item].Answer);
            }

            List<int> correctAnswersOnly = new List<int>();

            for (int i = 0; i < question.Answers.Count; i++)
            {
                if (question.Answers[i].IsCorrect)
                {
                    correctAnswersOnly.Add(i);
                }
            }

            bool gotItRight = false;

            if (answerIndexes.Count == correctAnswersOnly.Count)
            {
                for (int i = 0; i < correctAnswersOnly.Count; i++)
                {
                    if (answerIndexes[i] == correctAnswersOnly[i])
                    {
                        gotItRight = true;
                    }
                    else
                    {
                        gotItRight = false;
                        break;
                    }
                }
            }

            Ui.PressEnterToMoveOn();

            return new GivenAnswer(question.Id, givenAnswers, gotItRight);
        }
        else if (question.Type == 2)
        {
            PrintQuestionAndAnswers(question, strType, ruleByType, exType);

            int intAnswer = Ui.ReadIntInRange("answer", 1, question.Answers.Count, "", false);
            int realIntIndex = intAnswer - 1;

            bool gotItRight = (question.Answers[realIntIndex].IsCorrect);

            givenAnswers.Add(question.Answers[realIntIndex].Answer);

            Ui.PressEnterToMoveOn();

            return new GivenAnswer(question.Id, givenAnswers, gotItRight);
        }
        else
        {
            Console.WriteLine(@$"For this {strType} question you must choose {ruleByType}.");
            Console.WriteLine();

            bool boolAnswer = Ui.ReadBoolYesNo($"{question.Name}\n", true);

            bool gotItRight = (boolAnswer == question.Answers[0].IsCorrect);

            givenAnswers.Add(boolAnswer ? "Yes" : "No");

            Ui.PressEnterToMoveOn();

            GivenAnswer givenAnswer = new GivenAnswer(question.Id, givenAnswers, gotItRight);
            return givenAnswer;
        }
    }

    private static void PrintQuestionAndAnswers(Question question, string strType, string ruleByType, string exType)
    {
        Console.WriteLine($"{question.Name}\n");
        UiUtils.PrintListAnswers(question.Answers, false);
        Console.WriteLine();
        Console.WriteLine(@$"For this {strType} question you must choose {ruleByType}.");
        Console.WriteLine(exType);
        Console.WriteLine();
    }
}