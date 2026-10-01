import type { CreateAnswerOptionsRequest } from '../api/types';
import { PixelIcon } from '../components/PixelIcon';

type Props = {
  options: CreateAnswerOptionsRequest[];
  /** Mark the correct options. Off while a player is still guessing. */
  revealed: boolean;
};

/** A question's answer options, in a list box. */
export function AnswerList({ options, revealed }: Props) {
  return (
    <ul className="flex flex-col divide-y divide-dotted divide-dim/60 bg-white p-0.5 shadow-field">
      {options.map((option, index) => (
        <li
          key={index}
          className={`flex justify-between gap-4 px-3 py-1.5 ${
            revealed && option.isCorrect ? 'bg-success/10' : ''
          }`}
        >
          {option.text}
          {revealed && option.isCorrect && (
            <span className="flex shrink-0 items-center gap-1.5 text-success">
              <PixelIcon name="check" />
              Correct
            </span>
          )}
        </li>
      ))}
    </ul>
  );
}
