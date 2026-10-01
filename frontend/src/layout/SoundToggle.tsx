import { useState } from 'react';

import { PixelIcon } from '../components/PixelIcon';
import { isMuted, play, setMuted } from '../sound/player';

/** The volume icon in the taskbar tray: sound on or off, remembered in this browser. */
export function SoundToggle() {
  const [muted, setMutedState] = useState(isMuted);

  function toggle() {
    const next = !muted;
    setMuted(next);
    setMutedState(next);
    if (!next) play('exclamation');
  }

  return (
    <button
      type="button"
      onClick={toggle}
      aria-pressed={!muted}
      aria-label="Sound"
      title={muted ? 'Sound is off' : 'Sound is on'}
      className="flex size-7 shrink-0 items-center justify-center focus-visible:-outline-offset-2"
    >
      <PixelIcon name={muted ? 'speakerOff' : 'speaker'} />
    </button>
  );
}
