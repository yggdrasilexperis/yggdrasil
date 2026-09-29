import { useNavigate } from 'react-router-dom';

import { CreateQuizForm } from './CreateQuizForm';

export function CreateQuizPage() {
  const navigate = useNavigate();

  return (
    <div className="mx-auto w-full max-w-3xl px-6 py-12">
      <h1 className="text-4xl font-semibold ">Create a quiz</h1>
      <CreateQuizForm onCreated={(quiz) => navigate(`/quizzes/${quiz.id}`, { replace: true })} />
    </div>
  );
}
