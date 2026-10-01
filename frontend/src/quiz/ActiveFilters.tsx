import type { Category } from '../api/types';
import { Button } from '../components/Button';
import { CategoryChip } from './CategoryChip';

type Props = {
  categories: Category[];
  /** Slugs currently filtered on. */
  selected: string[];
  onChange: (slugs: string[]) => void;
  className?: string;
};

/**
 * The categories being filtered on, one line of chips that scrolls sideways rather than
 * wrapping, so picking one never makes the toolbar taller. Clear all stays put and greys
 * out when there is nothing to clear, as 98 did with commands that didn't apply.
 */
export function ActiveFilters({ categories, selected, onChange, className = '' }: Props) {
  // Resolved against every category, so a quiz already tagged Uncategorized still shows a chip.
  const active = selected
    .map((slug) => categories.find((category) => category.slug === slug))
    .filter((category) => category !== undefined);

  return (
    <div className={`flex min-h-9 min-w-0 items-center gap-1 ${className}`.trim()}>
      {selected.length > 0 ? (
        <ul
          aria-label="Active filters"
          className="flex min-w-0 flex-1 gap-1 overflow-x-auto [scrollbar-width:none]"
        >
          {active.map((category) => (
            <CategoryChip
              key={category.categoryId}
              name={category.name}
              removeLabel={`Remove ${category.name} filter`}
              onRemove={() => onChange(selected.filter((slug) => slug !== category.slug))}
            />
          ))}
        </ul>
      ) : (
        <span className="min-w-0 flex-1 truncate px-1 text-muted">All categories</span>
      )}
      <Button variant="utility" onClick={() => onChange([])} disabled={selected.length === 0}>
        Clear all
      </Button>
    </div>
  );
}
