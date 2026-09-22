import { useId, useState } from 'react';

import type { Category } from '../api/types';
import { Button } from '../components/Button';

type Props = {
  /** `null` while the list is still loading. */
  categories: Category[] | null;
  selectedIds: string[];
  onChange: (ids: string[]) => void;
  error?: string;
};

/** Attach categories from the backend's list with a select, detach them from the chips. */
export function CategoryPicker({ categories, selectedIds, onChange, error }: Props) {
  const selectId = useId();
  const [pendingId, setPendingId] = useState('');

  const selected = (categories ?? []).filter((c) => selectedIds.includes(c.categoryId));
  const available = (categories ?? []).filter((c) => !selectedIds.includes(c.categoryId));

  const placeholder =
    categories === null
      ? 'Loading…'
      : available.length === 0
        ? 'No more categories'
        : 'Choose a category...';

  function add() {
    if (!pendingId) return;
    onChange([...selectedIds, pendingId]);
    setPendingId('');
  }

  return (
    <div>
      <label htmlFor={selectId} className="mb-2 block text-sm">
        Categories
      </label>
      <div className="flex gap-2">
        <select
          id={selectId}
          value={pendingId}
          onChange={(event) => setPendingId(event.target.value)}
          disabled={available.length === 0}
          aria-invalid={error ? true : undefined}
          className={`h-11 w-full rounded-control border bg-white px-4 disabled:text-muted ${
            error ? 'border-red-600' : 'border-hairline'
          }`}
        >
          <option value="" disabled>
            {placeholder}
          </option>
          {available.map((category) => (
            <option key={category.categoryId} value={category.categoryId}>
              {category.name}
            </option>
          ))}
        </select>
        <Button variant="secondary" onClick={add} disabled={!pendingId}>
          Add
        </Button>
      </div>

      {selected.length > 0 && (
        <ul aria-label="Selected categories" className="mt-3 flex flex-wrap gap-2">
          {selected.map((category) => (
            <li
              key={category.categoryId}
              className="flex items-center rounded-control border border-hairline pl-4"
            >
              {category.name}
              <button
                type="button"
                onClick={() => onChange(selectedIds.filter((id) => id !== category.categoryId))}
                aria-label={`Remove ${category.name}`}
                className="flex h-11 w-11 items-center justify-center text-muted transition-transform active:scale-95"
              >
                x
              </button>
            </li>
          ))}
        </ul>
      )}

      {error && <p className="mt-2 text-sm text-red-600">{error}</p>}
    </div>
  );
}
