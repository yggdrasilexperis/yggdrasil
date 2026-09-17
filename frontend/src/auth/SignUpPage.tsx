import { useState } from 'react';
import type { FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';

import { ApiError } from '../api/ApiError';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { AuthShell } from './AuthShell';
import { useAuth } from './useAuth';

type FieldErrors = { email?: string; username?: string; password?: string };

// Mirrors RegisterRequestValidator — the server stays authoritative, this only
// saves an obviously-doomed round trip.
const EMAIL = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const USERNAME = /^[a-zA-Z0-9_-]+$/;

function validate(email: string, userName: string, password: string): FieldErrors {
  const errors: FieldErrors = {};

  if (!email.trim()) errors.email = 'Email is required';
  else if (!EMAIL.test(email.trim())) errors.email = 'Enter a valid email address';

  if (!userName.trim()) errors.username = 'Username is required';
  else if (userName.length < 3 || userName.length > 32)
    errors.username = 'Username must be 3–32 characters';
  else if (!USERNAME.test(userName))
    errors.username = 'Letters, numbers, underscores and dashes only';

  if (!password) errors.password = 'Password is required';
  else if (password.length < 8 || password.length > 32)
    errors.password = 'Password must be 8–32 characters';

  return errors;
}

export function SignUpPage() {
  const { signUp } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [userName, setUserName] = useState('');
  const [password, setPassword] = useState('');
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setFormError('');

    const clientErrors = validate(email, userName, password);
    setFieldErrors(clientErrors);
    if (Object.keys(clientErrors).length > 0) return;

    setSubmitting(true);
    try {
      await signUp(email.trim(), userName.trim(), password);
      navigate('/', { replace: true });
    } catch (error) {
      if (error instanceof ApiError && error.status === 400) {
        setFieldErrors({
          email: error.fieldError('email'),
          username: error.fieldError('userName'),
          password: error.fieldError('password'),
        });
      } else if (error instanceof ApiError && error.status === 409) {
        // 409 carries no field key — the detail text names email or username.
        setFormError(error.detail || 'That email or username is already registered.');
      } else if (error instanceof ApiError) {
        setFormError(error.detail || error.title);
      } else {
        setFormError('Something went wrong. Try again.');
      }
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <AuthShell
      title="Create your account"
      subtitle="Start building and sharing quizzes."
      footer={
        <>
          Already have an account?{' '}
          <Link to="/login" className="text-accent">
            Sign in
          </Link>
        </>
      }
    >
      <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-4">
        {formError && (
          <p role="alert" className="text-sm text-red-600">
            {formError}
          </p>
        )}
        <Input
          label="Email"
          type="email"
          autoComplete="email"
          value={email}
          onChange={(event) => setEmail(event.target.value)}
          error={fieldErrors.email}
        />
        <Input
          label="Username"
          autoComplete="username"
          value={userName}
          onChange={(event) => setUserName(event.target.value)}
          error={fieldErrors.username}
        />
        <Input
          label="Password"
          type="password"
          autoComplete="new-password"
          value={password}
          onChange={(event) => setPassword(event.target.value)}
          error={fieldErrors.password}
        />
        <Button type="submit" className="mt-2 w-full" disabled={submitting}>
          {submitting ? 'Creating account…' : 'Create account'}
        </Button>
      </form>
    </AuthShell>
  );
}
