namespace EldenBingoCommon
{
    #region Server to client

    public record ServerRoomNameSuggestion(string RoomName);
    public record ServerCreateRoomDenied(string Reason);
    public record ServerJoinRoomAccepted(string RoomName, UserInRoom[] Users, MatchStatus MatchStatus, bool Paused, int Timer);
    public record ServerJoinRoomDenied(string Reason);
    public record ServerUserJoinedRoom(UserInRoom User);
    public record ServerUserLeftRoom(UserInRoom User);
    public record ServerUserCoordinates(Guid UserGuid, float X, float Y, float Angle, bool IsUnderground);
    public record ServerAdminStatusMessage(string Message, int Color);
    public record ServerUserChat(Guid UserGuid, string Message);
    public record ServerMatchStatusUpdate(MatchStatus MatchStatus, bool Paused, int Timer);
    public record ServerEntireBingoBoardUpdate(int Size, BingoBoardSquare[] Squares, EldenRingClasses[] AvailableClasses);
    public record ServerScoreboardUpdate(TeamScore[] Scoreboard);
    public record ServerBingoAchievedUpdate(BingoLine Bingo);
    public record ServerSquareUpdate(BingoBoardSquare Square, int Index);
    public record ServerUserChecked(Guid UserGuid, int Index, int? TeamChecked);
    public record ServerCurrentGameSettings(BingoGameSettings GameSettings);
    public record ServerTeamNameChanged(Guid UserGuid, int Team, string TeamColorName, string Name);

    #endregion Server to client

    #region Client to server

    public record ClientRequestRoomName();
    public record ClientRequestCreateRoom(string RoomName, string AdminPass, string Nick, int Team, BingoGameSettings Settings);
    public record ClientRequestJoinRoom(string RoomName, string AdminPass, string Nick, int Team);
    public record ClientRequestLeaveRoom();
    public record ClientCoordinates(float X, float Y, float Angle, bool IsUnderground);
    public record ClientChat(string Message);
    public record ClientBingoJson(string Json);
    public record ClientRandomizeBoard();
    public record ClientChangeMatchStatus(MatchStatus MatchStatus);
    public record ClientTogglePause();
    public record ClientTryCheck(int Index, Guid ForUser);
    public record ClientTryMark(int Index);
    public record ClientTrySetCounter(int Index, int Change, Guid ForUser);
    public record ClientSetGameSettings(BingoGameSettings GameSettings);
    public record ClientRequestCurrentGameSettings();
    public record ClientSetTeamName(int Team, string Name);

    #endregion Client to server
}