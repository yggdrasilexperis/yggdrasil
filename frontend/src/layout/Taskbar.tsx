import { Link, NavLink, useLocation } from 'react-router-dom';

import { useAuth } from '../auth/useAuth';
import { PixelIcon } from '../components/PixelIcon';
import { SoundToggle } from './SoundToggle';
import { TrayClock } from './TrayClock';

const button =
  'flex h-8 min-w-0 items-center gap-1.5 bg-face px-2 select-none ' +
  'focus-visible:-outline-offset-5';
const raised = `${button} shadow-raised active:shadow-pressed`;

/**
 * A task is as wide as 98 made them, with its label left-aligned. It sits pressed,
 * dithered and bold while its page is the one open.
 */
const taskClass = ({ isActive }: { isActive: boolean }) =>
  `${isActive ? `${button} bg-dither font-bold shadow-pressed` : raised} sm:w-48`;

/** The site nav, docked to the bottom of the screen like the Windows taskbar. */
export function Taskbar() {
  const { user, signOut } = useAuth();
  const location = useLocation();
  const onAuthPage = ['/login', '/register'].includes(location.pathname);

  return (
    <header className="fixed inset-x-0 bottom-0 z-20 border-t border-light bg-face shadow-groove">
      <nav className="flex h-11 items-center gap-1 px-1">
        <Link to="/" className={`${raised} shrink-0 font-bold`}>
          <PixelIcon name="tree" />
          <span className="sr-only sm:not-sr-only">Yggdrasil</span>
        </Link>
        <span
          aria-hidden="true"
          className="mx-0.5 h-8 shrink-0 border-r border-l border-r-highlight border-l-dim"
        />

        {user ? (
          <NavLink to="/quizzes/new" className={taskClass}>
            <PixelIcon name="doc" />
            <span className="truncate">Create quiz</span>
          </NavLink>
        ) : (
          <>
            <NavLink
              to="/login"
              state={onAuthPage ? location.state : { from: location.pathname }}
              className={taskClass}
            >
              <PixelIcon name="key" />
              <span className="truncate">Sign in</span>
            </NavLink>
            <NavLink to="/register" className={taskClass}>
              <PixelIcon name="user" />
              <span className="truncate">Create account</span>
            </NavLink>
          </>
        )}

        <div className="ml-auto flex min-w-0 items-center gap-1">
          {user && (
            <button type="button" onClick={signOut} className={`${raised} shrink-0`}>
              Sign out
            </button>
          )}
          {/* The tray: sound, who is signed in, and the time, sunk into the bar. */}
          <span className="flex h-8 min-w-0 items-center gap-2 pr-2 pl-0.5 shadow-status">
            <SoundToggle />
            {user && (
              <span className="flex min-w-0 items-center gap-1.5">
                <PixelIcon name="user" />
                <span className="max-w-20 truncate sm:max-w-48">{user.userName}</span>
              </span>
            )}
            <TrayClock className="hidden whitespace-nowrap sm:inline" />
          </span>
        </div>
      </nav>
    </header>
  );
}
