import { SYNTHS } from './synths';

export type SoundName = keyof typeof SYNTHS;

const MUTED_KEY = 'yggdrasil.muted';

let context: AudioContext | null = null;
let master: AudioNode | null = null;
const lastPlayed = new Map<SoundName, number>();

/** Muting is a per-browser choice; storage can throw (private mode), so every access is guarded. */
export function isMuted(): boolean {
  try {
    return localStorage.getItem(MUTED_KEY) === 'true';
  } catch {
    return false;
  }
}

export function setMuted(muted: boolean): void {
  try {
    localStorage.setItem(MUTED_KEY, String(muted));
  } catch {
    // Not remembering is survivable; the toggle still works for this page load.
  }
  if (muted) void context?.suspend();
}

function output(): { ctx: AudioContext; out: AudioNode } | null {
  if (typeof AudioContext === 'undefined') return null;
  if (!context || !master) {
    context = new AudioContext();
    const volume = context.createGain();
    volume.gain.value = 0.6;
    const limiter = context.createDynamicsCompressor();
    volume.connect(limiter).connect(context.destination);
    master = volume;
  }
  if (context.state === 'suspended') void context.resume();
  return { ctx: context, out: master };
}

/**
 * Play a sound from the scheme. Stays silent while muted, and until the reader has clicked
 * or pressed a key: browsers refuse sound before that, and a sound queued while refused
 * would burst out late. The same sound twice within a quarter second plays once.
 */
export function play(name: SoundName): void {
  if (isMuted()) return;
  if (navigator.userActivation && !navigator.userActivation.hasBeenActive) return;

  const now = performance.now();
  if (now - (lastPlayed.get(name) ?? -Infinity) < 250) return;
  lastPlayed.set(name, now);

  const audio = output();
  if (audio) SYNTHS[name](audio.ctx, audio.out);
}

/** `window.confirm`, announced with the exclamation ding as every 98 message box was. */
export function confirmWithSound(message: string): boolean {
  play('exclamation');
  return window.confirm(message);
}
