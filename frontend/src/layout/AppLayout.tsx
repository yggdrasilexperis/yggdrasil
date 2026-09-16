import { Link, Outlet } from 'react-router-dom';

import { useAuth } from '../auth/useAuth';
import { Button } from '../components/Button';

/** Header + nav shared by every route; the routed page renders into the Outlet. */
export function AppLayout() {
  const { user, signOut } = useAuth();

  return (
    <div className="flex min-h-svh flex-col">
      <header className="border-b border-hairline">
        <div className="mx-auto flex h-16 max-w-5xl items-center justify-between px-6">
          <Link to="/" className="font-display text-xl font-semibold tracking-tight">
            Yggdrasil
          </Link>
          <nav className="flex items-center gap-4">
            {user ? (
              <>
                <span className="text-sm text-muted">{user.userName}</span>
                <Button variant="utility" onClick={signOut}>
                  Sign out
                </Button>
              </>
            ) : (
              <>
                <Link to="/login" className="text-sm text-ink">
                  Sign in
                </Link>
                <Link to="/register" className="text-sm text-accent">
                  Create account
                </Link>
              </>
            )}
          </nav>
        </div>
      </header>
      <main className="flex flex-1 flex-col">
        <Outlet />
      </main>
    </div>
  );
}
