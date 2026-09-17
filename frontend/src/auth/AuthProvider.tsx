import { useCallback, useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';

import { login, register } from '../api/auth';
import { setSessionExpiredHandler } from '../api/client';
import { clearSession, getSession, setSession } from '../api/session';
import type { User } from '../api/types';
import { AuthContext } from './AuthContext';

export function AuthProvider({ children }: { children: ReactNode }) {
  // Read once on mount so a refresh keeps the user signed in.
  const [user, setUser] = useState<User | null>(() => getSession()?.user ?? null);

  const signOut = useCallback(() => {
    clearSession();
    setUser(null);
  }, []);

  // A 401 on an authenticated request means the token is stale — drop the session
  // exactly like a sign-out, so RequireAuth bounces to /login on its own.
  useEffect(() => {
    setSessionExpiredHandler(signOut);
    return () => setSessionExpiredHandler(null);
  }, [signOut]);

  const signIn = useCallback(async (email: string, password: string) => {
    const response = await login({ email, password });
    setSession(response);
    setUser(response.user);
  }, []);

  const signUp = useCallback(async (email: string, userName: string, password: string) => {
    const response = await register({ email, userName, password });
    setSession(response);
    setUser(response.user);
  }, []);

  useEffect(() => {
    const session = getSession();
    if (!session) return;
    const id = setTimeout(signOut, Date.parse(session.expiresAt) - Date.now());
    return () => clearTimeout(id);
  }, [user, signOut]);

  const value = useMemo(() => ({ user, signIn, signUp, signOut }), [user, signIn, signUp, signOut]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
