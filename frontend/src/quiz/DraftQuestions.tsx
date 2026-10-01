import { useState } from 'react';

import type { CreateQuestionRequest } from '../api/types';
import { Button } from '../components/Button';
import { GroupBox } from '../components/GroupBox';
import { SectionHeading } from '../components/SectionHeading';
import { play } from '../sound/player';
import { AnswerList } from './AnswerList';
import { QuestionForm } from './QuestionForm';

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
    play('save');
  }

  function handleRemove(index: number) {
    onChange(drafts.filter((_, i) => i !== index));
    play('recycle');
  }

  return (
    <section className="flex flex-col gap-6">
      <SectionHeading>
        <span>
          Questions <span className="text-base font-normal text-muted">(optional)</span>
        </span>
      </SectionHeading>

      {drafts.length > 0 && (
        <ol className="flex flex-col gap-6">
          {drafts.map((draft, index) => (
            <li key={index}>
              <GroupBox label={`Question ${index + 1}`} className="flex flex-col gap-3">
                <div className="flex items-start justify-between gap-4">
                  <p className="font-bold">{draft.text}</p>
                  <Button
                    variant="utility"
                    onClick={() => handleRemove(index)}
                    disabled={disabled}
                    aria-label={`Remove question ${index + 1}`}
                  >
                    Remove
                  </Button>
                </div>
                <AnswerList options={draft.answerOptions} revealed />
              </GroupBox>
            </li>
          ))}
        </ol>
      )}

      {adding ? (
        <QuestionForm
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
