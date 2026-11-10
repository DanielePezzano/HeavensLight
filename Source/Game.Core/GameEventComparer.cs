using System.Collections.Generic;

namespace Game.Core;

public sealed class GameEventComparer : IComparer<GameEvent>
{
    public int Compare(GameEvent a, GameEvent b)
    {
        var c = a.Eta.CompareTo(b.Eta);
        if (c != 0) return c;
        c = ((int)a.Type).CompareTo((int)b.Type);
        if (c != 0) return c;
        c = ((int)a.Priority).CompareTo((int)b.Priority);
        
        return c != 0 ? c : a.TiebreakSeed.CompareTo(b.TiebreakSeed);
    }
}