import Link from "next/link";
import { notFound } from "next/navigation";
import { getExerciseById } from "../api";

type ExerciseDetailProps = {
  exerciseId: string;
};

export async function ExerciseDetail({ exerciseId }: ExerciseDetailProps) {
  const exercise = await getExerciseById(exerciseId);

  if (!exercise) {
    notFound();
  }

  return (
    <article className="mx-auto max-w-3xl space-y-6 rounded-2xl border border-gray-200 p-6 shadow-sm">
      <Link
        href="/exercises"
        className="inline-flex items-center text-sm font-medium rounded-2xl  p-1.5 text-purple-400 hover:font-bold"
      >
        ← Back to exercises
      </Link>
      <header className="space-y-2">
        <h1 className="text-3xl font-bold tracking-tight">{exercise.name}</h1>
        <p>{exercise.description}</p>
      </header>
      <dl className="grid grid-cols-[140px_1fr] gap-x-2 gap-y-4 rounded-xl p-4 text-sm">
        <dt className="font-bold">Muscle group:</dt>
        <dd>{exercise.muscleGroup}</dd>

        <dt className="font-bold">Equipment:</dt>
        <dd>{exercise.equipment}</dd>

        <dt className="font-bold">Difficulty:</dt>
        <dd>{exercise.difficultyLevel}</dd>
      </dl>

      <section className="space-y-3">
        <h2 className="text-xl font-semibold">Instructions</h2>
        <ol className="list-decimal space-y-2 pl-5  marker:font-semibold marker:text-gray-400">
          {exercise.instructions.map((instruction, index) => (
            <li key={`${exerciseId}-${index}-${instruction}`}>{instruction}</li>
          ))}
        </ol>
      </section>
    </article>
  );
}
