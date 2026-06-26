import Link from "next/link";

export default function Home() {
  return (
    <main className="text-center">
      <h1 className="">TrainMateX PRO</h1>
      <Link
        href="/exercises"
        className="rounded-full bg-purple-100 px-2.5 py-1 font-medium text-purple-700 hover:bg-purple-600 hover:text-purple-100"
      >
        Exercise Library
      </Link>
    </main>
  );
}
