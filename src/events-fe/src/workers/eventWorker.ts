import { eventsService } from '../services/eventsService';

self.onmessage = async (e: MessageEvent) => {
    const { type, payload } = e.data;

    try {
        await eventsService.create(type, payload);
        self.postMessage({ status: 'success', type });
    } catch (error) {
        const errorMessage = error instanceof Error ? error.message : 'Unknown error';
        self.postMessage({ status: 'error', error: errorMessage });
    }
};