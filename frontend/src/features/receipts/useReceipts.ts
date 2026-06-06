import { useCallback, useEffect, useState } from "react";
import { receiptsApi } from "@/features/receipts/api";
import type { ReceiptResponse } from "@/lib/types";

export function useReceipts() {
  const [receipts, setReceipts] = useState<ReceiptResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setReceipts(await receiptsApi.list());
    } catch (e) {
      setError(e instanceof Error ? e.message : "Kunde inte hämta kvitton.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refetch();
  }, [refetch]);

  return { receipts, loading, error, refetch };
}