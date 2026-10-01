import { useEffect, useRef, useState } from 'react';

import { play } from '../sound/player';
import { PixelIcon } from './PixelIcon';

type Props = {
  canEdit: boolean;
  canRemove: boolean;
  removing: boolean;
  onEdit: () => void;
  onRemove: () => void;
};

/** The highlight bar is the focus indicator here, so the dotted ring steps aside. */
const menuItem =
  'py-0.5 pr-6 pl-7 text-left hover:bg-accent hover:text-white ' +
  'focus-visible:bg-accent focus-visible:text-white focus-visible:outline-none ' +
  'disabled:text-dim disabled:hover:bg-transparent';

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
        onClick={() => {
          if (!open) play('menu');
          setOpen((prev) => !prev);
        }}
        className="flex size-7 items-center justify-center bg-face shadow-raised active:shadow-pressed aria-expanded:shadow-pressed"
      >
        <PixelIcon name="dots" />
      </button>

      {open && (
        <div
          role="menu"
          className="absolute top-full right-0 z-10 mt-0.5 flex min-w-40 flex-col bg-face p-1 shadow-window"
        >
          {canEdit && (
            <button
              type="button"
              role="menuitem"
              onClick={() => {
                setOpen(false);
                onEdit();
              }}
              className={menuItem}
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
              className={`${menuItem} text-danger`}
            >
              {removing ? 'Removing…' : 'Delete'}
            </button>
          )}
        </div>
      )}
    </div>
  );
}
