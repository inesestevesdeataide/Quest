using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using ProjQuest_InesAtaide.UserInterface;
using Quest.Utils;
using static System.Formats.Asn1.AsnWriter;

namespace ProjQuest_InesAtaide;

public class QuestManager
{
    public List<String> Subjects { get; set; }
    public List<Question> Questions { get; set; }
    public List<Quiz> Quizzes { get; set; }
    public List<Test> Tests { get; set; }
    public List<AnsweredTest> AnsweredTests { get; set; }
    public string ActiveUser { get; set; }
    public List<String> Students { get; set; }

    public QuestManager()
    {
        try
        {
            var jsonAnsweredTests = File.ReadAllText("../../../json/answeredTests.json");
            AnsweredTests = JsonSerializer.Deserialize<List<AnsweredTest>>(jsonAnsweredTests);
        }
        catch (Exception ex)
        {
            AnsweredTests = new List<AnsweredTest>();
        }

        try
        {
            var jsonQuestions = File.ReadAllText("../../../json/questions.json");
            Questions = JsonSerializer.Deserialize<List<Question>>(jsonQuestions);
        }
        catch (Exception ex)
        {
            Questions = new List<Question>();
        }

        try
        {
            var jsonQuizzes = File.ReadAllText("../../../json/quizzes.json");
            Quizzes = JsonSerializer.Deserialize<List<Quiz>>(jsonQuizzes);
        }
        catch (Exception ex)
        {
            Quizzes = new List<Quiz>();
        }

        try
        {
            var jsonStudents = File.ReadAllText("../../../json/students.json");
            Students = JsonSerializer.Deserialize<List<string>>(jsonStudents);
        }
        catch (Exception ex)
        {
            Students = new List<string>();
        }

        try
        {
            var jsonSubjects = File.ReadAllText("../../../json/subjects.json");
            Subjects = JsonSerializer.Deserialize<List<String>>(jsonSubjects);
        }
        catch (Exception ex)
        {
            Subjects = new List<string>();
        }

        try
        {
            var jsonTests = File.ReadAllText("../../../json/tests.json");
            Tests = JsonSerializer.Deserialize<List<Test>>(jsonTests);
        }
        catch (Exception ex)
        {
            Tests = new List<Test>();
        }
    }

    public void Start()
    {
        while (true)
        {
            LogIn();
        }
    }

    private void LogIn()
    {
        bool isValid = false;

        Console.Clear();
        Console.WriteLine(@"***********************
*  WELCOME TO QUEST!  *
***********************
");

        while (!isValid)
        {
            Console.WriteLine(@"Insert your username.
Follow the format <FirstName.LastName_studentIDNumber>:
");
            string username = Console.ReadLine().Trim();

            if (username == "admin")
            {
                Ui.ClearAndWriteLine("Insert your password:\n");
                string pass = Console.ReadLine().Trim();

                string password = "pass";

                if (pass == password)
                {
                    isValid = true;
                    ActiveUser = username;
                    ShowAdminMenu();
                }
                else
                {
                    Ui.ClearAndWriteLine(@"Invalid password.
-> Try again:
");
                }

            }
            else if (username != "admin" && username.Contains('.') &&
                username.Contains('_'))
            {
                isValid = true;
                ActiveUser = username;

                if (!(Students.Contains(username)))
                {
                    Students.Add(username);
                    var jsonStudents = JsonSerializer.Serialize(Students, new JsonSerializerOptions()
                    {
                        WriteIndented = true,
                    });

                    File.WriteAllText("../../../json/students.json", jsonStudents);
                }

                ShowStudentMenu();
            }
            else
            {
                Ui.ClearAndWriteLine("-> Invalid username.\n");
            }
        }
    }

    private void ShowAdminMenu()
    {
        bool isDone = false;
        string options = @"
1 - Create question
2 - Create test
3 - Consult tests report
4 - Cancel test (beta)

To exit, press '0' and enter.
";

        while (!isDone)
        {
            Console.Clear();
            int choice = Ui.ReadIntInRange("choice", 0, 4, options, false);

            switch (choice)
            {
                case 0:
                    isDone = true;
                    break;
                case 1:
                    CreateQuestion();
                    break;
                case 2:
                    CreateTest();
                    break;
                case 3:
                    PrintTestReport();
                    break;
                case 4:
                    CancelTest();
                    break;
                default:
                    isDone = true;
                    break;
            }
        }
    }

    private void ShowStudentMenu()
    {
        bool isDone = false;
        string options = @"
1 - Take a quiz
2 - See upcoming tests
3 - Consult your quiz report
4 - Consult your test report

To exit, press '0' and enter.
";

        while (!isDone)
        {
            Console.Clear();
            int choice = Ui.ReadIntInRange("choice", 0, 4, options, true);

            switch (choice)
            {
                case 0:
                    isDone = true;
                    break;
                case 1:
                    GenerateQuiz();
                    break;
                case 2:
                    UpcomingTests();
                    break;
                case 3:
                    PrintStudentQuizReport();
                    break;
                case 4:
                    PrintStudentTestReport();
                    break;
                default:
                    isDone = true;
                    break;
            }  
        }
    }

    private void CreateQuestion()
    {
        Question question = UiQuestion.DesignQuestion(Subjects);

        if (!Subjects.Contains(question.Subject))
        {
            Subjects.Add(question.Subject);
            Subjects.Sort();
        }

        var jsonSubjects = JsonSerializer.Serialize(Subjects, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        File.WriteAllText("../../../json/subjects.json", jsonSubjects);

        Questions.Add(question);

        var jsonQuestions = JsonSerializer.Serialize(Questions, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        File.WriteAllText("../../../json/questions.json", jsonQuestions);

        Ui.PressEnterToMoveOn();
    }

    private Question CreateQuestion(int subjectIndex, int level)
    {
        Question question = UiQuestion.DesignQuestion(subjectIndex, level, Subjects);

        Questions.Add(question);

        var jsonQuestions = JsonSerializer.Serialize(Questions, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        File.WriteAllText("../../../json/questions.json", jsonQuestions);

        Ui.PressEnterToMoveOn();

        return question;
    }

    private List<Question> FilterQuestions(string subject, int level, bool isTest)
    {
        List<Question> filteredQuestions = new List<Question>();

        foreach (var question in Questions)
        {
            if (question.Subject == subject && question.Level == level)
            {
                if ((!isTest && question.OnlyTest == false) || (isTest))
                {
                    filteredQuestions.Add(question);
                }
            }
        }

        return filteredQuestions;
    }

    private void GenerateQuiz()
    {
        Console.Clear();
        UiUtils.PrintListString(Subjects);
        Console.WriteLine();

        int subjectIndex = Ui.ReadIntInRange("subject number", 1, Subjects.Count, "", false) - 1;

        Console.Clear();
        int level = Ui.ReadIntInRange("quiz level", 1, 3, @"
1 - Elementary
2 - Intermediate
3 - Advanced
", true);

        // Por opção, um quiz tem 5 perguntas. Poderá ser perguntado ao user ou ao admin, alterando a variável.
        // Se para dado suject e level não houver 5 perguntas no banco, o quiz não está disponível 
        int quizDimension = 5;

        Console.Clear();
        List<Question> filteredQuestions = FilterQuestions(Subjects[subjectIndex], level, false);

        if (filteredQuestions.Count >= quizDimension)
        {
            List<int> shuffledIndexes = Console2.GenerateRandomlyOrderedIndexes(filteredQuestions.Count);
            Question[] shuffledQuestions = new Question[quizDimension];
            GivenAnswer[] givenAnswers = new GivenAnswer[quizDimension];
            double score = 0;

            for (int i = 0; i < shuffledQuestions.Length; i++)
            {
                Question q = filteredQuestions[shuffledIndexes[i]];

                List<int> shuffledIndexesAnswers = Console2.GenerateRandomlyOrderedIndexes(q.Answers.Count);
                List<AvailableAnswer> shuffledAnswers = new List<AvailableAnswer>();

                foreach (var item in shuffledIndexesAnswers)
                {
                    shuffledAnswers.Add(q.Answers[item]);
                }

                q.Answers = shuffledAnswers;

                shuffledQuestions[i] = q;

                Console.WriteLine($"* Question {i + 1} / {quizDimension} *\n");

                var givenAnswer = UiAnswer.AnswerQuestion(q);

                givenAnswers[i] = givenAnswer;

                if (givenAnswer.IsRight)
                {
                    score += 100.0 / quizDimension;
                }
            }

            if (score >= 80)
            {
                Console.WriteLine(@$"Congratulations!!!

You just got a score of {score}%
");
                Ui.PressEnterToMoveOn();
            }
            else
            {
                Console.WriteLine("Sorry!!!\nYou failed the quiz.\n\nDo not get discouraged and try as many times as you need...\n");
                Ui.PressEnterToMoveOn();
            }

            Quizzes.Add(new Quiz(ActiveUser, shuffledQuestions, givenAnswers, score));

            var jsonQuizzes = JsonSerializer.Serialize(Quizzes, new JsonSerializerOptions()
            {
                WriteIndented = true,
            });

            File.WriteAllText("../../../json/quizzes.json", jsonQuizzes);
        }
        else
        {
            Console.WriteLine($"Sorry, there is no available quiz for this {Subjects[subjectIndex]} level.");
        }
    }

    private void CreateTest()
    {
        Console.Clear();
        UiUtils.PrintListString(Subjects);
        Console.WriteLine();

        int subjectIndex = Ui.ReadIntInRange("subject number", 1, Subjects.Count, "", false) - 1;

        Console.Clear();
        int level = Ui.ReadIntInRange("test's level", 1, 3, @"
1 - Elementary
2 - Intermediate
3 - Advanced
", true);

        Console.Clear();
        var startDate = Ui.ReadDate();

        Console.Clear();
        var duration = Ui.ReadDuration();

        var endDate = startDate.AddHours(duration.hours)
            .AddMinutes(duration.minutes);

        Console.Clear();
        bool isDoneSelectingStudents = false;
        List<String> notYetSelectedStudents = new List<string>();

        foreach (var student in Students)
        {
            notYetSelectedStudents.Add(student);
        }

        List<String> selectedStudents = new List<string>();

        while (!isDoneSelectingStudents && notYetSelectedStudents.Count > 0)
        {
            UiUtils.PrintListString(notYetSelectedStudents);
            Console.WriteLine(@"
Select students for whom the test will be available.
Insert the number that matches the username.
If you do not want to include any more students, press '0' and enter.
");
            int selectedStudent = Ui.ReadIntInRange("student number", 0, notYetSelectedStudents.Count, "", false);

            if (selectedStudent == 0)
            {
                if (selectedStudents.Count >= 1)
                {
                    Console.Clear();
                    isDoneSelectingStudents = true;
                }
                else
                {
                    Ui.ClearAndWriteLine("-> You must select at least one student for this test.\n");
                }
            }
            else if (selectedStudent >= 1 && selectedStudent <= notYetSelectedStudents.Count)
            {
                int selectedStudentIndex = selectedStudent - 1;
                selectedStudents.Add(notYetSelectedStudents[selectedStudentIndex]);
                notYetSelectedStudents.RemoveAt(selectedStudentIndex);
                Console.Clear();
            }
        }

        List<Question> alreadySelectedQuestions = new List<Question>();
        bool isDoneSelectingQuestions = false;

        List<Question> filteredQuestions = FilterQuestions(Subjects[subjectIndex], level, true);

        while (!isDoneSelectingQuestions)
        {
            if (filteredQuestions.Count < 1)
            {
                Ui.ClearAndWriteLine(@"If you want to create a new question, type 'New'.
If you do not want to include any more answers, press 'e'.
");
            }
            else
            {
                Console.Clear();
                UiUtils.PrintListQuestions(filteredQuestions);
                Console.WriteLine(@"
Select a question for your test. Insert its number (after #).
If you want to create a new question, type 'New'.
If you do not want to include any more answers, press 'e'.
");
            }
            
            string strSelectedQuestion = Console.ReadLine().Trim();

            if (strSelectedQuestion == "")
            {
                Ui.ClearAndWriteLine(@$"-> You must insert a valid number.
To exit after at least one question inserted, press 'e'.
");
            }
            else if (strSelectedQuestion.ToLower() == "e")
            {
                if (alreadySelectedQuestions.Count >= 1)
                {
                    Console.Clear();
                    isDoneSelectingQuestions = true;
                }
                else
                {
                    Ui.ClearAndWriteLine("-> You must insert at least one question for this test.\n");
                }
            }
            else if (strSelectedQuestion.ToLower() == "new")
            {
                Question q = CreateQuestion(subjectIndex, level);
                alreadySelectedQuestions.Add(q);
            }
            else
            {
                if (int.TryParse(strSelectedQuestion, out int selectedQuestion))
                {
                    if (selectedQuestion > 0 && selectedQuestion <= filteredQuestions.Count)
                    {
                        Question chosenQuestion = filteredQuestions[selectedQuestion - 1];

                        if (alreadySelectedQuestions.Contains(chosenQuestion))
                        {
                            Ui.ClearAndWriteLine("-> That question has previously been selected.\n");
                        }
                        else
                        {
                            alreadySelectedQuestions.Add(chosenQuestion);
                            filteredQuestions.RemoveAt(selectedQuestion - 1);
                        }
                    }
                    else
                    {
                        Ui.ClearAndWriteLine("-> Select a valid question.");
                    }
                }
            }
        }


        Console.Clear();
        bool hasRandomQuestions = Ui.ReadBoolYesNo("Would you like to present questions in a random order?", true);

        Console.Clear();
        bool hasRandomAnswers = Ui.ReadBoolYesNo("Would you like to present answers in a random order?", true);

        Tests.Add(new Test(selectedStudents, startDate, endDate, hasRandomQuestions, hasRandomAnswers, alreadySelectedQuestions));

        var jsonTests = JsonSerializer.Serialize(Tests, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        File.WriteAllText("../../../json/tests.json", jsonTests);

        Ui.PressEnterToMoveOn();
    }

    private List<Test> FilterUpcomingTests()
    {
        List<Test> upcomingTests = new List<Test>();

        foreach (var test in Tests)
        {
            if (test.EndDate > DateTime.Now)
            {
                foreach (var puto in test.Student)
                {
                    if (puto == ActiveUser)
                    {
                        upcomingTests.Add(test);
                    }
                }
            }
        }

        Comparison<Test> comparison = delegate (Test x, Test y)
        {
            return x.EndDate.CompareTo(y.EndDate);
        };

        upcomingTests.Sort(comparison);

        return upcomingTests;
    }

    private bool IsQuizDone(Test test)
    {
        foreach (var quiz in Quizzes)
        {
            if (quiz.Student == ActiveUser && quiz.Questions[0].Subject == test.Questions[0].Subject
                && quiz.Questions[0].Level == test.Questions[0].Level && quiz.Score >= 80)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTestTime(Test test)
    {
        DateTime rightNow = DateTime.Now;

        if (rightNow >= test.StartDate && rightNow <= test.EndDate)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void PrintTestInfo(Test test)
    {
        string subject = test.Questions[0].Subject;
        int level = test.Questions[0].Level;
        string date = test.StartDate.ToString("yyyy/MM/dd");
        string time = test.StartDate.ToShortTimeString();
        string endTime = test.EndDate.ToShortTimeString();

        Console.WriteLine($"{subject} | Level {level} | {date} | {time} - {endTime}\n");
    }

    private void UpcomingTests()
    {
        List<Test> testsReadyToGo = new List<Test>();
        List<Test> testsNotTimeYet = new List<Test>();
        List<Test> testsNoQuizNorTimeYet = new List<Test>();
        List<Test> testsUrgentQuiz = new List<Test>();

        List<Test> upcomingTests = FilterUpcomingTests();

        foreach (var test in upcomingTests)
        {
            if (IsQuizDone(test) && IsTestTime(test))
            {
                testsReadyToGo.Add(test);

                for (int i = 0; i < AnsweredTests.Count; i++)
                {
                    if (test.Id == AnsweredTests[i].TestId && test.Student.Contains(ActiveUser))
                    {
                        testsReadyToGo.Remove(test);
                    }
                }        
            }
            else if (!IsQuizDone(test) && IsTestTime(test))
            {
                testsUrgentQuiz.Add(test);
            }
            else if (!IsQuizDone(test) && !IsTestTime(test))
            {
                testsNoQuizNorTimeYet.Add(test);
            }
            else if (IsQuizDone(test) && !IsTestTime(test))
            {
                testsNotTimeYet.Add(test);
            }
        }

        Console.Clear();
        if (testsReadyToGo.Count > 0)
        {
            Console.WriteLine(@"||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
|||  All set, you can enter your test now. Good Luck!  ||| 
||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
");
            for (int i = 0; i < testsReadyToGo.Count; i++)
            {
                Console.Write($"{i + 1} - ");
                PrintTestInfo(testsReadyToGo[i]);
            }
        }

        if (testsUrgentQuiz.Count > 0)
        {
            Console.WriteLine(@"|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
|||  You must pass a quiz to enter your test ASAP. Test has started already!  ||| 
|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
");

            foreach (var item in testsUrgentQuiz)
            {
                PrintTestInfo(item);
            }
        }

        if (testsNoQuizNorTimeYet.Count > 0)
        {
            Console.WriteLine(@"|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
|||  To be given access to your test, do not forget to take a quiz first.   ||| 
|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
");
            foreach (var item in testsNoQuizNorTimeYet)
            {
                PrintTestInfo(item);
            }    
        }

        if (testsNotTimeYet.Count > 0)
        {
            Console.WriteLine(@"||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
|||  You have successfully passed your quizz. See you soon :)  ||| 
||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
");
            foreach (var item in testsNotTimeYet)
            {
                PrintTestInfo(item);
            }
        }

        if (testsReadyToGo.Count > 0)
        {
            Console.WriteLine();
            int chosen = Ui.ReadIntInRange("test", 1, testsReadyToGo.Count,"", false);
            AnswerTest(testsReadyToGo[chosen - 1]);
            testsReadyToGo.Remove(testsReadyToGo[chosen - 1]);
        }
        else if (testsUrgentQuiz.Count > 0)
        {
            Console.WriteLine();
            int chosen = Ui.ReadIntInRange("option", 0, 0, "Type '0' to exit and go take your quiz.\n", false);
        }
        else
        {
            Console.WriteLine(@"||||||||||||||||||||||||||||||||||||||
|||  You have no tests right now.  |||
||||||||||||||||||||||||||||||||||||||
");
            Ui.PressEnterToMoveOn();
        } 
    }

    private AnsweredTest AnswerTest(Test test)
    {
        //TODO: Fazer refactor - 1 único for
        double score = 0.0;
        int counter = 0;

        List<GivenAnswer> givenAnswers = new List<GivenAnswer>();

        Console.Clear();
        if (!test.HasRandomQuestions && !test.HasRandomAnswers)
        {
            foreach (var q in test.Questions)
            {
                if (test.EndDate > DateTime.Now)
                {
                    var remaining = (test.EndDate - DateTime.Now);
                    var strRemaining = remaining.ToString(@"hh\:mm");

                    Console.WriteLine($"Remaining Time: {strRemaining}\n");
                    counter++;
                    Console.WriteLine($"* Question {counter} / {test.Questions.Count} *\n");

                    var givenAnswer = UiAnswer.AnswerQuestion(q);

                    if (test.EndDate > DateTime.Now)
                    {
                        givenAnswers.Add(givenAnswer);

                        if (givenAnswer.IsRight)
                        {
                            score += 100.0 / test.Questions.Count;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, you can not access the next question.\nTime is over.");
                        Ui.PressEnterToMoveOn();
                        break;
                    }
                }
            }

            if (givenAnswers.Count == test.Questions.Count)
            {
                IsOver80(score);
            }
            else
            {
                Console.WriteLine("Sorry, time is over.");
                Ui.PressEnterToMoveOn();
            }
        }
        else if (!test.HasRandomQuestions && test.HasRandomAnswers)
        {
            foreach (var q in test.Questions)
            {
                if (test.EndDate > DateTime.Now)
                {
                    var remaining = (test.EndDate - DateTime.Now);
                    var strRemaining = remaining.ToString(@"hh\:mm");
                    Console.WriteLine($"Remaining Time: {strRemaining}\n");

                    List<int> shuffledIndexesAnswers = Console2.GenerateRandomlyOrderedIndexes(q.Answers.Count);
                    List<AvailableAnswer> shuffledAnswers = new List<AvailableAnswer>();

                    foreach (var item in shuffledIndexesAnswers)
                    {
                        shuffledAnswers.Add(q.Answers[item]);
                    }

                    q.Answers = shuffledAnswers;

                    counter++;
                    Console.WriteLine($"* Question {counter} / {test.Questions.Count} *\n");

                    var givenAnswer = UiAnswer.AnswerQuestion(q);

                    if (test.EndDate > DateTime.Now)
                    {
                        givenAnswers.Add(givenAnswer);

                        if (givenAnswer.IsRight)
                        {
                            score += 100.0 / test.Questions.Count;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, you can not access the next question.\nTime is over.");
                        Ui.PressEnterToMoveOn();
                        break;
                    }
                }
            }

            if (givenAnswers.Count == test.Questions.Count)
            {
                IsOver80(score);
            }
            else
            {
                Console.WriteLine("Sorry, time is over.");
                Ui.PressEnterToMoveOn();
            }
        }
        else if (test.HasRandomQuestions && !test.HasRandomAnswers)
        {
            List<int> shuffledIndexes = Console2.GenerateRandomlyOrderedIndexes(test.Questions.Count);
            List<Question> shuffledQuestions = new List<Question>();

            for (int i = 0; i < test.Questions.Count; i++)
            {
                if (test.EndDate > DateTime.Now)
                {
                    var remaining = (test.EndDate - DateTime.Now);
                    var strRemaining = remaining.ToString(@"hh\:mm");
                    Console.WriteLine($"Remaining Time: {strRemaining}\n");

                    Question q = test.Questions[shuffledIndexes[i]];
                    shuffledQuestions.Add(q);

                    counter++;
                    Console.WriteLine($"* Question {counter} / {test.Questions.Count} *\n");

                    var givenAnswer = UiAnswer.AnswerQuestion(q);

                    if (test.EndDate > DateTime.Now)
                    {
                        givenAnswers.Add(givenAnswer);

                        if (givenAnswer.IsRight)
                        {
                            score += 100.0 / test.Questions.Count;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, you can not access the next question.\nTime is over.");
                        Ui.PressEnterToMoveOn();
                        break;
                    }
                }
            }

            if (givenAnswers.Count == test.Questions.Count)
            {
                IsOver80(score);
            }
            else
            {
                Console.WriteLine("Sorry, time is over.");
                Ui.PressEnterToMoveOn();
            }
        }
        else
        {
            List<int> shuffledIndexes = Console2.GenerateRandomlyOrderedIndexes(test.Questions.Count);
            List<Question> shuffledQuestions = new List<Question>();

            for (int i = 0; i < test.Questions.Count; i++)
            {
                if (test.EndDate > DateTime.Now)
                {
                    var remaining = (test.EndDate - DateTime.Now);
                    var strRemaining = remaining.ToString(@"hh\:mm");
                    Console.WriteLine($"Remaining Time: {strRemaining}\n");

                    Question q = test.Questions[shuffledIndexes[i]];

                    shuffledQuestions.Add(q);

                    List<int> shuffledIndexesAnswers = Console2.GenerateRandomlyOrderedIndexes(q.Answers.Count);
                    List<AvailableAnswer> shuffledAnswers = new List<AvailableAnswer>();

                    foreach (var item in shuffledIndexesAnswers)
                    {
                        shuffledAnswers.Add(q.Answers[item]);
                    }

                    q.Answers = shuffledAnswers;
                    shuffledQuestions[i] = q;

                    counter++;
                    Console.WriteLine($"* Question {counter} / {test.Questions.Count} *\n");

                    var givenAnswer = UiAnswer.AnswerQuestion(q);

                    if (test.EndDate > DateTime.Now)
                    {
                        givenAnswers.Add(givenAnswer);

                        if (givenAnswer.IsRight)
                        {
                            score += 100.0 / test.Questions.Count;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, you can not access the next question.\nTime is over.");
                        Ui.PressEnterToMoveOn();
                        break;
                    }
                }
            }
            if (givenAnswers.Count == test.Questions.Count)
            {
                IsOver80(score);
            }
            else
            {
                Console.WriteLine("Sorry, time is over.");
                Ui.PressEnterToMoveOn();
            }
        }

        AnsweredTest answeredTest = new AnsweredTest(test.Id, ActiveUser, givenAnswers, score);
        AnsweredTests.Add(answeredTest);

        var jsonAnsweredTests = JsonSerializer.Serialize(AnsweredTests, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        File.WriteAllText("../../../json/answeredTests.json", jsonAnsweredTests);

        return answeredTest;
    }

    private static void IsOver80(double score)
    {
        if (score >= 80)
        {
            Console.WriteLine(@$"Congratulations!!!

You just got a score of {Math.Round(score, 2)}%.
");
            Ui.PressEnterToMoveOn();
        }
        else
        {
            Console.WriteLine($"Your score is {Math.Round(score, 2)}%.\n");
            Ui.PressEnterToMoveOn();
        }
    }

    private List<TestReport> GenerateTestReport()
    {
        List<TestReport> testReports = new List<TestReport>();

        foreach (var test in Tests)
        {
            var testReport = new TestReport(test);

            foreach (var answeredTest in AnsweredTests)
            {
                if (answeredTest.TestId == test.Id)
                {
                    testReport.AnsweredTests.Add(answeredTest);
                }
            }

            testReports.Add(testReport);
        }

        return testReports;
    }

    private void PrintTestReport()
    {
        Console.Clear();
        List<TestReport> testReports = GenerateTestReport();

        foreach (var report in testReports)
        {
            string subject = report.Test.Questions[0].Subject;
            int level = report.Test.Questions[0].Level;
            string date = report.Test.StartDate.ToString("yyyy/MM/dd");

            List<String> students = new List<string>();

            foreach (var item in report.Test.Student)
            {
                students.Add(item);
            }

            Console.WriteLine($"|||||   TEST: {subject} | Level {level} | {date}   |||||\n");

            foreach (var item in report.AnsweredTests)
            {
                string student = item.Student;
                students.Remove(student);
                double score = Math.Round(item.Score, 2);

                Console.WriteLine($"\t{student} - {score}%");
            }

            foreach (var st in students)
            {
                if (DateTime.Now > report.Test.EndDate)
                {
                    Console.WriteLine($"\t{st} - NO-SHOW");
                }
                else
                {
                    Console.WriteLine($"\t{st}");
                }
            }

            Console.WriteLine();
        }

        Ui.PressEnterToMoveOn();
    }

    private void CancelTest()
    {
        //TODO: make it work
        Console.Clear();
        List<TestReport> testReports = GenerateTestReport();

        for (int i = 0; i < testReports.Count; i++)
        {
            string subject = testReports[i].Test.Questions[0].Subject;
            int level = testReports[i].Test.Questions[0].Level;
            string date = testReports[i].Test.StartDate.ToString("yyyy/MM/dd");

            Console.WriteLine($"|||||   {i} |TEST: {subject} | Level {level} | {date}   |||||\n");
        }

        int choice = Ui.ReadIntInRange("number of the test you want to cancel", 0, testReports.Count, "If you do not want to cancel afterall, type '0' and enter to go back.\n", false);

        if (choice != 0)
        {
            testReports[choice - 1].Test.Student = new List<string>();
            Ui.PressEnterToMoveOn();
        }
    }

    private List<Quiz> GenerateStudentQuizReport(string student)
    {
        var studentQuizzes = new List<Quiz>();

        foreach (var quiz in Quizzes)
        {
            if (student == quiz.Student)
            {
                studentQuizzes.Add(quiz);
            }
        }

        return studentQuizzes;
    }

    private List<TestReport> GenerateStudentTestReport(string student)
    {
        List<TestReport> testReports = new List<TestReport>();

        foreach (var test in Tests)
        {
            var testReport = new TestReport(test);

            foreach (var answeredTest in AnsweredTests)
            {
                if (answeredTest.TestId == test.Id && answeredTest.Student == student)
                {
                    testReport.AnsweredTests.Add(answeredTest);
                }
            }

            if (testReport.AnsweredTests.Count > 0)
            {
                testReports.Add(testReport);
            }
        }

        return testReports;
    }

    private void PrintStudentQuizReport()
    {
        Console.Clear();
        var studentQuizzes = GenerateStudentQuizReport(ActiveUser);

        foreach (var quiz in studentQuizzes)
        {
            string subject = quiz.Questions[0].Subject;
            int level = quiz.Questions[0].Level;
            string date = quiz.Date.ToString("yyyy/MM/dd");
            double score = Math.Round(quiz.Score, 2);

            Console.Clear();
            Console.WriteLine($"|||||    QUIZ: {subject} | Level {level} | {date}   |||||\n");  
            Console.WriteLine($"\t* Score: {score}% *\n");

            for (int i = 0; i < quiz.Questions.Length; i++)
            {
                Console.WriteLine(quiz.Questions[i].Name);
                Console.WriteLine();
                UiUtils.PrintListAnswers(quiz.Questions[i].Answers, true);
                Console.WriteLine();

                foreach (var answer in quiz.Answers[i].GivenAnswers)
                {
                    Console.WriteLine($"Your answer: {answer}\n");
                }
                
                Console.WriteLine(quiz.Answers[i].IsRight ? "\tYou got it right!\n" : "\tTry harder...\n");
            }

            Ui.PressEnterToMoveOn();

        }
    }

    private void PrintStudentTestReport()
    {
        var testReports = GenerateStudentTestReport(ActiveUser);

        Console.Clear();

        foreach (var report in testReports)
        {
            string subject = report.Test.Questions[0].Subject;
            int level = report.Test.Questions[0].Level;
            string date = report.Test.StartDate.ToString("yyyy/MM/dd");
            var answeredTest = report.AnsweredTests[report.AnsweredTests.Count - 1];
            double score = Math.Round(answeredTest.Score, 2);

            Console.Clear();
            Console.WriteLine($"|||||    TEST: {subject} | Level {level} | {date}   |||||\n");
            Console.WriteLine($"\t* Score: {score}% *\n");

            foreach (var q in report.Test.Questions)
            {
                Console.WriteLine();
                Console.WriteLine(q.Name);
                Console.WriteLine();
                UiUtils.PrintListAnswers(q.Answers, true);
                Console.WriteLine();

                foreach (var answer in answeredTest.GivenAnswers)
                {
                    if (q.Id == answer.QuestionId)
                    {
                        foreach (var item in answer.GivenAnswers)
                        {
                            Console.WriteLine($"Your answer: {item}\n");
                        }
                        
                        Console.WriteLine(answer.IsRight ? "\tYou got it right!\n" : "\tTry harder...\n");
                    }
                }
            }

            Ui.PressEnterToMoveOn();
        }
    }
}