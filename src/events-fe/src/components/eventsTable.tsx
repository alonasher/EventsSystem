// src/components/EventsTable.tsx
import { format } from 'date-fns';
import type { AppEvent } from '../types';

interface EventsTableProps {
    events: AppEvent[];
}

export const EventsTable = ({ events }: EventsTableProps) => {
    if (events.length === 0) {
        return <p style={{ textAlign: 'center', color: '#666' }}>No events found.</p>;
    }

    return (
        <div className="table-container">
            <table style={{ width: '100%', borderCollapse: 'collapse', marginTop: '1rem' }}>
                <thead>
                    <tr style={{ background: '#f4f4f4', textAlign: 'left' }}>
                        <th style={{ padding: '10px' }}>Time</th>
                        <th style={{ padding: '10px' }}>Type</th>
                        <th style={{ padding: '10px' }}>Payload</th>
                    </tr>
                </thead>
                <tbody>
                    {events.map((evt, index) => (
                        <tr key={index} style={{ borderBottom: '1px solid #ddd' }}>
                            <td style={{ padding: '10px' }}>
                                {format(new Date(evt.timestamp), 'dd/MM/yyyy HH:mm:ss')}
                            </td>
                            <td style={{ padding: '10px', fontWeight: 'bold' }}>{evt.type}</td>
                            <td style={{ padding: '10px' }}>{evt.payload}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};