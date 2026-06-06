import { useState } from "react";
import { Plus, Pencil, Trash2, Send, Check, X } from "lucide-react";
import { useAuth } from "@/lib/auth";
import { useExpenseReports } from "@/features/expense-reports/useExpenseReports";
import { expenseReportsApi } from "@/features/expense-reports/api";
import { ExpenseReportFormDialog } from "@/features/expense-reports/ExpenseReportFormDialog";
import { StatusBadge } from "@/features/expense-reports/StatusBadge";
import { ApiError } from "@/lib/api";
import { formatDate } from "@/lib/format";
import { ExpenseReportStatus, type ExpenseReportResponse } from "@/lib/types";
import { PageHeader } from "@/components/layout/PageHeader";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";

export function ExpenseReportsPage() {
  const { isAdmin } = useAuth();
  const { reports, loading, error, refetch } = useExpenseReports();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<ExpenseReportResponse | null>(null);
  const [deleting, setDeleting] = useState<ExpenseReportResponse | null>(null);
  const [actionError, setActionError] = useState<string | null>(null);

  function openCreate() {
    setEditing(null);
    setDialogOpen(true);
  }

  function openEdit(report: ExpenseReportResponse) {
    setEditing(report);
    setDialogOpen(true);
  }

  async function runAction(fn: () => Promise<unknown>) {
    setActionError(null);
    try {
      await fn();
      refetch();
    } catch (e) {
      setActionError(e instanceof ApiError ? e.message : "Åtgärden misslyckades.");
    }
  }

  async function confirmDelete() {
    if (!deleting) return;
    try {
      await expenseReportsApi.remove(deleting.id);
      refetch();
    } finally {
      setDeleting(null);
    }
  }

  return (
    <>
      <PageHeader
        title="Utläggsrapporter"
        subtitle="Samla kvitton och skicka för godkännande."
        actions={
          <Button onClick={openCreate}>
            <Plus className="h-4 w-4" />
            Ny rapport
          </Button>
        }
      />

      {actionError && <p className="mb-4 text-sm text-destructive">{actionError}</p>}

      <div className="rounded-xl border border-border bg-card">
        {loading ? (
          <p className="p-6 text-sm text-muted-foreground">Laddar…</p>
        ) : error ? (
          <p className="p-6 text-sm text-destructive">{error}</p>
        ) : reports.length === 0 ? (
          <p className="p-6 text-sm text-muted-foreground">Inga rapporter ännu.</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Titel</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Skapad</TableHead>
                <TableHead>Inskickad</TableHead>
                <TableHead className="w-0" />
              </TableRow>
            </TableHeader>
            <TableBody>
              {reports.map((r) => {
                const isDraft = r.status === ExpenseReportStatus.Draft;
                const isSubmitted = r.status === ExpenseReportStatus.Submitted;
                return (
                  <TableRow key={r.id}>
                    <TableCell className="font-medium">{r.title}</TableCell>
                    <TableCell><StatusBadge status={r.status} /></TableCell>
                    <TableCell className="tabular">{formatDate(r.createdAt)}</TableCell>
                    <TableCell className="tabular text-muted-foreground">
                      {r.submittedAt ? formatDate(r.submittedAt) : "—"}
                    </TableCell>
                    <TableCell className="whitespace-nowrap text-right">
                      <div className="flex items-center justify-end gap-1">
                        {isDraft && (
                          <>
                            <Button
                              variant="outline"
                              size="sm"
                              onClick={() => runAction(() => expenseReportsApi.submit(r.id))}
                            >
                              <Send className="h-3.5 w-3.5" />
                              Skicka in
                            </Button>
                            <Button variant="ghost" size="icon" onClick={() => openEdit(r)}>
                              <Pencil className="h-4 w-4" />
                            </Button>
                          </>
                        )}
                        {isSubmitted && isAdmin && (
                          <>
                            <Button
                              size="sm"
                              onClick={() => runAction(() => expenseReportsApi.approve(r.id))}
                            >
                              <Check className="h-3.5 w-3.5" />
                              Godkänn
                            </Button>
                            <Button
                              variant="outline"
                              size="sm"
                              onClick={() => runAction(() => expenseReportsApi.reject(r.id))}
                            >
                              <X className="h-3.5 w-3.5" />
                              Avslå
                            </Button>
                          </>
                        )}
                        <Button variant="ghost" size="icon" onClick={() => setDeleting(r)}>
                          <Trash2 className="h-4 w-4 text-destructive" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        )}
      </div>

      <ExpenseReportFormDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        report={editing}
        onSaved={refetch}
      />

      <AlertDialog open={deleting !== null} onOpenChange={(o) => !o && setDeleting(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Ta bort rapport?</AlertDialogTitle>
            <AlertDialogDescription>
              "{deleting?.title}" tas bort. Kvitton i rapporten kopplas loss men raderas inte.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Avbryt</AlertDialogCancel>
            <AlertDialogAction onClick={confirmDelete}>Ta bort</AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}