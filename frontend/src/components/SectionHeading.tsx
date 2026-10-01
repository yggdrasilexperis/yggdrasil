import type { ReactNode } from 'react';

/** A section title with an etched rule running out to the edge, like a property-sheet divider. */
export function SectionHeading({ children }: { children: ReactNode }) {
  return (
    <h2 className="flex items-center gap-3 text-lg after:h-0 after:min-w-8 after:flex-1 after:border-t after:border-b after:border-t-dim after:border-b-highlight">
      {children}
    </h2>
  );
}
