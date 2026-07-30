import {
  ExerciseDetails,
  ExerciseFormErrors,
  ExerciseFormField,
  ExerciseListItem,
  SaveExerciseRequest,
  SaveExerciseResult,
} from "./types";

type ApiErrors = Record<string, string[]>;

type ValidationProblemDetails = {
  errors?: ApiErrors;
};

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL ?? "http://localhost:5193";

const fieldByApiKey: Record<string, ExerciseFormField> = {
  Name: "name",
  Description: "description",
  Instructions: "instructions",
  MuscleGroup: "muscleGroup",
  Equipment: "equipment",
  DifficultyLevel: "difficultyLevel",
};

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
  const response = await fetch(`${API_BASE_URL}/api/exercises/${encodeURIComponent(id)}`, {
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

export async function createExercise(request: SaveExerciseRequest): Promise<SaveExerciseResult> {
  const response = await fetch(`${API_BASE_URL}/api/exercises`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (response.ok) {
    const exercise = (await response.json()) as ExerciseDetails;
    return { ok: true, exercise };
  }

  if (response.status == 400) {
    const problem = (await response.json()) as ValidationProblemDetails;

    return {
      ok: false,
      status: 400,
      errors: normalizeErrors(problem.errors ?? {}),
    };
  }

  if (response.status == 409) {
    const errors = (await response.json()) as ApiErrors;

    return {
      ok: false,
      status: 409,
      errors: normalizeErrors(errors),
    };
  }

  throw new Error(`Unexpected create response: ${response.status}`);
}

export async function updateExercise(
  id: string,
  request: SaveExerciseRequest
): Promise<SaveExerciseResult> {
  const response = await fetch(`${API_BASE_URL}/api/exercises/${encodeURIComponent(id)}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(request),
  });

  if (response.status == 400) {
    const problem = (await response.json()) as ValidationProblemDetails;

    return {
      ok: false,
      status: 400,
      errors: normalizeErrors(problem.errors ?? {}),
    };
  }

  if (response.status == 404) {
    return {
      ok: false,
      status: 404,
      errors: {
        form: ["The exercise was not found."],
      },
    };
  }

  if (!response.ok) {
    const errorMessage = await response.text();

    throw new Error(`Failed to update exercise: ${response.status} ${errorMessage}`);
  }

  const exercise = (await response.json()) as ExerciseDetails;

  return { ok: true, exercise };
}

function normalizeErrors(errors: ApiErrors): ExerciseFormErrors {
  const normalized: ExerciseFormErrors = {};

  for (const [apiKey, messages] of Object.entries(errors)) {
    const field = fieldByApiKey[apiKey];

    if (field) {
      normalized[field] = messages;
    } else {
      normalized.form = [...(normalized.form ?? []), ...messages];
    }
  }

  return normalized;
}
