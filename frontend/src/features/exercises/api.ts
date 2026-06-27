import { ExerciseDetails, ExerciseListItem } from "./types";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5193";

export async function getExercises(): Promise<ExerciseListItem[]> {
  const response = await fetch(`${API_BASE_URL}/api/exercises`, {
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("Failed to fetch exercises.");
  }

  return response.json();
}

export async function getExerciseById(id: string): Promise<ExerciseDetails | null> {
  const response = await fetch(`${API_BASE_URL}/api/exercises/${id}`, {
    cache: "no-store",
  });

  if (response.status == 404) {
    return null;
  }

  if (!response.ok) {
    throw new Error(`Failed to fetch exercise with id: ${id}`);
  }

  return response.json();
}
