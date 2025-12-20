// src/components/EventsTable.tsx
import  { useMemo, useState } from "react";
import { format } from "date-fns";
import type { AppEvent } from "../../types";
import { sortEvents, sortOptions } from "../../utils/eventsSort";
import { formatEventType } from "../../utils/formatters";
import "./eventsTable.css";

interface EventsTableProps {
  events: AppEvent[];
}

const EventsTable = (props: EventsTableProps) => {
  const { events } = props;
  const [sortOption, setSortOption] = useState<
    "datetime-asc" | "datetime-desc" | "type-asc" | "type-desc"
  >("datetime-desc");

  const sortedEvents = useMemo(() => sortEvents(events, sortOption), [events, sortOption]);

  

  if (events.length === 0) {
    return <p className="events-empty">No events found.</p>;
  }

  return (
    <div className="events-table-container">
      <div className="events-table-controls">
        <label htmlFor="sort-select">Sort:</label>
        <select
          id="sort-select"
          value={sortOption}
          onChange={(e) => setSortOption(e.target.value as any)}
        >
          {sortOptions.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
      </div>

      <table className="events-table">
        <thead>
          <tr>
            <th>Time</th>
            <th>Type</th>
            <th>Payload</th>
          </tr>
        </thead>
        <tbody>
          {sortedEvents.map((evt, index) => (
            <tr key={index}>
              <td>{format(new Date(evt.timestamp), "dd/MM/yyyy HH:mm:ss")}</td>
              <td className="events-type">{formatEventType(evt.type)}</td>
              <td>{evt.payload}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default EventsTable;