﻿namespace Neto.Shared
{
    public record ServerRegisterAccepted(string Message, Guid ClientGuid);
    public record ServerRegisterDenied(string Message);
    public record ClientRegister(string Message, string Version);
    public record ServerKicked(string Reason);
}