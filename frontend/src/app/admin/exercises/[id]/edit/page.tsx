import { getExerciseById } from "@/features/exercises/api";
import { ExerciseForm } from "@/features/exercises/components/ExerciseForm";
import { SaveExerciseRequest } from "@/features/exercises/types";
import { notFound } from "next/navigation";

type EditExercisePageProps = {
  params: Promise<{
    id: string;
  }>;
};

export default async function EditExercisePage({ params }: EditExercisePageProps) {
  const { id } = await params;
  const exercise = await getExerciseById(id);

  if (!exercise) {
    notFound();
  }

  const initialValues: SaveExerciseRequest = {
    name: exercise.name,
    description: exercise.description,
    instructions: exercise.instructions,
    muscleGroup: exercise.muscleGroup,
    equipment: exercise.equipment,
    difficultyLevel: exercise.difficultyLevel,
  };

  return (
    <main className="mx-auto max-w-5xl space-y-6 px-4 py-8 text-gray-900 dark:text-gray-100">
      <h1 className="text-3xl font-bold tracking-tight">Edit exercise</h1>
      <ExerciseForm mode="edit" exerciseId={exercise.id} initialValues={initialValues} />
    </main>
  );
}
