using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.GameInterop;

public class EventManager
{
    private readonly GameProcessHandler _gameHandler;
    private readonly Client _client;
    private bool _gameStarted = false;

    public EventManager(GameProcessHandler processHandler, Client client)
    {
        _gameHandler = processHandler;
        _client = client;
        resetGameStartedStatus();
        listenToClientEvents();
    }

    public bool SetEventFlag(uint eventId, bool state)
    {
        // Return if can't read coords
        if (!_gameHandler.LastCoordinates.HasValue) {
            return false;
        }

        // Return if EventManPtr is invalid
        var eventManPtr = _gameHandler.GetEventManPtr();
        if (eventManPtr <= 0)
        {
            MainForm.Instance?.PrintToConsole("Error: Couldn't find EventManPtr", Color.LightGray);
            return false;
        }

        // Return if IsEventFlag ptr is invalid
        var setEventPtr = _gameHandler.GetSetEventFlagPtr();
        if (setEventPtr <= 0)
        {
            MainForm.Instance?.PrintToConsole("Error: Couldn't find SetEventFlagPtr", Color.LightGray);
            return false;
        }
        
        // Prepare machine code
        var machineCode = MachineCode.SetEventFlag(eventId, state, eventManPtr, setEventPtr);
        MainForm.Instance?.PrintToConsole($"Setting Event ID {eventId} to {state}", Color.LightGray);
        
        // Execute machine code
        _gameHandler.ExecuteAsm(machineCode);
        return true;
    }
    
    public bool? IsEventFlag(uint eventId)
    {
        // Return if can't read coords
        if (!_gameHandler.LastCoordinates.HasValue) {
            return null;
        }
        
        // Return if EventManPtr is invalid
        var eventManPtr = _gameHandler.GetEventManPtr();
        if (eventManPtr <= 0)
        {
            MainForm.Instance?.PrintToConsole("Error: Couldn't find EventManPtr", Color.LightGray);
            return null;
        }
        
        // Return if IsEventFlag ptr is invalid
        var isEventPtr = _gameHandler.GetIsEventFlagPtr();
        if (isEventPtr <= 0)
        {
            MainForm.Instance?.PrintToConsole("Error: Couldn't find SetEventFlagPtr", Color.LightGray);
            return null;
        }
        
        // Setup return pointer
        var returnPtr = _gameHandler.GetPrefferedIntPtr(MachineCode.IsEventFlagMachineCode.Length, flProtect: WinAPI.PAGE_EXECUTE_READWRITE);
        _gameHandler.WriteToPtr(returnPtr, BitConverter.GetBytes(0ul));
        
        // Prepare machine code
        var machineCode = MachineCode.IsEventFlag(eventId, eventManPtr, isEventPtr, returnPtr);
        
        // Execute machine code
        _gameHandler.ExecuteAsm(machineCode);

        // Read return value
        byte[] bytes = new byte[sizeof(ulong)];
        _gameHandler.ReadFromPtr(returnPtr, bytes);
        
        // Free return pointer
        _gameHandler.Free(returnPtr);
        
        // Convert to bool
        var state = BitConverter.ToBoolean(bytes);
        MainForm.Instance?.PrintToConsole($"Event Id {eventId} is {state}", Color.LightGray);
        
        return state;
    }
    
    // Easy to call, has hard coded flag value.
    public void FireStartGameEvent()
    {
        _gameStarted |= SetEventFlag(GameData.GAME_STARTED_EVENT_ID, true);
    }

    private void listenToClientEvents()
    {
        _client.AddListener<ServerJoinRoomAccepted>(acceptedIntoRoom);
        _gameHandler.CoordinatesRead += gameHandler_CoordinatesRead;
    }

    private void resetGameStartedStatus() {
        _gameStarted = false;
    }

    private void acceptedIntoRoom(ClientModel? model, ServerJoinRoomAccepted accepted)
    {
        //Joined a new lobby, so we reset the game status...
        resetGameStartedStatus();
        //... and check if the current match requires us to start the game (using the last coordinates read)
        handleMatchStatus(accepted.MatchStatus, _gameHandler.LastCoordinates);
    }

    private void gameHandler_CoordinatesRead(object? sender, MapCoordinateEventArgs e)
    {
        //Whenever coordinates are read, check if we need to start the game
        handleMatchStatus(_client.Room?.Match?.MatchStatus, e.Coordinates);
    }

    private void handleMatchStatus(MatchStatus? status, MapCoordinates? coordinates)
    {
        //Whenever the match is not running, reset the game status
        if (!status.HasValue || status.Value != MatchStatus.Running)
        {
            resetGameStartedStatus();
            return;
        }
        //Else, the match is running, so lower the wall as soon as we read valid coordinates, and don't
        //try to start it again until joining a new lobby or the match ended/restarted
        if (!_gameStarted && coordinates.HasValue)
        {
            FireStartGameEvent();
        }
    }
}