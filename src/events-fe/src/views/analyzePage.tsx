import { useEffect, useState } from 'react';
import { eventsService } from '../services/eventsService';
import type { AppEvent } from '../types';
import { EventsTable } from '../components/EventsTable/eventsTable';

export const AnalyzePage = () => {
    const [events, setEvents] = useState<AppEvent[]>([]);
    const [loading, setLoading] = useState(false);
    
    const [fromDate, setFromDate] = useState('');
    const [toDate, setToDate] = useState('');

    const loadData = async () => {
        try {
            setLoading(true);
            const fromISO = fromDate ? new Date(fromDate).toISOString() : undefined;
            const toISO = toDate ? new Date(toDate).toISOString() : undefined;

            const data = await eventsService.getAll(fromISO, toISO);
            setEvents(data);
        } catch (err) {
            console.error(err);
            alert('Error fetching data');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, []);

    return (
        <div style={{ maxWidth: '800px', margin: '2rem auto' }}>
            <header style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
                <h1>📈 Analyze Page</h1>
            </header>

            <div style={{ 
                background: '#f1f1f1', padding: '1rem', borderRadius: '8px', marginBottom: '1rem',
                display: 'flex', gap: '1rem', alignItems: 'end'
            }}>
                <div>
                    <label style={{ display: 'block', marginBottom: '5px', fontSize: '0.9rem' }}>From:</label>
                    <input 
                        type="datetime-local" 
                        value={fromDate}
                        onChange={(e) => setFromDate(e.target.value)}
                        style={{ padding: '5px' }}
                    />
                </div>
                <div>
                    <label style={{ display: 'block', marginBottom: '5px', fontSize: '0.9rem' }}>To:</label>
                    <input 
                        type="datetime-local" 
                        value={toDate}
                        onChange={(e) => setToDate(e.target.value)}
                        style={{ padding: '5px' }}
                    />
                </div>
                <button 
                    onClick={loadData}
                    style={{ padding: '8px 16px', background: '#28a745', color: 'white', border: 'none', borderRadius: '4px', cursor: 'pointer' }}
                >
                    Filter & Refresh
                </button>
            </div>
            
            {loading ? <p>Loading...</p> : <EventsTable events={events} />}
        </div>
    );
};