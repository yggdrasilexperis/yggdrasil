import { PixelIcon } from '../components/PixelIcon';

type Props = {
  label: string;
  /** Pass to make the column sortable: clicking it sorts, or reverses the sort. */
  onClick?: () => void;
  /** Which way this column is sorted now, if it is the one sorted on. */
  direction?: 'ascending' | 'descending' | null;
};

const cell = 'flex h-8 items-center gap-2 bg-face px-2 text-left shadow-raised select-none';

/** A list-view column header: a raised button that shows the sort arrow when it leads. */
export function ColumnHeader({ label, onClick, direction = null }: Props) {
  if (!onClick) return <span className={cell}>{label}</span>;

  return (
    <button
      type="button"
      onClick={onClick}
      className={`${cell} active:shadow-pressed focus-visible:-outline-offset-5`}
    >
      {label}
      {direction && (
        <>
          <PixelIcon name={direction === 'ascending' ? 'arrowUp' : 'arrowDown'} />
          <span className="sr-only">(sorted {direction})</span>
        </>
      )}
    </button>
  );
}
