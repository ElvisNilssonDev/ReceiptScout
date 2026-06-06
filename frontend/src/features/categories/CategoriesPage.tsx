import { useState } from "react";
import { Plus, Pencil, Trash2 } from "lucide-react";
import { useAuth } from "@/lib/auth";
import { useCategories } from "@/features/categories/useCategories";
import { categoriesApi } from "@/features/categories/api";
import { CategoryFormDialog } from "@/features/categories/CategoryFormDialog";
import { formatVatRate } from "@/lib/format";
import type { CategoryResponse } from "@/lib/types";
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

export function CategoriesPage() {
  const { isAdmin } = useAuth();
  const { categories, loading, error, refetch } = useCategories();

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editing, setEditing] = useState<CategoryResponse | null>(null);
  const [deleting, setDeleting] = useState<CategoryResponse | null>(null);

  function openCreate() {
    setEditing(null);
    setDialogOpen(true);
  }

  function openEdit(category: CategoryResponse) {
    setEditing(category);
    setDialogOpen(true);
  }

  async function confirmDelete() {
    if (!deleting) return;
    try {
      await categoriesApi.remove(deleting.id);
      refetch();
    } finally {
      setDeleting(null);
    }
  }

  return (
    <>
      <PageHeader
        title="Kategorier"
        subtitle="BAS-konton som kvitton kategoriseras mot."
        actions={
          isAdmin && (
            <Button onClick={openCreate}>
              <Plus className="h-4 w-4" />
              Ny kategori
            </Button>
          )
        }
      />

      <div className="rounded-xl border border-border bg-card">
        {loading ? (
          <p className="p-6 text-sm text-muted-foreground">Laddar…</p>
        ) : error ? (
          <p className="p-6 text-sm text-destructive">{error}</p>
        ) : categories.length === 0 ? (
          <p className="p-6 text-sm text-muted-foreground">Inga kategorier ännu.</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Namn</TableHead>
                <TableHead>BAS-konto</TableHead>
                <TableHead>Moms</TableHead>
                {isAdmin && <TableHead className="w-0" />}
              </TableRow>
            </TableHeader>
            <TableBody>
              {categories.map((c) => (
                <TableRow key={c.id}>
                  <TableCell className="font-medium">{c.name}</TableCell>
                  <TableCell className="tabular">{c.basAccount}</TableCell>
                  <TableCell className="tabular">{formatVatRate(c.vatRate)}</TableCell>
                  {isAdmin && (
                    <TableCell className="whitespace-nowrap text-right">
                      <Button variant="ghost" size="icon" onClick={() => openEdit(c)}>
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button variant="ghost" size="icon" onClick={() => setDeleting(c)}>
                        <Trash2 className="h-4 w-4 text-destructive" />
                      </Button>
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}
      </div>

      <CategoryFormDialog
        open={dialogOpen}
        onOpenChange={setDialogOpen}
        category={editing}
        onSaved={refetch}
      />

      <AlertDialog open={deleting !== null} onOpenChange={(o) => !o && setDeleting(null)}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Ta bort kategori?</AlertDialogTitle>
            <AlertDialogDescription>
              "{deleting?.name}" tas bort. Kvitton kopplade till den blir okategoriserade.
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