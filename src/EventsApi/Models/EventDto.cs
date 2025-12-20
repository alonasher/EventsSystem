using System.ComponentModel.DataAnnotations;

namespace EventsApi;

/// <summary>
/// Represents a user event to be published and/or persisted.
/// This is a record type, providing immutability, value-based equality, and built-in ToString/GetHashCode.
/// </summary>
public record class EventDto(
    [Required(ErrorMessage = "Type is required")]
    [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters")]
    string Type,
    
    [Required(ErrorMessage = "Payload is required")]
    [StringLength(500, ErrorMessage = "Payload cannot exceed 500 characters")]
    string Payload,
    
    DateTime Timestamp
);
