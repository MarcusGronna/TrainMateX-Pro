import { getExercises } from "@/features/exercises/api";
import { AdminExerciseList } from "@/features/exercises/components/AdminExerciseList";
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
      <AdminExerciseList exercises={exercises} />
    </main>
  );
}
