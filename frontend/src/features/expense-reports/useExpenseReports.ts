import { useCallback, useEffect, useState } from "react";
import { expenseReportsApi } from "@/features/expense-reports/api";
import type { ExpenseReportResponse } from "@/lib/types";

export function useExpenseReports() {
  const [reports, setReports] = useState<ExpenseReportResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setReports(await expenseReportsApi.list());
    } catch (e) {
      setError(e instanceof Error ? e.message : "Kunde inte hämta rapporter.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refetch();
  }, [refetch]);

  return { reports, loading, error, refetch };
}