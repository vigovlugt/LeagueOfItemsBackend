# LeagueOfItems

League of Items is an analytics platform for Leauge of Legends statistics.  
You can think of League of Items as U.GG but for items and runes.  
If you ever liked an item but didn't know what champions to play it with, this site is for you. You will also find lots of other data about all items, runes, builds and patch note changes. All data is sourced from U.GG.

This repository contains the backend of the application, the frontend can be found [here](https://github.com/vigovlugt/LeagueOfItemsFrontend).

## Getting Started

1. cd src/ConsoleApp
2. dotnet restore
3. dotnet run riot
4. dotnet run ugg
5. dotnet run export
6. dotnet run github

dotnet user-secrets set "Cloudflare:AnalyticsApiToken" "XXXXXX"
dotnet user-secrets set "Cloudflare:AccountId" "XXXXXX"
