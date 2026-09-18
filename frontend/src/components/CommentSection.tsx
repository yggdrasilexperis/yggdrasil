import { useState } from 'react';
import type { SubmitEvent } from 'react';
import { Link } from 'react-router-dom';

import type { Comment } from '../api/types';
import { Button } from '../components/Button';

export function CommentSection({
  comments,
  canComment,
  onAdd,
}: {
  comments: Comment[];
  canComment: boolean;
  onAdd: (body: string) => void;
}) {
  const [body, setBody] = useState('');

  function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!body.trim()) return;
    onAdd(body.trim());
    setBody('');
  }

  return (
    <div className="flex flex-col gap-4">
      <h2 className="font-display text-2xl font-semibold tracking-tight">
        Comments {comments.length > 0 && `(${comments.length})`}
      </h2>

      {comments.length === 0 && <p className="text-muted">No comments yet.</p>}

      <ul className="flex flex-col gap-3">
        {comments.map((comment) => (
          <li key={comment.id} className="rounded-card border border-hairline p-4">
            {/* No username-lookup endpoint exists — this is the raw author id. */}
            <p className="text-sm text-muted">{comment.authorId}</p>
            <p className="mt-1">{comment.body}</p>
          </li>
        ))}
      </ul>

      {canComment ? (
        <form onSubmit={handleSubmit} className="flex flex-col gap-2">
          <label htmlFor="comment-body" className="text-sm">
            Add a comment
          </label>
          <textarea
            id="comment-body"
            value={body}
            onChange={(event) => setBody(event.target.value)}
            className="min-h-24 w-full rounded-control border border-hairline px-4 py-3"
          />
          <Button type="submit" className="self-start">
            Post comment
          </Button>
        </form>
      ) : (
        <p className="text-sm text-muted">
          <Link to="/login" className="text-accent">
            Sign in
          </Link>{' '}
          to leave a comment.
        </p>
      )}
    </div>
  );
}
