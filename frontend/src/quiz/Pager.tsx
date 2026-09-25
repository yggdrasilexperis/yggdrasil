import { Button } from '../components/Button';

type Props = {
  page: number;
  totalPages: number;
  totalCount: number;
  onChange: (page: number) => void;
};

export function Pager({ page, totalPages, totalCount, onChange }: Props) {
  return (
    <nav aria-label="Pagination" className="flex flex-wrap items-center justify-between gap-4">
      <p className="text-sm text-muted">
        Page {page} of {totalPages} · {totalCount} {totalCount === 1 ? 'quiz' : 'quizzes'}
      </p>

      {totalPages > 1 && (
        <div className="flex gap-2">
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
