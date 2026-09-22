import { request } from './client';
import type { PagedResult, QuizDetail, QuizSummary, UpdateQuizRequest } from './types';

/** An empty `categorySlug` lists every quiz. */
export function listQuizzes(categorySlug = ''): Promise<PagedResult<QuizSummary>> {
  const query = categorySlug ? `?${new URLSearchParams({ categorySlug })}` : '';
  return request<PagedResult<QuizSummary>>(`/api/quizzes${query}`, { authenticated: false });
}

export function getQuizDetail(id: string): Promise<QuizDetail> {
  return request<QuizDetail>(`/api/quizzes/${id}`, { authenticated: false });
}

/** Backend enforces ownership (403 unless owner or admin) — calling this is UX, not the guard. */
export function updateQuiz(id: string, quiz: UpdateQuizRequest): Promise<QuizSummary> {
  return request<QuizSummary>(`/api/quizzes/${id}`, { method: 'PUT', body: quiz });
}

/** Backend enforces ownership too (403 if you're not the owner) — this call is UX, not the guard. */
export function deleteQuiz(id: string): Promise<void> {
  return request<void>(`/api/quizzes/${id}`, { method: 'DELETE' });
}
