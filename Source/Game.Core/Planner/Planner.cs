namespace Game.Core.Planner
{
    public class Planner : IPlanner
    {
        public bool IsValidPlan(Plan plan)
        {
            return plan.Month is <= 12 and >= 1;
        }
    }
}