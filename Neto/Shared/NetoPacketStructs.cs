namespace Neto.Shared
{
    public record ServerRegisterAccepted(string Message, Guid ClientGuid);
    public record ClientRegister(string Message);
}