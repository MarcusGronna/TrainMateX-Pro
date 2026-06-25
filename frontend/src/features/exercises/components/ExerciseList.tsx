"use client";
import { useEffect, useState } from "react";

type Exercise = {
  id: string;
  name: string;
  description?: string;
  instructions: string[];
  muscleGroup: string;
  equipment?: string;
  difficultyLevel: string;
};

export default function ExerciseList() {
  const [exercises, setExercises] = useState<Exercise[]>([]);

  useEffect(() => {
    const loadExercises = async () => {
      try {
        const baseUrl: string = "http://localhost:5193/api/exercises";
        const response = await fetch(baseUrl);

        if (!response.ok) {
          throw new Error(`Fetch failed: ${response.status}`);
        }

        const data: Exercise[] = await response.json();
        setExercises(data);
      } catch (err) {
      } finally {
      }
    };

    loadExercises();
  }, []);

  return (
    <section className="space-y-4">
      <div className="text-sm text-gray-500">{exercises.length} exercises loaded</div>
      <ul className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
        {exercises.map((exercise) => (
          <button key={exercise.id} className="cursor-pointer">
            <li className="rounded-xl border border-gray-200 bg-blend-darken p-4 shadow-sm transition hover:shadow-md hover:bg-fuchsia-950">
              <h3>{exercise.name}</h3>
              <div className="mt-3 flex flex-wrap gap-2 text-xs">
                <span className="rounded-full bg-blue-100 px-2.5 py-1 font-medium text-blue-700">
                  {exercise.muscleGroup}
                </span>
                <span className="rounded-full bg-purple-100 px-2.5 py-1 font-medium text-purple-700">
                  {exercise.difficultyLevel}
                </span>
              </div>
            </li>
          </button>
        ))}
      </ul>
    </section>
  );
}
