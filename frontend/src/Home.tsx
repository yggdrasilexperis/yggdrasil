import { useAuth } from './auth/useAuth';
import { Button } from './components/Button';

/** Protected landing. Placeholder until the quiz screens exist — proves the session works. */
export function Home() {
  const { user, signOut } = useAuth();

  return (
    <main className="mx-auto flex min-h-svh max-w-3xl flex-col justify-center gap-4 px-6 py-12">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Yggdrasil</h1>
      <p className="text-muted">
        Signed in as <span className="text-ink">{user?.userName}</span>. Quiz tools are on the way.
      </p>
      <div>
        <Button variant="utility" onClick={signOut}>
          Sign out
        </Button>
      </div>
    </main>
  );
}
