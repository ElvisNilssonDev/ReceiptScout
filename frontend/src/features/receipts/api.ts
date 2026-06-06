import { api } from "@/lib/api";
import type {
  ReceiptResponse,
  CreateReceiptDto,
  UpdateReceiptDto,
  CategorySuggestion,
} from "@/lib/types";

export const receiptsApi = {
  list: () => api.get<ReceiptResponse[]>("/api/Receipts"),
  get: (id: string) => api.get<ReceiptResponse>(`/api/Receipts/${id}`),
  create: (dto: CreateReceiptDto) => api.post<ReceiptResponse>("/api/Receipts", dto),
  update: (id: string, dto: UpdateReceiptDto) =>
    api.put<ReceiptResponse>(`/api/Receipts/${id}`, dto),
  remove: (id: string) => api.del<void>(`/api/Receipts/${id}`),
  suggestCategory: (id: string) =>
    api.post<CategorySuggestion>(`/api/Receipts/${id}/suggest-category`),
};