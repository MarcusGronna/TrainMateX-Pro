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

const controlStyles =
  "w-full rounded-xl border border-gray-300 bg-white px-3 py-2 text-gray-900 shadow-sm outline-none focus:border-purple-500 focus:ring-2 focus:ring-purple-200 dark:border-gray-700 dark:bg-gray-900 dark:text-white dark:focus:border-purple-400 dark:focus:ring-purple-900";

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
    <form
      onSubmit={handleSubmit}
      className="mx-auto max-w-3xl space-y-6 rounded-2xl border border-gray-200 bg-white p-6 text-gray-900 shadow-sm dark:border-gray-700 dark:bg-gray-900 dark:text-gray-100"
    >
      {errors.form?.map((message) => (
        <p
          key={message}
          role="alert"
          className="rounded-xl bg-red-50 p-3 text-sm font-medium text-red-700"
        >
          {message}
        </p>
      ))}

      <div className="space-y-2">
        <label htmlFor="name" className="text-sm font-bold">
          Name
        </label>
        <input
          id="name"
          name="name"
          value={values.name}
          onChange={(event) => updateField("name", event.target.value)}
          aria-invalid={Boolean(errors.name?.length)}
          aria-describedby={errors.name ? "name-errors" : undefined}
          required
          className={controlStyles}
        />

        {errors.name?.map((message) => (
          <p id="name-errors" key={message} className="text-sm text-red-600">
            {message}
          </p>
        ))}
      </div>

      <div className="space-y-2">
        <label htmlFor="description" className="text-sm font-bold">
          Description
        </label>
        <textarea
          id="description"
          name="description"
          value={values.description}
          onChange={(event) => updateField("description", event.target.value)}
          aria-invalid={Boolean(errors.description?.length)}
          aria-describedby={errors.description ? "description-errors" : undefined}
          required
          rows={4}
          className={controlStyles}
        />

        {errors.description?.map((message) => (
          <p id="description-errors" key={message} className="text-sm text-red-600">
            {message}
          </p>
        ))}
      </div>

      <fieldset className="space-y-3 rounded-xl border border-gray-200 p-4">
        <legend className="px-1 text-sm font-bold">Instructions</legend>

        {values.instructions.map((instruction, index) => (
          <div key={index} className="flex gap-2">
            <input
              id={`instruction-${index}`}
              value={instruction}
              onChange={(event) => updateInstructions(index, event.target.value)}
              aria-label={`Instruction ${index + 1}`}
              className={controlStyles}
            />
            <button
              type="button"
              onClick={() => removeInstruction(index)}
              className="rounded-xl px-3 py-2 text-sm font-medium text-red-600 hover:bg-red-50"
            >
              Remove
            </button>
          </div>
        ))}

        <button
          type="button"
          onClick={addInstruction}
          className="rounded-xl border border-purple-200 px-3 py-2 text-sm font-medium text-purple-700 hover:bg-purple-50"
        >
          Add instruction
        </button>

        {errors.instructions?.map((message) => (
          <p key={message} className="text-sm text-red-600">
            {message}
          </p>
        ))}
      </fieldset>

      <div className="grid gap-4 sm:grid-cols-3">
        <div className="space-y-2">
          <label htmlFor="muscleGroup" className="text-sm font-bold">
            Muscle group
          </label>
          <select
            id="muscleGroup"
            name="muscleGroup"
            value={values.muscleGroup}
            onChange={(event) => updateField("muscleGroup", event.target.value)}
            aria-invalid={Boolean(errors.muscleGroup?.length)}
            aria-describedby={errors.muscleGroup ? "muscleGroup-errors" : undefined}
            required
            className={controlStyles}
          >
            {muscleGroups.map((group) => (
              <option
                key={group}
                value={group}
                className="bg-white text-gray-900 dark:bg-gray-900 dark:text-white"
              >
                {group}
              </option>
            ))}
          </select>

          {errors.muscleGroup?.map((message) => (
            <p id="muscleGroup-errors" key={message} className="text-sm text-red-600">
              {message}
            </p>
          ))}
        </div>

        <div className="space-y-2">
          <label htmlFor="equipment" className="text-sm font-bold">
            Equipment
          </label>
          <select
            id="equipment"
            name="equipment"
            value={values.equipment}
            onChange={(event) => updateField("equipment", event.target.value)}
            aria-invalid={Boolean(errors.equipment?.length)}
            aria-describedby={errors.equipment ? "equipment-errors" : undefined}
            required
            className={controlStyles}
          >
            {equipmentOptions.map((equipment) => (
              <option
                key={equipment}
                value={equipment}
                className="bg-white text-gray-900 dark:bg-gray-900 dark:text-white"
              >
                {equipment}
              </option>
            ))}
          </select>

          {errors.equipment?.map((message) => (
            <p id="equipment-errors" key={message} className="text-sm text-red-600">
              {message}
            </p>
          ))}
        </div>

        <div className="space-y-2">
          <label htmlFor="difficultyLevel" className="text-sm font-bold">
            Difficulty
          </label>
          <select
            id="difficultyLevel"
            name="difficultyLevel"
            value={values.difficultyLevel}
            onChange={(event) => updateField("difficultyLevel", event.target.value)}
            aria-invalid={Boolean(errors.difficultyLevel?.length)}
            aria-describedby={errors.difficultyLevel ? "difficultyLevel-errors" : undefined}
            required
            className={controlStyles}
          >
            {difficultyLevels.map((level) => (
              <option
                key={level}
                value={level}
                className="bg-white text-gray-900 dark:bg-gray-900 dark:text-white"
              >
                {level}
              </option>
            ))}
          </select>

          {errors.difficultyLevel?.map((message) => (
            <p id="difficultyLevel-errors" key={message} className="text-sm text-red-600">
              {message}
            </p>
          ))}
        </div>
      </div>

      <button
        type="submit"
        disabled={isSubmitting}
        className="rounded-xl bg-purple-600 px-4 py-2 font-medium text-white shadow-sm transition hover:bg-purple-700 disabled:cursor-not-allowed disabled:opacity-60"
      >
        {isSubmitting ? "Saving..." : props.mode === "create" ? "Create exercise" : "Save changes"}
      </button>
    </form>
  );
}
