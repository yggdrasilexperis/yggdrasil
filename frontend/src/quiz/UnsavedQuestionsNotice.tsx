import { Link } from 'react-router-dom';

import type { QuizSummary } from '../api/types';
import { Card } from '../components/Card';

export type FailedQuestion = { position: number; text: string; message: string };

type Props = {
  quiz: QuizSummary;
  failed: FailedQuestion[];
};

export function UnsavedQuestionsNotice({ quiz, failed }: Props) {
  const single = failed.length === 1;

  return (
    <Card className="mt-6 flex flex-col gap-4">
      <div role="alert">
        <p className="font-semibold">
          “{quiz.title}” was saved, but {single ? 'one question was' : 'some questions were'} not.
        </p>
        <p className="mt-2 text-muted">
          The other questions are on the quiz. Open it to add the {single ? 'one' : 'ones'} below.
        </p>
      </div>

      <ul className="flex flex-col gap-4">
        {failed.map((question) => (
          <li key={question.position} className="rounded-control border border-hairline px-4 py-4">
            <p>
              {question.position}. {question.text}
            </p>
            <p className="mt-2 text-sm text-red-600">{question.message}</p>
          </li>
        ))}
      </ul>

      <Link to={`/quizzes/${quiz.id}`} className="self-start text-accent">
        Open the quiz
      </Link>
    </Card>
  );
}
