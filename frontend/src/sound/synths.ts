/**
 * The sound scheme, synthesised with Web Audio. These are homages to the sounds Windows 98
 * made, not copies of Microsoft's recordings: each one is a recipe of oscillators, noise
 * and envelopes, so there are no audio files to ship or license.
 */

type Synth = (ctx: AudioContext, out: AudioNode) => void;

const midi = (note: number) => 440 * 2 ** ((note - 69) / 12);

type Tone = {
  type?: OscillatorType;
  freq: number;
  start: number;
  attack?: number;
  hold?: number;
  release: number;
  gain: number;
  detune?: number;
};

function tone(
  ctx: AudioContext,
  out: AudioNode,
  { type = 'sine', freq, start, attack = 0.005, hold = 0, release, gain, detune = 0 }: Tone,
) {
  const osc = ctx.createOscillator();
  osc.type = type;
  osc.frequency.value = freq;
  osc.detune.value = detune;
  const amp = ctx.createGain();
  amp.gain.setValueAtTime(0, start);
  amp.gain.linearRampToValueAtTime(gain, start + attack);
  amp.gain.setValueAtTime(gain, start + attack + hold);
  amp.gain.exponentialRampToValueAtTime(0.0001, start + attack + hold + release);
  osc.connect(amp).connect(out);
  osc.start(start);
  osc.stop(start + attack + hold + release + 0.05);
  return osc;
}

/** A struck bell: a fundamental, and two inharmonic partials that die away faster. */
function bell(
  ctx: AudioContext,
  out: AudioNode,
  freq: number,
  start: number,
  length: number,
  gain: number,
) {
  tone(ctx, out, { freq, start, release: length, gain });
  tone(ctx, out, { freq: freq * 2.76, start, release: length * 0.45, gain: gain * 0.35 });
  tone(ctx, out, { freq: freq * 5.4, start, release: length * 0.2, gain: gain * 0.15 });
}

/** A plucked, piano-ish note: bright at the strike, duller as it rings. */
function pluck(
  ctx: AudioContext,
  out: AudioNode,
  freq: number,
  start: number,
  length: number,
  gain: number,
) {
  const filter = ctx.createBiquadFilter();
  filter.type = 'lowpass';
  filter.frequency.setValueAtTime(7000, start);
  filter.frequency.exponentialRampToValueAtTime(700, start + length);
  filter.connect(out);
  tone(ctx, filter, { type: 'triangle', freq, start, attack: 0.002, release: length, gain });
  tone(ctx, filter, {
    type: 'sawtooth',
    freq,
    start,
    attack: 0.002,
    release: length * 0.3,
    gain: gain * 0.25,
  });
}

/** A brassy swell: detuned saws behind a filter that opens as the note speaks. */
function brass(
  ctx: AudioContext,
  out: AudioNode,
  freq: number,
  start: number,
  length: number,
  gain: number,
  vibrato = false,
) {
  const filter = ctx.createBiquadFilter();
  filter.type = 'lowpass';
  filter.frequency.setValueAtTime(500, start);
  filter.frequency.exponentialRampToValueAtTime(3200, start + 0.06);
  filter.frequency.exponentialRampToValueAtTime(1400, start + length);
  filter.connect(out);
  for (const detune of [-6, 6]) {
    const osc = tone(ctx, filter, {
      type: 'sawtooth',
      freq,
      start,
      attack: 0.03,
      hold: length * 0.6,
      release: length * 0.4,
      gain,
      detune,
    });
    if (vibrato) {
      const lfo = ctx.createOscillator();
      lfo.frequency.value = 5.5;
      const depth = ctx.createGain();
      depth.gain.setValueAtTime(0, start);
      depth.gain.linearRampToValueAtTime(freq * 0.012, start + length * 0.5);
      lfo.connect(depth).connect(osc.frequency);
      lfo.start(start);
      lfo.stop(start + length + 0.1);
    }
  }
}

const noiseBuffers = new WeakMap<AudioContext, AudioBuffer>();

/** A short burst of filtered white noise. */
function burst(
  ctx: AudioContext,
  out: AudioNode,
  start: number,
  length: number,
  gain: number,
  filterType: BiquadFilterType,
  freq: number,
  q = 1,
) {
  let buffer = noiseBuffers.get(ctx);
  if (!buffer) {
    buffer = ctx.createBuffer(1, ctx.sampleRate, ctx.sampleRate);
    const data = buffer.getChannelData(0);
    for (let i = 0; i < data.length; i++) data[i] = Math.random() * 2 - 1;
    noiseBuffers.set(ctx, buffer);
  }
  const source = ctx.createBufferSource();
  source.buffer = buffer;
  const filter = ctx.createBiquadFilter();
  filter.type = filterType;
  filter.frequency.value = freq;
  filter.Q.value = q;
  const amp = ctx.createGain();
  amp.gain.setValueAtTime(gain, start);
  amp.gain.exponentialRampToValueAtTime(0.0001, start + length);
  source.connect(filter).connect(amp).connect(out);
  source.start(start, Math.random() * 0.5);
  source.stop(start + length + 0.02);
}

const rooms = new WeakMap<AudioContext, ConvolverNode>();

/** A dry path plus a generated reverb, so a sound rings like a sound card's "hall" preset. */
function room(ctx: AudioContext, out: AudioNode, wet: number): AudioNode {
  let reverb = rooms.get(ctx);
  if (!reverb) {
    reverb = ctx.createConvolver();
    const length = Math.floor(ctx.sampleRate * 2.6);
    const impulse = ctx.createBuffer(2, length, ctx.sampleRate);
    for (let channel = 0; channel < 2; channel++) {
      const data = impulse.getChannelData(channel);
      for (let i = 0; i < length; i++) data[i] = (Math.random() * 2 - 1) * (1 - i / length) ** 3;
    }
    reverb.buffer = impulse;
    reverb.connect(out);
    rooms.set(ctx, reverb);
  }
  const input = ctx.createGain();
  const send = ctx.createGain();
  send.gain.value = wet;
  input.connect(out);
  input.connect(send).connect(reverb);
  return input;
}

export const SYNTHS = {
  /** Five seconds of swelling E-flat major chord, rising chimes and one last glint. */
  startup(ctx, out) {
    const t = ctx.currentTime + 0.05;
    const bus = room(ctx, out, 0.5);
    const filter = ctx.createBiquadFilter();
    filter.type = 'lowpass';
    filter.Q.value = 3;
    filter.frequency.setValueAtTime(250, t);
    filter.frequency.exponentialRampToValueAtTime(3800, t + 2.2);
    filter.frequency.exponentialRampToValueAtTime(900, t + 5.5);
    filter.connect(bus);
    for (const note of [51, 58, 63, 67, 70, 77]) {
      for (const detune of [-8, 8]) {
        tone(ctx, filter, {
          type: 'sawtooth',
          freq: midi(note),
          detune,
          start: t,
          attack: 1.6,
          hold: 1.2,
          release: 2.6,
          gain: 0.03,
        });
      }
    }
    tone(ctx, bus, { freq: midi(39), start: t, attack: 1.4, hold: 1.2, release: 2.4, gain: 0.16 });
    [70, 75, 79, 82, 87, 91].forEach((note, i) =>
      bell(ctx, bus, midi(note), t + 0.8 + i * 0.14, 2.4, 0.06),
    );
    bell(ctx, bus, midi(94), t + 2.3, 3.2, 0.045);
  },

  /** Internet Explorer's navigation click. */
  navigate(ctx, out) {
    const t = ctx.currentTime;
    burst(ctx, out, t, 0.02, 0.5, 'highpass', 3000);
    tone(ctx, out, {
      type: 'square',
      freq: 1700,
      start: t,
      attack: 0.001,
      release: 0.015,
      gain: 0.05,
    });
  },

  /** Critical stop: a bright, struck chord over a low root. */
  error(ctx, out) {
    const t = ctx.currentTime;
    const bus = room(ctx, out, 0.3);
    [60, 64, 67, 72].forEach((note, i) => pluck(ctx, bus, midi(note), t + i * 0.015, 1.1, 0.13));
    pluck(ctx, bus, midi(48), t, 1.2, 0.12);
  },

  /** The ding that came with every question a message box asked. */
  exclamation(ctx, out) {
    bell(ctx, room(ctx, out, 0.35), midi(88), ctx.currentTime, 1.6, 0.22);
  },

  /** Ta-daaa. */
  success(ctx, out) {
    const t = ctx.currentTime;
    const bus = room(ctx, out, 0.3);
    for (const note of [55, 59, 62]) brass(ctx, bus, midi(note), t, 0.14, 0.08);
    for (const note of [60, 64, 67, 72]) brass(ctx, bus, midi(note), t + 0.16, 1.1, 0.08, true);
  },

  /** Two quick chimes: saved. */
  save(ctx, out) {
    const t = ctx.currentTime;
    const bus = room(ctx, out, 0.3);
    bell(ctx, bus, midi(84), t, 0.9, 0.14);
    bell(ctx, bus, midi(91), t + 0.11, 1.2, 0.14);
  },

  /** The recycle bin: paper crumpling. */
  recycle(ctx, out) {
    const t = ctx.currentTime;
    const bus = room(ctx, out, 0.12);
    const crackles = 34;
    for (let i = 0; i < crackles; i++) {
      const start = t + (i / crackles) ** 1.3 * 0.75 + Math.random() * 0.03;
      burst(
        ctx,
        bus,
        start,
        0.015 + Math.random() * 0.05,
        0.35 + Math.random() * 0.55,
        'bandpass',
        700 + Math.random() * 4500,
        0.8 + Math.random() * 2,
      );
    }
  },

  /** Signing out: descending chimes, like shutting down. */
  shutdown(ctx, out) {
    const t = ctx.currentTime;
    const bus = room(ctx, out, 0.45);
    [79, 76, 72, 67].forEach((note, i) => bell(ctx, bus, midi(note), t + i * 0.24, 2.4, 0.13));
    tone(ctx, bus, { freq: midi(43), start: t, attack: 0.5, hold: 0.6, release: 1.5, gain: 0.08 });
  },

  /** A menu popping open. */
  menu(ctx, out) {
    const t = ctx.currentTime;
    const osc = tone(ctx, out, { freq: 900, start: t, attack: 0.003, release: 0.07, gain: 0.16 });
    osc.frequency.exponentialRampToValueAtTime(1600, t + 0.05);
  },

  /** An answer revealed: a quick upward sparkle. */
  reveal(ctx, out) {
    const t = ctx.currentTime;
    const bus = room(ctx, out, 0.35);
    [84, 88, 91, 96].forEach((note, i) => bell(ctx, bus, midi(note), t + i * 0.055, 0.7, 0.11));
  },
} satisfies Record<string, Synth>;
