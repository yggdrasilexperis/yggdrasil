import type { ReactNode } from 'react';

import { PixelIcon } from './PixelIcon';
import type { PixelIconName } from './pixelArt';

type Props = {
  title: string;
  /**
   * Title bar icon. A window without one is a dialog, and like a 98 dialog it gets only a
   * close box instead of minimise, maximise and close.
   */
  icon?: PixelIconName;
  /**
   * The title bar holds the page's h1. A page that puts its own heading in the body, where it
   * can be edited in place, passes `div` so the title bar only labels the window.
   */
  titleAs?: 'h1' | 'div';
  /** A strip of controls between the title bar and the body. */
  toolbar?: ReactNode;
  /** The status bar under the body. It ends in the resize grip. */
  status?: ReactNode;
  /** Let the body's content run to the frame, as Explorer's file pane does. */
  flush?: boolean;
  className?: string;
  children: ReactNode;
};

const control = 'flex h-5.5 w-6 items-center justify-center bg-face text-ink shadow-raised';

/** Every page is a window on the desktop. */
export function Window({
  title,
  icon,
  titleAs: Title = 'h1',
  toolbar,
  status,
  flush = false,
  className = '',
  children,
}: Props) {
  return (
    <div
      className={`flex flex-col bg-face p-1 shadow-window motion-safe:animate-window-open ${className}`.trim()}
    >
      <div className="flex h-8 items-center gap-1.5 bg-linear-to-r from-accent to-accent-end pr-1 pl-1.5 text-white select-none">
        {icon && <PixelIcon name={icon} />}
        <Title className="min-w-0 flex-1 truncate leading-none font-bold">{title}</Title>
        {/* Decorative, like the rest of the frame: nothing here minimises or closes. */}
        <div aria-hidden="true" className="flex">
          {icon && (
            <>
              <span className={control}>
                <PixelIcon name="minimize" />
              </span>
              <span className={control}>
                <PixelIcon name="maximize" />
              </span>
            </>
          )}
          <span className={`${control} ml-0.5`}>
            <PixelIcon name="close" />
          </span>
        </div>
      </div>
      {toolbar && (
        <div className="mt-0.5 flex flex-wrap items-center gap-1 p-1 shadow-panel">{toolbar}</div>
      )}
      <div className={`flex flex-1 flex-col ${flush ? 'pt-0.5' : 'p-3 sm:p-5'}`}>{children}</div>
      {status && (
        <div className="mt-0.5 flex items-end gap-0.5 px-0.5 pb-0.5">
          <div className="min-w-0 flex-1">{status}</div>
          <PixelIcon name="grip" className="mb-0.5" />
        </div>
      )}
    </div>
  );
}
