namespace LR1;

public sealed class Error
{
    private Error(Message message, int position)
    {
        Message = message;
        Position = position;
    }

    public Message Message { get; }
    public int Position { get; }

    public static Error Create(Message message, int position)
    {
        return new Error(message, position);
    }

    public void Display()
    {
        Console.WriteLine($"Позиція {Position}: {Message.Value}.");
    }
}