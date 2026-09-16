import { Link } from 'react-router-dom';

import { Card } from '../components/Card';
import { DIFFICULTY_LABELS } from '../api/types';
import type { QuizSummary } from '../api/types';

export function QuizCard({ quiz }: { quiz: QuizSummary }) {
    return (
        <Link to={`/quizzes/${quiz.id}`} className="block">
            <Card className="flex h-full flex-col gap-2 transition-transform active:scale-95">
                <h2 className="font-display text-xl font-semibold">{quiz.title}</h2>

                {quiz.description && <p className="line-clamp-2 text-muted">{quiz.description}</p>}

                <div className="mt-auto flex items-center gap-2 pt-2 text-sm text-muted">
                    <span>{DIFFICULTY_LABELS[quiz.difficulty]}</span>
                    {quiz.categories.length > 0 && (
                        <span>· {quiz.categories.map((c) => c.name).join(', ')}</span>
                    )}
                </div>
            </Card>
        </Link>
    );
}