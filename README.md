# Streamer.bot Channel Point Moderation

This repository provides a customizable moderation script for Twitch Channel Point redemptions using Streamer.bot. The script helps manage excessive redemptions by implementing cooldowns, issuing warnings, and applying timeouts when necessary.

## 📦 Files Included

* `moderation.cs`: The C# script that handles the moderation logic.
* `channel point redemption moderation.sb`: An importable Streamer.bot action file for easy integration.

## ⚙️ Features

* **Per-User Cooldown Tracking**: Maintains individual cooldowns to prevent abuse.
* **Customizable Settings**: Easily adjustable parameters for max redemptions, cooldown window, and timeout duration.
* **Moderator & VIP Exemptions**: Configurable options to exclude moderators and VIPs from moderation.
* **Two-Level Warning System**: Sends a chat warning on the first offense and applies a timeout on the second.
* **Non-Persistent Data**: Resets all tracking data upon Streamer.bot restart to ensure fresh moderation.

## 🔧 Installation

1. **Import the Action**: Open Streamer.bot and select "Import" to import the `channel point redemption moderation.sb` file.

* Just drag and drop the file into the import field, and click Import.

2. **Configure Arguments**:

   * `MaxRedemptions`: Maximum number of redemptions allowed per user within the specified time window (default: 5).
   * `WindowSeconds`: Time window in seconds for tracking redemptions (default: 600 seconds or 10 minutes).
   * `CheckIfMod`: Set to `true` to skip moderation for moderators (default: `true`).
   * `CheckIfVip`: Set to `true` to skip moderation for VIPs (default: `true`).
   * `TimeoutSeconds`: Duration of the timeout in seconds when a user exceeds the redemption limit (default: 120 seconds).
   * `TimeoutReason`: Reason displayed for the timeout (default: "Spamming Channel Redemptions").

## 🛠️ Usage

Upon setting up, the script will automatically monitor Channel Point redemptions. It will:

* Track individual user redemptions.
* Issue a chat warning if a user exceeds the redemption limit.
* Apply a timeout if the user exceeds the limit again within the same time window.

## 📄 License

This project is licensed under the MIT License.
