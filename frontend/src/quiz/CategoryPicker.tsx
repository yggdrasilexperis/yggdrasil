import { useId, useState } from 'react';

import type { Category } from '../api/types';
import { UNCATEGORIZED_SLUG } from '../api/types';
import { Button } from '../components/Button';
import { Select } from '../components/Select';
import { CategoryChip } from './CategoryChip';

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
  // Uncategorized is assigned automatically when nothing is picked, so it is not on offer.
  const available = (categories ?? []).filter(
    (c) => !selectedIds.includes(c.categoryId) && c.slug !== UNCATEGORIZED_SLUG,
  );

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
      <label htmlFor={selectId} className="mb-1 block">
        Categories
      </label>
      <div className="flex gap-1.5">
        <Select
          id={selectId}
          value={pendingId}
          onChange={(event) => setPendingId(event.target.value)}
          disabled={available.length === 0}
          aria-invalid={error ? true : undefined}
          className="flex-1"
        >
          <option value="" disabled>
            {placeholder}
          </option>
          {available.map((category) => (
            <option key={category.categoryId} value={category.categoryId}>
              {category.name}
            </option>
          ))}
        </Select>
        <Button variant="secondary" onClick={add} disabled={!pendingId}>
          Add
        </Button>
      </div>

      {selected.length > 0 && (
        <ul aria-label="Selected categories" className="mt-2 flex flex-wrap gap-1">
          {selected.map((category) => (
            <CategoryChip
              key={category.categoryId}
              name={category.name}
              removeLabel={`Remove ${category.name}`}
              onRemove={() => onChange(selectedIds.filter((id) => id !== category.categoryId))}
            />
          ))}
        </ul>
      )}

      {error && <p className="mt-1.5 text-danger">{error}</p>}
    </div>
  );
}
