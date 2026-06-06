import { useCallback, useEffect, useState } from "react";
import { categoriesApi } from "@/features/categories/api";
import type { CategoryResponse } from "@/lib/types";

export function useCategories() {
  const [categories, setCategories] = useState<CategoryResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setCategories(await categoriesApi.list());
    } catch (e) {
      setError(e instanceof Error ? e.message : "Kunde inte hämta kategorier.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refetch();
  }, [refetch]);

  return { categories, loading, error, refetch };
}