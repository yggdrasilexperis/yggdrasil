import { Outlet } from 'react-router-dom';

import { useSystemSounds } from '../sound/useSystemSounds';
import { Taskbar } from './Taskbar';

/** The desktop shared by every route: pages open as windows above the taskbar. */
export function AppLayout() {
  useSystemSounds();

  return (
    <div className="flex min-h-svh flex-col">
      <Taskbar />
      <main className="flex flex-1 flex-col px-3 pt-4 pb-18 sm:px-6 sm:pt-10 sm:pb-20">
        <Outlet />
      </main>
    </div>
  );
}
