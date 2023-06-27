using System.Drawing;

namespace EldenBingoCommon
{
    #region Server to client
    public record ServerRoomNameSuggestion(string RoomName);
    public record ServerCreateRoomDenied(string Reason);
    public record ServerJoinRoomAccepted(string RoomName, UserInRoom[] Users);
    public record ServerJoinRoomDenied(string Reason);
    public record ServerUserJoinedRoom(UserInRoom User);
    public record ServerUserLeftRoom(UserInRoom User);
    public record ServerUserCoordinates(Guid UserGuid, float X, float Y, float Angle, bool IsUnderground);
    public record ServerAdminStatusMessage(string Message, int Color);
    public record ServerUserChat(Guid UserGuid, string Message);
    public record ServerMatchStatusUpdate(MatchStatus MatchStatus, int Timer);
    public record ServerEntireBingoBoardUpdate(BingoBoardSquare[] Squares);
    public record ServerBingoBoardStatusUpdate(BingoSquareStatus[] BoardStatus);
    public record ServerUserChecked(Guid UserGuid, int Index, int? TeamChecked);
    public record ServerUserMarked(Guid UserGuid, int Index, bool Marked);
    public record ServerUserSetCounter(Guid UserGuid, int Index, TeamCounter[] Counters);

    /*
    public record MatchStatusUpdated(MatchStatus MatchStatus, int ServerTimer);
    public record BingoSquareText(string[] SquareTexts);
    public record BingoTooltipText(string[] Tooltips);
    public record BingoCheckStatus(bool[] Checked);*/
    #endregion

    #region Client to server
    public record ClientRequestRoomName();
    public record ClientRequestCreateRoom(string RoomName, string AdminPass, string Nick, int Team);
    public record ClientRequestJoinRoom(string RoomName, string AdminPass, string Nick, int Team);
    public record ClientRequestLeaveRoom();
    public record ClientCoordinates(float X, float Y, float Angle, bool IsUnderground);
    public record ClientChat(string Message);
    public record ClientBingoJson(string Json, int Seed);
    public record ClientRandomizeBoard(int Seed);
    public record ClientChangeMatchStatus(MatchStatus MatchStatus);
    public record ClientTryCheck(int Index, Guid ForUser);
    public record ClientTryMark(int Index);
    public record ClientTrySetCounter(int Index, int Change, Guid ForUser);
    #endregion
}
