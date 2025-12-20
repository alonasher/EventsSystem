export function formatEventType(type?: string | null): string {
  if (!type) return "";
  return String(type)
    .split(/[_\s-]+/)
    .map((w) => (w.length ? w[0].toUpperCase() + w.slice(1).toLowerCase() : ""))
    .join(" ");
}
