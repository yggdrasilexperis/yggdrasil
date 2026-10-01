import { useState } from 'react';
import type { SubmitEvent } from 'react';

import { ApiError } from '../api/ApiError';
import { addComment } from '../api/quiz';
import type { Comment } from '../api/types';
import { Button } from '../components/Button';

type Props = {
  quizId: string;
  onAdded: (comment: Comment) => void;
};

export function AddCommentForm({ quizId, onAdded }: Props) {
  const [body, setBody] = useState('');
  const [expanded, setExpanded] = useState<boolean>(false);
  const [fieldError, setFieldError] = useState<string | undefined>();
  const [formError, setFormError] = useState('');
  const [saving, setSaving] = useState(false);

  function handleCancel() {
    setBody('');
    setFieldError(undefined);
    setFormError('');
    setExpanded(false);
  }

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
      const comment = await addComment(quizId, { body: trimmed });
      onAdded(comment);
      setBody('');
      setExpanded(false);
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

  if (!expanded) {
    return (
      <>
        <label htmlFor="comment-body" className="sr-only">
          Add a comment
        </label>
        <input
          id="comment-body"
          value={body}
          onChange={(event) => setBody(event.target.value)}
          onFocus={() => setExpanded(true)}
          placeholder="Add a comment..."
          maxLength={2000}
          className="h-9 w-full bg-white px-2 shadow-field placeholder:text-dim"
        />
      </>
    );
  }

  return (
    <form onSubmit={handleSubmit} noValidate className="flex flex-col gap-3">
      {formError && (
        <p role="alert" className="text-danger">
          {formError}
        </p>
      )}
      <label htmlFor={'comment-body'} className="sr-only">
        Add a comment
      </label>

      <textarea
        id="comment-body"
        autoFocus
        value={body}
        onChange={(event) => setBody(event.target.value)}
        placeholder="Add a comment..."
        maxLength={2000}
        className="min-h-24 w-full bg-white px-2 py-1.5 shadow-field placeholder:text-dim"
      />
      {fieldError && <p className="text-danger">{fieldError}</p>}
      <div className="flex gap-1.5">
        <Button type="submit" disabled={saving}>
          {saving ? 'Posting…' : 'Post comment'}
        </Button>
        <Button variant="secondary" onClick={handleCancel} disabled={saving}>
          Cancel
        </Button>
      </div>
    </form>
  );
}
