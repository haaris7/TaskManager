import { cn } from '../../utils/helpers';

const GlassCard = ({ children, className, onClick }) => (
  <div
    onClick={onClick}
    className={cn(
      "bg-glass-100 backdrop-blur-xl border border-glass-border shadow-xl rounded-2xl transition-all duration-300",
      onClick && "hover:bg-glass-200 hover:shadow-2xl hover:-translate-y-1 cursor-pointer",
      className
    )}
  >
    {children}
  </div>
);

export default GlassCard;