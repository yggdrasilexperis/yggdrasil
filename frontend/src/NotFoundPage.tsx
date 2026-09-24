import { Link } from 'react-router-dom';

/** Catch-all route: anything that doesn't match a known path lands here, not a blank screen. */
export function NotFoundPage() {
  return (
    <div className="mx-auto flex w-full flex-1 max-w-3xl flex-col items-center justify-center gap-4 px-6 py-12 text-center">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Page not found</h1>
      <p className="text-muted">
        The page you&apos;re looking for doesn&apos;t exist or has moved.
      </p>
      <Link to="/" className="text-accent">
        Back to Yggdrasil
      </Link>
    </div>
  );
}
