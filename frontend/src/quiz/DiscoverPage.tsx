import { useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

import { ApiError } from '../api/ApiError';
import { listQuizzes } from '../api/quiz';
import { getCategories } from '../api/quizzes';
import type { Category, QuizSummary } from '../api/types';
import { ErrorState } from '../components/ErrorState';
import { Loading } from '../components/Loading';
import { CategoryFilter } from './CategoryFilter';
import { QuizCard } from './QuizCard';

export function DiscoverPage() {
  /** The filter lives in the URL (`/?category=music&category=games`) so it can be linked. */
  const [searchParams, setSearchParams] = useSearchParams();
  const query = searchParams.toString();
  const selected = useMemo(() => new URLSearchParams(query).getAll('category'), [query]);

  const [categories, setCategories] = useState<Category[]>([]);
  const [quizzes, setQuizzes] = useState<QuizSummary[] | null>(null);
  const [error, setError] = useState('');
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    // Without the filter the page still works, so a failure here stays silent.
    getCategories()
      .then(setCategories)
      .catch(() => {});
  }, []);

  useEffect(() => {
    let cancelled = false;
    listQuizzes(selected)
      .then((data) => {
        if (cancelled) return;
        setQuizzes(data.items);
        setError('');
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
  }, [selected, reloadKey]);

  function handleFilterChange(slugs: string[]) {
    const params = new URLSearchParams();
    slugs.forEach((slug) => params.append('category', slug));
    setSearchParams(params);
  }

  function retry() {
    setError('');
    setQuizzes(null);
    setReloadKey((k) => k + 1);
  }

  return (
    <div className="mx-auto flex w-full max-w-5xl flex-1 flex-col gap-6 px-6 py-12">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Discover</h1>

      {categories.length > 0 && (
        <CategoryFilter categories={categories} selected={selected} onChange={handleFilterChange} />
      )}

      {error && <ErrorState message={error} onRetry={retry} />}
      {!error && quizzes === null && <Loading label="Loading quizzes…" />}
      {!error && quizzes?.length === 0 && (
        <p className="text-muted">
          {selected.length > 0
            ? 'No quizzes match every category you picked.'
            : 'No quizzes yet. Be the first to create one.'}
        </p>
      )}

      {!error && quizzes && quizzes.length > 0 && (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 md:grid-cols-3">
          {quizzes.map((quiz) => (
            <QuizCard key={quiz.id} quiz={quiz} />
          ))}
        </div>
      )}
    </div>
  );
}
