import { useAuth } from './auth/useAuth';

/** Protected landing. Placeholder until the quiz screens exist — proves the session works. */
export function Home() {
  const { user } = useAuth();

  return (
    <div className="mx-auto flex flex-1 max-w-3xl flex-col justify-center gap-4 px-6 py-12">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Welcome back</h1>
      <p className="text-muted">
        Signed in as <span className="text-ink">{user?.userName}</span>. Quiz tools are on the way.
      </p>
    </div>
  );
}
