import type { Category } from '../api/types';
import { UNCATEGORIZED_SLUG } from '../api/types';

type Props = {
  categories: Category[];
  /** Slugs currently filtered on. */
  selected: string[];
  onChange: (slugs: string[]) => void;
};

/**
 * Filter by one or more categories. The dropdown is a fixed size however many categories
 * exist, and the active ones sit on their own row so picking one never moves it.
 */
export function CategoryFilter({ categories, selected, onChange }: Props) {
  // Uncategorized is the backend's fallback, not something worth browsing by.
  const available = categories.filter(
    (category) => !selected.includes(category.slug) && category.slug !== UNCATEGORIZED_SLUG,
  );

  // Resolved against every category, so a quiz already tagged Uncategorized still shows a chip.
  const active = selected
    .map((slug) => categories.find((category) => category.slug === slug))
    .filter((category) => category !== undefined);

  return (
    <div className="flex flex-col gap-3">
      <div className="flex items-center gap-4">
        <select
          aria-label="Filter by category"
          value=""
          onChange={(event) => onChange([...selected, event.target.value])}
          disabled={available.length === 0}
          className="h-11 rounded-control border border-hairline bg-white px-4 disabled:text-muted"
        >
          <option value="" disabled>
            {available.length === 0 ? 'No more categories' : 'Filter by category…'}
          </option>
          {available.map((category) => (
            <option key={category.categoryId} value={category.slug}>
              {category.name}
            </option>
          ))}
        </select>

        {selected.length > 0 && (
          <button
            type="button"
            onClick={() => onChange([])}
            className="text-sm text-accent transition-transform active:scale-95"
          >
            Clear all
          </button>
        )}
      </div>

      {active.length > 0 && (
        <ul aria-label="Active filters" className="flex flex-wrap gap-2">
          {active.map((category) => (
            <li
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
                ×
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
