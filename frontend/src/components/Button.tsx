import type { ButtonHTMLAttributes } from 'react';

/** DESIGN.md "Components": accent primary/secondary for real actions, ink utility for chrome. */
type Variant = 'primary' | 'secondary' | 'utility';

const base =
  'inline-flex items-center justify-center rounded-control transition-transform active:scale-95 ' +
  'disabled:opacity-50 disabled:active:scale-100';

const variants: Record<Variant, string> = {
  primary: 'min-h-11 bg-accent px-6 py-2 text-white',
  secondary: 'min-h-11 border border-accent px-6 py-2 text-accent',
  utility: 'min-h-11 bg-ink px-4 py-2 text-sm text-white',
};

type Props = ButtonHTMLAttributes<HTMLButtonElement> & { variant?: Variant };

export function Button({ variant = 'primary', className = '', type = 'button', ...props }: Props) {
  return (
    <button type={type} className={`${base} ${variants[variant]} ${className}`.trim()} {...props} />
  );
}
