import { PixelIcon } from '../components/PixelIcon';

type Props = { name: string; removeLabel: string; onRemove: () => void };

/** A chosen category, in the selection highlight, with a close box to let it go. */
export function CategoryChip({ name, removeLabel, onRemove }: Props) {
  return (
    <li className="flex h-7 items-center gap-1 bg-accent pr-0.5 pl-2 text-white">
      {name}
      <button
        type="button"
        onClick={onRemove}
        aria-label={removeLabel}
        className="flex size-6 items-center justify-center hover:bg-white/15 focus-visible:outline-white active:translate-px"
      >
        <PixelIcon name="close" />
      </button>
    </li>
  );
}
