using System;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        // CONFIGURATION
        int maxRedemptions = 5;        // allowed redemptions per rolling window
        int windowSeconds = 600;       // rolling window in seconds (10 minutes)

        // Get Twitch user info
        string userName = args["user"].ToString();
        string userId = args["userId"].ToString();

        // Unique key for this user's redemption history
        string userKey = $"cp_redeems_{userId}";

        // Clear temporary flag first
        CPH.SetGlobalVar("cp_limit_exceeded_flag", true, false);

        // Get user's redemption timestamps
        var timestamps = CPH.GetGlobalVar<List<long>>(userKey, true) ?? new List<long>();

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Remove timestamps outside the rolling window
        timestamps.RemoveAll(t => t <= now - windowSeconds);

        // Add current redemption
        timestamps.Add(now);

        // Save updated list
        CPH.SetGlobalVar(userKey, timestamps, true);

        int overLimit = timestamps.Count - maxRedemptions;

        if (overLimit >= 1)
        {
            // Send chat warning
            CPH.SendMessage($"@{userName} you’ve reached the channel point limit of {maxRedemptions} per {windowSeconds/60} minutes.");

            if (overLimit >= 2)
            {
                // Set flag for timeout logic
                CPH.SetGlobalVar("cp_limit_exceeded_flag", false, false);
                CPH.SendMessage($"@{userName}, you'll now be timed out for a few minutes.");
            }
        }

        return true; // continue with other actions
    }
}
