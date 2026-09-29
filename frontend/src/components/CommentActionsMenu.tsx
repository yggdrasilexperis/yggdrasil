import { useEffect, useRef, useState } from 'react';

type Props = {
  canEdit: boolean;
  canRemove: boolean;
  removing: boolean;
  onEdit: () => void;
  onRemove: () => void;
};

export function CommentActionsMenu({ canEdit, canRemove, removing, onEdit, onRemove }: Props) {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) return;
    function handleClickOutside(event: MouseEvent) {
      if (ref.current && !ref.current.contains(event.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [open]);

  if (!canEdit && !canRemove) return null;

  return (
    <div ref={ref} className="relative">
      <button
        type="button"
        aria-label="Comment actions"
        aria-haspopup="true"
        aria-expanded={open}
        onClick={() => setOpen((prev) => !prev)}
        className="flex h-8 w-8 items-center justify-center rounded-control text-xl leading-none text-muted hover:bg-parchment"
      >
        ⋮
      </button>

      {open && (
        <div
          role="menu"
          className="absolute right-0 top-full z-10 mt-1 flex w-32 flex-col overflow-hidden rounded-control border border-hairline bg-white shadow-sm"
        >
          {canEdit && (
            <button
              type="button"
              role="menuitem"
              onClick={() => {
                setOpen(false);
                onEdit();
              }}
              className="px-4 py-2 text-left text-sm hover:bg-parchment"
            >
              Edit
            </button>
          )}
          {canRemove && (
            <button
              type="button"
              role="menuitem"
              disabled={removing}
              onClick={() => {
                setOpen(false);
                onRemove();
              }}
              className="px-4 py-2 text-left text-sm text-red-600 hover:bg-parchment disabled:opacity-50"
            >
              {removing ? 'Removing…' : 'Delete'}
            </button>
          )}
        </div>
      )}
    </div>
  );
}
