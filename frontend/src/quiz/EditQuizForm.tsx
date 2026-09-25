import { useId, useState } from 'react';
import type { SubmitEvent } from 'react';

import { ApiError } from '../api/ApiError';
import { updateQuiz } from '../api/quiz';
import type { QuizSummary } from '../api/types';
import { Button } from '../components/Button';
import { Card } from '../components/Card';
import { Input } from '../components/Input';

type Props = {
  quiz: QuizSummary;
  onSaved: (quiz: QuizSummary) => void;
  onCancel: () => void;
};

type FieldErrors = {
  title?: string;
  description?: string;
};

function validate(title: string, description: string): FieldErrors {
  const errors: FieldErrors = {};

  if (!title.trim()) errors.title = 'Title is required';
  else if (title.trim().length > 200) errors.title = 'Title must be 200 characters or fewer';

  if (description.trim().length > 2000)
    errors.description = 'Description must be 2000 characters or fewer';

  return errors;
}

export function EditQuizForm({ quiz, onSaved, onCancel }: Props) {
  const descriptionId = useId();

  const [title, setTitle] = useState(quiz.title);
  const [description, setDescription] = useState(quiz.description ?? '');
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError('');

    const clientErrors = validate(title, description);
    setFieldErrors(clientErrors);
    if (Object.keys(clientErrors).length > 0) return;

    setSaving(true);
    try {
      const updated = await updateQuiz(quiz.id, {
        title: title.trim(),
        description: description.trim() || null,
        difficulty: quiz.difficulty,
        categoryIds: quiz.categories.map((c) => c.categoryId),
      });
      onSaved(updated);
    } catch (err) {
      if (err instanceof ApiError && err.status === 400) {
        setFieldErrors({
          title: err.fieldError('title'),
          description: err.fieldError('description'),
        });
      }
      setFormError(err instanceof ApiError ? err.detail || err.title : 'Could not save the quiz.');
      setSaving(false);
    }
  }

  return (
    <Card>
      <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-4">
        {formError && (
          <p role="alert" className="text-sm text-red-600">
            {formError}
          </p>
        )}

        <Input
          label="Title"
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          error={fieldErrors.title}
        />

        <div>
          <label htmlFor={descriptionId} className="mb-2 block text-sm">
            Description <span className="text-muted">(optional)</span>
          </label>
          <textarea
            id={descriptionId}
            rows={4}
            value={description}
            onChange={(event) => setDescription(event.target.value)}
            aria-invalid={fieldErrors.description ? true : undefined}
            className={`w-full rounded-control border px-4 py-4 ${
              fieldErrors.description ? 'border-red-600' : 'border-hairline'
            }`}
          />
          {fieldErrors.description && (
            <p className="mt-2 text-sm text-red-600">{fieldErrors.description}</p>
          )}
        </div>

        <div className="flex gap-3">
          <Button type="submit" disabled={saving}>
            {saving ? 'Saving…' : 'Save'}
          </Button>
          <Button variant="secondary" onClick={onCancel} disabled={saving}>
            Cancel
          </Button>
        </div>
      </form>
    </Card>
  );
}
