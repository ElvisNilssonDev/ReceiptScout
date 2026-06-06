import { useEffect, useState } from "react";
import { Sparkles } from "lucide-react";
import { receiptsApi } from "@/features/receipts/api";
import { ApiError } from "@/lib/api";
import { formatVatRate } from "@/lib/format";
import type {
  ReceiptResponse,
  CategoryResponse,
  ExpenseReportResponse,
  CategorySuggestion,
} from "@/lib/types";
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

const NONE = "none";

interface Props {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  receipt: ReceiptResponse | null; // null = create, set = edit
  categories: CategoryResponse[];
  reports: ExpenseReportResponse[];
  onSaved: () => void;
}

const today = () => new Date().toISOString().slice(0, 10);

export function ReceiptFormDialog({
  open,
  onOpenChange,
  receipt,
  categories,
  reports,
  onSaved,
}: Props) {
  const [merchant, setMerchant] = useState("");
  const [date, setDate] = useState(today());
  const [totalAmount, setTotalAmount] = useState("");
  const [vatAmount, setVatAmount] = useState("");
  const [description, setDescription] = useState("");
  const [imageUrl, setImageUrl] = useState("");
  const [categoryId, setCategoryId] = useState<string>(NONE);
  const [expenseReportId, setExpenseReportId] = useState<string>(NONE);

  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [suggesting, setSuggesting] = useState(false);
  const [suggestion, setSuggestion] = useState<CategorySuggestion | null>(null);

  useEffect(() => {
    if (open) {
      setMerchant(receipt?.merchant ?? "");
      setDate(receipt ? receipt.date.slice(0, 10) : today());
      setTotalAmount(receipt ? String(receipt.totalAmount) : "");
      setVatAmount(receipt ? String(receipt.vatAmount) : "");
      setDescription(receipt?.description ?? "");
      setImageUrl(receipt?.imageUrl ?? "");
      setCategoryId(receipt?.categoryId ?? NONE);
      setExpenseReportId(receipt?.expenseReportId ?? NONE);
      setError(null);
      setSuggestion(null);
    }
  }, [open, receipt]);

  async function handleSuggest() {
    if (!receipt) return;
    setSuggesting(true);
    setError(null);
    try {
      const result = await receiptsApi.suggestCategory(receipt.id);
      setSuggestion(result);
      if (result.categoryId) setCategoryId(result.categoryId);
    } catch (e) {
      setError(e instanceof ApiError ? e.message : "Kunde inte föreslå kategori.");
    } finally {
      setSuggesting(false);
    }
  }

  async function handleSave() {
    setError(null);
    setSaving(true);
    const dto = {
      merchant,
      date,
      totalAmount: Number(totalAmount),
      vatAmount: Number(vatAmount || 0),
      description: description || null,
      imageUrl: imageUrl || null,
      categoryId: categoryId === NONE ? null : categoryId,
      expenseReportId: expenseReportId === NONE ? null : expenseReportId,
    };
    try {
      if (receipt) await receiptsApi.update(receipt.id, dto);
      else await receiptsApi.create(dto);
      onSaved();
      onOpenChange(false);
    } catch (e) {
      setError(e instanceof ApiError ? e.message : "Kunde inte spara kvittot.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>{receipt ? "Redigera kvitto" : "Nytt kvitto"}</DialogTitle>
          <DialogDescription>
            Registrera handlare, belopp och koppla till kategori och rapport.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4 py-2">
          <div className="space-y-2">
            <Label htmlFor="merchant">Handlare</Label>
            <Input
              id="merchant"
              value={merchant}
              onChange={(e) => setMerchant(e.target.value)}
              placeholder="t.ex. Circle K"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="date">Datum</Label>
              <Input
                id="date"
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="total">Belopp (kr)</Label>
              <Input
                id="total"
                type="number"
                step="0.01"
                value={totalAmount}
                onChange={(e) => setTotalAmount(e.target.value)}
                placeholder="0,00"
                className="tabular"
              />
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="vat">Moms (kr)</Label>
              <Input
                id="vat"
                type="number"
                step="0.01"
                value={vatAmount}
                onChange={(e) => setVatAmount(e.target.value)}
                placeholder="0,00"
                className="tabular"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="report">Rapport</Label>
              <Select value={expenseReportId} onValueChange={setExpenseReportId}>
                <SelectTrigger id="report">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value={NONE}>Ingen rapport</SelectItem>
                  {reports.map((r) => (
                    <SelectItem key={r.id} value={r.id}>
                      {r.title}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          <div className="space-y-2">
            <div className="flex items-center justify-between">
              <Label htmlFor="category">Kategori</Label>
              <Button
                type="button"
                variant="outline"
                size="sm"
                onClick={handleSuggest}
                disabled={!receipt || suggesting}
                title={receipt ? "Låt AI föreslå kategori" : "Spara kvittot först"}
              >
                <Sparkles className="h-3.5 w-3.5" />
                {suggesting ? "Tänker…" : "Föreslå"}
              </Button>
            </div>
            <Select value={categoryId} onValueChange={setCategoryId}>
              <SelectTrigger id="category">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={NONE}>Ingen kategori</SelectItem>
                {categories.map((c) => (
                  <SelectItem key={c.id} value={c.id}>
                    {c.name} · {c.basAccount} ({formatVatRate(c.vatRate)})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {suggestion && (
              <div className="rounded-md border border-border bg-secondary/50 p-2.5 text-xs text-muted-foreground">
                <span className="font-medium text-foreground tabular">
                  Förslag: {suggestion.basAccount}
                </span>{" "}
                · {Math.round(suggestion.confidence * 100)} % säkerhet
                <br />
                {suggestion.rationale}
              </div>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="desc">Beskrivning (valfritt)</Label>
            <Input
              id="desc"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="t.ex. Tankning tjänstebil"
            />
          </div>

          {error && <p className="text-sm text-destructive">{error}</p>}
        </div>

        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Avbryt
          </Button>
          <Button onClick={handleSave} disabled={saving || !merchant || totalAmount === ""}>
            {saving ? "Sparar…" : "Spara"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}