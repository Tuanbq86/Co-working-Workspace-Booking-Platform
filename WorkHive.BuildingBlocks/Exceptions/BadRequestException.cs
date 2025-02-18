namespace WorkHive.BuildingBlocks.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }

    public BadRequestException(string message, string Details) : base(message)
    {
        this.Details = Details;
    }

    public string? Details { get; }
}
