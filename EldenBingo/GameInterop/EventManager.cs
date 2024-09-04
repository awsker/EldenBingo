using EldenBingoCommon;
using Neto.Shared;

namespace EldenBingo.GameInterop;

public class EventManager
{
    public const int EventFlagManOffset = 0x2;
    public const int SetEventFlagFunctionOffset = 0xC;
    public const int StateOffset = 0x16;
    public const int EventFlagIdOffset = 0x1E;

    // https://defuse.ca/online-x86-assembler.htm
    public static readonly byte[] Asm = {
        0x48, 0xb9, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0x0f,   // movabs rcx,0xfffffff00000000       ;EventFlagMan
        0x48, 0xb8, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0xff, 0x0f,   // movabs rax,0xfffffff00000000       ;SetEventFlagFunction
        0x41, 0xb0, 0x01,                                             // mov    r8b,0x1                     ;State
        0x48, 0x83, 0xec, 0x08,                                       // sub    rsp,0x8                     ;Flag needs to be written to memory
        0xc7, 0x04, 0x24, 0x00, 0x00, 0x00, 0x0F,                     // mov    DWORD PTR [rsp],0xF000000   ;Flag
        0x48, 0x8d, 0x14, 0x24,                                       // lea    rdx,[rsp]                   ;Flag Pointer
        0x48, 0x83, 0xec, 0x28,                                       // sub    rsp,0x28
        0xff, 0xd0,                                                   // call   rax
        0x48, 0x83, 0xc4, 0x30,                                       // add    rsp,0x30
        0xc3,                                                         // ret
    };

    private readonly GameProcessHandler _gameHandler;
    private readonly Client _client;
    private bool _hasLoweredWall = false;

    public EventManager(GameProcessHandler processHandler, Client client)
    {
        _gameHandler = processHandler;
        _client = client;
        resetHasLoweredStatus();
        listenToClientEvents();
    }

    public void SetEventFlag(uint eventId, bool state)
    {
        var asm = Asm.ToArray();
        // Set EventFlagMan pointer
        var eventManPtr = _gameHandler.GetEventManPtr();
        if (eventManPtr <= 0)
        {
            return;
        }
        Array.Copy(BitConverter.GetBytes(eventManPtr), 0, asm, EventFlagManOffset, sizeof(long));
        // Set Function Call Address
        var setEventPtr = _gameHandler.GetSetEventFlagPtr();
        if (setEventPtr <= 0)
        {
            return;
        }
        Array.Copy(BitConverter.GetBytes(setEventPtr), 0, asm, SetEventFlagFunctionOffset, sizeof(long));
        // Set State
        Array.Copy(BitConverter.GetBytes(state), 0, asm, StateOffset, sizeof(bool));
        // Set Event Id
        Array.Copy(BitConverter.GetBytes(eventId), 0, asm, EventFlagIdOffset, sizeof(uint));

        _gameHandler.ExecuteAsm(asm);
    }

    // Easy to call, has hard coded flag value.
    public void DestroyFogWall()
    {
        _hasLoweredWall = true;
        SetEventFlag(GameData.GAME_STARTED_EVENT_ID, true);
    }

    private void listenToClientEvents()
    {
        _client.AddListener<ServerJoinRoomAccepted>(acceptedIntoRoom);
        _gameHandler.CoordinatesRead += gameHandler_CoordinatesRead;
    }

    private void resetHasLoweredStatus()
    {
        _hasLoweredWall = false;
    }

    private void acceptedIntoRoom(ClientModel? model, ServerJoinRoomAccepted accepted)
    {
        //Joined a new lobby, so we reset the wall status...
        resetHasLoweredStatus();
        //... and check if the current match requires us to lower the fog wall (using the last coordinates read)
        handleMatchStatus(accepted.MatchStatus, _gameHandler.LastCoordinates);
    }

    private void gameHandler_CoordinatesRead(object? sender, MapCoordinateEventArgs e)
    {
        //Whenever coordinates are read, check if we need to lower the fog wall
        handleMatchStatus(_client.Room?.Match?.MatchStatus, e.Coordinates);
    }

    private void handleMatchStatus(MatchStatus? status, MapCoordinates? coordinates)
    {
        //Whenever the match is not running, reset the fog wall flag
        if (!status.HasValue || status.Value != MatchStatus.Running)
        {
            resetHasLoweredStatus();
            return;
        }
        //Else, the match is running, so lower the wall as soon as we read valid coordinates, and don't
        //try to lower it again until joining a new lobby or the match ended/restarted
        if (!_hasLoweredWall && coordinates.HasValue)
        {
            DestroyFogWall();
        }
    }
}