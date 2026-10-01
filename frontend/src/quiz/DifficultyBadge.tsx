import { DIFFICULTY_LABELS } from '../api/types';
import type { Difficulty } from '../api/types';

const BAR_HEIGHTS = ['h-1.5', 'h-2.25', 'h-3', 'h-3.75'];

/** The label, led by a signal-strength meter that fills one bar per step of difficulty. */
export function DifficultyBadge({ difficulty }: { difficulty: Difficulty }) {
  return (
    <span className="inline-flex items-center gap-1.5">
      <span aria-hidden="true" className="flex h-3.75 items-end gap-0.5">
        {BAR_HEIGHTS.map((height, level) => (
          <span
            key={height}
            className={`w-1.5 ${height} ${
              level <= difficulty
                ? 'bg-accent group-hover:bg-white group-focus-visible:bg-white'
                : 'bg-dim/40 group-hover:bg-white/35 group-focus-visible:bg-white/35'
            }`}
          />
        ))}
      </span>
      {DIFFICULTY_LABELS[difficulty]}
    </span>
  );
}
