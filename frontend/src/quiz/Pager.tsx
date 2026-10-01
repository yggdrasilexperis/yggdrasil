import { Button } from '../components/Button';

type Props = {
  page: number;
  /** Left out while the page is loading or failed, which blanks the fields. */
  totalPages?: number;
  totalCount?: number;
  onChange: (page: number) => void;
};

const field = 'flex min-h-9 items-center px-2 whitespace-nowrap shadow-status';

/**
 * Sits in the window's status bar: one sunken field per fact, the page buttons beside them.
 * Everything is always there, greyed out when it doesn't apply, so the bar never changes
 * height.
 */
export function Pager({ page, totalPages, totalCount, onChange }: Props) {
  const known = totalPages !== undefined && totalCount !== undefined;
  // An empty result still sits on a page.
  const pages = Math.max(totalPages ?? 1, 1);

  return (
    <nav aria-label="Pagination" className="flex flex-wrap items-stretch gap-0.5">
      <p className={`${field} flex-1`}>
        {known && `${totalCount} ${totalCount === 1 ? 'quiz' : 'quizzes'}`}
      </p>
      <p className={field}>{known && `Page ${page} of ${pages}`}</p>

      <div className="flex gap-0.5">
        <Button
          variant="secondary"
          onClick={() => onChange(page - 1)}
          disabled={!known || page <= 1}
        >
          Previous
        </Button>
        <Button
          variant="secondary"
          onClick={() => onChange(page + 1)}
          disabled={!known || page >= pages}
        >
          Next
        </Button>
      </div>
    </nav>
  );
}
