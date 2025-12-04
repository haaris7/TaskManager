import { Loader2 } from 'lucide-react';
import { cn } from '../../utils/helpers';

const Button = ({ children, variant = 'primary', className, isLoading, icon: Icon, ...props }) => {
  const variants = {
    // Bright Teal/Blue gradient for primary actions
    primary: "bg-gradient-to-r from-sky-500 to-teal-500 hover:from-sky-400 hover:to-teal-400 text-white shadow-lg shadow-sky-500/20 border-transparent",
    // Light glass for secondary
    secondary: "bg-white/50 hover:bg-white/80 text-slate-600 border-white/60 hover:shadow-sm",
    // Soft red for danger
    danger: "bg-rose-50 hover:bg-rose-100 text-rose-500 border-rose-200",
  };

  return (
    <button
      className={cn(
        "flex items-center justify-center gap-2 px-4 py-2.5 rounded-xl font-semibold text-sm transition-all duration-200 border disabled:opacity-50 active:scale-95",
        variants[variant],
        className
      )}
      disabled={isLoading}
      {...props}
    >
      {isLoading ? <Loader2 size={18} className="animate-spin" /> : Icon && <Icon size={18} />}
      {children}
    </button>
  );
};

export default Button;