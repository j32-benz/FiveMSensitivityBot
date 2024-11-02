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
- BattleBit
- Battlefield
- Fortnite

## Setup

1. **Bot Token**: Ensure your bot token is set as an environment variable named `DISCORD_BOT_TOKEN`.

### Build the Bot

To build the bot, run the following command in the project directory:

```bash
dotnet build
```

This compiles the project and prepares it for execution.

### Run the Bot

After building, run the bot with:

```bash
dotnet run
```

## Commands

- **/sens**
  - **Usage**: `/sens game:<game> sens:<sensitivity> dpi:<DPI>`
  - **Description**: Calculates CM360 based on the selected game, sensitivity, and DPI.

- **/cm360**
  - **Usage**: `/cm360 cm360:<CM360 value> dpi:<DPI>`
  - **Description**: Directly computes the Profile_MouseOnFootScale from CM360 and DPI.
