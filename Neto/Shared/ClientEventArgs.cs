namespace Neto.Shared
{
    public class ClientEventArgs<T> where T : ClientModel
    {
        public ClientEventArgs(T client)
        {
            Client = client;
        }

        public T Client { get; init; }
    }
}