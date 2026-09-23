import { Button } from './Button';

type Props = { message: string; onRetry?: () => void; className?: string };

export function ErrorState({ message, onRetry, className = '' }: Props) {
  return (
    <div role="alert" className={`flex flex-col items-start gap-3 ${className}`.trim()}>
      <p className="text-sm text-red-600">{message}</p>
      {onRetry && (
        <Button variant="secondary" onClick={onRetry}>
          Try again
        </Button>
      )}
    </div>
  );
}
