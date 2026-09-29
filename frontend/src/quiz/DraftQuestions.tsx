import { useState } from 'react';

import type { CreateQuestionRequest } from '../api/types';
import { Button } from '../components/Button';
import { AddQuestionForm } from './AddQuestionForm';

type Props = {
  drafts: CreateQuestionRequest[];
  onChange: (drafts: CreateQuestionRequest[]) => void;
  disabled: boolean;
};

export function DraftQuestions({ drafts, onChange, disabled }: Props) {
  const [adding, setAdding] = useState(false);

  async function handleAdd(question: CreateQuestionRequest) {
    onChange([...drafts, question]);
    setAdding(false);
  }

  function handleRemove(index: number) {
    onChange(drafts.filter((_, i) => i !== index));
  }

  return (
    <section className="flex flex-col gap-4">
      <h2 className="font-display text-2xl font-semibold tracking-tight">
        Questions <span className="text-base font-normal text-muted">(optional)</span>
      </h2>

      {drafts.length > 0 && (
        <ol className="flex flex-col gap-4">
          {drafts.map((draft, index) => (
            <li key={index} className="rounded-control border border-hairline px-4 py-4">
              <div className="flex items-start justify-between gap-4">
                <p className="font-semibold">
                  {index + 1}. {draft.text}
                </p>
                <button
                  type="button"
                  onClick={() => handleRemove(index)}
                  disabled={disabled}
                  aria-label={`Remove question ${index + 1}`}
                  className="min-h-12 text-sm text-accent transition-transform active:scale-95"
                >
                  Remove
                </button>
              </div>
              <ul className="mt-2 flex flex-col gap-1 text-sm text-muted">
                {draft.answerOptions.map((option, optionIndex) => (
                  <li key={optionIndex}>
                    {option.text}
                    {option.isCorrect && <span className="ml-2 text-green-600">Correct</span>}
                  </li>
                ))}
              </ul>
            </li>
          ))}
        </ol>
      )}

      {adding ? (
        <AddQuestionForm
          submitLabel="Add question"
          onSave={handleAdd}
          onCancel={() => setAdding(false)}
        />
      ) : (
        <Button
          variant="secondary"
          className="self-start"
          onClick={() => setAdding(true)}
          disabled={disabled}
        >
          Add question
        </Button>
      )}
    </section>
  );
}
