type Props = { label?: string; className?: string };

export function Loading({ label = 'Loading…', className = '' }: Props) {
  return (
    <div role="status" className={`flex items-center gap-2 text-muted ${className}`.trim()}>
      <span className="h-8 w-8 animate-spin rounded-full border-4 border-hairline border-t-accent" />
      <span>{label}</span>
    </div>
  );
}
