/** Mirrors backend/Yggdrasil.Application/Contracts. Confirm against the OpenAPI doc. */

export type User = {
  id: string;
  email: string;
  userName: string;
};

export type AuthResponse = {
  token: string;
  expiresAt: string;
  user: User;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = {
  email: string;
  userName: string;
  password: string;
};

/** RFC 7807. The backend returns this shape for every error. */
export type ProblemDetails = {
  status: number;
  title: string;
  detail: string;
  instance: string;
  errors?: Record<string, string[]>;
};
