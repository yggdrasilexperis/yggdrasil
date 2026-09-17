import { ApiError } from './ApiError';
import { getSession } from './session';
import type { ProblemDetails } from './types';

const baseUrl = import.meta.env.VITE_API_BASE_URL;

if (!baseUrl) {
  throw new Error('VITE_API_BASE_URL is not set. Copy it into frontend/.env.');
}

type RequestOptions = {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE';
  body?: unknown;
  /** Attach the bearer token. Off for the auth endpoints, which are anonymous. */
  authenticated?: boolean;
};

/**
 * Called when an authenticated request comes back 401 — which means the stored token is
 * stale or revoked. AuthProvider registers its `signOut` here so a plain module can react
 * to that without importing React state directly.
 */
let onSessionExpired: (() => void) | null = null;

export function setSessionExpiredHandler(handler: (() => void) | null): void {
  onSessionExpired = handler;
}

/**
 * The only place the app talks to the network. Owns the base URL, the bearer
 * token, JSON handling, and turning ProblemDetails into an ApiError.
 */
export async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { method = 'GET', body, authenticated = true } = options;

  const headers: Record<string, string> = { Accept: 'application/json' };
  if (body !== undefined) headers['Content-Type'] = 'application/json';

  if (authenticated) {
    const session = getSession();
    if (session) headers.Authorization = `Bearer ${session.token}`;
  }

  let response: Response;
  try {
    response = await fetch(`${baseUrl}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch {
    // fetch only rejects on network/CORS failure, never on a 4xx/5xx.
    throw new ApiError(
      0,
      'Could not reach the server',
      'The API did not respond. Check that it is running.',
    );
  }

  if (!response.ok) {
    if (response.status === 401 && authenticated) onSessionExpired?.();
    throw await toApiError(response);
  }

  if (response.status === 204 || response.headers.get('Content-Length') === '0') {
    return undefined as T;
  }

  return (await response.json()) as T;
}

async function toApiError(response: Response): Promise<ApiError> {
  let problem: Partial<ProblemDetails> = {};
  try {
    problem = (await response.json()) as Partial<ProblemDetails>;
  } catch {
    // A proxy or an unhandled crash can return a non-JSON body.
  }

  return new ApiError(
    response.status,
    problem.title ?? 'Request failed',
    problem.detail ?? '',
    normalizeFieldErrors(problem.errors),
  );
}

/**
 * FluentValidation keys `errors` by C# property name ("Email"), and ASP.NET does not
 * camel-case dictionary keys. Lower-case them so callers can look fields up by their
 * own naming.
 */
function normalizeFieldErrors(errors: Record<string, string[]> | undefined) {
  if (!errors) return {};

  return Object.fromEntries(
    Object.entries(errors).map(([field, messages]) => [field.toLowerCase(), messages]),
  );
}
