import { PIXEL_ART } from './pixelArt';
import type { PixelIconName } from './pixelArt';

type Props = {
  name: PixelIconName;
  /**
   * Screen pixels per art pixel. 1.5 is the UI's grid: the same size as a pixel of the
   * font, and whole device pixels on the 2x screens this is mostly read on.
   */
  scale?: 1.5 | 2 | 3 | 4;
  className?: string;
};

/** Decorative: the text next to an icon always says what it means. */
export function PixelIcon({ name, scale = 1.5, className = '' }: Props) {
  const { width, height, paths } = PIXEL_ART[name];
  return (
    <svg
      aria-hidden="true"
      width={width * scale}
      height={height * scale}
      viewBox={`0 0 ${width} ${height}`}
      shapeRendering="crispEdges"
      className={`shrink-0 ${className}`.trim()}
    >
      {paths.map(({ fill, d }) => (
        <path key={fill} fill={fill} d={d} />
      ))}
    </svg>
  );
}
