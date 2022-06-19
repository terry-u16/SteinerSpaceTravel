using System.Runtime.Serialization;

namespace SteinerSpaceTravel.Core.Parsers;

[Serializable]
public class ParseFailedException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public ParseFailedException()
    {
    }

    public ParseFailedException(string message) : base(message)
    {
    }

    public ParseFailedException(string message, Exception inner) : base(message, inner)
    {
    }

    protected ParseFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}