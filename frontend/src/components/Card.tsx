import type { ReactNode } from "react";

type Props = { children: ReactNode; className?: string };

export function Card({ children, className = '' }: Props) {
    return (
        <div className={`rounded-card border border-hairline bg-white p-6 ${className}`.trim()}>
            {children}
        </div>
    );
}