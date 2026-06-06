import { useState } from "react";
import { Plus, Pencil, Trash2 } from "lucide-react";
import { useReceipts } from "@/features/receipts/useReceipts";
import { receiptsApi } from "@/features/receipts/api";
import { ReceiptFormDialog } from "@/features/receipts/ReceiptFormDialog";
import { useCategories } from "@/features/categories/useCategories";
import { useExpenseReports } from "@/features/expense-reports/useExpenseReports";
import { formatKr, formatDate } from "@/lib/format";
import type { ReceiptResponse } from "@/lib/types";
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

export function ReceiptsPage() {
  const { receipts, loading, error, refetch } = useReceipts();
  const { categories } = useCategories();
  const { reports } = useExpenseReports();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<ReceiptResponse | null>(null);
  const [deleting, setDeleting] = useState<ReceiptResponse | null>(null);

  const categoryName = (id: string | null) =>
    id ? categories.find((c) => c.id === id)?.name ?? "—" : "—";

  function openCreate() {
    setEditing(null);
    setDialogOpen(true);
  }

  function openEdit(receipt: ReceiptResponse) {
    setEditing(receipt);
    setDialogOpen(true);
  }

  async function confirmDelete() {
    if (!deleting) return;
    try {
      await receiptsApi.remove(deleting.id);
      refetch();
    } finally {
      setDeleting(null);
    }
  }

  return (
    <>
      <PageHeader
        title="Kvitton"
        subtitle="Registrera, kategorisera och samla kvitton."
        actions={
          <Button onClick={openCreate}>
            <Plus className="h-4 w-4" />
            Nytt kvitto
          </Button>
        }
      />

      <div className="rounded-xl border border-border bg-card">
        {loading ? (
          <p className="p-6 text-sm text-muted-foreground">Laddar…</p>
        ) : error ? (
          <p className="p-6 text-sm text-destructive">{error}</p>
        ) : receipts.length === 0 ? (
          <p className="p-6 text-sm text-muted-foreground">Inga kvitton ännu.</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Handlare</TableHead>
                <TableHead>Datum</TableHead>
                <TableHead className="text-right">Belopp</TableHead>
                <TableHead className="text-right">Moms</TableHead>
                <TableHead>Kategori</TableHead>
                <TableHead className="w-0" />
              </TableRow>
            </TableHeader>
            <TableBody>
              {receipts.map((r) => (
                <TableRow key={r.id}>
                  <TableCell className="font-medium">{r.merchant}</TableCell>
                  <TableCell className="tabular">{formatDate(r.date)}</TableCell>
                  <TableCell className="tabular text-right">{formatKr(r.totalAmount)}</TableCell>
                  <TableCell className="tabular text-right">{formatKr(r.vatAmount)}</TableCell>
                  <TableCell className="text-muted-foreground">{categoryName(r.categoryId)}</TableCell>
                  <TableCell className="whitespace-nowrap text-right">
                    <Button variant="ghost" size="icon" onClick={() => openEdit(r)}>
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button variant="ghost" size="icon" onClick={() => setDeleting(r)}>
                      <Trash2 className="h-4 w-4 text-destructive" />
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </div>

      <ReceiptFormDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        receipt={editing}
        categories={categories}
        reports={reports}
        onSaved={refetch}
      />

      <AlertDialog open={deleting !== null} onOpenChange={(o) => !o && setDeleting(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Ta bort kvitto?</AlertDialogTitle>
            <AlertDialogDescription>
              Kvittot från "{deleting?.merchant}" tas bort permanent.
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