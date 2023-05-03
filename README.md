# Elden Bingo
This application makes running, administrating, spectating and streaming Elden Ring Bingo races easier. It was made with [Bingo Brawlers](https://bingobrawlers.com) in mind. It's built on .NET 6.0, so requires that the [runtimes](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.16-windows-x64-installer) are installed.

![eldenbingo-window](https://user-images.githubusercontent.com/604653/235782762-8af61e81-fd18-4ec0-87c8-e2893d895c85.png)  

# Overview of Features
* Host a server capable of running multiple bingo races at once
* Create bingo lobbies with optional administrator password and upload your own bingo .json file
* Join lobbies as an individual player or part of a team, or even as spectator
* Watch your teammates' positions live on the built in map of The Lands Between, or see everyone as a spectator
* Matches can be started, paused and stopped by the referees
* Squares can be checked/unchecked by players or by referees on behalf of players
* Right click to mark squares with stars, visible to everyone on the team
* Scroll Wheel up/down over a square allows players or referees to keep track of their progress of a square. This progress can only be seen by teammates or spectators, but not by opposing players/teams
* Customize bingo board color/size/font

# Hosting your own server
Download and install the latest server binaries (not yet available) or download the regular application and open Settings, and then enable "Host bingo server on launch".  
![host](https://user-images.githubusercontent.com/604653/235767838-ae5752a7-e9e7-4abb-a1d1-c8e6a59292aa.png)

Remember to set-up the appropriate port forwarding.

# Connecting to a server and joining a lobby
Connect to a server by pressing the 'Connect' button in the top left corner. You can choose to auto-connect to the same server every time you launch the application.

After you've successfully connected to the server, you can create your own lobby or join an existing one. A 'Lobby' in this application is a private room where you can run your bingo game. Any player that wishes to connect to a lobby needs that lobby's room name. There is no lobby browser.

## Joining a lobby
When joining (or creating) a lobby, you will be asked to input a nickname and select a team. In the 'Team' dropdown you can also select 'Spectator'.  
![join_spectator](https://user-images.githubusercontent.com/604653/235904929-2adf97ee-e6c4-4fc3-a8c7-586c383453d1.png)

## Creating a lobby
When creating a lobby you can enter any Room name you want, or use the one that was generated. If you enter an admin password, any player that connects to the lobby with that same admin password also becomes an administrator. If you leave it empty, only you will be able to administrate.

# Administrating a lobby
You get no unfair gameplay advantage as an administrator, so you can join the game just fine. Only AdminSpectators (ie. a player that is both administrator and spectator) have special privileges (see [AdminSpectators](#adminspectators)).

When you've joined a lobby as an administrator, the administrator tools will show under the bingo board. Use these tools to upload a Bingo .json file, following the same format as Bingo Brawlers and BingoSync. [Here is an example file](https://bingobrawlers.com/files/bingo-brawlers.json).

Once you've uploaded the file, a board is generated but will not be made visible to the players until the match is started. AdminSpectators can see the board and generate new boards if necessary.

Use the match control buttons at the bottom to start, pause or stop the match.  
![admin-controls](https://user-images.githubusercontent.com/604653/235774234-1d690243-9827-4510-9e51-a0befd3f0b78.png)  

# Viewing the map
Click the 'Open Map'-button at the top to display the Map window. This OpenGL window will show a map over The Lands Between and the position of all the players on your team. As a spectator, you will be able to see all players simultaneously. The map will try to fit all players in view at the same time, but if you want to follow a specific player you can press ethe number keys on your keyboard (1-9). 0 will reset to 'Fit all'. Press the 'N' key to toggle name tags.  
![mapview](https://user-images.githubusercontent.com/604653/235779143-aa708a4e-0443-49fb-96b7-b8c3dce73e67.png)  
*Spectating two players*

## Showing up on the map
The application will try to detect a running instance of EldenRing.exe. You can also press the 'Start Elden Ring' button in the top right corner to have the application start Elden Ring without Easy Anti-Cheat enabled. This is required for the application to be able to read the game memory and fetch your current position. It will always show the map of the overworld, even if players are underground. Players underground will be rendered as slightly transparent.

# Bingo board controls
* Left click - Check or uncheck a square for you/your team. Visible for everyone.
* Right click - Mark a square with a star. The star can be used for anything, like a reminder for yourself or for coordinating a plan with your teammates. The star is only visible to your own team.
* Mouse wheel up/down - Increase/decrease the count of this square. The counter is useful for squares that have a set number of tasks that need to be completed, where it's easy to lose track of your progress. The counter is only visible to your own team and spectators.

# AdminSpectators
As an AdminSpectator, you are basically the referee of the match. You can view the generated bingo board before the game has started, and generate new boards. If you mark a player in the client list, you can perform board actions on behalf of that player, like checking/unchecking squares and incrementing/decrementing the count of a square.  
![counters](https://user-images.githubusercontent.com/604653/235781324-d6e7f488-9c25-4920-b6be-682e061e8987.png)  

# Settings
The settings are mostly for the convenience of a streamer, to set up the UI components to the right size and position to be easily captured in the streaming software. You can also enable server hosting from here.

# Credits
* Process reading code from [EldenRingFPSUnlockerAndMore](https://github.com/uberhalit/EldenRingFpsUnlockAndMore) by [uberhalit](https://github.com/uberhalit)
* Button images shamelessly borrowed from [EldenRingMap](https://eldenringmap.com), drawn by [Caio Razera](https://dcaier.artstation.com/)
