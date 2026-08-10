import { ExerciseList } from "@/features/exercises/components/ExerciseList";

export default function ExercisesPage() {
  return (
    <main className="mx-auto max-w-5xl space-y-6 px-4 py-8 text-gray-900 dark:text-gray-100">
      <h1 className="text-3xl font-bold tracking-tight">Exercise Library</h1>
      <ExerciseList />
    </main>
  );
}
