import { cn } from '../../utils/helpers';

const Input = ({ label, error, className, ...props }) => (
  <div className="space-y-1.5 w-full text-left">
    {label && <label className="text-sm font-medium text-slate-300 ml-1">{label}</label>}
    <input
      className={cn(
        "w-full bg-black/20 border border-white/10 rounded-xl px-4 py-3 text-white placeholder:text-slate-500 focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:border-indigo-500/50 transition-all backdrop-blur-sm",
        error && "border-rose-500/50 focus:ring-rose-500/20",
        className
      )}
      {...props}
    />
  </div>
);

export default Input;