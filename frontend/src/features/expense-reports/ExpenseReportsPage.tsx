import { PageHeader } from "@/components/layout/PageHeader";

export function ExpenseReportsPage() {
  return (
    <>
      <PageHeader title="Utläggsrapporter" subtitle="Samla kvitton och skicka för godkännande." />
      <div className="rounded-xl border border-border bg-card p-6 text-sm text-muted-foreground">
        Rapporthantering kommer här.
      </div>
    </>
  );
}