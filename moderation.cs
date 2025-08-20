/*
 * Streamer.bot Channel Point Moderation Script
 * GitHub: https://github.com/ksduster/Streamer.bot-Channel-Point-Moderation
 * Developer: Duster (ksduster)
 *
 * Features:
 * - Per-user cooldown tracking for channel point redemptions
 * - Configurable max redemptions, cooldown window, and timeout
 * - Moderator and VIP exemptions
 * - Two-level warning system with optional timeout
 * - Non-persistent globals reset on Streamer.bot restart
 */
using System;
using System.Collections.Generic;

public class CPHInline
{
    public bool Execute()
    {
        // ----------------------
        // ARGUMENTS / CONFIGURATION
        // ----------------------
        int maxRedemptions = args.ContainsKey("MaxRedemptions") ? Convert.ToInt32(args["MaxRedemptions"]) : 5; // default max redemptions
        int windowSeconds = args.ContainsKey("WindowSeconds") ? Convert.ToInt32(args["WindowSeconds"]) : 600; // default 10-minute window
        bool checkIfMod = args.ContainsKey("CheckIfMod") ? Convert.ToBoolean(args["CheckIfMod"]) : true;
        bool checkIfVip = args.ContainsKey("CheckIfVip") ? Convert.ToBoolean(args["CheckIfVip"]) : true;
        int timeoutDuration = args.ContainsKey("TimeoutSeconds") ? Convert.ToInt32(args["TimeoutSeconds"]) : 30; // default timeout 30 seconds
        string timeoutReason = args.ContainsKey("TimeoutReason") ? args["TimeoutReason"].ToString() : "Exceeded channel point redemption limit";
        // ----------------------
        // USER INFO
        // ----------------------
        string userId = args["userId"].ToString();
        string userName = args["user"].ToString();
        var userInfo = CPH.TwitchGetExtendedUserInfoById(userId);
        bool isModerator = userInfo.IsModerator;
        bool isVip = userInfo.IsVip;
        // Reset escalation flag (non-persistent)
        CPH.SetGlobalVar("cp_limit_exceeded_flag", true, false);
        // Skip script for Mods/VIPs if configured
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

        // ----------------------
        // REDEMPTION TRACKING
        // ----------------------
        string userKey = $"cp_redeems_{userId}";
        var timestamps = CPH.GetGlobalVar<List<long>>(userKey, false) ?? new List<long>();
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // Remove old timestamps outside window
        timestamps.RemoveAll(t => t <= now - windowSeconds);
        // Add current redemption
        timestamps.Add(now);
        // Save updated list (non-persistent)
        CPH.SetGlobalVar(userKey, timestamps, false);
        int overLimit = timestamps.Count - maxRedemptions;
        // ----------------------
        // WARNINGS & TIMEOUTS
        // ----------------------
        if (overLimit >= 1)
        {
            // First-level warning
            CPH.SendMessage($"@{userName}, you’ve reached the channel point limit of {maxRedemptions} per {windowSeconds / 60} minutes.");
            if (overLimit >= 2)
            {
                // Second-level: set flag and execute timeout
                CPH.SetGlobalVar("cp_limit_exceeded_flag", false, false);
                // Execute timeout
                CPH.TwitchTimeoutUser(userName, timeoutDuration, timeoutReason, false);
                // Optional log
                CPH.LogInfo($"Timed out {userName} for {timeoutDuration} seconds: {timeoutReason}");
            }
        }

        return true;
    }
}
        // ----------------------
        // WARNINGS & TIMEOUTS
        // ----------------------
        if (overLimit >= 1)
        {
            // First-level warning
            CPH.SendMessage($"@{userName}, you’ve reached the channel point limit of {maxRedemptions} per {windowSeconds / 60} minutes.");

            if (overLimit >= 2)
            {
                // Second-level: set flag and execute timeout
                CPH.SetGlobalVar("cp_limit_exceeded_flag", false, false);

                // Execute timeout
                CPH.TwitchTimeoutUser(userName, timeoutDuration, timeoutReason, false);

                // Optional log
                CPH.LogInfo($"Timed out {userName} for {timeoutDuration} seconds: {timeoutReason}");
            }
        }

        return true;
    }
}
