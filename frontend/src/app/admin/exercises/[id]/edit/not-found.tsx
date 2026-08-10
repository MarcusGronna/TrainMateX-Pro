import Link from "next/link";

export default function EditExerciseNotFound() {
  return (
    <main>
      <h1>Exercise not found</h1>
      <p>The request exercise could not be edited.</p>
      <Link href="/admin/exercises">Return to exercise management</Link>
    </main>
  );
}
