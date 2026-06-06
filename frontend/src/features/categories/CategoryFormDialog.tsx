import { useEffect, useState } from "react";
import { categoriesApi } from "@/features/categories/api";
import { ApiError } from "@/lib/api";
import type { CategoryResponse } from "@/lib/types";
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

const VAT_RATES = [
  { value: "0.25", label: "25 %" },
  { value: "0.12", label: "12 %" },
  { value: "0.06", label: "6 %" },
  { value: "0", label: "0 %" },
];

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  category: CategoryResponse | null; // null = create, set = edit
  onSaved: () => void;
}

export function CategoryFormDialog({ open, onOpenChange, category, onSaved }: Props) {
  const [name, setName] = useState("");
  const [basAccount, setBasAccount] = useState("");
  const [vatRate, setVatRate] = useState("0.25");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (open) {
      setName(category?.name ?? "");
      setBasAccount(category?.basAccount ?? "");
      setVatRate(category ? String(category.vatRate) : "0.25");
      setError(null);
    }
  }, [open, category]);

  async function handleSave() {
    setError(null);
    setSaving(true);
    const dto = { name, basAccount, vatRate: Number(vatRate) };
    try {
      if (category) await categoriesApi.update(category.id, dto);
      else await categoriesApi.create(dto);
      onSaved();
      onOpenChange(false);
    } catch (e) {
      setError(e instanceof ApiError ? e.message : "Kunde inte spara kategorin.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>{category ? "Redigera kategori" : "Ny kategori"}</DialogTitle>
          <DialogDescription>
            Ett BAS-konto som kvitton kan kategoriseras mot.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-2">
          <div className="space-y-2">
            <Label htmlFor="name">Namn</Label>
            <Input
              id="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="t.ex. Drivmedel"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="bas">BAS-konto</Label>
            <Input
              id="bas"
              value={basAccount}
              onChange={(e) => setBasAccount(e.target.value)}
              placeholder="t.ex. 5611"
              className="tabular"
            />
          </div>
          <div className="space-y-2">
            <Label htmlFor="vat">Moms</Label>
            <Select value={vatRate} onValueChange={setVatRate}>
              <SelectTrigger id="vat">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {VAT_RATES.map((r) => (
                  <SelectItem key={r.value} value={r.value}>
                    {r.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          {error && <p className="text-sm text-destructive">{error}</p>}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Avbryt
          </Button>
          <Button onClick={handleSave} disabled={saving || !name || !basAccount}>
            {saving ? "Sparar…" : "Spara"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}