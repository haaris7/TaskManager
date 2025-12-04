import { Loader2 } from 'lucide-react';
import { cn } from '../../utils/helpers';

const Button = ({ children, variant = 'primary', className, isLoading, icon: Icon, ...props }) => {
  const variants = {
    primary: "bg-indigo-600 hover:bg-indigo-500 text-white shadow-lg shadow-indigo-500/30 border-transparent",
    secondary: "bg-white/10 hover:bg-white/20 text-white border-white/10",
    danger: "bg-rose-500/20 hover:bg-rose-500/30 text-rose-300 border-rose-500/30",
  };

  return (
    <button
      className={cn(
        "flex items-center justify-center gap-2 px-4 py-2.5 rounded-xl font-medium transition-all duration-200 border disabled:opacity-50 active:scale-95",
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