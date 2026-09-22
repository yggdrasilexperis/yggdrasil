import { useEffect, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';

import { listQuizzes } from '../api/quiz';
import { getCategories } from '../api/quizzes';
import type { Category, QuizSummary } from '../api/types';
import { QuizCard } from './QuizCard';

const chip = (active: boolean) =>
  `flex h-10 items-center rounded-control border px-4 transition-transform active:scale-95 ${
    active ? 'border-accent text-accent' : 'border-hairline'
  }`;

export function DiscoverPage() {
  /** The filter lives in the URL (`/?category=music`) so it survives a refresh and can be linked to. */
  const [searchParams] = useSearchParams();
  const categorySlug = searchParams.get('category') ?? '';

  const [categories, setCategories] = useState<Category[]>([]);
  const [quizzes, setQuizzes] = useState<QuizSummary[] | null>(null);
  const [error, setError] = useState('');

  useEffect(() => {
    // Without the filter row the page still works, so a failure here stays silent for now.
    getCategories()
      .then(setCategories)
      .catch(() => {});
  }, []);

  useEffect(() => {
    let cancelled = false;
    listQuizzes(categorySlug)
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
  }, [categorySlug]);

  return (
    <div className="mx-auto flex max-w-5xl flex-1 flex-col gap-6 px-6 py-12">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Discover</h1>

      {categories.length > 0 && (
        <nav aria-label="Filter by category" className="flex flex-wrap gap-2">
          <Link to="/" aria-current={!categorySlug || undefined} className={chip(!categorySlug)}>
            All
          </Link>
          {categories.map((category) => {
            const active = category.slug === categorySlug;
            return (
              <Link
                key={category.categoryId}
                to={`/?category=${category.slug}`}
                aria-current={active || undefined}
                className={chip(active)}
              >
                {category.name}
              </Link>
            );
          })}
        </nav>
      )}

      {error && (
        <p role="alert" className="text-sm text-red-600">
          {error}
        </p>
      )}
      {!error && quizzes === null && <p className="text-muted">Loading quizzes…</p>}
      {!error && quizzes?.length === 0 && (
        <p className="text-muted">
          {categorySlug
            ? 'No quizzes in this category yet.'
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
