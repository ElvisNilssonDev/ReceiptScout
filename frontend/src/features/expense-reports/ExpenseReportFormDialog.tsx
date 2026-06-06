import { useEffect, useState } from "react";
import { expenseReportsApi } from "@/features/expense-reports/api";
import { ApiError } from "@/lib/api";
import type { ExpenseReportResponse } from "@/lib/types";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  report: ExpenseReportResponse | null; // null = create, set = edit
  onSaved: () => void;
}

export function ExpenseReportFormDialog({ open, onOpenChange, report, onSaved }: Props) {
  const [title, setTitle] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (open) {
      setTitle(report?.title ?? "");
      setError(null);
    }
  }, [open, report]);

  async function handleSave() {
    setError(null);
    setSaving(true);
    try {
      if (report) await expenseReportsApi.update(report.id, { title });
      else await expenseReportsApi.create({ title });
      onSaved();
      onOpenChange(false);
    } catch (e) {
      setError(e instanceof ApiError ? e.message : "Kunde inte spara rapporten.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{report ? "Redigera rapport" : "Ny utläggsrapport"}</DialogTitle>
          <DialogDescription>
            En rapport samlar kvitton och skickas in för godkännande.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-2">
          <div className="space-y-2">
            <Label htmlFor="title">Titel</Label>
            <Input
              id="title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="t.ex. Tjänsteresa Stockholm, maj"
            />
          </div>
          {error && <p className="text-sm text-destructive">{error}</p>}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Avbryt
          </Button>
          <Button onClick={handleSave} disabled={saving || !title}>
            {saving ? "Sparar…" : "Spara"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}