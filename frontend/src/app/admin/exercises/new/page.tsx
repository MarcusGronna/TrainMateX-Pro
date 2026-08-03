import { ExerciseForm } from "@/features/exercises/components/ExerciseForm";

export default function NewExercisePage() {
  return (
    <main>
      <h1>Create exercise</h1>
      <ExerciseForm mode="create" />
    </main>
  );
}
