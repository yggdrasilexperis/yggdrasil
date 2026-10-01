import type { ReactNode } from 'react';

import type { QuizSort, QuizSummary } from '../api/types';
import { ColumnHeader } from './ColumnHeader';
import { QuizRow } from './QuizRow';

/** Name takes what is left; the rest are sized for their longest usual value. */
const COLUMNS = 'md:grid md:grid-cols-[minmax(0,1fr)_9.5rem_12rem_8.5rem] md:items-start';

type Props = {
  quizzes: QuizSummary[];
  /** How many rows a full page holds. The pane is always that tall, however many it has. */
  rows: number;
  sort: QuizSort;
  onSortChange: (sort: QuizSort) => void;
  /** Centred in the space under the last row: loading, an error, or the end of the list. */
  children?: ReactNode;
};

/**
 * Explorer's details view: a header of column buttons over one row per quiz. Name and
 * Created sort on click, the two orders the backend offers; clicking the sorted column
 * again reverses it. Below `md` the header folds away and the toolbar's sort menu stands in.
 *
 * Every row is the same height, three lines when stacked and two from `md` up, and the
 * pane is a full page of them, so the window keeps one size whatever the page holds.
 */
export function QuizList({ quizzes, rows, sort, onSortChange, children }: Props) {
  const byTitle = sort === 'title-asc' ? 'ascending' : sort === 'title-desc' ? 'descending' : null;
  const byDate = sort === 'oldest' ? 'ascending' : sort === 'newest' ? 'descending' : null;

  return (
    <div className="flex flex-col bg-white p-0.5 shadow-field [--row:calc(3lh+1rem+1px)] md:[--row:calc(2lh+1rem+1px)]">
      <div className={`hidden ${COLUMNS}`}>
        <ColumnHeader
          label="Name"
          direction={byTitle}
          onClick={() => onSortChange(sort === 'title-asc' ? 'title-desc' : 'title-asc')}
        />
        <ColumnHeader label="Difficulty" />
        <ColumnHeader label="Categories" />
        <ColumnHeader
          label="Created"
          direction={byDate}
          onClick={() => onSortChange(sort === 'newest' ? 'oldest' : 'newest')}
        />
      </div>

      <div className="flex flex-col" style={{ height: `calc(var(--row) * ${rows})` }}>
        <ul>
          {quizzes.map((quiz) => (
            <li key={quiz.id} className="h-(--row) border-b border-dotted border-dim/50">
              <QuizRow quiz={quiz} columns={COLUMNS} />
            </li>
          ))}
        </ul>
        {children && (
          <div className="flex min-h-0 flex-1 items-center justify-center p-3">{children}</div>
        )}
      </div>
    </div>
  );
}
