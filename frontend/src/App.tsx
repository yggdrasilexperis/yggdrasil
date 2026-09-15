import { Route, Routes } from 'react-router-dom';

import { RequireAuth } from './auth/RequireAuth';
import { SignInPage } from './auth/SignInPage';
import { SignUpPage } from './auth/SignUpPage';
import { Home } from './Home';
import { AppLayout } from './layout/AppLayout';
import { NotFoundPage } from './NotFoundPage';

function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/login" element={<SignInPage />} />
        <Route path="/register" element={<SignUpPage />} />
        <Route
          path="/"
          element={
            <RequireAuth>
              <Home />
            </RequireAuth>
          }
        />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}

export default App;
