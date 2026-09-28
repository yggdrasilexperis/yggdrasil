import { useState } from 'react';
import type { SubmitEvent } from 'react';
import { Link } from 'react-router-dom';

import type { Comment } from '../api/types';
import { Button } from '../components/Button';

const relativeTime = new Intl.RelativeTimeFormat('en', { numeric: 'auto' });
const fullDate = new Intl.DateTimeFormat('en', { dateStyle: 'medium', timeStyle: 'short' });
const shortDate = new Intl.DateTimeFormat('en', { dateStyle: 'medium' });

/** "just now", "5 minutes ago", "yesterday", then the date once it is a week old. */
function timeAgo(iso: string) {
  const seconds = Math.round((Date.parse(iso) - Date.now()) / 1000);
  if (seconds > -60) return 'just now';
  const minutes = Math.round(seconds / 60);
  if (minutes > -60) return relativeTime.format(minutes, 'minute');
  const hours = Math.round(minutes / 60);
  if (hours > -24) return relativeTime.format(hours, 'hour');
  const days = Math.round(hours / 24);
  if (days > -7) return relativeTime.format(days, 'day');
  return shortDate.format(new Date(iso));
}

function isEdited(comment: Comment) {
  return Date.parse(comment.updatedAt) - Date.parse(comment.createdAt) > 1000;
}

export function CommentSection({
  comments,
  quizOwnerId,
  canComment,
  onAdd,
}: {
  comments: Comment[];
  quizOwnerId: string;
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
            <div className="flex flex-wrap items-center justify-between text-sm text-muted">
              <div>
                <span className="font-semibold text-ink">{comment.authorUsername}</span>
                {comment.authorId === quizOwnerId && (
                  <span className="ml-2 rounded-control bg-parchment px-2">Quiz author</span>
                )}
              </div>

              <time
                dateTime={comment.createdAt}
                title={fullDate.format(new Date(comment.createdAt))}
              >
                {timeAgo(comment.createdAt)}
              </time>
              {isEdited(comment) && (
                <span title={`Edited ${fullDate.format(new Date(comment.updatedAt))}`}>edited</span>
              )}
            </div>
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
