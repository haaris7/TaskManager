import { cn } from '../../utils/helpers';

const GlassCard = ({ children, className, onClick }) => (
  <div
    onClick={onClick}
    className={cn(
      "bg-glass-100 backdrop-blur-xl border border-white/60 shadow-glass rounded-2xl transition-all duration-300",
      onClick && "hover:bg-glass-200 hover:-translate-y-0.5 cursor-pointer hover:shadow-lg",
      className
    )}
  >
    {children}
  </div>
);

export default GlassCard;