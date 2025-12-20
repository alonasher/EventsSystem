// src/components/EventsTable.tsx
import { format } from "date-fns";
import type { AppEvent } from "../../types";
import "./eventsTable.css";

interface EventsTableProps {
  events: AppEvent[];
}

export const EventsTable = ({ events }: EventsTableProps) => {
  if (events.length === 0) {
    return <p className="events-empty">No events found.</p>;
  }

  return (
    <div className="events-table-container">
      <table className="events-table">
        <thead>
          <tr>
            <th>Time</th>
            <th>Type</th>
            <th>Payload</th>
          </tr>
        </thead>
        <tbody>
          {events.map((evt, index) => (
            <tr key={index}>
              <td>{format(new Date(evt.timestamp), "dd/MM/yyyy HH:mm:ss")}</td>
              <td className="events-type">{evt.type}</td>
              <td>{evt.payload}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};
