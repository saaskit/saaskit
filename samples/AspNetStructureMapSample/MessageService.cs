using System;

namespace AspNetStructureMapSample
{
    public interface IMessageService
    {
        Guid Id { get; }
        string Format(string message);
    }

    public class MessageService : IMessageService
    {
        public MessageService()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public string Format(string message)
        {
            return $"{Id}: {message}";
        }
    }
}
