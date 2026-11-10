using System;

namespace Game.Core.TurnResolver;

public readonly struct GameTime : IComparable<GameTime>, IEquatable<GameTime>
{
    public readonly int TotalTick; // tick assoluto dalla fondazione
    public GameTime(int totalTick) => TotalTick = totalTick;
    public int Year => TotalTick / TimeConstants.TicksPerYear;
    public int Month => (TotalTick / TimeConstants.TicksPerMonth) % 12;
    public int SubTick => TotalTick % TimeConstants.TicksPerMonth;

    public static GameTime FromYMS(int year, int month, int subTick) =>
        new GameTime((year * 12 + month) * TimeConstants.TicksPerMonth + subTick);

    public int CompareTo(GameTime other) => TotalTick.CompareTo(other.TotalTick);
    public bool Equals(GameTime other) => TotalTick == other.TotalTick;
    public override int GetHashCode() => TotalTick;
}