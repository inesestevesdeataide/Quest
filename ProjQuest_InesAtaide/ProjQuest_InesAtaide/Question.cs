using System;
namespace ProjQuest_InesAtaide;

public class Question
{
    private static int _counter;

    public int Id { get; set; }
    public string Subject { get; set; }
    public int Level { get; set; }
    public List<String> Tag { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public bool OnlyTest { get; set; }
    public List<AvailableAnswer> Answers { get; set; }

    public Question()
    {
        _counter++;
    }

    public Question(string subject, int level, List<String> tag, string name, int type, bool onlyTest, List<AvailableAnswer> answers) : this()
    {
        Id = _counter;
        Subject = subject;
        Level = level;
        Tag = tag;
        Name = name;
        Type = type;
        OnlyTest = onlyTest;
        Answers = answers;
    }
}