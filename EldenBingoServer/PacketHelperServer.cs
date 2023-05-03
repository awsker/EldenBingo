using EldenBingoCommon;

namespace EldenBingoServer
{
    internal static class PacketHelperServer
    {
        public static Packet CreateBoardCheckStatusPacket(Guid guid, int indexChecked, UserInRoom recipient, ServerBingoBoard board)
        {
            return createBoardChangedPacket(NetConstants.PacketTypes.ServerBingoBoardCheckChanged, guid, indexChecked, recipient, board);
        }

        public static Packet CreateBoardCountStatusPacket(Guid guid, int indexSet, UserInRoom recipient, ServerBingoBoard board)
        {
            return createBoardChangedPacket(NetConstants.PacketTypes.ServerBingoBoardCountChanged, guid, indexSet, recipient, board);
        }

        public static Packet CreateBoardMarkedStatusPacket(Guid guid, int indexMarked, UserInRoom recipient, ServerBingoBoard board)
        {
            return createBoardChangedPacket(NetConstants.PacketTypes.ServerBingoBoardMarkChanged, guid, indexMarked, recipient, board);
        }

        public static Packet CreateJoinRoomDeniedPacket(string reason)
        {
            var data = PacketHelper.GetStringBytes(reason);
            return new Packet(NetConstants.PacketTypes.ServerJoinRoomDenied, data);
        }

        public static Packet CreateRoomDataPacket(ServerRoom room, UserInRoom user)
        {
            return new Packet(NetConstants.PacketTypes.ServerJoinAcceptedRoomData, room.GetBytes(user));
        }

        public static Packet CreateServerUserChatMessagePacket(Guid userGuid, string text)
        {
            var data = PacketHelper.ConcatBytes(userGuid.ToByteArray(), PacketHelper.GetStringBytes(text));
            return new Packet(NetConstants.PacketTypes.ServerUserChat, data);
        }

        public static Packet CreateServerUserCoordinatesPacket(Guid userGuid, MapCoordinates coordinates)
        {
            var data = PacketHelper.ConcatBytes(userGuid.ToByteArray(), coordinates.GetBytes());
            return new Packet(NetConstants.PacketTypes.ServerUserCoordinates, data);
        }

        public static Packet CreateUserEnteredRoomPacket(UserInRoom user)
        {
            return new Packet(NetConstants.PacketTypes.ServerUserJoinedRoom, user.GetBytes());
        }

        public static Packet CreateUserLeftRoomPacket(Guid userGuid)
        {
            return new Packet(NetConstants.PacketTypes.ServerUserLeftRoom, userGuid.ToByteArray());
        }

        public static Packet CreateUserRegisterAcceptedPacket(Guid clientGuid)
        {
            var data = PacketHelper.ConcatBytes(PacketHelper.GetStringBytes(NetConstants.ServerRegisterString), clientGuid.ToByteArray());
            return new Packet(NetConstants.PacketTypes.ServerRegisterAccepted, data);
        }

        private static Packet createBoardChangedPacket(NetConstants.PacketTypes status, Guid guid, int indexMarked, UserInRoom recipient, ServerBingoBoard board)
        {
            var list = new List<byte[]>();
            board.TransferSquareColors(recipient);
            list.Add(guid.ToByteArray());
            list.Add(new byte[] { (byte)indexMarked });
            list.Add(board.GetStatusBytes(recipient));
            return new Packet(status, PacketHelper.ConcatBytes(list));
        }
    }
}