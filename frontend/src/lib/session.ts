import type { AuthResponse } from "@/lib/types";

const KEY = "receiptscout.session";

export function getSession(): AuthResponse | null {
  const raw = localStorage.getItem(KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as AuthResponse;
  } catch {
    return null;
  }
}

export function setSession(session: AuthResponse): void {
  localStorage.setItem(KEY, JSON.stringify(session));
}

export function clearSession(): void {
  localStorage.removeItem(KEY);
}

export function getToken(): string | null {
  return getSession()?.accessToken ?? null;
}