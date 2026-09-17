import type { ReactNode } from 'react';

type Props = {
  title: string;
  subtitle: string;
  children: ReactNode;
  /** The alternate-action line under the card (e.g. a link to the other form). */
  footer: ReactNode;
};

/** Shared chrome for the sign-in and sign-up forms: a centred card on parchment. */
export function AuthShell({ title, subtitle, children, footer }: Props) {
  return (
    <div className="flex flex-1 flex-col items-center justify-center bg-parchment px-6 py-12">
      <div className="w-full max-w-sm">
        <div className="rounded-card border border-hairline bg-white p-6">
          <h1 className="font-display text-2xl font-semibold tracking-tight">{title}</h1>
          <p className="mt-2 text-sm text-muted">{subtitle}</p>
          <div className="mt-6">{children}</div>
        </div>
        <p className="mt-6 text-center text-sm text-muted">{footer}</p>
      </div>
    </div>
  );
}
