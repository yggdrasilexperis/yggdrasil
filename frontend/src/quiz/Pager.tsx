import { Button } from '../components/Button';

type Props = {
  page: number;
  totalPages: number;
  totalCount: number;
  onChange: (page: number) => void;
};

const field = 'flex min-h-9 items-center px-2 whitespace-nowrap shadow-status';

/** Sits in the window's status bar: one sunken field per fact, the page buttons beside them. */
export function Pager({ page, totalPages, totalCount, onChange }: Props) {
  return (
    <nav aria-label="Pagination" className="flex flex-wrap items-stretch gap-0.5">
      <p className={`${field} flex-1`}>
        {totalCount} {totalCount === 1 ? 'quiz' : 'quizzes'}
      </p>
      <p className={field}>
        Page {page} of {totalPages}
      </p>

      {totalPages > 1 && (
        <div className="flex gap-0.5">
          <Button variant="secondary" onClick={() => onChange(page - 1)} disabled={page <= 1}>
            Previous
          </Button>
          <Button
            variant="secondary"
            onClick={() => onChange(page + 1)}
            disabled={page >= totalPages}
          >
            Next
          </Button>
        </div>
      )}
    </nav>
  );
}
