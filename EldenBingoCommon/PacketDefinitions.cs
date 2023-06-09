﻿using System.Drawing;

namespace EldenBingoCommon
{
    #region Server to client
    public record ServerRoomNameSuggestion(string RoomName);
    public record ServerCreateRoomDenied(string Reason);
    public record ServerJoinRoomAccepted(string RoomName, UserInRoom[] Users, MatchStatus MatchStatus, int Timer);
    public record ServerJoinRoomDenied(string Reason);
    public record ServerUserJoinedRoom(UserInRoom User);
    public record ServerUserLeftRoom(UserInRoom User);
    public record ServerUserCoordinates(Guid UserGuid, float X, float Y, float Angle, bool IsUnderground);
    public record ServerAdminStatusMessage(string Message, int Color);
    public record ServerUserChat(Guid UserGuid, string Message);
    public record ServerMatchStatusUpdate(MatchStatus MatchStatus, int Timer);
    public record ServerEntireBingoBoardUpdate(BingoBoardSquare[] Squares);
    public record ServerAvailableClasses(EldenRingClasses[] Classes);
    public record ServerUserChecked(Guid UserGuid, int Index, int? TeamChecked);
    public record ServerUserMarked(Guid UserGuid, int Index, bool Marked);
    public record ServerUserSetCounter(Guid UserGuid, int Index, TeamCounter[] Counters);
    public record ServerCurrentGameSettings(BingoGameSettings GameSettings);
    #endregion

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
    public record ClientTryCheck(int Index, Guid ForUser);
    public record ClientTryMark(int Index);
    public record ClientTrySetCounter(int Index, int Change, Guid ForUser);
    public record ClientSetGameSettings(BingoGameSettings GameSettings);
    public record ClientRequestCurrentGameSettings();
    #endregion
}
