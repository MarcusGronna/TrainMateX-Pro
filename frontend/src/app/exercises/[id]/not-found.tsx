import Link from "next/link";

export default function ExerciseNotFound() {
  return (
    <main className="mx-auto max-w-3xl space-y-6 rounded-2xl border border-gray-200 p-6 shadow-sm">
      <h1 className="text-3xl font-bold tracking-tight">Exercise not notFound</h1>
      <p>The exercise you requested does not exist.</p>
      <Link
        href="/exercises"
        className="inline-flex items-center text-sm font-medium rounded-2xl  p-1.5 text-purple-400 hover:font-bold"
      >
        ← Back to exercises
      </Link>
    </main>
  );
}
