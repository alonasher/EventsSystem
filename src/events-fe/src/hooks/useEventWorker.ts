import { useEffect, useRef, useState } from 'react';

const useEventWorker = () => {
  const workerRef = useRef<Worker | null>(null);
  const [lastStatus, setLastStatus] = useState<string>('Ready to record');

  useEffect(() => {
    workerRef.current = new Worker(
      new URL('../workers/eventWorker.ts', import.meta.url),
      { type: 'module' }
    );

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

  return { trackEvent, lastStatus };
};

export default useEventWorker;