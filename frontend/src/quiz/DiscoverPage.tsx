import { useEffect, useState } from 'react';

import { listQuizzes } from '../api/quiz';
import type { QuizSummary } from '../api/types';
import { QuizCard } from './QuizCard';
import { Loading } from '../components/Loading';
import { ApiError } from '../api/ApiError';
import { ErrorState } from '../components/ErrorState';

export function DiscoverPage() {
  const [quizzes, setQuizzes] = useState<QuizSummary[] | null>(null);
  const [error, setError] = useState('');
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    let cancelled = false;
    listQuizzes()
      .then((data) => {
        if (!cancelled) setQuizzes(data.items);
      })
      .catch((err) => {
        if (!cancelled)
          setError(
            err instanceof ApiError
              ? err.detail || err.title
              : 'Could not load quizzes. Try again.',
          );
      });
    return () => {
      cancelled = true;
    };
  }, [reloadKey]);

  function retry() {
    setError('');
    setQuizzes(null);
    setReloadKey((k) => k + 1);
  }

  return (
    <div className="mx-auto flex max-w-5xl flex-1 flex-col gap-6 px-6 py-12">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Discover</h1>

      {error && <ErrorState message={error} onRetry={retry} />}
      {!error && quizzes === null && <Loading label="Loading quizzes…" />}
      {quizzes?.length === 0 && (
        <p className="text-muted">No quizzes yet. Be the first to create one.</p>
      )}

      {quizzes && quizzes.length > 0 && (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 md:grid-cols-3">
          {quizzes.map((quiz) => (
            <QuizCard key={quiz.id} quiz={quiz} />
          ))}
        </div>
      )}
    </div>
  );
}
