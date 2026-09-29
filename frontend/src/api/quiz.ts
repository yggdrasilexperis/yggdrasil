import { request } from './client';
import type {
  CreateCommentRequest,
  CreateQuestionRequest,
  GetQuizzesRequest,
  PagedResult,
  Question,
  QuizDetail,
  Comment,
  QuizSummary,
  UpdateCommentRequest,
  UpdateQuizRequest,
} from './types';

/** One page of the quizzes carrying every slug given, in the order asked for. */
export function listQuizzes(query: GetQuizzesRequest): Promise<PagedResult<QuizSummary>> {
  const params = new URLSearchParams({
    page: String(query.page),
    pageSize: String(query.pageSize),
    sortBy: query.sortBy,
    sortDirection: query.sortDirection,
  });

  query.categorySlugs.forEach((slug) => params.append('categorySlugs', slug));
  return request<PagedResult<QuizSummary>>(`/api/quizzes?${params}`, { authenticated: false });
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

export function addQuestion(quizId: string, question: CreateQuestionRequest): Promise<Question> {
  return request<Question>(`/api/quizzes/${quizId}/questions`, { method: 'POST', body: question });
}

export function deleteQuestion(quizId: string, questionId: string): Promise<void> {
  return request<void>(`/api/quizzes/${quizId}/questions/${questionId}`, { method: 'DELETE' });
}

export function deleteComment(quizId: string, commentId: string): Promise<void> {
  return request<void>(`/api/quizzes/${quizId}/comments/${commentId}`, { method: 'DELETE' });
}
export function updateComment(
  quizId: string,
  commentId: string,
  comment: UpdateCommentRequest,
): Promise<Comment> {
  return request<Comment>(`/api/quizzes/${quizId}/comments/${commentId}`, {
    method: 'PUT',
    body: comment,
  });
}
export function addComment(quizId: string, comment: CreateCommentRequest): Promise<Comment> {
  return request<Comment>(`/api/quizzes/${quizId}/comments`, { method: 'POST', body: comment });
}
