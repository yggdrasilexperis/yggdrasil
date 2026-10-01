import { useEffect, useState } from 'react';

const time = new Intl.DateTimeFormat(undefined, { timeStyle: 'short' });

/** The clock in the taskbar tray, in the reader's own locale. */
export function TrayClock({ className = '' }: { className?: string }) {
  const [now, setNow] = useState(() => new Date());

  // Checking every few seconds keeps the minute from turning over late.
  useEffect(() => {
    const id = setInterval(() => setNow(new Date()), 5000);
    return () => clearInterval(id);
  }, []);

  return (
    <time dateTime={now.toISOString()} className={className}>
      {time.format(now)}
    </time>
  );
}
