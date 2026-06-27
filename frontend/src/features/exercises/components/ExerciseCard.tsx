import Link from "next/link";
import { ExerciseListItem } from "../types";

type ExerciseCardProps = {
  exercise: ExerciseListItem;
};

export function ExerciseCard({ exercise }: ExerciseCardProps) {
  return (
    <Link href={`/exercises/${exercise.id}`}>
      <article className="rounded-xl border border-gray-200 bg-blend-darken p-4 shadow-sm transition hover:shadow-md hover:bg-fuchsia-950">
        <h2>{exercise.name}</h2>
        <div className="mt-3 flex flex-wrap gap-2 text-xs">
          <span className="rounded-full bg-blue-100 px-2.5 py-1 font-medium text-blue-700">
            {exercise.muscleGroup}
          </span>
          <span className="rounded-full bg-purple-100 px-2.5 py-1 font-medium text-purple-700">
            {exercise.difficultyLevel}
          </span>
        </div>
      </article>
    </Link>
  );
}
