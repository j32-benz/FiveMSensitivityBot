# FiveMSensitivityBot

## Overview
FiveMSensitivityBot is a Discord bot designed to help players calculate their "Profile_MouseOnFootScale" for FiveM based on their game sensitivity, DPI, and CM360 settings. The bot supports several popular games, providing quick, accurate calculations via Discord commands.

## Features
- **/sens**: Calculate CM360 based on game sensitivity and DPI for supported games.
- **/cm360**: Directly compute Profile_MouseOnFootScale from CM360 and DPI values.

## Supported Games
- Apex Legends
- Counter-Strike
- Valorant
- Overwatch
- Battlefield
- Fortnite
- Call of Duty: Warzone
- The Finals

## Setup

1. **Bot Token**: Ensure your bot token is set as an environment variable named `DISCORD_BOT_TOKEN`.

### Installing .NET on Debian

To run this project, you need to have .NET installed. Follow these steps to install .NET on Debian.

1. **Add the Microsoft Package Repository**:
   ```sh
   wget https://packages.microsoft.com/config/debian/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   rm packages-microsoft-prod.deb
   ```

2. **Update the package list**:
   ```sh
   sudo apt update
   ```

3. **Install the .NET SDK** (required to build the bot):
   ```sh
   sudo apt install dotnet-sdk-7.0
   ```
   *(Replace `7.0` with your desired version if needed)*

4. **Verify the Installation**:
   ```sh
   dotnet --version
   ```
   This should display the version number, confirming that .NET is installed.

### Build the Bot

Once .NET is installed, you can build the bot by running the following command in the project directory:

```bash
dotnet build
```

This compiles the project to an executable.

## Commands

- **/sens**
  - **Usage**: `/sens game:<game> sens:<sensitivity> dpi:<DPI>`
  - **Description**: Calculates CM360 based on the selected game, sensitivity, and DPI.

- **/cm360**
  - **Usage**: `/cm360 cm360:<CM360 value> dpi:<DPI>`
  - **Description**: Directly computes the Profile_MouseOnFootScale from CM360 and DPI.
