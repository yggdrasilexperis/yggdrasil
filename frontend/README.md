# Frontend

React 19 and TypeScript, built with Vite and styled with Tailwind. It's a client
for the backend API and has no business rules of its own.

```
src/api/          typed API client, the only place that calls fetch
src/auth/         sign in, sign up, login state
src/quiz/         discover, create and quiz pages, questions and comments
src/components/   shared UI bits like buttons, inputs and cards
src/layout/       the app shell
```

There's no separate frontend server in Docker or on Azure: the API serves the built
files from its `wwwroot`. The look and feel rules are in [DESIGN.md](DESIGN.md).
