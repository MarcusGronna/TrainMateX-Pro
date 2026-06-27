import { ExerciseDetail } from "@/features/exercises/components/ExerciseDetail";

type ExerciseDetailsPageProps = {
  params: Promise<{
    id: string;
  }>;
};

export default async function ExerciseDetailsPage({ params }: ExerciseDetailsPageProps) {
  const { id } = await params;

  return (
    <main>
      <ExerciseDetail exerciseId={id} />
    </main>
  );
}
