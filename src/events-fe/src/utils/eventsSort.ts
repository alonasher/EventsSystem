import type { AppEvent } from "../types";

export type SortOption =
  | "datetime-asc"
  | "datetime-desc"
  | "type-asc"
  | "type-desc";

export const sortOptions = [
  { value: "datetime-desc", label: "Datetime ↓" },
  { value: "datetime-asc", label: "Datetime ↑" },
  { value: "type-asc", label: "Type A→Z" },
  { value: "type-desc", label: "Type Z→A" },
];

export function sortEvents(events: AppEvent[] = [], sortOption: SortOption) {
  if (!events || events.length === 0) return [] as AppEvent[];
  const copy = [...events];
  switch (sortOption) {
    case "datetime-asc": {
      copy.sort((a, b) => {
        const ta = typeof a.timestamp === "number" ? a.timestamp : new Date(a.timestamp).getTime();
        const tb = typeof b.timestamp === "number" ? b.timestamp : new Date(b.timestamp).getTime();
        return ta - tb;
      });
      break;
    }
    case "datetime-desc": {
      copy.sort((a, b) => {
        const ta = typeof a.timestamp === "number" ? a.timestamp : new Date(a.timestamp).getTime();
        const tb = typeof b.timestamp === "number" ? b.timestamp : new Date(b.timestamp).getTime();
        return tb - ta;
      });
      break;
    }
    case "type-asc":
      copy.sort((a, b) => a.type.localeCompare(b.type));
      break;
    case "type-desc":
      copy.sort((a, b) => b.type.localeCompare(a.type));
      break;
    default:
      break;
  }
  return copy;
}
