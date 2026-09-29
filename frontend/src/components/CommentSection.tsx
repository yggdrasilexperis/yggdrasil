import type { Comment } from '../api/types';
import { EditCommentForm } from '../quiz/EditCommentForm';
import { AddCommentForm } from '../quiz/AddCommentForm';
import { Link, useLocation } from 'react-router-dom';
import { CommentActionsMenu } from './CommentActionsMenu';

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
  quizId,
  quizOwnerId,
  canComment,
  removingCommentId,
  editingCommentId,
  onAdded,
  onRemove,
  onEdit,
  onCancelEdit,
  onSaved,
  isAdmin,
  currentUserId,
}: {
  comments: Comment[];
  quizId: string;
  quizOwnerId: string;
  canComment: boolean;
  removingCommentId: string | null;
  currentUserId: string | undefined;
  editingCommentId: string | null;
  isAdmin: boolean;
  onCancelEdit: () => void;
  onEdit: (commentId: string) => void;
  onAdded: (comment: Comment) => void;
  onRemove: (commentId: string) => void;
  onSaved: (comment: Comment) => void;
}) {
  const location = useLocation();

  return (
    <div className="flex flex-col gap-4">
      <h2 className="font-display text-2xl font-semibold tracking-tight">
        Comments {comments.length > 0 && `(${comments.length})`}
      </h2>
      {canComment ? (
        <AddCommentForm quizId={quizId} onAdded={onAdded} />
      ) : (
        <p className="text-sm text-muted">
          <Link to="/login" state={{ from: location.pathname }} className="text-accent">
            Sign in
          </Link>{' '}
          to leave a comment.
        </p>
      )}
      {comments.length === 0 && <p className="text-muted">No comments yet.</p>}

      <ul className="flex flex-col gap-3">
        {comments.map((comment) => (
          <li key={comment.id} className="rounded-card border border-hairline p-4">
            <div className="flex flex-col gap-2 text-sm text-muted sm:flex-row sm:items-center sm:justify-between">
              <div className="flex flex-wrap items-center gap-x-2 gap-y-1">
                <span className="font-semibold text-ink">{comment.authorUsername}</span>
                {comment.authorId === quizOwnerId && (
                  <span className="rounded-control bg-parchment px-2">Quiz author</span>
                )}
                <time
                  dateTime={comment.createdAt}
                  title={fullDate.format(new Date(comment.createdAt))}
                >
                  {timeAgo(comment.createdAt)}
                </time>
                {isEdited(comment) && (
                  <span title={`Edited ${fullDate.format(new Date(comment.updatedAt))}`}>
                    edited
                  </span>
                )}
              </div>
              <div className="self-end sm:self-auto">
                <CommentActionsMenu
                  canEdit={comment.authorId === currentUserId && editingCommentId !== comment.id}
                  canRemove={comment.authorId === currentUserId || isAdmin}
                  removing={removingCommentId === comment.id}
                  onEdit={() => onEdit(comment.id)}
                  onRemove={() => onRemove(comment.id)}
                />
              </div>
            </div>
            {editingCommentId === comment.id ? (
              <EditCommentForm
                quizId={quizId}
                commentId={comment.id}
                initialBody={comment.body}
                onSaved={onSaved}
                onCancel={onCancelEdit}
              />
            ) : (
              <p className="mt-1">{comment.body}</p>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}
