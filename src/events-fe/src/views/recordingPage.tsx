import { useEffect, useRef, useState, type ChangeEvent } from 'react';

export const RecordingPage = () => {
    const workerRef = useRef<Worker | null>(null);
    const [lastStatus, setLastStatus] = useState<string>('Ready to record');

    useEffect(() => {
        //worker creation
        workerRef.current = new Worker(new URL('../workers/eventWorker.ts', import.meta.url), {
            type: 'module'
        });

        workerRef.current.onmessage = (e) => {
            if (e.data.status === 'success') {
                setLastStatus(`✅ Sent: ${e.data.type}`);
                setTimeout(() => setLastStatus('Ready to record'), 2000);
            } else if (e.data.status === 'error') {
                setLastStatus(`❌ Error: ${e.data.error}`);
            }
        };

        return () => {
            workerRef.current?.terminate();
        };
    }, []);

    const trackEvent = (type: string, payload: string) => {
        if (workerRef.current) {
            workerRef.current.postMessage({ type, payload });
        }
    };

    const inputDebounceRef = useRef<number | undefined>(undefined);

    useEffect(() => {
        return () => {
            if (inputDebounceRef.current) {
                clearTimeout(inputDebounceRef.current);
            }
        };
    }, []);

    return (
        <div style={{ maxWidth: '600px', margin: '2rem auto', padding: '1rem' }}>
            <h1>🔴 Recording Page</h1>
            <p>Events are processed in a background thread (Web Worker).</p>

            <div style={{ background: '#fff', padding: '2rem', borderRadius: '8px', display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
                
                {/* Button */}
                <div>
                    <button 
                        onClick={() => trackEvent('button_click', 'Main button clicked')}
                        style={{ padding: '10px 20px', background: '#007bff', color: 'white', border: 'none', borderRadius: '4px' }}
                    >
                        Click Me
                    </button>
                </div>

                {/* Input */}
                <div>
                    <input
                        type="text"
                        placeholder="Type to send (debounced)..."
                        onChange={(e: ChangeEvent<HTMLInputElement>) => {
                            const val = e.target.value;
                            if (inputDebounceRef.current) clearTimeout(inputDebounceRef.current);
                            inputDebounceRef.current = window.setTimeout(() => {
                                trackEvent('input_change', `Value: ${val}`);
                            }, 300);
                        }}
                        style={{ padding: '8px', width: '100%', border: '1px solid #ccc' }}
                    />
                </div>

                {/* Checkbox */}
                <div>
                    <label style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                        <input 
                            type="checkbox" 
                            onChange={(e) => trackEvent('checkbox_toggle', `Checked: ${e.target.checked}`)}
                        />
                        Toggle Option
                    </label>
                </div>

                <div style={{ marginTop: '1rem', padding: '10px', background: '#f8f9fa', borderRadius: '4px' }}>
                    <strong>Status:</strong> {lastStatus}
                </div>
            </div>
        </div>
    );
};