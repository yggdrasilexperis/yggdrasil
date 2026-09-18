import { request } from './client';
import type { Category, CreateQuizRequest, QuizSummary } from './types';

export function createQuiz(quiz: CreateQuizRequest): Promise<QuizSummary> {
  return request<QuizSummary>('/api/quizzes/create', { method: 'POST', body: quiz });
}

export function getCategories(): Promise<Category[]> {
  return request<Category[]>('/api/categories', { authenticated: false });
}
