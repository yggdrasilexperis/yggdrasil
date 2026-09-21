import { Route, Routes } from 'react-router-dom';

import { RequireAuth } from './auth/RequireAuth';
import { SignInPage } from './auth/SignInPage';
import { SignUpPage } from './auth/SignUpPage';
import { AppLayout } from './layout/AppLayout';
import { NotFoundPage } from './NotFoundPage';
import { CreateQuizPage } from './quiz/CreateQuizPage';
import { DiscoverPage } from './quiz/DiscoverPage';
import { QuizDetailPage } from './quiz/QuizDetailPage';
import { QuizEditStub } from './quiz/QuizEditStub';

function App() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route path="/login" element={<SignInPage />} />
        <Route path="/register" element={<SignUpPage />} />

        <Route path="/" element={<DiscoverPage />} />
        <Route path="/quizzes/:id" element={<QuizDetailPage />} />

        <Route
          path="/quizzes/:id/edit"
          element={
            <RequireAuth>
              <QuizEditStub />
            </RequireAuth>
          }
        />
        <Route
          path="quizzes/new"
          element={
            <RequireAuth>
              <CreateQuizPage />
            </RequireAuth>
          }
        />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}

export default App;
