/**
 * A failed request, built from the backend's ProblemDetails body.
 *
 * `status` is 0 when the request never reached the server.
 */
export class ApiError extends Error {
  readonly status: number;
  readonly title: string;
  readonly detail: string;
  /** Per-field validation messages, keyed by lower-cased field name. */
  readonly fieldErrors: Record<string, string[]>;

  constructor(
    status: number,
    title: string,
    detail: string,
    fieldErrors: Record<string, string[]> = {},
  ) {
    super(detail || title);
    this.name = 'ApiError';
    this.status = status;
    this.title = title;
    this.detail = detail;
    this.fieldErrors = fieldErrors;
  }

  /** First message for a field, if the backend reported one. */
  fieldError(field: string): string | undefined {
    return this.fieldErrors[field.toLowerCase()]?.[0];
  }
}
