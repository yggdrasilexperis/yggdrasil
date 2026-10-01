import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';

import { ApiError } from '../api/ApiError';
import {
  addQuestion,
  deleteQuestion,
  deleteQuiz,
  getQuizDetail,
  updateQuestion,
  deleteComment,
} from '../api/quiz';
import type { QuizDetail } from '../api/types';
import { useAuth } from '../auth/useAuth';
import { Button } from '../components/Button';
import { CommentSection } from '../components/CommentSection';
import { ErrorState } from '../components/ErrorState';
import { GroupBox } from '../components/GroupBox';
import { Loading } from '../components/Loading';
import { SectionHeading } from '../components/SectionHeading';
import { Window } from '../components/Window';
import { confirmWithSound, play } from '../sound/player';
import { AnswerList } from './AnswerList';
import { CategoryEditor } from './CategoryEditor';
import { DifficultyBadge } from './DifficultyBadge';
import { EditQuizForm } from './EditQuizForm';
import { QuestionForm } from './QuestionForm';

export function QuizDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { user, isAdmin } = useAuth();
  const navigate = useNavigate();

  const [detail, setDetail] = useState<QuizDetail | null>(null);
  const [error, setError] = useState('');
  const [deleting, setDeleting] = useState(false);
  const [editingCommentId, setEditingCommentId] = useState<string | null>(null);
  const [removingCommentId, setRemovingCommentId] = useState<string | null>(null);
  const [editingQuiz, setEditingQuiz] = useState(false);
  const [editingCategories, setEditingCategories] = useState(false);
  const [reloadKey, setReloadKey] = useState(0);
  const [addingQuestion, setAddingQuestion] = useState(false);
  const [editingQuestionId, setEditingQuestionId] = useState<string | null>(null);
  const [removingQuestionId, setRemovingQuestionId] = useState<string | null>(null);
  const [questionError, setQuestionError] = useState('');
  const [commentError, setCommentError] = useState('');
  const [revealedIds, setRevealedIds] = useState<Set<string>>(new Set());

  useEffect(() => {
    if (!id) return;
    let cancelled = false;
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

  function retry() {
    setError('');
    setDetail(null);
    setReloadKey((k) => k + 1);
  }

  async function handleDelete() {
    if (!id || !confirmWithSound('Delete this quiz? This cannot be undone.')) return;
    setDeleting(true);
    try {
      await deleteQuiz(id);
      play('recycle');
      navigate('/', { replace: true });
    } catch (err) {
      setError(err instanceof ApiError ? err.detail || err.title : 'Could not delete this quiz.');
      setDeleting(false);
    }
  }

  async function handleRemoveQuestion(questionId: string) {
    if (!id || !confirmWithSound('Remove this question?')) return;
    setQuestionError('');
    setRemovingQuestionId(questionId);
    try {
      await deleteQuestion(id, questionId);
      play('recycle');
      setDetail(
        (prev) => prev && { ...prev, questions: prev.questions.filter((q) => q.id !== questionId) },
      );
    } catch (err) {
      setQuestionError(
        err instanceof ApiError ? err.detail || err.title : 'Could not remove this question',
      );
    } finally {
      setRemovingQuestionId(null);
    }
  }

  function toggleAnswer(questionId: string) {
    if (!revealedIds.has(questionId)) play('reveal');
    setRevealedIds((prev) => {
      const next = new Set(prev);
      if (next.has(questionId)) next.delete(questionId);
      else next.add(questionId);
      return next;
    });
  }

  async function handleRemoveComment(commentId: string) {
    if (!id || !confirmWithSound('Remove this comment?')) return;
    setCommentError('');
    setRemovingCommentId(commentId);
    try {
      await deleteComment(id, commentId);
      play('recycle');
      setDetail(
        (prev) => prev && { ...prev, comments: prev.comments.filter((q) => q.id !== commentId) },
      );
    } catch (err) {
      setCommentError(
        err instanceof ApiError ? err.detail || err.title : 'Could not remove this comment',
      );
    } finally {
      setRemovingCommentId(null);
    }
  }

  if (error && !detail) {
    return (
      <div className="mx-auto w-full max-w-3xl">
        <Window title="Quiz" titleAs="div" icon="doc">
          <ErrorState message={error} onRetry={retry} />
        </Window>
      </div>
    );
  }

  if (!detail) {
    return (
      <div className="mx-auto w-full max-w-3xl">
        <Window title="Quiz" titleAs="div" icon="doc">
          <Loading label="Loading quiz…" />
        </Window>
      </div>
    );
  }

  const { quiz, questions } = detail;
  const canManage = user?.id === quiz.ownerId || isAdmin;

  const meta = (
    <div className="mt-3 flex flex-wrap items-center gap-x-2 gap-y-1 text-muted">
      <DifficultyBadge difficulty={quiz.difficulty} />
      {quiz.categories.length > 0 && <span>·</span>}
      {quiz.categories.map((category) => (
        <Link
          key={category.categoryId}
          to={`/?category=${category.slug}`}
          className="text-accent underline underline-offset-2"
        >
          {category.name}
        </Link>
      ))}
    </div>
  );

  return (
    <div className="mx-auto w-full max-w-3xl">
      <Window
        title={quiz.title}
        titleAs="div"
        icon="doc"
        // The toolbar stays put while the title is edited in place, so nothing below it moves;
        // its buttons are simply unavailable until the edit is saved or cancelled.
        toolbar={
          canManage && (
            <>
              <Button variant="utility" onClick={() => setEditingQuiz(true)} disabled={editingQuiz}>
                Edit
              </Button>

              <Button
                variant="utility"
                onClick={() => setEditingCategories(true)}
                disabled={editingQuiz || editingCategories}
              >
                Edit categories
              </Button>

              <span
                aria-hidden="true"
                className="mx-1 h-6 border-r border-l border-r-highlight border-l-dim"
              />

              <Button variant="utility" onClick={handleDelete} disabled={editingQuiz || deleting}>
                {deleting ? 'Deleting…' : 'Delete'}
              </Button>
            </>
          )
        }
      >
        <div className="flex flex-col gap-10">
          <div>
            {canManage && editingQuiz ? (
              <EditQuizForm
                quiz={quiz}
                onSaved={(updatedQuiz) => {
                  play('save');
                  setDetail((prev) => prev && { ...prev, quiz: updatedQuiz });
                  setEditingQuiz(false);
                }}
                onCancel={() => setEditingQuiz(false)}
              >
                {meta}
              </EditQuizForm>
            ) : (
              <>
                <h1 className="text-xl font-bold break-words">{quiz.title}</h1>
                {quiz.description && <p className="mt-2 text-muted">{quiz.description}</p>}
                {meta}
              </>
            )}

            {/* This can later be placed in the Edit page/ component */}
            {canManage && editingCategories && (
              <div className="mt-8">
                <CategoryEditor
                  quiz={quiz}
                  onSaved={(updated) => {
                    play('save');
                    setDetail((prev) => prev && { ...prev, quiz: updated });
                    setEditingCategories(false);
                  }}
                  onCancel={() => setEditingCategories(false)}
                />
              </div>
            )}

            {error && (
              <p role="alert" className="mt-3 text-danger">
                {error}
              </p>
            )}
          </div>

          <div className="flex flex-col gap-6">
            <SectionHeading>Questions</SectionHeading>
            {questionError && (
              <p role="alert" className="text-danger">
                {questionError}
              </p>
            )}
            {questions.length === 0 && <p className="text-muted">No questions yet.</p>}
            {questions.map((question, index) =>
              canManage && editingQuestionId === question.id ? (
                <QuestionForm
                  key={question.id}
                  question={question}
                  onSave={async (request) => {
                    const saved = await updateQuestion(quiz.id, question.id, request);
                    play('save');
                    setDetail(
                      (prev) =>
                        prev && {
                          ...prev,
                          questions: prev.questions.map((q) => (q.id === saved.id ? saved : q)),
                        },
                    );
                    setEditingQuestionId(null);
                  }}
                  onCancel={() => setEditingQuestionId(null)}
                />
              ) : (
                <GroupBox
                  key={question.id}
                  label={`Question ${index + 1}`}
                  className="flex flex-col gap-3"
                >
                  <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between sm:gap-4">
                    <p className="font-bold">{question.text}</p>
                    {canManage && (
                      <div className="flex shrink-0 gap-1">
                        <Button
                          variant="utility"
                          onClick={() => setEditingQuestionId(question.id)}
                          disabled={editingQuestionId !== null}
                        >
                          Edit
                        </Button>
                        <Button
                          variant="utility"
                          onClick={() => handleRemoveQuestion(question.id)}
                          disabled={removingQuestionId !== null}
                        >
                          {removingQuestionId === question.id ? 'Removing...' : 'Remove'}
                        </Button>
                      </div>
                    )}
                  </div>
                  <AnswerList
                    options={question.answerOptions}
                    revealed={revealedIds.has(question.id)}
                  />
                  <Button
                    variant="secondary"
                    className="self-start"
                    onClick={() => toggleAnswer(question.id)}
                  >
                    {revealedIds.has(question.id) ? 'Hide answer' : 'Show answer'}
                  </Button>
                </GroupBox>
              ),
            )}

            {canManage &&
              (addingQuestion ? (
                <QuestionForm
                  onSave={async (question) => {
                    const saved = await addQuestion(quiz.id, question);
                    play('save');
                    setDetail((prev) => prev && { ...prev, questions: [...prev.questions, saved] });
                    setAddingQuestion(false);
                  }}
                  onCancel={() => setAddingQuestion(false)}
                />
              ) : (
                <Button
                  variant="secondary"
                  className="self-start"
                  onClick={() => setAddingQuestion(true)}
                >
                  Add question
                </Button>
              ))}
          </div>

          <div className="flex flex-col gap-4">
            <CommentSection
              comments={detail.comments}
              quizId={quiz.id}
              quizOwnerId={quiz.ownerId}
              currentUserId={user?.id}
              isAdmin={isAdmin}
              canComment={!!user}
              removingCommentId={removingCommentId}
              editingCommentId={editingCommentId}
              onAdded={(comment) => {
                play('save');
                setDetail((prev) => prev && { ...prev, comments: [...prev.comments, comment] });
              }}
              onRemove={handleRemoveComment}
              onEdit={(commentId) => setEditingCommentId(commentId)}
              onCancelEdit={() => setEditingCommentId(null)}
              onSaved={(comment) => {
                play('save');
                setDetail(
                  (prev) =>
                    prev && {
                      ...prev,
                      comments: prev.comments.map((c) => (c.id === comment.id ? comment : c)),
                    },
                );
                setEditingCommentId(null);
              }}
            />
            {commentError && (
              <p role="alert" className="text-danger">
                {commentError}
              </p>
            )}
          </div>
        </div>
      </Window>
    </div>
  );
}
