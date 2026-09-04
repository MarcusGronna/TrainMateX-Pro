<!-- BEGIN:nextjs-agent-rules -->
# This is NOT the Next.js you know

This version has breaking changes — APIs, conventions, and file structure may all differ from your training data. Read the relevant guide in `node_modules/next/dist/docs/` before writing any code. Heed deprecation notices.
<!-- END:nextjs-agent-rules -->

# TrainMateX-Pro

- TrainMateX-Pro is a monorepo fullstack exercise-library app: ASP.NET Core Minimal API on .NET 10/C#, Next.js 16.2.9 + React 19.2.4 + TypeScript 5 + Tailwind CSS 4, with PostgreSQL + EF Core for persistence.
- Read `/project-guidance.md` before substantial implementation, debugging, architecture, mentoring, or learning-oriented work; it contains the detailed repository guidance that this file intentionally does not repeat.
- Inspect existing repository patterns in the relevant area before introducing new conventions.
- Build incrementally by vertical slice. Read the relevant spec in `/docs/slices/` and supporting ADRs in `/docs/decisions/` before making meaningful changes.
- Implement only the current slice or explicitly requested scope. Do not silently expand scope or pre-build future slices.
- Keep responsibilities clear: Next.js handles UI, routes, and data fetching; ASP.NET Core owns business rules, validation, persistence, and security-sensitive behavior.
- Keep security-sensitive changes explicit and carefully reasoned.
- If the user asks only for analysis, explanation, planning, mentoring, documentation, or review, do not modify product code.
- Verify meaningful changes with appropriate existing checks such as `dotnet test`, `npm run build`, and `npm run lint` when relevant. Never claim checks passed unless you actually ran them.
