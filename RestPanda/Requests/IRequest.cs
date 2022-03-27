namespace RestPanda.Requests;

/// <summary>
/// Interface for all requests
/// </summary>
internal interface IRequest
{
    public string Path { get; }
}