import { useState } from 'react';

import type { Quiz } from '../api/types';
import { Button } from '../components/Button';
import { CreateQuizForm } from './CreateQuizForm';

export function CreateQuizPage() {
  const [created, setCreated] = useState<Quiz | null>(null);

  return (
    <div className="mx-auto w-full max-w-3xl px-6 py-12">
      <h1 className="text-4xl font-semibold ">Create a quiz</h1>
      {created ? (
        <div className="mt-6 rounded-card border border-green-600 bg-white p-6">
          <p className="font-semibold">Quiz created</p>
          <p className="mt-2 text-muted">“{created.title}” has been saved.</p>
          <Button className="mt-6" onClick={() => setCreated(null)}>
            Create another
          </Button>
        </div>
      ) : (
        <CreateQuizForm onCreated={setCreated} />
      )}
    </div>
  );
}
