import { Link } from 'react-router-dom';
import type { LinkProps } from 'react-router-dom';

import { buttonClasses, buttonLabel } from './buttonStyles';
import type { ButtonVariant } from './buttonStyles';

type Props = LinkProps & { variant?: ButtonVariant };

/** Navigation that is the next step in a dialog, so it looks like the dialog's button. */
export function ButtonLink({ variant = 'primary', className = '', children, ...props }: Props) {
  return (
    <Link className={buttonClasses(variant, className)} {...props}>
      <span className={buttonLabel}>{children}</span>
    </Link>
  );
}
