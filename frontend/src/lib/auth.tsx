import { createContext, useContext, useState, type ReactNode } from "react";
import { api } from "@/lib/api";
import { getSession, setSession, clearSession } from "@/lib/session";
import type { AuthResponse, LoginDto, RegisterDto } from "@/lib/types";

interface AuthState {
  email: string | null;
  roles: string[];
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (dto: LoginDto) => Promise<void>;
  register: (dto: RegisterDto) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthState | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSessionState] = useState<AuthResponse | null>(() => getSession());

  const apply = (res: AuthResponse) => {
    setSession(res);
    setSessionState(res);
  };

  const login = async (dto: LoginDto) => {
    apply(await api.post<AuthResponse>("/api/Auth/login", dto));
  };

  const register = async (dto: RegisterDto) => {
    apply(await api.post<AuthResponse>("/api/Auth/register", dto));
  };

  const logout = () => {
    clearSession();
    setSessionState(null);
  };

  const value: AuthState = {
    email: session?.email ?? null,
    roles: session?.roles ?? [],
    isAuthenticated: session !== null,
    isAdmin: session?.roles.includes("Admin") ?? false,
    login,
    register,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth(): AuthState {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within an AuthProvider.");
  return ctx;
}