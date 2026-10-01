import { Link } from 'react-router-dom';

import type { QuizSummary } from '../api/types';
import { PixelIcon } from '../components/PixelIcon';
import { DifficultyBadge } from './DifficultyBadge';

const created = new Intl.DateTimeFormat('en', { dateStyle: 'medium' });

type Props = { quiz: QuizSummary; columns: string };

/**
 * One quiz in the Discover list, laid out on the list's columns from `md` up. Below that
 * the columns fold into a line under the title. The whole row is the link, and it takes
 * the selection highlight on hover and focus, so there is never a doubt which quiz a
 * detail belongs to.
 */
export function QuizRow({ quiz, columns }: Props) {
  return (
    <Link
      to={`/quizzes/${quiz.id}`}
      className={`group flex flex-col gap-0.5 px-1 py-1.5 hover:bg-accent hover:text-white focus-visible:bg-accent focus-visible:text-white focus-visible:-outline-offset-2 focus-visible:outline-white ${columns}`}
    >
      <span className="flex min-w-0 gap-2 px-1">
        <PixelIcon name="doc" className="mt-0.5" />
        <span className="min-w-0">
          <span className="block font-bold">{quiz.title}</span>
          {quiz.description && (
            <span className="line-clamp-2 text-muted group-hover:text-white group-focus-visible:text-white md:line-clamp-1">
              {quiz.description}
            </span>
          )}
        </span>
      </span>

      <span className="flex flex-wrap gap-x-4 pl-9 text-muted group-hover:text-white group-focus-visible:text-white md:contents md:text-ink">
        <span className="md:px-2">
          <DifficultyBadge difficulty={quiz.difficulty} />
        </span>
        <span className="md:px-2">{quiz.categories.map((c) => c.name).join(', ')}</span>
        <time dateTime={quiz.createdAt} className="md:px-2">
          {created.format(new Date(quiz.createdAt))}
        </time>
      </span>
    </Link>
  );
}
