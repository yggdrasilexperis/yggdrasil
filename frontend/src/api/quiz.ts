import { request } from './client';
import type { PagedResult, QuizDetail, QuizSummary } from './types';

export function listQuizzes(): Promise<PagedResult<QuizSummary>> {
  return request<PagedResult<QuizSummary>>('/api/quizzes', { authenticated: false });
}

export function getQuizDetail(id: string): Promise<QuizDetail> {
  return request<QuizDetail>(`/api/quizzes/${id}`, { authenticated: false });
}

/** Backend enforces ownership too (403 if you're not the owner) — this call is UX, not the guard. */
export function deleteQuiz(id: string): Promise<void> {
  return request<void>(`/api/quizzes/delete/${id}`, { method: 'DELETE' });
}
