type Props = { label?: string; className?: string };

const BLOCKS = 6;

/** The block progress bar, running as a marquee because we never know how far along we are. */
export function Loading({ label = 'Loading…', className = '' }: Props) {
  return (
    <div role="status" className={`flex cursor-progress flex-col gap-2 ${className}`.trim()}>
      <span>{label}</span>
      <span
        aria-hidden="true"
        className="relative block h-5 w-full max-w-xs overflow-hidden bg-white shadow-field"
      >
        <span className="absolute inset-y-1 left-1 flex gap-0.5 motion-safe:animate-progress">
          {Array.from({ length: BLOCKS }, (_, index) => (
            <span key={index} className="w-2 bg-accent" />
          ))}
        </span>
      </span>
    </div>
  );
}
