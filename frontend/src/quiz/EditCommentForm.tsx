import { useState } from 'react';
import type { SubmitEvent } from 'react';

import { ApiError } from '../api/ApiError';
import { updateComment } from '../api/quiz';
import type { Comment } from '../api/types';
import { Button } from '../components/Button';

type Props = {
  quizId: string;
  commentId: string;
  initialBody: string;
  onSaved: (comment: Comment) => void;
  onCancel: () => void;
};

export function EditCommentForm({ quizId, commentId, initialBody, onSaved, onCancel }: Props) {
  const [body, setBody] = useState(initialBody);
  const [fieldError, setFieldError] = useState<string | undefined>();
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError('');

    const trimmed = body.trim();
    if (!trimmed) {
      setFieldError('Comment text is required');
      return;
    }
    setFieldError(undefined);

    setSaving(true);
    try {
      const comment = await updateComment(quizId, commentId, { body: trimmed });
      onSaved(comment);
    } catch (err) {
      if (err instanceof ApiError && err.status === 400) {
        setFieldError(err.fieldError('body'));
        setFormError('The comment was not saved. Fix the field below.');
      } else {
        setFormError(
          err instanceof ApiError ? err.detail || err.title : 'Could not save the comment.',
        );
      }
    }
    setSaving(false);
  }

  return (
    <form onSubmit={handleSubmit} noValidate className="mt-2 flex flex-col gap-2">
      {formError && (
        <p role="alert" className="text-sm text-red-600">
          {formError}
        </p>
      )}
      <label htmlFor={`edit-comment-${commentId}`} className="sr-only">
        Edit comment
      </label>
      <textarea
        id={`edit-comment-${commentId}`}
        value={body}
        onChange={(event) => setBody(event.target.value)}
        maxLength={2000}
        className="min-h-24 w-full rounded-control border border-hairline px-4 py-3"
      />
      {fieldError && <p className="text-sm text-red-600">{fieldError}</p>}
      <div className="flex gap-3">
        <Button type="submit" disabled={saving}>
          {saving ? 'Saving…' : 'Save'}
        </Button>
        <Button variant="secondary" onClick={onCancel} disabled={saving}>
          Cancel
        </Button>
      </div>
    </form>
  );
}
