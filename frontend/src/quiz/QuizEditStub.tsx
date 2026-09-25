import { Link, useParams } from 'react-router-dom';

/** Placeholder — the real editor is a separate issue. */
export function QuizEditStub() {
  const { id } = useParams<{ id: string }>();

  return (
    <div className="mx-auto flex w-full max-w-3xl flex-1 flex-col gap-4 px-6 py-12">
      <h1 className="font-display text-4xl font-semibold tracking-tight">Editing coming soon</h1>
      <p className="text-muted">Quiz editing isn&apos;t built yet.</p>
      <Link to={`/quizzes/${id}`} className="text-accent">
        Back to quiz
      </Link>
    </div>
  );
}
