import { getExercises } from "../api";
import { ExerciseCard } from "./ExerciseCard";

export async function ExerciseList() {
  const exercises = await getExercises();

  return (
    <section className="space-y-4">
      <div className="text-sm text-gray-500">{exercises.length} exercises loaded</div>
      <ul className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
        {exercises.map((exercise) => (
          <ExerciseCard key={exercise.id} exercise={exercise} />
        ))}
      </ul>
    </section>
  );
}
