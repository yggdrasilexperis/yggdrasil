import { useState } from 'react';
import type { SubmitEvent } from 'react';

import { ApiError } from '../api/ApiError';
import type { CreateAnswerOptionsRequest, CreateQuestionRequest, Question } from '../api/types';
import { Button } from '../components/Button';
import { GroupBox } from '../components/GroupBox';
import { Input } from '../components/Input';

type Props = {
  question?: Question;
  onSave: (question: CreateQuestionRequest) => Promise<void>;
  onCancel: () => void;
  submitLabel?: string;
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

function dropEmptyOptions(options: CreateAnswerOptionsRequest[]) {
  const filled = options.filter((option) => option.text.trim());
  if (filled.length < 2) return options;
  return options.filter((option) => option.text.trim() || option.isCorrect);
}

export function QuestionForm({ question, onSave, onCancel, submitLabel = 'Save question' }: Props) {
  const [text, setText] = useState(question?.text ?? '');
  const [options, setOptions] = useState<CreateAnswerOptionsRequest[]>(
    question?.answerOptions.map((option) => ({
      text: option.text,
      isCorrect: option.isCorrect,
    })) ?? [blankOption, blankOption],
  );
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

    const kept = dropEmptyOptions(options);
    setOptions(kept);

    const clientErrors = validate(text, kept);
    setFieldErrors(clientErrors);
    if (Object.keys(clientErrors).length > 0) return;

    setSaving(true);
    try {
      await onSave({
        text: text.trim(),
        answerOptions: kept.map((option) => ({ ...option, text: option.text.trim() })),
      });
    } catch (err) {
      if (err instanceof ApiError && err.status === 400) {
        setFieldErrors({
          text: err.fieldError('text'),
          answerOptions: err.fieldError('answerOptions'),
          optionTexts: kept.map((_, i) => err.fieldError(`answerOptions[${i}].text`)),
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
    <GroupBox label={question ? 'Edit question' : 'New question'}>
      <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-6">
        {formError && (
          <p role="alert" className="text-danger">
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

        <GroupBox as="fieldset" label="Answer options" className="flex flex-col gap-4">
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
                <label className="flex min-h-8 items-center gap-2">
                  <input
                    type="checkbox"
                    checked={option.isCorrect}
                    onChange={(event) => updateOption(index, { isCorrect: event.target.checked })}
                  />
                  Correct
                </label>
                {options.length > 2 && (
                  <Button
                    variant="utility"
                    onClick={() => removeOption(index)}
                    aria-label={`Remove option ${index + 1}`}
                  >
                    Remove
                  </Button>
                )}
              </div>
            </div>
          ))}

          {fieldErrors.answerOptions && <p className="text-danger">{fieldErrors.answerOptions}</p>}

          <Button
            variant="secondary"
            className="self-start"
            onClick={() => setOptions((prev) => [...prev, blankOption])}
          >
            Add option
          </Button>
        </GroupBox>

        <div className="flex gap-1.5">
          <Button type="submit" disabled={saving}>
            {saving ? 'Saving…' : submitLabel}
          </Button>
          <Button variant="secondary" onClick={onCancel} disabled={saving}>
            Cancel
          </Button>
        </div>
      </form>
    </GroupBox>
  );
}
