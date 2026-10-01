import type { Category } from '../api/types';
import { UNCATEGORIZED_SLUG } from '../api/types';
import { Select } from '../components/Select';

type Props = {
  categories: Category[];
  /** Slugs currently filtered on. */
  selected: string[];
  onChange: (slugs: string[]) => void;
  className?: string;
};

/**
 * Adds a category to the filter. The dropdown is a fixed size however many categories
 * exist; the ones already picked show in `ActiveFilters` instead.
 */
export function CategoryFilter({ categories, selected, onChange, className }: Props) {
  // Uncategorized is the backend's fallback, not something worth browsing by.
  const available = categories.filter(
    (category) => !selected.includes(category.slug) && category.slug !== UNCATEGORIZED_SLUG,
  );

  return (
    <Select
      aria-label="Filter by category"
      value=""
      onChange={(event) => onChange([...selected, event.target.value])}
      disabled={available.length === 0}
      className={className}
    >
      <option value="" disabled>
        {categories.length > 0 && available.length === 0
          ? 'No more categories'
          : 'Filter by category…'}
      </option>
      {available.map((category) => (
        <option key={category.categoryId} value={category.slug}>
          {category.name}
        </option>
      ))}
    </Select>
  );
}
