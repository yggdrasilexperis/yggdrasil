import type { QuizSort, QuizSummary } from '../api/types';
import { ColumnHeader } from './ColumnHeader';
import { QuizRow } from './QuizRow';

/** Name takes what is left; the rest are sized for their longest usual value. */
const COLUMNS = 'md:grid md:grid-cols-[minmax(0,1fr)_9.5rem_12rem_8.5rem] md:items-start';

type Props = {
  quizzes: QuizSummary[];
  sort: QuizSort;
  onSortChange: (sort: QuizSort) => void;
};

/**
 * Explorer's details view: a header of column buttons over one row per quiz. Name and
 * Created sort on click, the two orders the backend offers; clicking the sorted column
 * again reverses it. Below `md` the header folds away and the toolbar's sort menu stands in.
 */
export function QuizList({ quizzes, sort, onSortChange }: Props) {
  const byTitle = sort === 'title-asc' ? 'ascending' : sort === 'title-desc' ? 'descending' : null;
  const byDate = sort === 'oldest' ? 'ascending' : sort === 'newest' ? 'descending' : null;

  return (
    <div className="bg-white p-0.5 shadow-field">
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

      <ul className="flex flex-col divide-y divide-dotted divide-dim/50">
        {quizzes.map((quiz) => (
          <li key={quiz.id}>
            <QuizRow quiz={quiz} columns={COLUMNS} />
          </li>
        ))}
      </ul>
    </div>
  );
}
