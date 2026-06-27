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
