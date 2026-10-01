import { useNavigate } from 'react-router-dom';

import { Window } from '../components/Window';
import { play } from '../sound/player';
import { CreateQuizForm } from './CreateQuizForm';

export function CreateQuizPage() {
  const navigate = useNavigate();

  return (
    <div className="mx-auto w-full max-w-3xl">
      <Window title="Create a quiz" icon="doc">
        <CreateQuizForm
          onCreated={(quiz) => {
            play('success');
            navigate(`/quizzes/${quiz.id}`, { replace: true });
          }}
        />
      </Window>
    </div>
  );
}
