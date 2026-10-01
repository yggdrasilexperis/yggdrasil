import { useId } from 'react';
import type { InputHTMLAttributes } from 'react';

type Props = InputHTMLAttributes<HTMLInputElement> & {
  label: string;
  /** Message shown below the field; also rings the field in red. Never colours the label. */
  error?: string;
};

export function Input({ label, error, className = '', id, ...props }: Props) {
  const fallbackId = useId();
  const inputId = id ?? fallbackId;
  const errorId = `${inputId}-error`;

  return (
    <div className={className}>
      <label htmlFor={inputId} className="mb-1 block">
        {label}
      </label>
      <input
        id={inputId}
        aria-invalid={error ? true : undefined}
        aria-describedby={error ? errorId : undefined}
        className={`h-9 w-full bg-white px-2 shadow-field placeholder:text-dim ${
          error ? 'ring-2 ring-danger' : ''
        }`}
        {...props}
      />
      {error && (
        <p id={errorId} className="mt-1.5 text-danger">
          {error}
        </p>
      )}
    </div>
  );
}
