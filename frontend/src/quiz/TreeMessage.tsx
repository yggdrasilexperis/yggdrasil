import type { ReactNode } from 'react';

import { PixelIcon } from '../components/PixelIcon';

type Props = { title?: string; children: ReactNode };

/**
 * The world tree, drawn large over a line or two. It fills the quiz list where rows are
 * missing, drawn tone on tone in the window's greys, a step lighter than the words under
 * it, so it sits in the pane like a watermark and the words stay the message.
 */
export function TreeMessage({ title, children }: Props) {
  return (
    <div className="flex flex-col items-center gap-3 text-center">
      <PixelIcon name="worldTree" scale={4} />
      <p>
        {title && <span className="block font-bold">{title}</span>}
        <span className="text-muted">{children}</span>
      </p>
    </div>
  );
}
