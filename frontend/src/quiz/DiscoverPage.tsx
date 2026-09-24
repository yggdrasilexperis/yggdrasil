import { useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

import { ApiError } from '../api/ApiError';
import { listQuizzes } from '../api/quiz';
import { getCategories } from '../api/quizzes';
import { QUIZ_SORTS } from '../api/types';
import type { Category, PagedResult, QuizSort, QuizSummary } from '../api/types';
import { ErrorState } from '../components/ErrorState';
import { Loading } from '../components/Loading';
import { CategoryFilter } from './CategoryFilter';
import { Pager } from './Pager';
import { QuizCard } from './QuizCard';

const PAGE_SIZE = 9;
const DEFAULT_SORT: QuizSort = 'newest';

function readSort(value: string | null): QuizSort {
  return value !== null && Object.hasOwn(QUIZ_SORTS, value) ? (value as QuizSort) : DEFAULT_SORT;
}

function readPage(value: string | null): number {
  const page = Number(value);
  return Number.isInteger(page) && page > 0 ? page : 1;
}

function toSearchParams(slugs: string[], sort: QuizSort, page: number): URLSearchParams {
  const params = new URLSearchParams();
  slugs.forEach((slug) => params.append('category', slug));
  if (sort !== DEFAULT_SORT) params.set('sort', sort);
  if (page > 1) params.set('page', String(page));
  return params;
}

export function DiscoverPage() {
  /** The filter lives in the URL (`/?category=music&category=games`) so it can be linked. */
  const [searchParams, setSearchParams] = useSearchParams();
  const query = searchParams.toString();
  const { selected, sort, page } = useMemo(() => {
    const params = new URLSearchParams(query);
    return {
      selected: params.getAll('category'),
      sort: readSort(params.get('sort')),
      page: readPage(params.get('page')),
    };
  }, [query]);

  const [categories, setCategories] = useState<Category[]>([]);
  const [result, setResult] = useState<PagedResult<QuizSummary> | null>(null);
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
    const { sortBy, sortDirection } = QUIZ_SORTS[sort];
    listQuizzes({ page, pageSize: PAGE_SIZE, sortBy, sortDirection, categorySlugs: selected })
      .then((data) => {
        if (cancelled) return;
        if (page > data.totalPages && data.totalPages > 0) {
          setSearchParams(toSearchParams(selected, sort, data.totalPages), { replace: true });
          return;
        }
        setResult(data);
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
  }, [selected, sort, page, reloadKey, setSearchParams]);

  function handleFilterChange(slugs: string[]) {
    setSearchParams(toSearchParams(slugs, sort, 1));
  }

  function handleSortChange(next: QuizSort) {
    setSearchParams(toSearchParams(selected, next, 1));
  }

  function handlePageChange(next: number) {
    setSearchParams(toSearchParams(selected, sort, next));
    window.scrollTo({ top: 0 });
  }

  function retry() {
    setError('');
    setResult(null);
    setReloadKey((k) => k + 1);
  }

  const quizzes = result?.items;

  return (
    <div className="mx-auto flex w-full max-w-5xl flex-1 flex-col gap-6 px-6 py-12">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Discover</h1>

      <div className="flex flex-wrap items-start gap-4">
        {categories.length > 0 && (
          <CategoryFilter
            categories={categories}
            selected={selected}
            onChange={handleFilterChange}
          />
        )}
        <select
          aria-label="Sort quizzes"
          value={sort}
          onChange={(event) => handleSortChange(event.target.value as QuizSort)}
          className="ml-auto h-11 rounded-control border border-hairline bg-white px-4"
        >
          {Object.entries(QUIZ_SORTS).map(([key, option]) => (
            <option key={key} value={key}>
              {option.label}
            </option>
          ))}
        </select>
      </div>

      {error && <ErrorState message={error} onRetry={retry} />}
      {!error && quizzes === null && <Loading label="Loading quizzes…" />}
      {!error && quizzes?.length === 0 && (
        <p className="text-muted">
          {selected.length > 0
            ? 'No quizzes match every category you picked.'
            : 'No quizzes yet. Be the first to create one.'}
        </p>
      )}

      {!error && result && result.items.length > 0 && (
        <>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 md:grid-cols-3">
            {result.items.map((quiz) => (
              <QuizCard key={quiz.id} quiz={quiz} />
            ))}
          </div>
          <Pager
            page={result.page}
            totalPages={result.totalPages}
            totalCount={result.totalCount}
            onChange={handlePageChange}
          />
        </>
      )}
    </div>
  );
}
