import { createBrowserRouter } from "react-router-dom";
import { ProtectedRoute } from "@/components/ProtectedRoute";
import { AppShell } from "@/components/layout/AppShell";
import { LoginPage } from "@/features/auth/LoginPage";
import { DashboardPage } from "@/app/DashboardPage";
import { ReceiptsPage } from "@/features/receipts/ReceiptsPage";
import { CategoriesPage } from "@/features/categories/CategoriesPage";
import { ExpenseReportsPage } from "@/features/expense-reports/ExpenseReportsPage";

export const router = createBrowserRouter([
  { path: "/login", element: <LoginPage /> },
  {
    element: <ProtectedRoute />,
    children: [
      {
        element: <AppShell />,
        children: [
          { path: "/", element: <DashboardPage /> },
          { path: "/receipts", element: <ReceiptsPage /> },
          { path: "/categories", element: <CategoriesPage /> },
          { path: "/expense-reports", element: <ExpenseReportsPage /> },
        ],
      },
    ],
  },
]);