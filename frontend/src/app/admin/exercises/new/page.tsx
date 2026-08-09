import { ExerciseForm } from "@/features/exercises/components/ExerciseForm";

export default function NewExercisePage() {
  return (
    <main className="mx-auto max-w-5xl space-y-6 px-4 py-8 text-gray-900 dark:text-gray-100">
      <h1 className="text-3xl font-bold tracking-tight">Create exercise</h1>
      <ExerciseForm mode="create" />
    </main>
  );
}
