using System.Collections.Generic;
public class CooldownHandler
{

    private static List<string> Cooldowns = new List<string>();

    public static void CreateFor(string data, float seconds)
    {
        if (IsUnderCooldown(data)) return;
        Cooldowns.Add(data);
        new ActionTimer(() => Cooldowns.Remove(data), seconds, seconds).Run();
    }

    public static bool IsUnderCooldown(string data)
    {
        return Cooldowns.Contains(data);
    }

    /// <summary>
    /// Checks if the given data is under cooldown if not it puts them into cooldown after the condition.
    /// </summary>
    /// <param name="data">The string data</param>
    /// <returns>if is under cooldown</returns>
    public static bool IsUnderCreateIfNot(string data, float seconds)
    {
        bool result = IsUnderCooldown(data);
        if (!result) CreateFor(data, seconds);
        return false;
    }
}
