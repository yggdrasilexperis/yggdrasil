import type { AuthResponse, User } from './types';

/**
 * The stored session. Persisted so a refresh does not sign the user out;
 * localStorage can throw (private mode, blocked site data) so every access is guarded.
 */
export type Session = {
  token: string;
  expiresAt: string;
  user: User;
};

const STORAGE_KEY = 'yggdrasil.session';

export function getSession(): Session | null {
  let raw: string | null;
  try {
    raw = localStorage.getItem(STORAGE_KEY);
  } catch {
    return null;
  }
  if (!raw) return null;

  let session: Session;
  try {
    session = JSON.parse(raw) as Session;
  } catch {
    clearSession();
    return null;
  }

  if (!session.token || Date.parse(session.expiresAt) <= Date.now()) {
    clearSession();
    return null;
  }

  return session;
}

export function setSession(response: AuthResponse): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
  } catch {
    // Not persisting is survivable — the session still lives in React state.
  }
}

export function clearSession(): void {
  try {
    localStorage.removeItem(STORAGE_KEY);
  } catch {
    // Nothing to do.
  }
}
