import { ButtonLink } from './components/ButtonLink';
import { PixelIcon } from './components/PixelIcon';
import { Window } from './components/Window';

/** Catch-all route: anything that doesn't match a known path lands here, not a blank screen. */
export function NotFoundPage() {
  return (
    <div className="flex flex-1 flex-col items-center justify-center py-6">
      <Window title="Page not found" className="w-full max-w-md">
        <div className="flex items-start gap-4">
          <PixelIcon name="error" scale={2} />
          <p className="pt-1">The page you&apos;re looking for doesn&apos;t exist or has moved.</p>
        </div>
        <ButtonLink to="/" className="mt-6 self-center">
          Back to Yggdrasil
        </ButtonLink>
      </Window>
    </div>
  );
}
