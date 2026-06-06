import { api } from "@/lib/api";
import type {
  ExpenseReportResponse,
  CreateExpenseReportDto,
  UpdateExpenseReportDto,
  ReceiptResponse,
} from "@/lib/types";

export const expenseReportsApi = {
  list: () => api.get<ExpenseReportResponse[]>("/api/ExpenseReports"),
  create: (dto: CreateExpenseReportDto) =>
    api.post<ExpenseReportResponse>("/api/ExpenseReports", dto),
  update: (id: string, dto: UpdateExpenseReportDto) =>
    api.put<ExpenseReportResponse>(`/api/ExpenseReports/${id}`, dto),
  remove: (id: string) => api.del<void>(`/api/ExpenseReports/${id}`),
  submit: (id: string) =>
    api.post<ExpenseReportResponse>(`/api/ExpenseReports/${id}/submit`),
  approve: (id: string) =>
    api.post<ExpenseReportResponse>(`/api/ExpenseReports/${id}/approve`),
  reject: (id: string) =>
    api.post<ExpenseReportResponse>(`/api/ExpenseReports/${id}/reject`),
  receipts: (id: string) =>
    api.get<ReceiptResponse[]>(`/api/ExpenseReports/${id}/receipts`),
};