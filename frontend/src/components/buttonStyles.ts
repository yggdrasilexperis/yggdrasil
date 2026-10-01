/** DESIGN.md "Components": one raised bevel for every button; the default action gets a black rim. */
export type ButtonVariant = 'primary' | 'secondary' | 'utility';

const base =
  'group inline-flex items-center justify-center bg-face text-ink shadow-raised ' +
  'select-none focus-visible:-outline-offset-5 active:shadow-pressed disabled:text-dim ' +
  'disabled:text-shadow-emboss disabled:shadow-raised';

const variants: Record<ButtonVariant, string> = {
  primary: 'min-h-9 min-w-24 px-4 ring-1 ring-ink',
  secondary: 'min-h-9 min-w-24 px-4',
  utility: 'min-h-8 px-3',
};

export function buttonClasses(variant: ButtonVariant, className = '') {
  return `${base} ${variants[variant]} ${className}`.trim();
}

/** Wraps the label, which sinks a pixel down-right while the button is held. */
export const buttonLabel =
  'inline-flex items-center gap-2 group-active:translate-x-px group-active:translate-y-px ' +
  'group-disabled:translate-none';
