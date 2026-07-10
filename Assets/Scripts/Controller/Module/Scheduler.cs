using UnityEngine;

public class Scheduler : FleetBaseModule
{
    /// <summary>
    /// Tìm robot phù hợp nhất để thực hiện task.
    /// </summary>
    public Robot FindBestRobot(RobotTask task)
    {
        Robot bestRobot = null;
        float bestScore = float.MaxValue;

        foreach (var pair in fleet.RobotManager.GetAllRobots())
        {
            Robot robot = pair.Value;

            if (!robot.StatModule.IsAvailableForTask())
                continue;

            float score = CalculateScore(robot, task);

            if (score < bestScore)
            {
                bestScore = score;
                bestRobot = robot;
            }
        }

        return bestRobot;
    }

    private float CalculateScore(Robot robot, RobotTask task)
    {
        MapPoint target = fleet.MapManager.GetPoint(task.PickupPoint);

        if (target == null) return float.MaxValue;

        return Vector3.Distance(robot.transform.position, target.Position);
    }
}