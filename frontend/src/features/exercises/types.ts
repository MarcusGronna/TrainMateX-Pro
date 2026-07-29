export type ExerciseListItem = {
  id: string;
  name: string;
  muscleGroup: string;
  difficultyLevel: string;
};

export type ExerciseDetails = {
  id: string;
  name: string;
  description: string;
  instructions: string[];
  muscleGroup: string;
  equipment: string;
  difficultyLevel: string;
};

export type SaveExerciseRequest = Omit<ExerciseDetails, "id">;

export type ExerciseFormField = keyof SaveExerciseRequest;

export type ExerciseFormErrors = Partial<Record<ExerciseFormField | "form", string[]>>;

export type SaveExerciseResult =
  | { ok: true; exercise: ExerciseDetails }
  | {
      ok: false;
      status: 400 | 404 | 409;
      errors: ExerciseFormErrors;
    };
