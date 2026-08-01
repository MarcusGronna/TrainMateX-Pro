"use client";
import { useRouter } from "next/navigation";
import { ExerciseFormErrors, ExerciseFormField, SaveExerciseRequest } from "../types";
import { useState } from "react";

type ExerciseFormProps =
  | {
      mode: "create";
      initialValues?: never;
      exerciseId?: never;
    }
  | {
      mode: "edit";
      initialValues: SaveExerciseRequest;
      exerciseId: string;
    };

type ScalarField = Exclude<ExerciseFormField, "instructions">;

const emptyExercise: SaveExerciseRequest = {
  name: "",
  description: "",
  instructions: [""],
  muscleGroup: "Chest",
  equipment: "Barbell",
  difficultyLevel: "Beginner",
};

const muscleGroup = ["Chest", "Back", "Legs", "Shoulders", "Arms", "Core"];
const equipmentOptions = ["Barbell", "Dumbell", "Bodyweight", "Machine", "Cable", "Kettlebell"];
const difficultyLevels = ["Beginner", "Intermediate", "Advanced"];

export function ExerciseForm(props: ExerciseFormProps) {
  const router = useRouter();

  const [values, setValues] = useState<SaveExerciseRequest>(() => {
    const initial = props.mode === "edit" ? props.initialValues : emptyExercise;

    return {
      ...initial,
      instructions: [...initial.instructions],
    };
  });

  const [errors, setErrors] = useState<ExerciseFormErrors>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  function updateField(field: ScalarField, value: string) {
    setValues((current) => ({
      ...current,
      [field]: value,
    }));

    setErrors((current) => ({
      ...current,
      [field]: undefined,
    }));
  }
}
