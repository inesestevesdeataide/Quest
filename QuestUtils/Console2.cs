using System;
namespace Quest.Utils;

public static class Console2
{
    public static void EnableUTF8Encoding()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
    }

    public static List<int> GenerateRandomlyOrderedIndexes(int n)
    {
        List<int> orderedIndexes = new List<int>();

        for (int i = 0; i < n; i++)
        {
            orderedIndexes.Add(i);
        }

        Random rnd = new Random();
        List<int> shuffledIndexes = new List<int>();

        while (orderedIndexes.Count > 0)
        {
            int nextRandom = rnd.Next(0, orderedIndexes.Count);
            shuffledIndexes.Add(orderedIndexes[nextRandom]);
            orderedIndexes.RemoveAt(nextRandom);
        }

        return shuffledIndexes;
    }
}