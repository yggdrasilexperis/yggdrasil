import { useEffect, useId, useState } from 'react';
import type { SubmitEvent } from 'react';

import { ApiError } from '../api/ApiError';
import { createQuiz, getCategories } from '../api/quizzes';
import type { Category, Difficulty, Quiz } from '../api/types';
import { Button } from '../components/Button';
import { Input } from '../components/Input';

type FieldErrors = {
  title?: string;
  description?: string;
  difficulty?: string;
  categoryIds?: string;
};

const DIFFICULTIES = ['Easy', 'Normal', 'Hard', 'Expert'];

function validate(
  title: string,
  description: string,
  difficulty: Difficulty | null,
  categoryIds: string[],
): FieldErrors {
  const errors: FieldErrors = {};

  if (!title.trim()) errors.title = 'Title is required';
  else if (title.trim().length > 200) errors.title = 'Title must be 200 characters or fewer';

  if (description.trim().length > 2000)
    errors.description = 'Description must be 2000 characters or fewer';

  if (difficulty === null) errors.difficulty = 'Pick a difficulty';
  if (categoryIds.length === 0) errors.categoryIds = 'Pick at least one category';

  return errors;
}

export function CreateQuizForm({ onCreated }: { onCreated: (quiz: Quiz) => void }) {
  const categoriesId = useId();
  const descriptionId = useId();
  const difficultyId = useId();

  const [categories, setCategories] = useState<Category[] | null>(null);
  const [pendingCategoryId, setPendingCategoryId] = useState('');
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [difficulty, setDifficulty] = useState<Difficulty | null>(null);
  const [categoryIds, setCategoryIds] = useState<string[]>([]);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    getCategories()
      .then(setCategories)
      .catch(() => {
        setCategories([]);
        setFormError('Could not load categories. Refresh to try again.');
      });
  }, []);

  function addCategory() {
    if (!pendingCategoryId) return;
    setCategoryIds((ids) => [...ids, pendingCategoryId]);
    setPendingCategoryId('');
  }

  function removeCategory(id: string) {
    setCategoryIds((ids) => ids.filter((x) => x !== id));
  }

  function toggleCategory(id: string) {
    setCategoryIds((ids) => (ids.includes(id) ? ids.filter((x) => x !== id) : [...ids, id]));
  }

  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError('');

    const clientErrors = validate(title, description, difficulty, categoryIds);
    setFieldErrors(clientErrors);
    if (difficulty === null || Object.keys(clientErrors).length > 0) return;

    setSubmitting(true);
    try {
      const quiz = await createQuiz({
        title: title.trim(),
        description: description.trim(),
        difficulty,
        categoryIds,
      });
      onCreated(quiz);
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
    } finally {
      setSubmitting(false);
    }
  }

  const field = (error?: string) =>
    `w-full rounded-control border px-4 ${error ? 'border-red-600' : 'border-hairline'}`;

  const selectedCategories = (categories ?? []).filter((c) => categoryIds.includes(c.categoryId));
  const availableCategories = (categories ?? []).filter((c) => !categoryIds.includes(c.categoryId));

  const categoryPlaceholder =
    categories === null
      ? 'Loading…'
      : availableCategories.length === 0
        ? 'No more categories'
        : 'Choose a category...';

  return (
    <form onSubmit={handleSubmit} noValidate className="mt-6 flex flex-col gap-6">
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
          className={`${field(fieldErrors.difficulty)} h-11 bg-white`}
        >
          <option value="" disabled>
            Choose…
          </option>
          {DIFFICULTIES.map((label, value) => (
            <option key={label} value={value}>
              {label}
            </option>
          ))}
        </select>
        {fieldErrors.difficulty && (
          <p className="mt-2 text-sm text-red-600">{fieldErrors.difficulty}</p>
        )}
      </div>

      <div>
        <label htmlFor={categoriesId} className="mb-2 block text-sm">
          Categories
        </label>
        <div className="flex gap-2">
          <select
            id={categoriesId}
            value={pendingCategoryId}
            onChange={(event) => setPendingCategoryId(event.target.value)}
            disabled={availableCategories.length === 0}
            aria-invalid={fieldErrors.categoryIds ? true : undefined}
            className={`${field(fieldErrors.categoryIds)} h-11 bg-white disabled:text-muted`}
          >
            <option value="" disabled>
              {categoryPlaceholder}
            </option>
            {availableCategories.map((category) => (
              <option key={category.categoryId} value={category.categoryId}>
                {category.name}
              </option>
            ))}
          </select>
          <Button variant="secondary" onClick={addCategory} disabled={!pendingCategoryId}>
            Add
          </Button>
        </div>

        {selectedCategories.length > 0 && (
          <ul aria-label="Selected categories" className="mt-3 flex flex-wrap gap-2">
            {selectedCategories.map((category) => (
              <li
                key={category.categoryId}
                className="flex items-center rounded-control border border-hairline pl-4"
              >
                {category.name}
                <button
                  type="button"
                  onClick={() => removeCategory(category.categoryId)}
                  aria-label={`Remove ${category.name}`}
                  className="flex h-11 w-11 items-center justify-center text-muted transition-transform active:scale-95"
                >
                  x
                </button>
              </li>
            ))}
          </ul>
        )}

        {fieldErrors.categoryIds && (
          <p className="mt-2 text-sm text-red-600">{fieldErrors.categoryIds}</p>
        )}
      </div>

      <Button type="submit" className="self-start" disabled={submitting}>
        {submitting ? 'Saving…' : 'Create quiz'}
      </Button>
    </form>
  );
}
