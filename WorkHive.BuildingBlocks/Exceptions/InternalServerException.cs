namespace WorkHive.BuildingBlocks.Exceptions;

public class InternalServerException : Exception
{
    public InternalServerException(string message) : base(message) { }

    public InternalServerException(string message, string Details) : base(message)
    {
        this.Details = Details;
    }

    public string? Details { get; }
}
