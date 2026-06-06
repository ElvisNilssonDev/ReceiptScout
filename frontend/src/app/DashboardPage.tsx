import { PageHeader } from "@/components/layout/PageHeader";
import { useAuth } from "@/lib/auth";

export function DashboardPage() {
  const { email } = useAuth();
  return (
    <>
      <PageHeader title="Instrumentpanel" subtitle={`Välkommen, ${email}`} />
      <div className="rounded-xl border border-border bg-card p-6 text-sm text-muted-foreground">
        Nyckeltal och senaste kvitton kommer här.
      </div>
    </>
  );
}