"use client";

import { useRouter } from "next/navigation";
import { ExerciseListItem } from "../types";
import Link from "next/link";
import { useState } from "react";

type AdminExerciseListProps = {
  exercises: ExerciseListItem[];
};

export function AdminExerciseList({ exercises }: AdminExerciseListProps) {
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const router = useRouter();

  return (
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
  );
}
