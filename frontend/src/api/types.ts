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

export type Difficulty = 0 | 1 | 2 | 3;

export type Category = {
  categoryId: string;
  name: string;
  slug: string;
};

export type CreateQuizRequest = {
  title: string;
  description: string;
  difficulty: Difficulty;
  categoryIds: string[];
};

export type Quiz = {
  id: string;
  title: string;
  description: string | null;
  ownerId: string;
  difficulty: Difficulty;
  createdAt: string;
  updatedAt: string;
  categories: Category[];
};
