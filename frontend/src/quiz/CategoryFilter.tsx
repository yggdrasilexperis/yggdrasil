import type { Category } from '../api/types';
import { UNCATEGORIZED_SLUG } from '../api/types';
import { Button } from '../components/Button';
import { Select } from '../components/Select';
import { CategoryChip } from './CategoryChip';

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
    <div className="flex flex-col gap-1">
      <div className="flex items-center gap-1">
        <Select
          aria-label="Filter by category"
          value=""
          onChange={(event) => onChange([...selected, event.target.value])}
          disabled={available.length === 0}
        >
          <option value="" disabled>
            {available.length === 0 ? 'No more categories' : 'Filter by category…'}
          </option>
          {available.map((category) => (
            <option key={category.categoryId} value={category.slug}>
              {category.name}
            </option>
          ))}
        </Select>

        {selected.length > 0 && (
          <Button variant="utility" onClick={() => onChange([])} className="min-h-9">
            Clear all
          </Button>
        )}
      </div>

      {active.length > 0 && (
        <ul aria-label="Active filters" className="flex flex-wrap gap-1">
          {active.map((category) => (
            <CategoryChip
              key={category.categoryId}
              name={category.name}
              removeLabel={`Remove ${category.name} filter`}
              onRemove={() => onChange(selected.filter((slug) => slug !== category.slug))}
            />
          ))}
        </ul>
      )}
    </div>
  );
}
