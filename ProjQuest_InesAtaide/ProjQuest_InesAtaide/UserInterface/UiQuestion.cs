using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Quest.Utils;

namespace ProjQuest_InesAtaide.UserInterface;

// do lado do admin/professor
public class UiQuestion
{
    public static Question DesignQuestion(List<String> subjects)
    {
        string subject = DesignSubject(subjects);

        Console.Clear();
        int level = Ui.ReadIntInRange("question's level", 1, 3, @"
1 - Elementary
2 - Intermediate
3 - Advanced
", true);

        string name = "";
        bool nameIsFilled = false;

        Console.Clear();
        while (!nameIsFilled)
        {
            Console.WriteLine($"Insert your {subject} question:");
            name = Console.ReadLine().Trim();

            if (name != "")
            {
                nameIsFilled = true;
            }
            else
            {
                Ui.ClearAndWriteLine("-> You must insert a valid question.\n");
            }
        }

        Console.Clear();
        int type = Ui.ReadIntInRange("question's type", 1, 3, @"
1 - CheckBox
2 - DropDown
3 - YesNo
", true);

        Console.Clear();
        List<AvailableAnswer> answers = DesignAnswers(name, type);

        Console.Clear();
        List<String> tags = DesignTags();

        Console.Clear();
        bool onlyTest = Ui.ReadBoolYesNo("Is this question for tests only?\n", true);

        Question question = new Question(subject, level, tags, name, type, onlyTest, answers);

        return question;
    }

    public static Question DesignQuestion(int subjectIndex, int level, List<String> subjects)
    {
        string subject = subjects[subjectIndex];

        string name = "";
        bool nameIsFilled = false;

        Console.Clear();
        while (!nameIsFilled)
        {
            Console.WriteLine($"Insert your question:");
            name = Console.ReadLine().Trim();

            if (name != "")
            {
                nameIsFilled = true;
            }
            else
            {
                Ui.ClearAndWriteLine("-> You must insert a valid question.\n");
            }
        }

        Console.Clear();
        int type = Ui.ReadIntInRange("question's type", 1, 3, @"
1 - CheckBox
2 - DropDown
3 - YesNo
", true);

        Console.Clear();
        List<AvailableAnswer> answers = DesignAnswers(name, type);

        Console.Clear();
        List<String> tags = DesignTags();

        Console.Clear();
        bool onlyTest = Ui.ReadBoolYesNo("Is this question for tests only?\n", true);

        Question question = new Question(subject, level, tags, name, type, onlyTest, answers);

        return question;
    }

    private static string DesignSubject(List<String> subjects)
    {
        string subject = "";
        bool subjectIsValid = false;

        Console.Clear();
        while (!subjectIsValid)
        {
            UiUtils.PrintListString(subjects);

            Console.WriteLine("\nChoose a subject from the presented list (type the correspondent number) or insert a new subject:");
            subject = Console.ReadLine().Trim();

            bool isIndex = int.TryParse(subject, out int subjectIndex);

            if (isIndex)
            {
                int realIndex = subjectIndex - 1;

                if (realIndex >= 0 && realIndex < subjects.Count())
                {
                    subject = subjects[realIndex];
                    subjectIsValid = true;
                }
                else
                {
                    Ui.ClearAndWriteLine("-> You must choose a valid option.\n");
                }
            }
            else
            {
                bool subjectExists = false;

                foreach (var item in subjects)
                {
                    if (item.ToLower() == subject.ToLower())
                    {
                        subjectExists = true;
                        subject = item;
                    }
                }

                if (!subjectExists)
                {
                    subject = subject.Substring(0, 1).ToUpper() + subject.Substring(1).ToLower();
                }

                subjectIsValid = true;
            }
        }

        return subject;
    }

    private static List<String> DesignTags()
    {
        List<String> tags = new List<string>();
        bool tagsIsValid = false;

        Console.Clear();
        while (!tagsIsValid)
        {
            Console.WriteLine(@"Insert as many tags as you wish for this question.
Use '#' for each tag and separate them with an empty space - e.g. #tag #anotherTag

If you do not want to tag the question, press enter.
");
            string strTags = Console.ReadLine();

            if (strTags == "")
            {
                tagsIsValid = true;
            }
            else if (strTags.IndexOf('#') == 0)
            {
                string[] splittedTags = strTags.Split(" ");

                // para não guardar as tags inseridas corretamente em interações anteriores
                tags = new List<String>();

                for (int i = 0; i < splittedTags.Length; i++)
                {
                    if (splittedTags[i].IndexOf('#') == 0 && splittedTags[i].Substring(1).IndexOf('#') == -1)
                    {
                        tags.Add(splittedTags[i].Trim());
                    }
                    else
                    {
                        Ui.ClearAndWriteLine($"-> {splittedTags[i]} is invalid.\n");
                    }
                }

                if (tags.Count() == splittedTags.Length)
                {
                    tagsIsValid = true;
                }
            }
            else
            {
                Ui.ClearAndWriteLine("-> You must use '#' for each tag. \n");
            }
        }

        return tags;
    }

    private static List<AvailableAnswer> DesignAnswers(string name, int type)
    {
        List<AvailableAnswer> answers = new List<AvailableAnswer>();
        string answer = "";

        Console.Clear();
        if (type == 3)
        {
            Console.WriteLine($"{name}\n");

            bool isCorrect = Ui.ReadBoolYesNo("Mark question as correct?", true);
            answer = isCorrect ? "Yes" : "No";

            AvailableAnswer avAnswer = new AvailableAnswer(answer, isCorrect);
            answers.Add(avAnswer);
        }
        else
        {
            List<String> alreadyInsertedAnswers = new List<String>();
            bool isDoneInserting = false;

            Console.WriteLine($"{name}\n");

            while (!isDoneInserting)
            {
                Console.WriteLine(@"Insert a possible answer for this question.
If you do not want to include any more answers, press 'e'.
");
                answer = Console.ReadLine().Trim();

                if (answer == "")
                {
                    Ui.ClearAndWriteLine(@$"-> You must insert a valid answer.
To exit after minimal number of answers inserted, press 'e'.
");
                    Console.WriteLine($"{name}\n");
                }
                else if (answer.ToLower() == "e")
                {
                    if (answers.Count >= 2)
                    {
                        Console.Clear();
                        isDoneInserting = true;
                    }
                    else
                    {
                        Ui.ClearAndWriteLine("-> You must insert at least two possible answers for this question.\n");
                        Console.WriteLine($"{name}\n");
                    }
                }
                else
                {
                    if (alreadyInsertedAnswers.Contains(answer))
                    {
                        Ui.ClearAndWriteLine("-> That answer has previously been inserted.\n");
                        Console.WriteLine($"{name}\n");

                    }
                    else
                    {
                        AvailableAnswer avAnswer = new AvailableAnswer(answer, false);
                        alreadyInsertedAnswers.Add(answer);
                        answers.Add(avAnswer);
                        Console.WriteLine();
                    }
                }
            }

            Console.WriteLine($"{name}\n");
            UiUtils.PrintListAnswers(answers, false);
            Console.WriteLine();

            if (type == 2)
            {
                int correctAnswer = Ui.ReadIntInRange("correct answer", 1, answers.Count, "", false);

                answers[correctAnswer - 1].IsCorrect = true;
            }
            else if (type == 1)
            {
                int numCorrect = 0;
                bool isNumCorrect = false;

                while (!isNumCorrect)
                {
                    numCorrect = Ui.ReadInt("How many correct answers do you have?");

                    if (numCorrect > 0 && numCorrect <= answers.Count)
                    {
                        isNumCorrect = true;
                    }
                    else
                    {
                        Ui.ClearAndWriteLine($"-> Your number of correct answers must be between 1 and {answers.Count}.\n");
                        Console.WriteLine($"{name}\n");
                        UiUtils.PrintListAnswers(answers, false);
                        Console.WriteLine();
                    }
                }

                for (int i = 1; i <= numCorrect; i++)
                {
                    bool isValid = false;

                    Console.Clear();
                    while (!isValid)
                    {
                        Console.WriteLine($"{name}\n");
                        UiUtils.PrintListAnswers(answers, false);
                        int correctAnswer = Ui.ReadInt($"\nInsert correct answer #{i}:");

                        if (correctAnswer >= 1 && correctAnswer <= answers.Count)
                        {
                            if (answers[correctAnswer - 1].IsCorrect == true)
                            {
                                Ui.ClearAndWriteLine($"-> Answer {correctAnswer} is already selected as correct.\n");
                            }
                            else
                            {
                                isValid = true;
                                answers[correctAnswer - 1].IsCorrect = true;
                            }
                        }
                        else
                        {
                            Ui.ClearAndWriteLine($"-> Correct answers must be between 1 and {answers.Count}.\n");
                        }
                    }
                }
            }
        }

        return answers;
    }
}