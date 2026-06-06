import { useEffect, useState } from "react";
import { expenseReportsApi } from "@/features/expense-reports/api";
import { StatusBadge } from "@/features/expense-reports/StatusBadge";
import { formatKr, formatDate } from "@/lib/format";
import type {
  ExpenseReportResponse,
  ReceiptResponse,
  CategoryResponse,
} from "@/lib/types";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  report: ExpenseReportResponse | null;
  categories: CategoryResponse[];
}

export function ReportDetailDialog({ open, onOpenChange, report, categories }: Props) {
  const [receipts, setReceipts] = useState<ReceiptResponse[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!open || !report) return;
    let cancelled = false;
    setLoading(true);
    setError(null);
    expenseReportsApi
      .receipts(report.id)
      .then((data) => {
        if (!cancelled) setReceipts(data);
      })
      .catch((e) => {
        if (!cancelled) setError(e instanceof Error ? e.message : "Kunde inte hämta kvitton.");
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => {
      cancelled = true;
    };
  }, [open, report]);

  const categoryName = (id: string | null) =>
    id ? categories.find((c) => c.id === id)?.name ?? "—" : "—";

  const total = receipts.reduce((s, r) => s + r.totalAmount, 0);
  const totalVat = receipts.reduce((s, r) => s + r.vatAmount, 0);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <div className="flex items-center gap-3">
            <DialogTitle>{report?.title}</DialogTitle>
            {report && <StatusBadge status={report.status} />}
          </div>
          <DialogDescription>Kvitton kopplade till rapporten.</DialogDescription>
        </DialogHeader>

        {loading ? (
          <p className="py-6 text-sm text-muted-foreground">Laddar…</p>
        ) : error ? (
          <p className="py-6 text-sm text-destructive">{error}</p>
        ) : receipts.length === 0 ? (
          <p className="py-6 text-sm text-muted-foreground">Inga kvitton i rapporten ännu.</p>
        ) : (
          <div className="space-y-3">
            <div className="rounded-lg border border-border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Handlare</TableHead>
                    <TableHead>Datum</TableHead>
                    <TableHead>Kategori</TableHead>
                    <TableHead className="text-right">Moms</TableHead>
                    <TableHead className="text-right">Belopp</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {receipts.map((r) => (
                    <TableRow key={r.id}>
                      <TableCell className="font-medium">{r.merchant}</TableCell>
                      <TableCell className="tabular">{formatDate(r.date)}</TableCell>
                      <TableCell className="text-muted-foreground">
                        {categoryName(r.categoryId)}
                      </TableCell>
                      <TableCell className="tabular text-right">{formatKr(r.vatAmount)}</TableCell>
                      <TableCell className="tabular text-right">{formatKr(r.totalAmount)}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>

            <div className="flex items-center justify-between rounded-lg border border-border bg-secondary/40 px-4 py-3">
              <span className="text-sm text-muted-foreground">
                {receipts.length} kvitto{receipts.length === 1 ? "" : "n"} · varav moms{" "}
                <span className="tabular font-medium text-foreground">{formatKr(totalVat)}</span>
              </span>
              <span className="font-display text-lg font-semibold tabular">{formatKr(total)}</span>
            </div>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}