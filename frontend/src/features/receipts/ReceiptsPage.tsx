import { PageHeader } from "@/components/layout/PageHeader";

export function ReceiptsPage() {
  return (
    <>
      <PageHeader title="Kvitton" subtitle="Registrera, kategorisera och samla kvitton." />
      <div className="rounded-xl border border-border bg-card p-6 text-sm text-muted-foreground">
        Kvittolistan byggs härnäst.
      </div>
    </>
  );
}