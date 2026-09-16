import { request } from './client';
import type { AuthResponse, LoginRequest, RegisterRequest } from './types';

export function login(credentials: LoginRequest): Promise<AuthResponse> {
  return request<AuthResponse>('/api/auth/login', {
    method: 'POST',
    body: credentials,
    authenticated: false,
  });
}

export function register(details: RegisterRequest): Promise<AuthResponse> {
  return request<AuthResponse>('/api/auth/register', {
    method: 'POST',
    body: details,
    authenticated: false,
  });
}
