namespace ClickView.Extensions.Events
{
    public struct EventId
    {
        public EventId(string id)
        {
            Id = id;
        }

        public string Id { get; }

        public static implicit operator EventId(string id)
        {
            return new EventId(id);
        }

        public override string ToString()
        {
            return Id;
        }
    }
}