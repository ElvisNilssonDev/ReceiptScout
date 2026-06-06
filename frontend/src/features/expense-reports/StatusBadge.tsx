import { ExpenseReportStatus } from "@/lib/types";

const config: Record<ExpenseReportStatus, { label: string; className: string }> = {
  [ExpenseReportStatus.Draft]: { label: "Utkast", className: "bg-status-draft/10 text-status-draft" },
  [ExpenseReportStatus.Submitted]: { label: "Inskickad", className: "bg-status-submitted/10 text-status-submitted" },
  [ExpenseReportStatus.Approved]: { label: "Godkänd", className: "bg-status-approved/10 text-status-approved" },
  [ExpenseReportStatus.Rejected]: { label: "Avslagen", className: "bg-status-rejected/10 text-status-rejected" },
};

export function StatusBadge({ status }: { status: ExpenseReportStatus }) {
  const { label, className } = config[status];
  return (
    <span
      className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-0.5 text-xs font-medium ${className}`}
    >
      <span className="h-1.5 w-1.5 rounded-full bg-current" />
      {label}
    </span>
  );
}