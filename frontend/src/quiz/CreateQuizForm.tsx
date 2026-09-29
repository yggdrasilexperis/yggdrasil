import { useEffect, useId, useState } from 'react';
import type { SubmitEvent } from 'react';

import { ApiError } from '../api/ApiError';
import { addQuestion } from '../api/quiz';
import { createQuiz, getCategories } from '../api/quizzes';
import { DIFFICULTY_LABELS } from '../api/types';
import type { Category, CreateQuestionRequest, Difficulty, QuizSummary } from '../api/types';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { CategoryPicker } from './CategoryPicker';
import { DraftQuestions } from './DraftQuestions';
import { UnsavedQuestionsNotice } from './UnsavedQuestionsNotice';
import type { FailedQuestion } from './UnsavedQuestionsNotice';

type FieldErrors = {
  title?: string;
  description?: string;
  difficulty?: string;
  categoryIds?: string;
};

type PartialFailure = { quiz: QuizSummary; failed: FailedQuestion[] };

const QUIZ_FORM_ID = 'create-quiz-form';

function validate(title: string, description: string, difficulty: Difficulty | null): FieldErrors {
  const errors: FieldErrors = {};

  if (!title.trim()) errors.title = 'Title is required';
  else if (title.trim().length > 200) errors.title = 'Title must be 200 characters or fewer';

  if (description.trim().length > 2000)
    errors.description = 'Description must be 2000 characters or fewer';

  if (difficulty === null) errors.difficulty = 'Pick a difficulty';

  return errors;
}

function questionErrorMessage(error: unknown): string {
  if (!(error instanceof ApiError)) return 'Could not save this question.';

  if (error.status === 400) {
    const firstFieldMessage = Object.values(error.fieldErrors)[0]?.[0];
    if (firstFieldMessage) return firstFieldMessage;
  }

  return error.detail || error.title;
}

async function saveQuestions(
  quizId: string,
  questions: CreateQuestionRequest[],
  onProgress: (position: number) => void,
): Promise<FailedQuestion[]> {
  const failed: FailedQuestion[] = [];

  for (const [index, question] of questions.entries()) {
    const position = index + 1;
    onProgress(position);
    try {
      await addQuestion(quizId, question);
    } catch (error) {
      failed.push({ position, text: question.text, message: questionErrorMessage(error) });
    }
  }

  return failed;
}

export function CreateQuizForm({ onCreated }: { onCreated: (quiz: QuizSummary) => void }) {
  const descriptionId = useId();
  const difficultyId = useId();

  const [categories, setCategories] = useState<Category[] | null>(null);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [difficulty, setDifficulty] = useState<Difficulty | null>(null);
  const [categoryIds, setCategoryIds] = useState<string[]>([]);
  const [questions, setQuestions] = useState<CreateQuestionRequest[]>([]);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [savingQuestion, setSavingQuestion] = useState<number | null>(null);
  const [partialFailure, setPartialFailure] = useState<PartialFailure | null>(null);

  useEffect(() => {
    getCategories()
      .then(setCategories)
      .catch(() => {
        setCategories([]);
        setFormError('Could not load categories. Refresh to try again.');
      });
  }, []);

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError('');

    const clientErrors = validate(title, description, difficulty);
    setFieldErrors(clientErrors);
    if (difficulty === null || Object.keys(clientErrors).length > 0) return;

    setSubmitting(true);

    let quiz: QuizSummary;
    try {
      quiz = await createQuiz({
        title: title.trim(),
        description: description.trim(),
        difficulty,
        categoryIds,
      });
    } catch (error) {
      if (error instanceof ApiError && error.status === 400) {
        setFieldErrors({
          title: error.fieldError('title'),
          description: error.fieldError('description'),
          difficulty: error.fieldError('difficulty'),
          categoryIds: error.fieldError('categoryIds'),
        });
        setFormError('The quiz was not saved. Fix the fields below.');
      } else if (error instanceof ApiError) {
        setFormError(error.detail || error.title);
      } else {
        setFormError('Something went wrong. The quiz was not saved.');
      }
      setSubmitting(false);
      return;
    }

    const failed = await saveQuestions(quiz.id, questions, setSavingQuestion);

    if (failed.length > 0) setPartialFailure({ quiz, failed });
    else onCreated(quiz);
  }

  const field = (error?: string) =>
    `w-full rounded-control border px-4 ${error ? 'border-red-600' : 'border-hairline'}`;

  if (partialFailure) {
    return <UnsavedQuestionsNotice quiz={partialFailure.quiz} failed={partialFailure.failed} />;
  }

  let submitLabel = 'Create quiz';
  if (savingQuestion !== null) {
    submitLabel = `Saving question ${savingQuestion} of ${questions.length}…`;
  } else if (submitting) {
    submitLabel = 'Saving…';
  }

  return (
    <div className="mt-6 flex flex-col gap-8">
      <form onSubmit={handleSubmit} noValidate id={QUIZ_FORM_ID} className="flex flex-col gap-6">
        {formError && (
          <p role="alert" className="text-sm text-red-600">
            {formError}
          </p>
        )}

        <Input
          label="Title"
          value={title}
          onChange={(event) => setTitle(event.target.value)}
          error={fieldErrors.title}
        />

        <div>
          <label htmlFor={descriptionId} className="mb-2 block text-sm">
            Description <span className="text-muted">(optional)</span>
          </label>
          <textarea
            id={descriptionId}
            rows={4}
            value={description}
            onChange={(event) => setDescription(event.target.value)}
            aria-invalid={fieldErrors.description ? true : undefined}
            className={`${field(fieldErrors.description)} py-4`}
          />
          {fieldErrors.description && (
            <p className="mt-2 text-sm text-red-600">{fieldErrors.description}</p>
          )}
        </div>

        <div>
          <label htmlFor={difficultyId} className="mb-2 block text-sm">
            Difficulty
          </label>
          <select
            id={difficultyId}
            value={difficulty ?? ''}
            onChange={(event) => setDifficulty(Number(event.target.value) as Difficulty)}
            aria-invalid={fieldErrors.difficulty ? true : undefined}
            className={`${field(fieldErrors.difficulty)} h-12 bg-white`}
          >
            <option value="" disabled>
              Choose…
            </option>
            {Object.entries(DIFFICULTY_LABELS).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
          {fieldErrors.difficulty && (
            <p className="mt-2 text-sm text-red-600">{fieldErrors.difficulty}</p>
          )}
        </div>

        <CategoryPicker
          categories={categories}
          selectedIds={categoryIds}
          onChange={setCategoryIds}
          error={fieldErrors.categoryIds}
        />
      </form>

      <DraftQuestions drafts={questions} onChange={setQuestions} disabled={submitting} />

      <Button type="submit" form={QUIZ_FORM_ID} className="self-start" disabled={submitting}>
        {submitLabel}
      </Button>
    </div>
  );
}
