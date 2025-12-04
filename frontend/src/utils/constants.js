// Role definitions
export const ROLES = {
  ADMIN: 'Admin',
  PM: 'ProjectManager',
  EMPLOYEE: 'Employee',
  CLIENT: 'Client'
};

// Task status styling configuration
// Using 'dot' class for color and shadow
export const STATUS_CONFIG = {
  NotStarted: {
    label: 'Not Started',
    color: 'bg-slate-300',
    shadow: ''
  },
  InProgress: {
    label: 'In Progress',
    color: 'bg-amber-400',
    shadow: 'shadow-glow-yellow'
  },
  Completed: {
    label: 'Completed',
    color: 'bg-emerald-400',
    shadow: 'shadow-glow-green'
  },
  OnHold: {
    label: 'On Hold',
    color: 'bg-sky-400',
    shadow: 'shadow-glow-blue'
  },
  Cancelled: {
    label: 'Cancelled',
    color: 'bg-rose-500',
    shadow: 'shadow-glow-red'
  }
};