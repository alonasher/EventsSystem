import { useEffect, useState } from 'react';
import { eventsService } from '../../services/eventsService';
import type { AppEvent } from '../../types';
import EventsTable  from '../../components/EventsTable/eventsTable';
import Filter from '../../components/Filter/filter';
import { formatDateTimeLocal } from '../../utils/dateUtils';
import './analyzePage.css';

export const AnalyzePage = () => {
    const [events, setEvents] = useState<AppEvent[]>([]);
    const [loading, setLoading] = useState(false);
    
    const [fromDate, setFromDate] = useState('');
    const [toDate, setToDate] = useState('');

    useEffect(() => {
        const now = new Date();
        const oneHourAgo = new Date(now.getTime() - 60 * 60 * 1000);
        
        const toDateLocal = formatDateTimeLocal(now);
        const fromDateLocal = formatDateTimeLocal(oneHourAgo);
        
        setToDate(toDateLocal);
        setFromDate(fromDateLocal);
    }, []);



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
        <div className="analyze-page">
            <header className="analyze-header">
                <h1>Analyze Page</h1>
            </header>

            <Filter 
                fromDate={fromDate}
                toDate={toDate}
                onFromDateChange={setFromDate}
                onToDateChange={setToDate}
                onFilter={loadData}
                maxDateTime={formatDateTimeLocal(new Date())}
            />
            
            {loading ? <p>Loading...</p> : <EventsTable events={events} />}
        </div>
    );
};