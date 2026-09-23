import { useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

import { listQuizzes } from '../api/quiz';
import { getCategories } from '../api/quizzes';
import type { Category, QuizSummary } from '../api/types';
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
      .catch(() => {
        if (!cancelled) setError('Could not load quizzes. Try again.');
      });
    return () => {
      cancelled = true;
    };
  }, [selected]);

  function handleFilterChange(slugs: string[]) {
    const params = new URLSearchParams();
    slugs.forEach((slug) => params.append('category', slug));
    setSearchParams(params);
  }

  return (
    <div className="mx-auto flex max-w-5xl flex-1 flex-col gap-6 px-6 py-12">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Discover</h1>

      {categories.length > 0 && (
        <CategoryFilter categories={categories} selected={selected} onChange={handleFilterChange} />
      )}

      {error && (
        <p role="alert" className="text-sm text-red-600">
          {error}
        </p>
      )}
      {!error && quizzes === null && <p className="text-muted">Loading quizzes…</p>}
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
