public class SocketMessage<T>
{
    public string Type { get; set; } = string.Empty;
    public T Payload { get; set; } = default!;
}