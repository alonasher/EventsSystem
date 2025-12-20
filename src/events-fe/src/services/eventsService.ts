
import axios from 'axios';
import type { AppEvent } from '../types';

const API_URL = '/api/events';

export const eventsService = {
    getAll: async (from?: string, to?: string): Promise<AppEvent[]> => {
        const response = await axios.get<AppEvent[]>(API_URL, { params: { from, to }});
        return response.data;
    },

    create: async (type: string, payload: string): Promise<void> => {
        await axios.post(API_URL, { type, payload, timestamp: new Date().toISOString() });
    }
};