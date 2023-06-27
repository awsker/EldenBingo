namespace Neto.Shared
{
    public class TypeContainer<CM, T> : TypeContainer<CM> where CM : ClientModel
    {
        public TypeContainer()
        {
            Type = typeof(T);
        }

        public event Action<CM?, T>? OnDispatch;

        public override Type Type { get; }

        public void Dispatch(CM? sender, T obj)
        {
            OnDispatch?.Invoke(sender, obj);
        }

        public override void Dispatch(CM? sender, object obj)
        {
            if (obj is T objT)
                OnDispatch?.Invoke(sender, objT);
        }
    }

    public abstract class TypeContainer<CM> where CM : ClientModel
    {
        public abstract Type Type { get; }

        public abstract void Dispatch(CM? sender, object obj);
    }
}