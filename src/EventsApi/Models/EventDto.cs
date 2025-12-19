namespace EventsApi;

/// <summary>
/// Represents a user event to be published and/or persisted.
/// This is a record type, providing immutability, value-based equality, and built-in ToString/GetHashCode.
/// </summary>
public record class EventDto(
    string Type,
    string Payload,
    DateTime Timestamp
);
