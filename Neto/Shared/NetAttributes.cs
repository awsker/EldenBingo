namespace Neto.Shared
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class NetworkSerializableAttribute : System.Attribute
    {
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class SkipSerializationAttribute : System.Attribute
    {
    }
}