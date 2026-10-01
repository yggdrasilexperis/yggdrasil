import type { ButtonHTMLAttributes } from 'react';

import { buttonClasses, buttonLabel } from './buttonStyles';
import type { ButtonVariant } from './buttonStyles';

type Props = ButtonHTMLAttributes<HTMLButtonElement> & { variant?: ButtonVariant };

export function Button({
  variant = 'primary',
  className = '',
  type = 'button',
  children,
  ...props
}: Props) {
  return (
    <button type={type} className={buttonClasses(variant, className)} {...props}>
      <span className={buttonLabel}>{children}</span>
    </button>
  );
}
