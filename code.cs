using System;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        // Read arguments (with defaults if missing)
        int maxRedemptions = args.ContainsKey("MaxRedemptions") 
            ? Convert.ToInt32(args["MaxRedemptions"]) 
            : 5;

        int windowSeconds = args.ContainsKey("WindowSeconds") 
            ? Convert.ToInt32(args["WindowSeconds"]) 
            : 600;

        bool checkIfMod = args.ContainsKey("CheckIfMod") 
            ? Convert.ToBoolean(args["CheckIfMod"]) 
            : true;

        bool checkIfVip = args.ContainsKey("CheckIfVip") 
            ? Convert.ToBoolean(args["CheckIfVip"]) 
            : true;

        // Grab Twitch user info from redemption args
        string userId = args["userId"].ToString();
        string userName = args["user"].ToString();

        var userInfo = CPH.TwitchGetExtendedUserInfoById(userId);
        bool isModerator = userInfo.IsModerator;
        bool isVip = userInfo.IsVip;

        // Respect Mod/VIP toggles
        if (checkIfMod && isModerator)
        {
            CPH.LogInfo($"Skipping cooldown for moderator {userName}");
            return true;
        }

        if (checkIfVip && isVip)
        {
            CPH.LogInfo($"Skipping cooldown for VIP {userName}");
            return true;
        }

        // Unique key per user
        string userKey = $"cp_redeems_{userId}";

        // Reset flag
        CPH.SetGlobalVar("cp_limit_exceeded_flag", true, false);

        // Redemption history
        var timestamps = CPH.GetGlobalVar<List<long>>(userKey, true) ?? new List<long>();
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Trim old redemptions
        timestamps.RemoveAll(t => t <= now - windowSeconds);

        // Add this redemption
        timestamps.Add(now);

        // Save history
        CPH.SetGlobalVar(userKey, timestamps, true);

        int overLimit = timestamps.Count - maxRedemptions;

        if (overLimit >= 1)
        {
            // First level warning
            CPH.SendMessage($"@{userName}, you’ve reached the channel point limit of {maxRedemptions} per {windowSeconds / 60} minutes.");

            if (overLimit >= 2)
            {
                // Escalation flag
                CPH.SetGlobalVar("cp_limit_exceeded_flag", false, false);
            }
        }

        return true;
    }
}
