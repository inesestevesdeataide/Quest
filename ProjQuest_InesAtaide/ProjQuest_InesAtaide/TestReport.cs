using System;
namespace ProjQuest_InesAtaide;

public class TestReport
{
    public Test Test { get; set; }
    public List<AnsweredTest> AnsweredTests { get; set; }

    public TestReport()
    {
        AnsweredTests = new List<AnsweredTest>();
    }

    public TestReport(Test test) : this()
    {
        Test = test;
    }
}

