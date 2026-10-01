import { useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'react-router-dom';

import { ApiError } from '../api/ApiError';
import { listQuizzes } from '../api/quiz';
import { getCategories } from '../api/quizzes';
import { QUIZ_SORTS } from '../api/types';
import type { Category, PagedResult, QuizSort, QuizSummary } from '../api/types';
import { ErrorState } from '../components/ErrorState';
import { Loading } from '../components/Loading';
import { Select } from '../components/Select';
import { Window } from '../components/Window';
import { CategoryFilter } from './CategoryFilter';
import { Pager } from './Pager';
import { QuizList } from './QuizList';

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

  const hasItems = !error && result && result.items.length > 0;

  return (
    <div className="mx-auto w-full max-w-5xl">
      <Window
        title="Discover"
        icon="folder"
        flush
        toolbar={
          <>
            {categories.length > 0 && (
              <CategoryFilter
                categories={categories}
                selected={selected}
                onChange={handleFilterChange}
              />
            )}
            <Select
              aria-label="Sort quizzes"
              value={sort}
              onChange={(event) => handleSortChange(event.target.value as QuizSort)}
              className="ml-auto self-start"
            >
              {Object.entries(QUIZ_SORTS).map(([key, option]) => (
                <option key={key} value={key}>
                  {option.label}
                </option>
              ))}
            </Select>
          </>
        }
        status={
          hasItems && (
            <Pager
              page={result.page}
              totalPages={result.totalPages}
              totalCount={result.totalCount}
              onChange={handlePageChange}
            />
          )
        }
      >
        {error && <ErrorState message={error} onRetry={retry} className="p-3 sm:p-5" />}
        {!error && result?.items === null && (
          <Loading label="Loading quizzes…" className="p-3 sm:p-5" />
        )}
        {!error && result?.items?.length === 0 && (
          <p className="bg-white px-4 py-16 text-center text-muted shadow-field">
            {selected.length > 0
              ? 'No quizzes match every category you picked.'
              : 'No quizzes yet. Be the first to create one.'}
          </p>
        )}

        {hasItems && (
          <QuizList quizzes={result.items} sort={sort} onSortChange={handleSortChange} />
        )}
      </Window>
    </div>
  );
}
