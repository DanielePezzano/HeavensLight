namespace Game.Core.Tests;

public class Tests
{
    [TestCase(-1, false)]
    [TestCase(0, false)]
    [TestCase(1, true)]
    [TestCase(12, true)]
    [TestCase(13, false)]
    public void AcceptOnlyValidMonths(int  month, bool isValid)
    {
        var playerPlan = new Plan(1, month);
        var planner = new Planner.Planner();
        
        var actual = planner.IsValidPlan(playerPlan);
        
        Assert.AreEqual(isValid, actual);
    }
}