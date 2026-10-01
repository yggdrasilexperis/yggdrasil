import type { SelectHTMLAttributes } from 'react';

import { PixelIcon } from './PixelIcon';

type Props = SelectHTMLAttributes<HTMLSelectElement>;

/**
 * A native select dressed as a combo box: sunken white field, raised arrow button.
 * `className` sizes and places the whole control; everything else goes to the select.
 */
export function Select({ className = '', children, ...props }: Props) {
  return (
    <div className={`relative ${className}`.trim()}>
      <select
        className="h-9 w-full appearance-none bg-white pr-9 pl-2 shadow-field disabled:bg-face disabled:text-dim aria-invalid:ring-2 aria-invalid:ring-danger"
        {...props}
      >
        {children}
      </select>
      <span
        aria-hidden="true"
        className="pointer-events-none absolute inset-y-0.5 right-0.5 flex w-6 items-center justify-center bg-face shadow-raised"
      >
        <PixelIcon name="arrowDown" />
      </span>
    </div>
  );
}
