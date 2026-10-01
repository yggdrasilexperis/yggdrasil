---
name: Yggdrasil design language
description: A 1998 desktop, redrawn with restraint. Every page is a window on a teal desktop, the nav is a taskbar, one bitmap-style face does all the talking, and all depth comes from bevels.
---

## Overview

Yggdrasil looks like a Windows 95/98 desktop. It is an interpretation, not a replica.
What carries over is the **grammar** of that era:

- **Every page is a window** on a desktop. It has a navy title bar and a grey face, and
  its depth is drawn as 1px bevels.
- **The nav is a taskbar** docked to the bottom of the screen. The page you're on shows as
  a pressed task. The tray holds the sound toggle, who you are and the time.
- **One face, one size.** Windows 98 said everything in MS Sans Serif at one size, and so
  do we: W95FA for chrome and content alike. Hierarchy comes from bold, colour, bevels
  and layout, not from a second typeface.
- **Light falls from the top left.** Raised things have a white top-left edge and a dark
  bottom-right edge, and sunken things are the reverse. No exceptions.
- **Navy means selected or active.** Title bars, the selection highlight, links and chosen
  categories use it.
- **The UI is drawn at 150%.** A font pixel is 1.5 CSS px, and pixel art uses the same
  grid. On a 2x screen both land on whole device pixels.

What makes it ours rather than a copy:

- A warm grey face (from Windows 2000) instead of cold `#c0c0c0`, and a muted teal instead
  of `#008080`.
- A hard drop shadow, so windows sit _on_ the desktop.
- A pixel-art Yggdrasil, the world tree, used as the Start button, the favicon and a
  tone-on-tone wallpaper.

**Tokens live in `src/index.css` under `@theme`, not in this file.** Tailwind generates the
utilities from them, so this document names Tailwind classes and never repeats a hex value.

## Tokens

### Colour

| Utility      | Use                                                                  |
| ------------ | -------------------------------------------------------------------- |
| `desktop`    | The page background. Nothing else                                    |
| `face`       | Every window, button, bar and group box                              |
| `highlight`  | Bevel: the lit outer edge (white)                                    |
| `light`      | Bevel: the lit inner edge                                            |
| `dim`        | Bevel: the shaded edge; also etched frames, rules and disabled text  |
| `ink`        | All text, and the darkest bevel edge. Never pure black               |
| `muted`      | Secondary copy: descriptions, metadata, help text                    |
| `accent`     | Navy: title bars, selection highlight, links, chosen chips           |
| `accent-end` | Where the title-bar gradient lands. Title bars only                  |
| `danger`     | Validation and error text, error rings                               |
| `success`    | The correct answer, once revealed                                    |
| `tooltip`    | The pale yellow of a Windows tooltip, used for the "Quiz author" tag |

`bg-white` is for anything you type into or read a list from: fields, list boxes, the
Discover pane. `danger` and `success` carry meaning, never decoration. Both pass 4.5:1 on
`face` and on white.

### Bevels

Depth is drawn only with these `shadow-*` tokens. Each one is a stack of 1px inset lines.

| Utility          | Looks like                        | Use                                                 |
| ---------------- | --------------------------------- | --------------------------------------------------- |
| `shadow-window`  | Raised, 2 lines, plus drop shadow | `Window` and pop-up menus                           |
| `shadow-raised`  | Raised, 2 lines                   | Buttons, tasks, column headers, caption buttons     |
| `shadow-pressed` | `raised` inverted                 | `:active` buttons, the current task                 |
| `shadow-field`   | Sunken, 2 lines                   | Inputs, selects, checkboxes, list boxes, the pane   |
| `shadow-panel`   | Raised, 1 line                    | Window toolbars                                     |
| `shadow-status`  | Sunken, 1 line                    | Status-bar fields, the taskbar tray                 |
| `shadow-etched`  | Groove (with `border border-dim`) | `GroupBox`                                          |
| `shadow-groove`  | Rule (with `border-t border-dim`) | A carved line above a dialog's buttons, the taskbar |
| `shadow-rename`  | White margin and black line       | Text edited in place (Explorer's rename box)        |

Also: `text-shadow-emboss` (disabled text looks pressed into the face), `bg-dither` (the
50% checkerboard used for the current task and scrollbar tracks), `animate-window-open`
and `animate-progress`.

**Nothing is rounded.** There is no radius token, and `rounded-*` should never appear.

## Typography

One face: **W95FA**, an open redraw of the Windows 95 system font. It's bundled in
`src/assets/fonts/` with its OFL licence and loaded by `@font-face` in `index.css`, so there
are no third-party font requests. The few glyphs it lacks (`… · — ð`) fall back to
Tahoma.

**W95FA is drawn on a grid of 1/12.5 em, so it is only sharp where a font pixel lands on
whole screen pixels.** The type scale is replaced outright (`--text-*: initial`), so only
these sizes exist:

| Utility     | Size    | Font pixel | Use                              |
| ----------- | ------- | ---------- | -------------------------------- |
| `text-base` | 18.75px | 1.5px      | Everything: body, chrome, labels |
| `text-lg`   | 25px    | 2px        | Section headings                 |
| `text-xl`   | 31.25px | 2.5px      | The quiz title                   |

`text-sm`, `text-2xl` and the rest don't exist, and using them silently does nothing. There
is no small print. Secondary copy is `text-muted` at the base size, as 98 did it.

Bold is synthetic (W95FA has one weight) and reads like 98's bold system font. Use it for
titles: the title bar, the quiz title, question text, a quiz's name in a list, a comment's
author. Don't use it for emphasis inside prose. There is no italic, and no letter-spacing
or leading overrides, since the tokens carry the line heights.

## Layout

- The desktop (`main` in `AppLayout`) pads `px-3 pt-4` on phones and `sm:px-6 sm:pt-10`
  above that. The bottom padding keeps the last window clear of the taskbar.
- Window widths are `max-w-5xl` for Discover, `max-w-3xl` for reading and forms, and
  `max-w-md` for dialogs (sign in, sign up, not found). Dialogs are centred on the desktop,
  and other windows sit near the top.
- Window body padding is `p-3 sm:p-5`. A `flush` window (Discover) lets its pane run to the
  frame, as Explorer's does.
- Sections are `gap-10` apart. Lists of group boxes need `gap-6`, because a group box's
  label sticks up half a line.
- **Buttons sit close together:** `gap-1` in toolbars and `gap-1.5` in button rows, the way
  98 packed them.
- A dialog's main action sits at the bottom right, and a message box centres its only
  button.

## Components

Recipes live in code; this is the index. Containers form a small, strict set: **window →
group box → list box / field**, with toolbar and status strips on the window. There is no
free-floating card.

- **`Window`** is the unit of every page. It takes `title` and optional `icon`, `toolbar`,
  `status` (the status bar, which ends in the resize grip) and `flush`.
  - The title bar holds the page's `h1`. Pass `titleAs="div"` when the page puts its own
    heading in the body, as the quiz page does so the title can be edited in place.
  - A window with no icon is a dialog, and gets only a close box.
  - The caption buttons are decoration: `aria-hidden`, not focusable, and they do
    nothing.
- **`GroupBox`** is an etched frame with its label set into the top edge. Each question is
  one ("Question 1"), and so are the in-place editors ("New question", "Edit question",
  "Edit categories"). For a group of form controls use `<GroupBox as="fieldset">`, never a
  bare `fieldset`: the browser's legend notch cuts the border but not the etched highlight,
  so the corner breaks.
- **List box**: `bg-white shadow-field p-0.5` with `divide-y divide-dotted` rows. See
  `AnswerList` (a question's options, in both the quiz page and the drafts) and the
  comment list.
- **`QuizList` / `QuizRow` / `ColumnHeader`**: Explorer's details view.
  - Raised column headers sit over one row per quiz. Name and Created sort on click, and
    clicking again reverses the order.
  - The whole row is the link, and it takes the navy selection highlight on hover and
    focus.
  - Below `md` the columns fold into a line under the title, and the toolbar's sort menu
    stands in for the headers.
  - Every row is the same height (two lines from `md` up, three stacked), and every line
    is cut short with an ellipsis rather than wrapped. A missing description leaves its
    line blank.
  - The pane is always a full page of rows tall, so the window never resizes. Loading,
    errors and "no results" sit in the empty pane. Where four or more rows are missing,
    `TreeMessage` draws the world tree (`worldTree`, the one illustration in
    `pixelArt.ts`, at scale 4) with a line under it. It is drawn tone on tone in
    the window greys (`dim` outline), a step lighter than the caption, so it sits behind it.
  - The toolbar and status bar don't change height either. Active filters scroll
    sideways on one line rather than wrapping, and Clear all and the page buttons grey
    out instead of disappearing.
- **`Button`** has three variants that share one bevel:
  - `primary` is the default action, with a 1px black rim.
  - `secondary` is a plain button.
  - `utility` is a compact toolbar button.

  The label sinks a pixel while the button is held. **`ButtonLink`** is the same look on a
  router `Link`.

- **`Input`**, **`Select`** and checkboxes are sunken white fields. `Select` wraps the
  native select and adds the raised arrow button. Errors add `ring-2 ring-danger` and a
  `text-danger` message below; the label never changes colour.
- **`SectionHeading`** is the `text-lg` `h2` with an etched rule running out to the edge.
- **`CategoryChip`** is a chosen category, navy with a close box. **`DifficultyBadge`** is
  the label, led by a four-bar meter that turns white inside a highlighted row.
- **Menu** (`CommentActionsMenu`): a `shadow-window` pop-up with tight rows that take the
  navy highlight on hover and focus.
- **`Loading`** is the block progress bar. **`ErrorState`** is a message box with the error
  icon, and the text itself stays in ink.
- **`Taskbar`**, left to right:
  - the Start button (home)
  - an etched separator
  - 98-width task buttons (`NavLink`: pressed, dithered and bold when current)
  - on the right, the sign-out button and the tray (`SoundToggle`, user and `TrayClock`)

  Task labels truncate rather than overflow, and the clock hides on phones.

## Sound

The app has a sound scheme, as Windows did. Every sound is synthesised with Web Audio in
`src/sound/synths.ts`. They are homages, not Microsoft's recordings, so there are no audio
files to ship or license.

| Sound         | Plays when                                                                      |
| ------------- | ------------------------------------------------------------------------------- |
| `startup`     | The first click or key press of a visit (once per tab session)                  |
| `navigate`    | Every route change, filters and paging included (Internet Explorer's click)     |
| `error`       | An alert appears, a field turns invalid, or a still-invalid form is resubmitted |
| `exclamation` | A confirm dialog opens (`confirmWithSound`), and sound is switched back on      |
| `success`     | Signing in or up, and creating a quiz (ta-da)                                   |
| `save`        | Saving a quiz, question, category change or comment; adding a draft question    |
| `recycle`     | Deleting a quiz, question, comment or draft question (paper crumple)            |
| `shutdown`    | Signing out, including when a session expires                                   |
| `menu`        | A pop-up menu opens                                                             |
| `reveal`      | Show answer                                                                     |

- **Where they fire.** Event sounds are one `play('name')` call where the event happens. The
  `startup`, `navigate` and `error` sounds come from `useSystemSounds`, mounted once in
  `AppLayout`. That hook watches the page (route key, `role="alert"`, `aria-invalid`), so
  forms never have to know about sound.
- **Rules `play` enforces.** Nothing plays before the reader's first click or key press,
  because browsers forbid it and a sound queued before then would burst out late. Nothing
  plays while muted. The same sound twice within 250ms plays once.
- **Muting** is the speaker in the tray, remembered in this browser. It is the only control
  for sound, so it's always visible, on phones and when signed out too.
- Add a sound only for an event the reader caused or needs to notice. Never for hover, and
  never on a timer.

## Pixel art

Icons are drawn as string grids in `src/components/pixelArt.ts`: one character per pixel,
`.` for transparent, `c` for the current text colour. `PixelIcon` renders them, merging
each row into runs so an icon is one `<path>` per colour.

Icons are 16×16 and glyphs are smaller. `PixelIcon` defaults to `scale={1.5}`, the UI's
grid, which puts an art pixel at a font pixel's size. Use `scale={2}` for the 32px icons in
message boxes. Icons are always decorative, and the text beside them carries the meaning.

`public/favicon.svg` and `public/wallpaper.svg` are the `tree` grid exported as static SVG.
The wallpaper uses translucent light and shade instead of colour. If the tree changes,
re-export both.

## Interaction

- **Focus** is the dotted rectangle: `outline-1 outline-dotted outline-ink`, set once in
  `index.css`. It sits inside buttons (`-outline-offset-5`) and outside everything else. On
  a highlighted list row it turns white. In menus, the highlight bar is the indicator.
  Never remove focus without replacing it.
- **Press**: the bevel inverts (`active:shadow-pressed`) and the label shifts 1px down and
  right. There are no scale transforms.
- **Hover**: buttons don't change, as in 98. List rows and menu items take the selection
  highlight.
- **Disabled**: `text-dim` with `text-shadow-emboss`, and the bevel stays raised.
- **Motion** always uses `steps()`, so it moves in frames rather than easing. Windows open
  in four frames and progress blocks march, both behind `motion-safe:`.
- Touch targets are at least 32px tall (`min-h-8`), and 36px for standard buttons and
  fields.

## Responsive

Tailwind's default breakpoints, mobile first.

- The Start button drops its label below `sm`, but keeps it for screen readers
  (`sr-only sm:not-sr-only`). The tray clock hides below `sm`.
- Windows keep a 12px strip of desktop on each side on phones.
- Action buttons that sit beside text (a question's Edit and Remove) stack under it below
  `sm`.
- The Discover list is a details view from `md` up. Below that, each row stacks.

## Do / Don't

**Do**

- Draw depth with a bevel token, and keep the light coming from the top left.
- Use navy for "selected / active / you can follow this".
- Group related things in a `GroupBox` and lists in a list box. Put pages in a `Window`.
- Stay on the three type sizes, and let bold, `text-muted` and bevels make the hierarchy.

**Don't**

- Don't add a second typeface or a size outside the scale.
- Don't round corners, blur shadows, or add gradients other than the title bar.
- Don't wrap content in a raised panel. Use a group box.
- Don't colour a button on hover, or animate without `steps()`.
- Don't make decorative chrome focusable or give it an accessible name.

## Deliberately out of scope

These are left out on purpose: a Start menu, draggable or resizable windows, working
minimise/maximise/close buttons, File/Edit/View menu bars, custom cursors, and the
literal `#008080` / `#c0c0c0` palette. Each one is either a toy that gets in the way of a
quiz app or a period detail that costs legibility. Add one only when there is a concrete
need.
