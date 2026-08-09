import { getExercises } from "@/features/exercises/api";
import Link from "next/link";

export default async function AdminExercisePage() {
  const exercises = await getExercises();

  return (
    <main className="mx-auto max-w-5xl space-y-6 px-4 py-8 text-gray-900 dark:text-gray-100">
      <header className="flex flex-wrap items-center justify-between gap-4">
        <h1 className="text-3xl font-bold tracking-tight">Manage exercises</h1>
        <Link
          href="/admin/exercises/new"
          className="rounded-xl bg-purple-600 px-4 py-2 font-medium text-white shadow-sm transition hover:bg-purple-700"
        >
          Create exercise
        </Link>
      </header>

      <ul className="space-y-3">
        {exercises.map((exercise) => (
          <li
            key={exercise.id}
            className="flex flex-wrap items-center justify-between gap-4 rounded-xl border border-gray-200 bg-white p-4 shadow-sm transition hover:bg-fuchsia-50 hover:shadow-md dark:border-gray-700 dark:bg-gray-900 dark:hover:bg-fuchsia-950"
          >
            <div className="space-y-2">
              <strong className="text-lg">{exercise.name}</strong>
              <div className="flex flex-wrap gap-2 text-xs">
                <span className="rounded-full bg-blue-100 px-2.5 py-1 font-medium text-blue-700 dark:bg-blue-950 dark:text-blue-200">
                  {exercise.muscleGroup}
                </span>
                <span className="rounded-full bg-purple-100 px-2.5 py-1 font-medium text-purple-700 dark:bg-purple-950 dark:text-purple-200">
                  {exercise.difficultyLevel}
                </span>
              </div>
            </div>

            <Link
              href={`/admin/exercises/${encodeURIComponent(exercise.id)}/edit`}
              className="rounded-xl border border-purple-200 px-3 py-2 text-sm font-medium text-purple-700 transition hover:bg-purple-50 dark:border-purple-800 dark:text-purple-300 dark:hover:bg-purple-950"
            >
              Edit
            </Link>
          </li>
        ))}
      </ul>
    </main>
  );
}
