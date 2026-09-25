import { useEffect, useState } from 'react';

import { ApiError } from '../api/ApiError';
import { updateQuiz } from '../api/quiz';
import { getCategories } from '../api/quizzes';
import type { Category, QuizSummary } from '../api/types';
import { Button } from '../components/Button';
import { Card } from '../components/Card';
import { CategoryPicker } from './CategoryPicker';

type Props = {
  quiz: QuizSummary;
  onSaved: (quiz: QuizSummary) => void;
  onCancel: () => void;
};

/**
 * Re-tag an existing quiz. PUT replaces the whole quiz, so the fields this editor
 * does not touch are sent back exactly as they came.
 */
export function CategoryEditor({ quiz, onSaved, onCancel }: Props) {
  const [categories, setCategories] = useState<Category[] | null>(null);
  const [categoryIds, setCategoryIds] = useState(() => quiz.categories.map((c) => c.categoryId));
  const [error, setError] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    let cancelled = false;
    getCategories()
      .then((data) => {
        if (!cancelled) setCategories(data);
      })
      .catch(() => {
        if (!cancelled) setError('Could not load categories. Try again.');
      });
    return () => {
      cancelled = true;
    };
  }, []);

  async function handleSave() {
    setError('');
    setSaving(true);
    try {
      const updated = await updateQuiz(quiz.id, {
        title: quiz.title,
        description: quiz.description,
        difficulty: quiz.difficulty,
        categoryIds,
      });
      onSaved(updated);
    } catch (err) {
      setError(
        err instanceof ApiError
          ? (err.fieldError('categoryIds') ?? (err.detail || err.title))
          : 'Could not save categories.',
      );
      setSaving(false);
    }
  }

  return (
    <Card className="flex flex-col gap-4">
      <CategoryPicker
        categories={categories}
        selectedIds={categoryIds}
        onChange={setCategoryIds}
        error={error}
      />
      <div className="flex gap-3">
        <Button onClick={handleSave} disabled={saving}>
          {saving ? 'Saving…' : 'Save'}
        </Button>
        <Button variant="secondary" onClick={onCancel} disabled={saving}>
          Cancel
        </Button>
      </div>
    </Card>
  );
}
