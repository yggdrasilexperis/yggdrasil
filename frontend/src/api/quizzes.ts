import { request } from './client';
import type { Category, CreateQuizRequest, Quiz } from './types';

export function createQuiz(quiz: CreateQuizRequest): Promise<Quiz> {
  return request<Quiz>('/api/quizzes', { method: 'POST', body: quiz });
}

export function getCategories(): Promise<Category[]> {
  return request<Category[]>('/api/categories', { authenticated: false });
}
