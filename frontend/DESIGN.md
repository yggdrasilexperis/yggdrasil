---
name: Yggdrasil design language
description: Apple's web design language, reduced to the parts a quiz app actually uses. Single blue accent, near-black ink on white and parchment, tight display type, flat chrome with hairline borders, and one consistent control radius.
---

## Overview

Adapted from an analysis of apple.com. That system is built for a product catalog —
full-bleed photography tiles, store grids, a buy-page configurator. We are building a
quiz app, so what carries over is the **grammar**, not the components:

- One accent colour for every interactive element. No second accent, ever.
- Near-black ink on white, with parchment as the only alternate surface.
- Confident, tight display type over a quiet 17px body.
- Flat chrome. Depth comes from surface change and hairlines, not shadows.
- One consistent corner radius on every control; cards a single step softer.

**Tokens live in `src/index.css` under `@theme`, not in this file.** Tailwind generates
the utilities from them, so this document names Tailwind classes and never repeats a hex
value. Change a token in one place.

## Tokens

| Utility        | Value     | Use                                                 |
| -------------- | --------- | --------------------------------------------------- |
| `accent`       | `#0066cc` | Every interactive element: buttons, links, selected |
| `accent-focus` | `#0071e3` | Keyboard focus ring only                            |
| `ink`          | `#1d1d1f` | All headings and body text. Never pure black        |
| `muted`        | `#6e6e73` | Secondary copy, captions, placeholders              |
| `parchment`    | `#f5f5f7` | The alternate surface: page background, footers     |
| `hairline`     | `#e0e0e0` | 1px borders on cards and inputs                     |
| `white`        | —         | Default canvas and card fill                        |

Six colours is the whole palette. Semantic states (a wrong answer, a validation error)
are the one sanctioned exception — use Tailwind's `red-600` / `green-600` directly, and
only for meaning, never decoration.

Two radius tokens, and nothing else rounds:

| Utility           | Value     | Use                                            |
| ----------------- | --------- | ---------------------------------------------- |
| `rounded-control` | `0.5rem`  | Every control: buttons, inputs, answer options |
| `rounded-card`    | `0.75rem` | Cards and other containers — one step softer   |

## Typography

Display and body are two different faces of the same family — `font-display` for
headings, `font-sans` (the default) for everything else.

| Role       | Classes                                              | Notes                      |
| ---------- | ---------------------------------------------------- | -------------------------- |
| Page title | `font-display text-4xl font-semibold tracking-tight` | 36px. One per page         |
| Section    | `font-display text-2xl font-semibold tracking-tight` | 24px                       |
| Question   | `font-display text-xl font-semibold`                 | 20px. The quiz's workhorse |
| Body       | — (base)                                             | 17px/1.47, set on `body`   |
| Caption    | `text-sm text-muted`                                 | 14px. Metadata, help text  |

Three rules that produce the look:

- **Negative tracking on display sizes only** (`tracking-tight`). Never below 17px.
- **Body is 17px, not 16px.** Set globally; you should never need a size class on a
  paragraph.
- **The weight ladder is 400 / 600.** Body is 400, emphasis and headings are 600.
  Weight 500 is deliberately absent — reaching for `font-medium` is a smell.

## Layout & spacing

- 8px base grid. Use Tailwind's default scale (`p-2 p-3 p-4 p-6 p-8 p-12`) and nothing
  arbitrary.
- Card padding `p-6`. Section rhythm `py-12` — not the 80px of a marketing tile; a quiz
  is a working surface, not a gallery.
- Content max width `max-w-3xl` for reading and forms, `max-w-5xl` for quiz grids.
  Never full-bleed.
- Whitespace is the main compositional tool. Before adding a border, a background or a
  shadow to separate two things, add space.

## Depth

Flat by default. The system has three levels and no more:

1. **Flat** — page sections, nav, footer. No border, no shadow.
2. **Hairline** — `border border-hairline` on cards and inputs.
3. **Surface change** — `bg-parchment` against `bg-white` to separate a region.

**No shadows on UI.** Apple's single drop-shadow exists to give product photography
physical weight; we have no product photography, so we have no shadows. `shadow-*` in a
commit is a design bug.

## Interaction

Consistent across every interactive element:

- Focus: `focus-visible:outline-2 focus-visible:outline-offset-2
focus-visible:outline-accent-focus`. Applied globally in `index.css` — don't
  re-declare it per component, and never remove it.
- Press: `active:scale-95 transition-transform`. The system-wide micro-interaction.
- Hover: subtle or absent. Never introduce a new colour on hover; darken or shift
  opacity.
- Touch targets are minimum 44px tall (`h-11`), including on desktop.

## Components

Recipes, not a library. Build each one when a second caller needs it.

**Primary button** — `rounded-control bg-accent px-6 py-3 text-white active:scale-95`.
The accent fill is the action signal; reserve it for real actions.

**Secondary button** — same shape, `border border-accent text-accent` on transparent.
Used as the second of a pair.

**Utility button** — nav and toolbar actions. `rounded-control bg-ink px-4 py-2 text-sm
text-white`. Same radius as the rest; compact, and ink rather than accent.

**Card** — `rounded-card border border-hairline bg-white p-6`. One step softer than a
control, so a card reads as the surface a control sits on. No shadow.

**Input** — `h-11 w-full rounded-control border border-hairline px-4`. Errors:
`border-red-600` plus a `text-sm text-red-600` message below; never colour the label.

**Answer option** — a selectable control, the quiz's most important one. Default
`rounded-control border border-hairline bg-white px-4 py-3 text-left`. Selected
`border-2 border-accent-focus`. Note the selected state changes border weight and
colour only — not the fill, which would fight the text. Correct/incorrect reveal
states use `red-600` / `green-600` borders.

## Do / Don't

**Do**

- Route every "click me" signal through `accent`.
- Separate regions with `bg-parchment`, not with borders or shadows.
- Let the 17px body and generous line-height set an unhurried reading pace.
- Use `rounded-control` for anything a user clicks or types into; `rounded-card` for
  the containers around them.

**Don't**

- Don't add a second accent colour, or use `accent` for anything non-interactive.
- Don't add shadows, gradients, or decorative borders.
- Don't use `font-medium` (500) — the ladder is 400 / 600.
- Don't set body copy tighter than the default leading; the airy leading is the brand.
- Don't reach past the two radius tokens — no `rounded-full`, no `rounded-xl`, no
  arbitrary radius. Controls are `rounded-control`, containers are `rounded-card`.
- Don't reach for arbitrary values (`p-[17px]`, `text-[19px]`). If the scale can't
  express it, the design is wrong, not the scale.

## Responsive

Use Tailwind's default breakpoints — `sm` 640 / `md` 768 / `lg` 1024 is the full range
this app needs. Design mobile-first; a quiz is read on a phone as often as a laptop.

- Content columns collapse to one below `md`.
- Section padding tightens `py-12` → `py-8` below `sm`.
- Display type steps down one size below `md` (`text-4xl` → `text-3xl`).

## Deliberately out of scope

Dropped from the source analysis because this is an app, not a product catalog. Don't
reintroduce them without a concrete need: full-bleed alternating light/dark tiles, the
three micro-stepped near-black tile surfaces, product photography geometry and
`srcset` art direction, the product drop-shadow, store/accessories utility grids, the
frosted sub-nav and floating sticky purchase bar, dense multi-column footer link lists,
10px micro-legal type, and weight-300 display copy.
