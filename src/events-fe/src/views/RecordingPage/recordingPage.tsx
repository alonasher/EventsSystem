import { useEffect, useRef, type ChangeEvent } from 'react';
import useEventWorker from '../../hooks/useEventWorker';
import './recordingPage.css';

const RecordingPage = () => {
    const { trackEvent, lastStatus } = useEventWorker();

    const inputDebounceRef = useRef<number | undefined>(undefined);

    useEffect(() => {
        return () => {
            if (inputDebounceRef.current) {
                clearTimeout(inputDebounceRef.current);
            }
        };
    }, []);

    const handleInputChange = (e: ChangeEvent<HTMLInputElement>) => {
        const val = e.target.value;
        if (inputDebounceRef.current) {
            clearTimeout(inputDebounceRef.current);
        }
        inputDebounceRef.current = window.setTimeout(() => {
            trackEvent('input_change', `Value: ${val}`);
        }, 300);
    };

    return (
        <div className="recording-page">
            <h1>Recording Page</h1>
            <p>Events are processed in a background thread (Web Worker).</p>

            <div className="recording-container">
                
                <div>
                    <button 
                        onClick={() => trackEvent('button_click', 'Main button clicked')}
                        className="recording-button"
                    >
                        Click Me
                    </button>
                </div>

                <div>
                    <input
                        type="text"
                        placeholder="Type to send (debounced)..."
                        onChange={handleInputChange}
                        className="recording-input"
                    />
                </div>

                <div>
                    <label className="recording-checkbox-label">
                        <input 
                            type="checkbox" 
                            onChange={(e) => trackEvent('checkbox_toggle', `Checked: ${e.target.checked}`)}
                        />
                        Toggle Option
                    </label>
                </div>

                <div className="recording-status">
                    <strong>Status:</strong> {lastStatus}
                </div>
            </div>
        </div>
    );
};

export default RecordingPage;