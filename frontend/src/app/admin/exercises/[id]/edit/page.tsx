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
    <main>
      <h1>Edit exercise</h1>
      <ExerciseForm mode="edit" exerciseId={exercise.id} initialValues={initialValues} />
    </main>
  );
}
