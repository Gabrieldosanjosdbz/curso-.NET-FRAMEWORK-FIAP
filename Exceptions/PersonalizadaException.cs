
public class PersonalizadaExceptionException : Exception
{
    public PersonalizadaExceptionException() { }
    public PersonalizadaExceptionException(string message) : base(message) { }
    public PersonalizadaExceptionException(string message, System.Exception inner) : base(message, inner) { }
    protected PersonalizadaExceptionException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}