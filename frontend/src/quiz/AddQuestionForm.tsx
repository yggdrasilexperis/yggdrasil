import { useState } from 'react';
import type { SubmitEvent } from 'react';

import { ApiError } from '../api/ApiError';
import { addQuestion } from '../api/quiz';
import type { CreateAnswerOptionsRequest, Question } from '../api/types';
import { Button } from '../components/Button';
import { Card } from '../components/Card';
import { Input } from '../components/Input';

type Props = {
  quizId: string;
  onAdded: (question: Question) => void;
  onCancel: () => void;
};

type FieldErrors = {
  text?: string;
  answerOptions?: string;
  /** One entry per option, by index. */
  optionTexts?: (string | undefined)[];
};

const blankOption: CreateAnswerOptionsRequest = { text: '', isCorrect: false };

function validate(text: string, options: CreateAnswerOptionsRequest[]): FieldErrors {
  const errors: FieldErrors = {};

  if (!text.trim()) errors.text = 'Text is required';

  const optionTexts = options.map((option) =>
    option.text.trim() ? undefined : 'Answer option text is required',
  );
  if (optionTexts.some(Boolean)) errors.optionTexts = optionTexts;

  if (!options.some((option) => option.isCorrect))
    errors.answerOptions = 'At least one answer option must be correct';

  return errors;
}

export function AddQuestionForm({ quizId, onAdded, onCancel }: Props) {
  const [text, setText] = useState('');
  const [options, setOptions] = useState([blankOption, blankOption]);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);

  function updateOption(index: number, change: Partial<CreateAnswerOptionsRequest>) {
    setOptions((prev) =>
      prev.map((option, i) => (i === index ? { ...option, ...change } : option)),
    );
  }

  function removeOption(index: number) {
    setOptions((prev) => prev.filter((_, i) => i !== index));
    setFieldErrors((prev) => ({
      ...prev,
      optionTexts: prev.optionTexts?.filter((_, i) => i !== index),
    }));
  }

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError('');

    const clientErrors = validate(text, options);
    setFieldErrors(clientErrors);
    if (Object.keys(clientErrors).length > 0) return;

    setSaving(true);
    try {
      const question = await addQuestion(quizId, {
        text: text.trim(),
        answerOptions: options.map((option) => ({ ...option, text: option.text.trim() })),
      });
      onAdded(question);
    } catch (err) {
      if (err instanceof ApiError && err.status === 400) {
        setFieldErrors({
          text: err.fieldError('text'),
          answerOptions: err.fieldError('answerOptions'),
          optionTexts: options.map((_, i) => err.fieldError(`answerOptions[${i}].text`)),
        });
        setFormError('The question was not saved. Fix the fields below.');
      } else {
        setFormError(
          err instanceof ApiError ? err.detail || err.title : 'Could not save the question.',
        );
      }
      setSaving(false);
    }
  }

  return (
    <Card>
      <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-6">
        {formError && (
          <p role="alert" className="text-sm text-red-600">
            {formError}
          </p>
        )}

        <Input
          label="Question"
          value={text}
          maxLength={1000}
          onChange={(event) => setText(event.target.value)}
          error={fieldErrors.text}
        />

        <fieldset className="flex flex-col gap-4">
          <legend className="mb-2 text-sm">Answer options</legend>

          {options.map((option, index) => (
            <div key={index} className="flex flex-col gap-2">
              <Input
                label={`Option ${index + 1}`}
                value={option.text}
                maxLength={500}
                onChange={(event) => updateOption(index, { text: event.target.value })}
                error={fieldErrors.optionTexts?.[index]}
              />
              <div className="flex items-center gap-4">
                <label className="flex min-h-11 items-center gap-2 text-sm">
                  <input
                    type="checkbox"
                    checked={option.isCorrect}
                    onChange={(event) => updateOption(index, { isCorrect: event.target.checked })}
                    className="h-4 w-4 accent-accent"
                  />
                  Correct
                </label>
                {options.length > 2 && (
                  <button
                    type="button"
                    onClick={() => removeOption(index)}
                    aria-label={`Remove option ${index + 1}`}
                    className="min-h-11 text-sm text-accent transition-transform active:scale-95"
                  >
                    Remove
                  </button>
                )}
              </div>
            </div>
          ))}

          {fieldErrors.answerOptions && (
            <p className="text-sm text-red-600">{fieldErrors.answerOptions}</p>
          )}

          <Button
            variant="secondary"
            className="self-start"
            onClick={() => setOptions((prev) => [...prev, blankOption])}
          >
            Add option
          </Button>
        </fieldset>

        <div className="flex gap-3">
          <Button type="submit" disabled={saving}>
            {saving ? 'Saving…' : 'Save question'}
          </Button>
          <Button variant="secondary" onClick={onCancel} disabled={saving}>
            Cancel
          </Button>
        </div>
      </form>
    </Card>
  );
}
