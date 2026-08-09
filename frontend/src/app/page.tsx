import Link from "next/link";

export default function Home() {
  return (
    <main className="mx-auto flex min-h-[60vh] max-w-5xl flex-col items-center justify-center gap-6 px-4 py-8 text-center text-gray-900 dark:text-gray-100">
      <h1 className="text-4xl font-bold tracking-tight">TrainMateX PRO</h1>
      <Link
        href="/exercises"
        className="rounded-full bg-purple-100 px-4 py-2 font-medium text-purple-700 transition hover:bg-purple-600 hover:text-purple-100 dark:bg-purple-950 dark:text-purple-200 dark:hover:bg-purple-700 dark:hover:text-white"
      >
        Exercise Library
      </Link>
    </main>
  );
}
