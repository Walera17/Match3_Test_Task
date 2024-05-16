using System.Collections.Generic;

public static class Extensions
{
    public static void AddToMatches(this Gem gem, List<Gem> matches)
    {
        if (!matches.Contains(gem))
            matches.Add(gem);
    }
}