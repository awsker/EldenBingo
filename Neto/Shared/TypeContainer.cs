namespace Neto.Shared
{
    public class TypeContainer<CM, T> : TypeContainer<CM> where CM : ClientModel
    {
        public TypeContainer()
        {
            Type = typeof(T);
        }

        public event Func<CM?, T, Task>? OnDispatch;

        public override Type Type { get; }

        public async Task Dispatch(CM? sender, T obj)
        {
            if (OnDispatch == null)
                return;
            var handlers = OnDispatch.GetInvocationList().Cast<Func<CM?, T, Task>>();

            var tasks = handlers.Select(handler => handler(sender, obj));

            await Task.WhenAll(tasks);
        }

        public override async Task Dispatch(CM? sender, object obj)
        {
            if (obj is not T objT || OnDispatch == null)
                return;
            var handlers = OnDispatch.GetInvocationList().Cast<Func<CM?, T, Task>>();

            var tasks = handlers.Select(handler => handler(sender, objT));
            
            await Task.WhenAll(tasks);
        }
    }

    public abstract class TypeContainer<CM> where CM : ClientModel
    {
        public abstract Type Type { get; }

        public abstract Task Dispatch(CM? sender, object obj);
    }
}