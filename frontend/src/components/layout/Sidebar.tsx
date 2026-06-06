import { NavLink } from "react-router-dom";
import { LayoutDashboard, ReceiptText, Tags, FileCheck2, LogOut } from "lucide-react";
import { useAuth } from "@/lib/auth";
import { cn } from "@/lib/utils";

const navItems = [
  { to: "/", label: "Instrumentpanel", icon: LayoutDashboard, end: true },
  { to: "/receipts", label: "Kvitton", icon: ReceiptText, end: false },
  { to: "/categories", label: "Kategorier", icon: Tags, end: false },
  { to: "/expense-reports", label: "Utläggsrapporter", icon: FileCheck2, end: false },
];

export function Sidebar() {
  const { email, isAdmin, logout } = useAuth();
  const initials = email?.slice(0, 2).toUpperCase() ?? "??";

  return (
    <aside className="flex h-screen w-64 flex-col border-r border-border bg-card p-4">
      <div className="flex items-center gap-2.5 px-2 pb-6 pt-2">
        <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary text-primary-foreground">
          <ReceiptText className="h-5 w-5" />
        </div>
        <span className="font-display text-lg font-semibold tracking-tight">
          ReceiptScout
        </span>
      </div>

      <nav className="flex flex-col gap-1">
        {navItems.map(({ to, label, icon: Icon, end }) => (
          <NavLink
            key={to}
            to={to}
            end={end}
            className={({ isActive }) =>
              cn(
                "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
                isActive
                  ? "bg-accent text-accent-foreground"
                  : "text-muted-foreground hover:bg-secondary hover:text-foreground",
              )
            }
          >
            <Icon className="h-[18px] w-[18px]" />
            {label}
          </NavLink>
        ))}
      </nav>

      <div className="flex-1" />

      <div className="flex items-center gap-3 rounded-md border border-border p-2.5">
        <div className="flex h-8 w-8 items-center justify-center rounded-full bg-accent text-xs font-semibold text-accent-foreground">
          {initials}
        </div>
        <div className="min-w-0 flex-1">
          <p className="truncate text-sm font-semibold">{email}</p>
          <p className="text-xs text-muted-foreground">
            {isAdmin ? "Administratör" : "Användare"}
          </p>
        </div>
        <button
          onClick={logout}
          title="Logga ut"
          className="text-muted-foreground transition-colors hover:text-foreground"
        >
          <LogOut className="h-4 w-4" />
        </button>
      </div>
    </aside>
  );
}