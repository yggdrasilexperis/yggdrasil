import type { ReactNode } from 'react';

type Props = {
  label: string;
  /** `fieldset` for a group of form controls, so the label names the group for assistive tech. */
  as?: 'section' | 'fieldset';
  className?: string;
  children: ReactNode;
};

/**
 * 98's group box: an etched frame with its label set into the top edge. The label sticks
 * up half a line, so leave `gap-6` or more above a group box.
 *
 * The label is positioned over an unbroken frame instead of using the browser's legend
 * notch: the notch cuts the border but not the etched highlight, so the corners would not
 * meet. An absolutely positioned legend gets no notch but still names its fieldset.
 */
export function GroupBox({ label, as: Box = 'section', className = '', children }: Props) {
  const Label = Box === 'fieldset' ? 'legend' : 'h3';
  return (
    <Box
      className={`relative min-w-0 border border-dim px-3 pt-5 pb-3 shadow-etched sm:px-4 ${className}`.trim()}
    >
      <Label className="absolute -top-3.5 left-2 bg-face px-1">{label}</Label>
      {children}
    </Box>
  );
}
