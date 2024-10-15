using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.GameInterop;

public class EventManager
{
    private readonly GameProcessHandler _gameHandler;
    private readonly Client _client;
    private bool _gameStarted = false;
    private bool _initializingPointers = false;
    private bool _settingEventFlags = false;

    public EventManager(GameProcessHandler processHandler, Client client)
    {
        _gameHandler = processHandler;
        _client = client;
        resetGameStartedStatus();
        listenToClientEvents();
    }

    public async Task<(bool, long, long)> InitPointers()
    {
        _initializingPointers = true;
        long eventManPtr = -1;
        long setEventPtr = -1;
        await Task.Run(() =>
        {
            // Init pointers and make sure the addresses are correct
            eventManPtr = _gameHandler.GetEventManPtr();
            setEventPtr = _gameHandler.GetSetEventFlagPtr();
        });
        _initializingPointers = false;
        return (eventManPtr > 0 && setEventPtr > 0, eventManPtr, setEventPtr);
    }

    public async Task<bool> SetEventFlag(uint eventId, bool state)
    {
        _settingEventFlags = true;
        // Return if can't read coords
        if (!_gameHandler.LastCoordinates.HasValue)
        {
            return false;
        }
        bool success;
        long eventManPtr, setEventPtr;
        (success, eventManPtr, setEventPtr) = await InitPointers();
        if (!success)
        {
            if (eventManPtr <= 0)
            {
                MainForm.Instance?.PrintToConsole("Error: Couldn't find EventManPtr", Color.LightGray);
            }
            else if (setEventPtr <= 0)
            {
                MainForm.Instance?.PrintToConsole("Error: Couldn't find SetEventFlagPtr", Color.LightGray);
            }
            _settingEventFlags = false;
            return false;
        }
        await Task.Run(() =>
        {
            // Prepare machine code
            var machineCode = MachineCode.SetEventFlag(eventId, state, eventManPtr, setEventPtr);
            MainForm.Instance?.PrintToConsole($"Setting Event ID {eventId} to {state}", Color.LightGray);

            // Execute machine code
            _gameHandler.ExecuteAsm(machineCode);
            success = true;
        });
        _settingEventFlags = false;
        return success;
    }

    public bool? IsEventFlag(uint eventId)
    {
        // Return if can't read coords
        if (!_gameHandler.LastCoordinates.HasValue)
        {
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
    public async Task FireStartGameEvent()
    {
        _settingEventFlags = true;
        _gameStarted |= await SetEventFlag(GameData.GAME_STARTED_EVENT_ID, true);
        _settingEventFlags = false;
    }

    private void listenToClientEvents()
    {
        _client.AddListener<ServerJoinRoomAccepted>(acceptedIntoRoom);
        _gameHandler.CoordinatesRead += gameHandler_CoordinatesRead;
    }

    private void resetGameStartedStatus()
    {
        _gameStarted = false;
        _initializingPointers = false;
        _settingEventFlags = false;
        _gameHandler.ResetEventManPtrs();
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
        if (!status.HasValue || (status.Value != MatchStatus.Starting && status.Value != MatchStatus.Running))
        {
            resetGameStartedStatus();
            return;
        }
        if (status == MatchStatus.Starting)
        {
            //Initialize the pointers during game startup so we don't need to do it when the match starts
            if (!_gameHandler.AreEventManagerAddressesEstablished() && !_initializingPointers)
            {
                _ = InitPointers();
            }
        }
        else if (status == MatchStatus.Running)
        {
            //Else, the match is running, so lower the wall as soon as we read valid coordinates, and don't
            //try to start it again until joining a new lobby or the match ended/restarted
            if (!_gameStarted && coordinates.HasValue && !_settingEventFlags)
            {
                _ = FireStartGameEvent();
            }
        }
    }
}