import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';

import { ApiError } from '../api/ApiError';
import { deleteQuiz, getQuizDetail } from '../api/quiz';
import { DIFFICULTY_LABELS } from '../api/types';
import type { Comment, QuizDetail } from '../api/types';
import { useAuth } from '../auth/useAuth';
import { Button } from '../components/Button';
import { Card } from '../components/Card';
import { CommentSection } from '../components/CommentSection';
import { Loading } from '../components/Loading';
import { ErrorState } from '../components/ErrorState';

export function QuizDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { user, isAdmin } = useAuth();
  const navigate = useNavigate();

  const [detail, setDetail] = useState<QuizDetail | null>(null);
  const [error, setError] = useState('');
  const [deleting, setDeleting] = useState(false);
  /** No POST /comments endpoint exists yet — comments posted here live only in memory and are gone on reload. */
  const [localComments, setLocalComments] = useState<Comment[]>([]);
  const [reloadKey, setReloadKey] = useState(0);

  useEffect(() => {
    if (!id) return;
    let cancelled = false;
    setError('');
    setDetail(null);
    getQuizDetail(id)
      .then((data) => {
        if (!cancelled) setDetail(data);
      })
      .catch((err) => {
        if (!cancelled)
          setError(err instanceof ApiError ? err.detail || err.title : 'Could not load this quiz.');
      });
    return () => {
      cancelled = true;
    };
  }, [id, reloadKey]);

  async function handleDelete() {
    if (!id || !window.confirm('Delete this quiz? This cannot be undone.')) return;
    setDeleting(true);
    try {
      await deleteQuiz(id);
      navigate('/', { replace: true });
    } catch (err) {
      setError(err instanceof ApiError ? err.detail || err.title : 'Could not delete this quiz.');
      setDeleting(false);
    }
  }

  function handleAddComment(body: string) {
    if (!user) return;
    setLocalComments((prev) => [
      ...prev,
      {
        id: crypto.randomUUID(),
        authorId: user.id,
        body,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
      },
    ]);
  }

  if (error && !detail) {
    return (
      <div className="mx-auto max-w-3xl px-6 py-12">
        <ErrorState message={error} onRetry={() => setReloadKey((k) => k + 1)} />
      </div>
    );
  }

  if (!detail) {
    return (
      <div className="mx-auto max-w-3xl px-6 py-12">
        <Loading label="Loading quiz…" />
      </div>
    );
  }

  const { quiz, questions } = detail;
  const canManage = user?.id === quiz.ownerId || isAdmin;
  const comments = [...detail.comments, ...localComments];

  return (
    <div className="mx-auto flex max-w-3xl flex-1 flex-col gap-8 px-6 py-12">
      <div>
        <h1 className="font-display text-4xl font-semibold tracking-tight">{quiz.title}</h1>
        {quiz.description && <p className="mt-2 text-muted">{quiz.description}</p>}
        <div className="mt-3 flex items-center gap-2 text-sm text-muted">
          <span>{DIFFICULTY_LABELS[quiz.difficulty]}</span>
          {quiz.categories.length > 0 && (
            <span>· {quiz.categories.map((c) => c.name).join(', ')}</span>
          )}
        </div>

        {canManage && (
          <div className="mt-4 flex gap-3">
            <Link to={`/quizzes/${quiz.id}/edit`}>
              <Button variant="secondary">Edit</Button>
            </Link>
            <Button variant="utility" onClick={handleDelete} disabled={deleting}>
              {deleting ? 'Deleting…' : 'Delete'}
            </Button>
          </div>
        )}

        {error && (
          <p role="alert" className="mt-3 text-sm text-red-600">
            {error}
          </p>
        )}
      </div>

      <div className="flex flex-col gap-4">
        <h2 className="font-display text-2xl font-semibold tracking-tight">Questions</h2>
        {questions.length === 0 && <p className="text-muted">No questions yet.</p>}
        {questions.map((question, index) => (
          <Card key={question.id} className="flex flex-col gap-3">
            <p className="font-display text-xl font-semibold">
              {index + 1}. {question.text}
            </p>
            <ul className="flex flex-col gap-2">
              {question.answerOptions.map((option) => (
                <li
                  key={option.id}
                  className={`rounded-control border px-4 py-3 ${
                    option.isCorrect ? 'border-green-600' : 'border-hairline'
                  }`}
                >
                  {option.text}
                </li>
              ))}
            </ul>
          </Card>
        ))}
      </div>

      <CommentSection comments={comments} canComment={!!user} onAdd={handleAddComment} />
    </div>
  );
}
