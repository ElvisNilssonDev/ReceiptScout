import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "@/lib/auth";

interface ProtectedRouteProps {
  adminOnly?: boolean;
}

export function ProtectedRoute({ adminOnly = false }: ProtectedRouteProps) {
  const { isAuthenticated, isAdmin } = useAuth();

  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (adminOnly && !isAdmin) return <Navigate to="/" replace />;

  return <Outlet />;
}