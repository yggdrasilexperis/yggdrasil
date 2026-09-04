import { createContext } from 'react';

import type { User } from '../api/types';

export type AuthValue = {
  user: User | null;
  /** Throws ApiError on a failed sign-in; callers render the message. */
  signIn: (email: string, password: string) => Promise<void>;
  /** Registers and signs in — the backend returns a token on 201. Throws ApiError. */
  signUp: (email: string, userName: string, password: string) => Promise<void>;
  signOut: () => void;
};

export const AuthContext = createContext<AuthValue | null>(null);
