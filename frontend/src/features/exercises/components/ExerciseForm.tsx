"use client";
import { useRouter } from "next/navigation";
import { ExerciseFormErrors, ExerciseFormField, SaveExerciseRequest } from "../types";
import { useState } from "react";
import type { SubmitEvent as ReactSubmitEvent } from "react";
import { createExercise, updateExercise } from "../api";

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

const muscleGroups: string[] = ["Chest", "Back", "Legs", "Shoulders", "Arms", "Core"];
const equipmentOptions: string[] = [
  "Barbell",
  "Dumbbell",
  "Bodyweight",
  "Machine",
  "Cable",
  "Kettlebell",
];
const difficultyLevels: string[] = ["Beginner", "Intermediate", "Advanced"];

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

  function updateInstructions(index: number, value: string) {
    setValues((current) => ({
      ...current,
      instructions: current.instructions.map((instruction, itemIndex) =>
        itemIndex === index ? value : instruction
      ),
    }));

    setErrors((current) => ({
      ...current,
      instructions: undefined,
    }));
  }
  function addInstruction() {
    setValues((current) => ({
      ...current,
      instructions: [...current.instructions, ""],
    }));
  }

  function removeInstruction(index: number) {
    setValues((current) => ({
      ...current,
      instructions:
        current.instructions.length === 1
          ? [""]
          : current.instructions.filter((_, itemIndex) => itemIndex !== index),
    }));
  }

  async function handleSubmit(event: ReactSubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    setErrors({});
    setIsSubmitting(true);

    try {
      const result =
        props.mode === "create"
          ? await createExercise(values)
          : await updateExercise(props.exerciseId, values);

      if (!result.ok) {
        setErrors(result.errors);
        return;
      }

      router.push(`/exercises/${encodeURIComponent(result.exercise.id)}`);
    } catch {
      setErrors({
        form: ["The exercise could not be saved. Please try again."],
      });
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      {errors.form?.map((message) => (
        <p key={message} role="alert">
          {message}
        </p>
      ))}

      <div>
        <label htmlFor="name">Name</label>
        <input
          id="name"
          name="name"
          value={values.name}
          onChange={(event) => updateField("name", event.target.value)}
          aria-invalid={Boolean(errors.name?.length)}
          aria-describedby={errors.name ? "name-errors" : undefined}
          required
        />

        {errors.name?.map((message) => (
          <p id="name-errors" key={message}>
            {message}
          </p>
        ))}
      </div>

      <div>
        <label htmlFor="description">Description</label>
        <textarea
          id="description"
          name="description"
          value={values.description}
          onChange={(event) => updateField("description", event.target.value)}
          aria-invalid={Boolean(errors.description?.length)}
          aria-describedby={errors.description ? "description-errors" : undefined}
          required
        />

        {errors.description?.map((message) => (
          <p id="description-errors" key={message}>
            {message}
          </p>
        ))}
      </div>

      <fieldset>
        <legend>Instructions</legend>

        {values.instructions.map((instruction, index) => (
          <div key={index}>
            <label htmlFor={`instruction-${index}`}>Instruction {index + 1}</label>
            <input
              id={`instruction-${index}`}
              value={instruction}
              onChange={(event) => updateInstructions(index, event.target.value)}
            />
            <button type="button" onClick={() => removeInstruction(index)}>
              Remove
            </button>
          </div>
        ))}

        <button type="button" onClick={addInstruction}>
          Add instruction
        </button>

        {errors.instructions?.map((message) => (
          <p key={message}>{message}</p>
        ))}
      </fieldset>

      <label htmlFor="muscleGroup">Muscle group</label>
      <select
        id="muscleGroup"
        value={values.muscleGroup}
        onChange={(event) => updateField("muscleGroup", event.target.value)}
      >
        {muscleGroups.map((group) => (
          <option key={group} value={group}>
            {group}
          </option>
        ))}
      </select>

      <div>
        <label htmlFor="equipment">Equipment</label>
        <select
          id="equipment"
          name="equipment"
          value={values.equipment}
          onChange={(event) => updateField("equipment", event.target.value)}
          aria-invalid={Boolean(errors.equipment?.length)}
          aria-describedby={errors.equipment ? "equipment-errors" : undefined}
          required
        >
          {equipmentOptions.map((equipment) => (
            <option key={equipment} value={equipment}>
              {equipment}
            </option>
          ))}
        </select>

        {errors.equipment?.map((message) => (
          <p id="equipment-errors" key={message}>
            {message}
          </p>
        ))}
      </div>

      <div>
        <label htmlFor="difficultyLevel">Difficulty level</label>
        <select
          id="difficultyLevel"
          name="difficultyLevel"
          value={values.difficultyLevel}
          onChange={(event) => updateField("difficultyLevel", event.target.value)}
          aria-invalid={Boolean(errors.difficultyLevel?.length)}
          aria-describedby={errors.difficultyLevel ? "difficultyLevel-errors" : undefined}
          required
        >
          {difficultyLevels.map((difficultyLevel) => (
            <option key={difficultyLevel} value={difficultyLevel}>
              {difficultyLevel}
            </option>
          ))}
        </select>

        {errors.difficultyLevel?.map((message) => (
          <p id="difficultyLevel-errors" key={message}>
            {message}
          </p>
        ))}
      </div>

      <button type="submit" disabled={isSubmitting}>
        {isSubmitting ? "Saving..." : props.mode === "create" ? "Create exercise" : "Save changes"}
      </button>
    </form>
  );
}
