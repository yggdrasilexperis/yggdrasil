import type { QuizSummary } from '../api/types';
import { ButtonLink } from '../components/ButtonLink';
import { PixelIcon } from '../components/PixelIcon';

export type FailedQuestion = { position: number; text: string; message: string };

type Props = {
  quiz: QuizSummary;
  failed: FailedQuestion[];
};

export function UnsavedQuestionsNotice({ quiz, failed }: Props) {
  const single = failed.length === 1;

  return (
    <div className="flex flex-col gap-4">
      <div role="alert" className="flex items-start gap-4">
        <PixelIcon name="warning" scale={2} />
        <div>
          <p className="font-bold">
            “{quiz.title}” was saved, but {single ? 'one question was' : 'some questions were'} not.
          </p>
          <p className="mt-2 text-muted">
            The other questions are on the quiz. Open it to add the {single ? 'one' : 'ones'} below.
          </p>
        </div>
      </div>

      <ul className="flex flex-col divide-y divide-dotted divide-dim/60 bg-white p-1 shadow-field">
        {failed.map((question) => (
          <li key={question.position} className="px-3 py-3">
            <p>
              {question.position}. {question.text}
            </p>
            <p className="mt-1 text-danger">{question.message}</p>
          </li>
        ))}
      </ul>

      <ButtonLink to={`/quizzes/${quiz.id}`} className="self-end">
        Open the quiz
      </ButtonLink>
    </div>
  );
}
