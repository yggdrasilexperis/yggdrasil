import { Button } from './Button';
import { PixelIcon } from './PixelIcon';

type Props = { message: string; onRetry?: () => void; className?: string };

/** A message box: the icon carries the alarm, so the message itself stays in plain ink. */
export function ErrorState({ message, onRetry, className = '' }: Props) {
  return (
    <div role="alert" className={`flex items-start gap-4 ${className}`.trim()}>
      <PixelIcon name="error" scale={2} />
      <div className="flex flex-col items-start gap-3 pt-1">
        <p>{message}</p>
        {onRetry && (
          <Button variant="secondary" onClick={onRetry}>
            Try again
          </Button>
        )}
      </div>
    </div>
  );
}
