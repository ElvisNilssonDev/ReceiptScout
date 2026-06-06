// Mirrors the ReceiptScout API DTOs. Guids -> string, DateTime -> ISO string,
// decimal -> number. Keep in sync with the Application layer.

export const ExpenseReportStatus = {
  Draft: 0,
  Submitted: 1,
  Approved: 2,
  Rejected: 3,
} as const;

export type ExpenseReportStatus =
  (typeof ExpenseReportStatus)[keyof typeof ExpenseReportStatus];

// ---- Auth ----
export interface RegisterDto {
  email: string;
  password: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  email: string;
  roles: string[];
}

// ---- Categories ----
export interface CategoryResponse {
  id: string;
  name: string;
  slug: string;
  basAccount: string;
  vatRate: number;
}

export interface CreateCategoryDto {
  name: string;
  basAccount: string;
  vatRate: number;
}

export type UpdateCategoryDto = CreateCategoryDto;

// ---- Receipts ----
export interface ReceiptResponse {
  id: string;
  merchant: string;
  date: string;
  totalAmount: number;
  vatAmount: number;
  description: string | null;
  imageUrl: string | null;
  userId: string;
  categoryId: string | null;
  expenseReportId: string | null;
  createdAt: string;
}

export interface CreateReceiptDto {
  merchant: string;
  date: string;
  totalAmount: number;
  vatAmount: number;
  description?: string | null;
  imageUrl?: string | null;
  categoryId?: string | null;
  expenseReportId?: string | null;
}

export type UpdateReceiptDto = CreateReceiptDto;

// ---- Expense reports ----
export interface ExpenseReportResponse {
  id: string;
  title: string;
  status: ExpenseReportStatus;
  userId: string;
  approvedByAdminId: string | null;
  createdAt: string;
  submittedAt: string | null;
}

export interface CreateExpenseReportDto {
  title: string;
}

export type UpdateExpenseReportDto = CreateExpenseReportDto;

// ---- AI categorization ----
export interface CategorySuggestion {
  categoryId: string | null;
  basAccount: string;
  confidence: number;
  rationale: string;
}

// ---- Errors (RFC 7807, as returned by the global exception handler) ----
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
}