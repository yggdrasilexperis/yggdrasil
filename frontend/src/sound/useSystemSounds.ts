import { useEffect, useRef } from 'react';
import { useLocation } from 'react-router-dom';

import { play } from './player';

const BOOTED_KEY = 'yggdrasil.booted';

function isAlert(node: Node) {
  return (
    node instanceof Element &&
    (node.matches('[role="alert"]') || node.querySelector('[role="alert"]') !== null)
  );
}

/**
 * The sounds the app makes on its own, the way Windows attached sounds to system events
 * rather than to individual programs. Mounted once, in `AppLayout`.
 */
export function useSystemSounds() {
  const { key } = useLocation();
  const lastKey = useRef(key);

  // Startup, once per visit. Browsers only allow sound after the reader's first click or
  // key press, so the startup sound waits for it.
  useEffect(() => {
    try {
      if (sessionStorage.getItem(BOOTED_KEY)) return;
    } catch {
      // Without storage it simply plays on every page load.
    }

    function boot() {
      window.removeEventListener('click', boot, true);
      window.removeEventListener('keydown', boot, true);
      try {
        sessionStorage.setItem(BOOTED_KEY, 'true');
      } catch {
        // See above.
      }
      play('startup');
    }

    window.addEventListener('click', boot, true);
    window.addEventListener('keydown', boot, true);
    return () => {
      window.removeEventListener('click', boot, true);
      window.removeEventListener('keydown', boot, true);
    };
  }, []);

  // Internet Explorer clicked on every page change; so do we, filters and paging included.
  useEffect(() => {
    if (key === lastKey.current) return;
    lastKey.current = key;
    play('navigate');
  }, [key]);

  // Errors. Windows sounded with every message box; here any alert that appears, or any
  // field that turns invalid, sounds the chord, and so does resubmitting a form that is
  // still wrong.
  useEffect(() => {
    const observer = new MutationObserver((records) => {
      const failed = records.some((record) =>
        record.type === 'attributes'
          ? (record.target as Element).getAttribute('aria-invalid') === 'true'
          : [...record.addedNodes].some(isAlert),
      );
      if (failed) play('error');
    });
    observer.observe(document.body, {
      childList: true,
      subtree: true,
      attributes: true,
      attributeFilter: ['aria-invalid'],
    });

    function handleSubmit(event: Event) {
      const form = event.target as HTMLFormElement;
      // After React has rendered the result of the submit handler.
      setTimeout(() => {
        if (form.querySelector('[aria-invalid="true"], [role="alert"]')) play('error');
      });
    }
    document.addEventListener('submit', handleSubmit, true);

    return () => {
      observer.disconnect();
      document.removeEventListener('submit', handleSubmit, true);
    };
  }, []);
}
