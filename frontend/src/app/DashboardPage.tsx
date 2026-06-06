import { Link } from "react-router-dom";
import { ReceiptText, Banknote, Percent, FileCheck2, Plus } from "lucide-react";
import type { ComponentType } from "react";
import { useReceipts } from "@/features/receipts/useReceipts";
import { useExpenseReports } from "@/features/expense-reports/useExpenseReports";
import { useCategories } from "@/features/categories/useCategories";
import { formatKr, formatDate } from "@/lib/format";
import { ExpenseReportStatus } from "@/lib/types";
import { useAuth } from "@/lib/auth";
import { PageHeader } from "@/components/layout/PageHeader";

function StatCard({
  icon: Icon,
  label,
  value,
  sub,
}: {
  icon: ComponentType<{ className?: string }>;
  label: string;
  value: string;
  sub?: string;
}) {
  return (
    <div className="rounded-xl border border-border bg-card p-5">
      <div className="flex items-center gap-2 text-muted-foreground">
        <span className="flex h-7 w-7 items-center justify-center rounded-md bg-primary/10 text-primary">
          <Icon className="h-4 w-4" />
        </span>
        <span className="text-sm font-medium">{label}</span>
      </div>
      <p className="mt-3 font-display text-2xl font-semibold tabular tracking-tight">{value}</p>
      {sub && <p className="mt-1 text-xs text-muted-foreground">{sub}</p>}
    </div>
  );
}

export function DashboardPage() {
  const { email } = useAuth();
  const { receipts, loading: receiptsLoading } = useReceipts();
  const { reports, loading: reportsLoading } = useExpenseReports();
  const { categories } = useCategories();

  const loading = receiptsLoading || reportsLoading;

  const totalAmount = receipts.reduce((s, r) => s + r.totalAmount, 0);
  const totalVat = receipts.reduce((s, r) => s + r.vatAmount, 0);
  const submittedCount = reports.filter(
    (r) => r.status === ExpenseReportStatus.Submitted,
  ).length;

  const categoryName = (id: string | null) =>
    id ? categories.find((c) => c.id === id)?.name ?? "—" : "—";

  const recent = [...receipts]
    .sort((a, b) => b.createdAt.localeCompare(a.createdAt))
    .slice(0, 5);

  const v = (value: string) => (loading ? "…" : value);

  return (
    <>
      <PageHeader title="Instrumentpanel" subtitle={`Välkommen, ${email}`} />

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard icon={ReceiptText} label="Kvitton" value={v(String(receipts.length))} sub="registrerade totalt" />
        <StatCard icon={Banknote} label="Total summa" value={v(formatKr(totalAmount))} sub="inkl. moms" />
        <StatCard icon={Percent} label="Varav moms" value={v(formatKr(totalVat))} />
        <StatCard
          icon={FileCheck2}
          label="Rapporter"
          value={v(String(reports.length))}
          sub={submittedCount > 0 ? `${submittedCount} väntar på godkännande` : "inga inskickade"}
        />
      </div>

      <div className="mt-6 rounded-xl border border-border bg-card">
        <div className="flex items-center justify-between border-b border-border px-5 py-3.5">
          <h2 className="font-display text-sm font-semibold tracking-tight">Senaste kvitton</h2>
          <Link to="/receipts" className="text-xs font-medium text-primary hover:underline">
            Visa alla
          </Link>
        </div>

        {loading ? (
          <p className="p-5 text-sm text-muted-foreground">Laddar…</p>
        ) : recent.length === 0 ? (
          <div className="flex flex-col items-start gap-3 p-5">
            <p className="text-sm text-muted-foreground">Inga kvitton ännu.</p>
            <Link
              to="/receipts"
              className="inline-flex items-center gap-1.5 rounded-md bg-primary px-3 py-1.5 text-sm font-medium text-primary-foreground hover:opacity-90"
            >
              <Plus className="h-4 w-4" />
              Lägg till ditt första kvitto
            </Link>
          </div>
        ) : (
          <ul className="divide-y divide-border">
            {recent.map((r) => (
              <li key={r.id} className="flex items-center justify-between px-5 py-3">
                <div className="min-w-0">
                  <p className="truncate text-sm font-medium">{r.merchant}</p>
                  <p className="text-xs text-muted-foreground">
                    {formatDate(r.date)} · {categoryName(r.categoryId)}
                  </p>
                </div>
                <span className="tabular text-sm font-medium">{formatKr(r.totalAmount)}</span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </>
  );
}