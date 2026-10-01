import type { ReactNode } from 'react';

import { PixelIcon } from '../components/PixelIcon';
import { Window } from '../components/Window';

type Props = {
  title: string;
  subtitle: string;
  children: ReactNode;
  /** The alternate-action line under the form (e.g. a link to the other form). */
  footer: ReactNode;
};

/** Shared chrome for the sign-in and sign-up forms: a log-on dialog centred on the desktop. */
export function AuthShell({ title, subtitle, children, footer }: Props) {
  return (
    <div className="flex flex-1 flex-col items-center justify-center py-6">
      <Window title={title} className="w-full max-w-md">
        <div className="flex items-center gap-4">
          <PixelIcon name="key" scale={2} />
          <p>{subtitle}</p>
        </div>
        <div className="mt-5">{children}</div>
        <p className="mt-6 border-t border-dim pt-3 text-muted shadow-groove">{footer}</p>
      </Window>
    </div>
  );
}
