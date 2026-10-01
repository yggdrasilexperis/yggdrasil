import type { Comment } from '../api/types';
import { EditCommentForm } from '../quiz/EditCommentForm';
import { AddCommentForm } from '../quiz/AddCommentForm';
import { Link, useLocation } from 'react-router-dom';
import { CommentActionsMenu } from './CommentActionsMenu';
import { SectionHeading } from './SectionHeading';

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
      <SectionHeading>Comments {comments.length > 0 && `(${comments.length})`}</SectionHeading>
      {canComment ? (
        <AddCommentForm quizId={quizId} onAdded={onAdded} />
      ) : (
        <p className="text-muted">
          <Link
            to="/login"
            state={{ from: location.pathname }}
            className="text-accent underline underline-offset-2"
          >
            Sign in
          </Link>{' '}
          to leave a comment.
        </p>
      )}
      {comments.length === 0 && <p className="text-muted">No comments yet.</p>}

      <ul className="flex flex-col divide-y divide-dotted divide-dim bg-white p-1 shadow-field empty:hidden">
        {comments.map((comment) => (
          <li key={comment.id} className="px-3 py-3">
            <div className="flex items-start justify-between gap-3 text-muted">
              <div className="flex flex-wrap items-center gap-x-2 gap-y-1">
                <span className="font-bold text-ink">{comment.authorUsername}</span>
                {comment.authorId === quizOwnerId && (
                  <span className="border border-ink bg-tooltip px-1.5 leading-6 text-ink">
                    Quiz author
                  </span>
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
              <div className="shrink-0">
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
