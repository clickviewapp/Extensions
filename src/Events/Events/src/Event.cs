namespace ClickView.Extensions.Events
{
    using System;

    public abstract class Event
    {
        /// <summary>
        /// The unique id for this event
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// The id of the event
        /// </summary>
        public EventId EventId { get; }

        /// <summary>
        /// The time in which the event happened
        /// </summary>
        public DateTimeOffset TimeStamp { get; }

        protected Event(EventId eventId)
        {
            Id = Guid.NewGuid();
            EventId = eventId;
            TimeStamp = DateTimeOffset.UtcNow;
        }
    }
}
