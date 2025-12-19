namespace EventsApi;

public record class EventDto
(
    string Type,        // סוג האירוע (למשל: "click", "navigate")
    string Payload,     // מידע נוסף (למשל: "Button A clicked")
    DateTime Timestamp  // מתי זה קרה
);
