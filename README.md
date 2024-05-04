# Elden Bingo
This application makes running, administrating, spectating and streaming Elden Ring Bingo races easier. It was made with [Bingo Brawlers](https://bingobrawlers.com) in mind. It's built on .NET 6.0, so requires that the [runtimes](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.19-windows-x64-installer) are installed.

![eldenbingo-window](https://user-images.githubusercontent.com/604653/236489862-7a69d672-9243-49fb-88fc-236ae502f655.png)  
![mapview](https://user-images.githubusercontent.com/604653/235779143-aa708a4e-0443-49fb-96b7-b8c3dce73e67.png)  
*Spectating two players*

# Overview of Features
* Host a server capable of running multiple bingo races at once
* Create bingo lobbies with optional administrator password and upload your own bingo .json file
* Join lobbies as an individual player or part of a team, or even as spectator
* Chat in lobbies
* Watch your teammates' positions live on the built in map of The Lands Between, or see everyone as a spectator
* Configure your lobby with custom rules, like randomized starting classes
* Matches can be started, paused and stopped by the referees
* Squares can be checked/unchecked by players or by referees on behalf of players
* Right click to mark squares with stars, visible only to the player marking the square
* Scroll Wheel up/down over a square allows players or referees to keep track of their progress of a square. This progress can only be seen by the owning player and spectators, but not by opposing players/teams
* Customize bingo board color/size/font

# Hosting your own server
Download the regular application and open Settings, and then enable "Host bingo server on launch".  
![host](https://user-images.githubusercontent.com/604653/235767838-ae5752a7-e9e7-4abb-a1d1-c8e6a59292aa.png)

Remember to set-up the appropriate port forwarding.

You can also host a dedicated lobby server. I don't release server binaries with the regular releases anymore but if you need them you can easily download the code and compile it yourself.

# Connecting to a server and joining a lobby
Connect to a server by pressing the 'Connect' button in the top left corner. You can choose to auto-connect to the same server every time you launch the application.

After you've successfully connected to the server, you can create your own lobby or join an existing one. A 'Lobby' in this application is a private room where you can run your bingo game. Any player that wishes to connect to a lobby needs that lobby's room name. There is no lobby browser.

## Joining a lobby
When joining (or creating) a lobby, you will be asked to input a nickname and select a team. In the 'Team' dropdown you can also select 'Spectator'.  
![join_spectator](https://user-images.githubusercontent.com/604653/235904929-2adf97ee-e6c4-4fc3-a8c7-586c383453d1.png)

## Creating a lobby
When creating a lobby you can enter any Room name you want, or use the one that was generated. If you enter an admin password, any player that connects to the lobby with that same admin password also becomes an administrator. If you leave it empty, only you will be able to administrate.

You can also configure the rules of the lobby:  
![Lobby settings](https://github.com/awsker/EldenBingo/assets/604653/e257606b-3db2-46b9-8b2e-211a2cedaecd)

* *Board size* specifies how large the bingo board will be. This takes effect the next time a board is generated.  
* *Random seed* will ensure that the same sequence of boards and random classes are generated/picked. This sequence will reset when a new json is uploaded. **0 means a random seed will be used**.  
* *Preparation Time* creates an extra preparation phase at the beginning of the match, after the initial countdown, in which players can see the board and the available classes and plan ahead before the match starts.  **0 means no preparation phase**
* *Bonus points for bingo* can be used if you want bingo lines to be worth a set number of points instead of immediately ending the match.  
* *Limit starting classes* can be used if you want to limit the choice of starting classes in order to introduce some variation. Set the pool of possible classes below.
* Setting *Max square in same category* will ensure that at most that many squares in the same category will be included in one board. **0 means this feature is disabled**. For more info on categories and the json format, see [Json Format](#json-format). 

# Administrating a lobby
You get no unfair gameplay advantage as an administrator, so you can join the game just fine. Only Admin-Spectators (ie. a player that is both administrator and spectator) have special privileges (see [AdminSpectators](#adminspectators)).

When you've joined a lobby as an administrator, the administrator tools will show under the bingo board. Use these tools to upload a Bingo .json file, following the same format as Bingo Brawlers and BingoSync but with some extensions. [Here is an example file](https://bingobrawlers.com/files/bingo-brawlers.json). For more info on the json format, see [Json Format](#json-format).

Once you've uploaded the file, a board is generated but will not be made visible to the players until the match is started. AdminSpectators can see the board and generate new boards if necessary.

Use the match control buttons at the bottom to start, pause or stop the match.  
![admin-controls](https://user-images.githubusercontent.com/604653/235774234-1d690243-9827-4510-9e51-a0befd3f0b78.png)  

# Map View
Click the 'Open Map'-button at the top to display the Map window. This OpenGL window will show a map over The Lands Between and the position of all the players on your team. As a spectator, you will be able to see all players simultaneously. The map will try to fit all players in view at the same time.

## Showing up on the map
The application will try to detect a running instance of EldenRing.exe. You can also press the 'Start Elden Ring' button in the top right corner to have the application start Elden Ring without Easy Anti-Cheat enabled. This is required for the application to be able to read the game memory and fetch your current position. It will always show the map of the overworld, even if players are underground. Players underground will be rendered as slightly transparent.

## Drawing on the map
You can use the right mouse button to draw on the map. This is meant for live streaming, and is completely client side at the moment.  
![telestrator](https://github.com/awsker/EldenBingo/assets/604653/98aa472b-fffd-48d4-9420-aba1b8df25b4)

## Map Controls
* Left Click - Pan the map (stops following players)
* Right Click - Draw on the map
* Mouse wheel - Zoom in and out
* N - Toggle name tags visibility
* Z - Undo last line drawn
* C - Clear all lines
* F - Fit all players in view
* 1-9 - Follow a specific player

## Class choice display
If you enable the setting "Show random classes in map", the available starting classes will be displayed in the Map Window when the match starts. This is useful if you're a streamer and want your viewers to be able to see the selection themselves. Just set up a scene in your streaming software that captures the map window (as a Game capture) and you're good to go. Clicking left mouse, pressing Space or Escape will close the classes display.  
![random classes](https://github.com/awsker/EldenBingo/assets/604653/562f384c-231e-42fd-8234-9715887b377d)


# Bingo board controls
* Left click - Check or uncheck a square for you/your team. Visible for everyone.
* Right click - Mark a square with a star. The star can be used for anything, like a reminder for yourself or for coordinating a plan with your teammates. The star is only visible to your own team.
* Mouse wheel up/down - Increase/decrease the count of this square. The counter is useful for squares that have a set number of tasks that need to be completed, where it's easy to lose track of your progress. The counter is only visible to your own team and spectators.

# AdminSpectators
As an AdminSpectator, you are basically the referee of the match. You can view the generated bingo board before the game has started, and generate new boards. If you mark a player in the client list, you can perform board actions on behalf of that player, like checking/unchecking squares and incrementing/decrementing the count of a square.  
![counters](https://user-images.githubusercontent.com/604653/235781324-d6e7f488-9c25-4920-b6be-682e061e8987.png)  

# Settings
The settings are mostly for the convenience of a streamer, to set up the UI components to the right size and position to be easily captured in the streaming software. You can also enable server hosting from here.

# Json Format
The format is the same as is used by Bingo Brawlers and BingoSync but with extensions for tooltips, categories and counters.  
![image](https://github.com/awsker/EldenBingo/assets/604653/a560d869-6954-4db7-9218-f9c40b838909)

Use the **tooltip** key to define a tooltip when hovering that square:  
 ![image](https://github.com/awsker/EldenBingo/assets/604653/a5f97ed4-9454-462a-bd31-8b2de1e186f7)

Use the **category** key to define a single category, or the **categories** key to define an array of categories. These categories can be used in conjunction with the lobby setting *Max square in same category* to ensure that at most that number of categories will be present in one bingo board, in order to generate more balanced bingo boards.

Use the **count** key to define that the square requires a set number clicks to complete. When users click this square, it will increment the counter by 1 (as if scrolling the mouse wheel up) until this value is reached. This behaviour is optional and can be disabled in the Settings dialog by the user, in which case they must use their mouse wheel to manually track the count and click the square only when it's completed.

# Credits
* Tremwil on The Grand Archives discord
* Process reading code from [EldenRingFPSUnlockerAndMore](https://github.com/uberhalit/EldenRingFpsUnlockAndMore) by [uberhalit](https://github.com/uberhalit)
* Button images shamelessly borrowed from [EldenRingMap](https://eldenringmap.com), drawn by [Caio Razera](https://dcaier.artstation.com/)
