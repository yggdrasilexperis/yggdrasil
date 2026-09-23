import { useId } from 'react';

import type { Category } from '../api/types';
import { Button } from '../components/Button';

type Props = {
  categories: Category[];
  /** Slugs currently filtered on. */
  selected: string[];
  onChange: (slugs: string[]) => void;
};

/**
 * Filter by one or more categories. A dropdown rather than a row of chips, so the control
 * stays the same size however many categories exist; the active ones show as chips.
 */
export function CategoryFilter({ categories, selected, onChange }: Props) {
  const selectId = useId();

  const available = categories.filter((category) => !selected.includes(category.slug));
  const active = selected
    .map((slug) => categories.find((category) => category.slug === slug))
    .filter((category) => category !== undefined);

  return (
    <div className="flex flex-wrap items-center gap-2">
      <label htmlFor={selectId} className="text-sm text-muted">
        Filter
      </label>
      <select
        id={selectId}
        value=""
        onChange={(event) => onChange([...selected, event.target.value])}
        disabled={available.length === 0}
        className="h-11 rounded-control border border-hairline bg-white px-4 disabled:text-muted"
      >
        <option value="" disabled>
          {available.length === 0 ? 'No more categories' : 'Add a category...'}
        </option>
        {available.map((category) => (
          <option key={category.categoryId} value={category.slug}>
            {category.name}
          </option>
        ))}
      </select>

      {active.map((category) => (
        <span
          key={category.categoryId}
          className="flex items-center rounded-control border border-accent pl-4 text-accent"
        >
          {category.name}
          <button
            type="button"
            onClick={() => onChange(selected.filter((slug) => slug !== category.slug))}
            aria-label={`Remove ${category.name} filter`}
            className="flex h-11 w-11 items-center justify-center transition-transform active:scale-95"
          >
            x
          </button>
        </span>
      ))}

      {selected.length > 0 && (
        <Button variant="secondary" onClick={() => onChange([])}>
          Clear
        </Button>
      )}
    </div>
  );
}
