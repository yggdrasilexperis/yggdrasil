import { useEffect, useId, useRef, useState } from 'react';
import type { KeyboardEvent, ReactNode, SubmitEvent } from 'react';

import { ApiError } from '../api/ApiError';
import { updateQuiz } from '../api/quiz';
import type { QuizSummary } from '../api/types';
import { Button } from '../components/Button';

type Props = {
  quiz: QuizSummary;
  onSaved: (quiz: QuizSummary) => void;
  onCancel: () => void;
  /** Shown between the fields and the buttons, so what sits under the title keeps its place. */
  children?: ReactNode;
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

/**
 * Takes the place of the quiz's title and description on the detail page while editing.
 * The fields keep the heading's and paragraph's type and position, so the text stays where
 * it was; like renaming a file in Explorer, only a thin white box appears around it.
 */
export function EditQuizForm({ quiz, onSaved, onCancel, children }: Props) {
  const titleErrorId = useId();
  const descriptionErrorId = useId();

  const [title, setTitle] = useState(quiz.title);
  const [description, setDescription] = useState(quiz.description ?? '');
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);
  const titleRef = useRef<HTMLTextAreaElement>(null);

  // Put the caret after the title, not before it, so typing picks up where the title ends.
  useEffect(() => {
    const titleField = titleRef.current;
    titleField?.focus();
    titleField?.setSelectionRange(titleField.value.length, titleField.value.length);
  }, []);

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

  // The title is a textarea so a long one wraps like the heading does; Enter still saves.
  function handleTitleKeyDown(event: KeyboardEvent<HTMLTextAreaElement>) {
    if (event.key === 'Enter' && !event.nativeEvent.isComposing) {
      event.preventDefault();
      event.currentTarget.form?.requestSubmit();
    }
  }

  function handleKeyDown(event: KeyboardEvent<HTMLFormElement>) {
    if (event.key === 'Escape' && !saving) onCancel();
  }

  // field-sizing makes each box exactly as tall as its text. Chrome still reports 1px to
  // scroll and flashes a scrollbar, so hide overflow, but only where the box can grow.
  // The rename box is a spread shadow, so it frames the text without moving it.
  const field =
    'block w-full resize-none bg-white shadow-rename field-sizing-content placeholder:text-dim focus-visible:outline-offset-5 supports-field-sizing:overflow-hidden';

  return (
    <form onSubmit={handleSubmit} onKeyDown={handleKeyDown} noValidate>
      <textarea
        aria-label="Title"
        ref={titleRef}
        placeholder="Title"
        rows={1}
        value={title}
        onChange={(event) => setTitle(event.target.value.replace(/[\r\n]+/g, ' '))}
        onKeyDown={handleTitleKeyDown}
        aria-invalid={fieldErrors.title ? true : undefined}
        aria-describedby={fieldErrors.title ? titleErrorId : undefined}
        className={`${field} text-xl font-bold`}
      />
      {fieldErrors.title && (
        <p id={titleErrorId} className="mt-2 text-danger">
          {fieldErrors.title}
        </p>
      )}

      <textarea
        aria-label="Description"
        placeholder="Add a description"
        rows={1}
        value={description}
        onChange={(event) => setDescription(event.target.value)}
        aria-invalid={fieldErrors.description ? true : undefined}
        aria-describedby={fieldErrors.description ? descriptionErrorId : undefined}
        className={`${field} mt-2`}
      />
      {fieldErrors.description && (
        <p id={descriptionErrorId} className="mt-2 text-danger">
          {fieldErrors.description}
        </p>
      )}

      {children}

      <div className="mt-4 flex gap-1.5">
        <Button type="submit" disabled={saving}>
          {saving ? 'Saving…' : 'Save'}
        </Button>
        <Button variant="secondary" onClick={onCancel} disabled={saving}>
          Cancel
        </Button>
      </div>

      {formError && (
        <p role="alert" className="mt-3 text-danger">
          {formError}
        </p>
      )}
    </form>
  );
}
